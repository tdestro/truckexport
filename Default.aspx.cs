
namespace truck_manifest
{
    using CodeProject;
    using FileHelpers;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using T4Templates;



    public partial class Default : System.Web.UI.Page
    {
        SortedDictionary<string, Truck> TruckHash;
        Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawTotalsCarrierHash260;
        SortedDictionary<string, locationtranslation> FDtranslationHash;
        SortedDictionary<string, locationtranslation> SDtranslationHash;
        SortedDictionary<string, locationtranslation> translationHash;
        Dictionary<string, Dictionary<string, pakman>> DTI_CTPAKMAN_BundleSizes;

        SortedSet<String> toFrom;
        HashSet<DateTime> DateHashSet;
        List<manifestfile> dateList;
        List<manifestfile> manifestList;
        List<manifestfile> advanceList;
        MultiRecordEngine engine;
        List<string> removeR2s;
        int lowrange;
        int highrange;
        string fileslocation = AppDomain.CurrentDomain.BaseDirectory + "cm";
        
        protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
        {
            e.Day.IsSelectable = false;
            if (DateHashSet.Contains(e.Day.Date))
            {
                e.Day.IsSelectable = true;
                e.Cell.BackColor = System.Drawing.Color.Green;
            }
        }
        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            ((Label)FindControl("Label1")).Text = "";
            ((Label)FindControl("Label2")).Text = "";
            Session["RBText"] = "";

            correctRadioButtonVisiblity();

            if (PlaceHolder1.FindControl("filterGroup") != null)
                ((RadioButtonList)PlaceHolder1.FindControl("filterGroup")).SelectedIndex = -1;

            if (PlaceHolder1.FindControl("filterGroup1") != null)
                ((RadioButtonList)PlaceHolder1.FindControl("filterGroup1")).SelectedIndex = -1;
        }

        private DateTime getDateSelected()
        {
            Calendar Calendar1 = (Calendar)this.FindControl("Calendar1");
            return Calendar1.SelectedDate;
        }


        private void createRequiredRadioButtons()
        {
            RadioButtonList radBtnList = new RadioButtonList();
            radBtnList.Items.Add(new ListItem("Saturday Advance", "0"));
            radBtnList.Items.Add(new ListItem("DC Advance", "1"));
            radBtnList.Items.Add(new ListItem("Sunday Five Star (Bulldog)", "2"));
            radBtnList.Items.Add(new ListItem("Sunday Three Star (Late Main)", "3"));
            radBtnList.Items.Add(new ListItem("Tuesday Color", "4"));
            radBtnList.Items.Add(new ListItem("Wednesday Color", "5"));
            radBtnList.Items.Add(new ListItem("Thursday Color", "6"));
            radBtnList.Items.Add(new ListItem("Friday Color", "7"));
            radBtnList.ID = "filterGroup";
            radBtnList.SelectedIndexChanged += new System.EventHandler(RadioButtonList_CheckChanged);
            radBtnList.AutoPostBack = true;
            radBtnList.RepeatDirection = RepeatDirection.Vertical;
            PlaceHolder1.Controls.Add(radBtnList);

            RadioButtonList radBtnList2 = new RadioButtonList();
            radBtnList2.Items.Add(new ListItem("Daily Advance", "0"));
            radBtnList2.Items.Add(new ListItem("Daily", "1"));
            radBtnList2.ID = "filterGroup1";
            radBtnList2.SelectedIndexChanged += new System.EventHandler(RadioButtonListDaily_CheckChanged);
            radBtnList2.AutoPostBack = true;
            radBtnList2.RepeatDirection = RepeatDirection.Vertical;
            PlaceHolder1.Controls.Add(radBtnList2);
        }

        /// <summary>
        /// Set the radio buttons that are visible according to the day.
        /// </summary>
        private void correctRadioButtonVisiblity()
        {
            if (getDateSelected().DayOfWeek.ToString() != "Sunday")
            {
                ((RadioButtonList)PlaceHolder1.FindControl("filterGroup")).Visible = false;
                ((RadioButtonList)PlaceHolder1.FindControl("filterGroup1")).Visible = true;
            }
            else
            {
                ((RadioButtonList)PlaceHolder1.FindControl("filterGroup1")).Visible = false;
                ((RadioButtonList)PlaceHolder1.FindControl("filterGroup")).Visible = true;
            }

        }



        private void load_datafile_populate(string dayofweek, List<manifestfile> files)
        {
            String loadingmsg = "";
            String truckIDshort;
            toFrom = new SortedSet<String>();
            LinkedList<object> CSVManifest = new LinkedList<object>();

            foreach (manifestfile i in files)
            {
                loadingmsg += Path.GetFileName(i.file) + " ";
                try
                {
                    using (StreamReader reader = new StreamReader(i.file))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            engine.BeginReadString(line);
                            while (engine.ReadNext() != null)
                            {
                                CSVManifest.AddFirst(engine.LastRecord);
                                BaseRecordExtended item = (BaseRecordExtended)engine.LastRecord;
                                truckIDshort = item.TruckID.Substring(0, 3);
                                toFrom.Add(truckIDshort);
                            }
                            engine.Close();
                        }
                    }

                }
                catch (Exception exception)
                {
                    //  unc.NetUseDelete();
                    Debug.Write(exception.Message);
                }

            }

            /*
                }
                unc.Dispose();
            }*/
            Session["lastCSV"] = CSVManifest;
            Label1.Text = Server.HtmlEncode("Using file(s): " + loadingmsg);


         
            dropdownlistFrom.DataSource = toFrom;
            dropdownlistFrom.DataBind();
            dropdownlistTo.DataSource = toFrom;
            dropdownlistTo.DataBind();
            dropdownlistTo.SelectedIndex = dropdownlistTo.Items.Count - 1;
        }


        private void parsedatafile(string dayofweek, bool loadkeydropdowns, LinkedList<object> CSVManifestRecords)
        {
            string rb = (string)Session["RBText"];
            // Here is a sample of rows from the DTI_CTPAKMAN table on ncsCirc database.  
            // You can 
            // select * from DTI_CTPAKMAN 
            // where routeid='10000002'              (using your routeid)
            // and publicationdate='2014-11-13'  (using your pub date)
            //
            // if you are doing a color manifest   also include
            // and medium_id=1                           
            // if you are doing a Sun-main or sun_bulldog  
            // and medium_id<>1
            // THIS LOADS DATA REQUIRED FOR COLOR PREPRINT BUNDLE SIZES. THE BUNDLE SIZES ARE PUT INTO DTI THROUGH A MANUAL METHOD. INSTEAD,
            // WE'RE GOING TO ACCESS THE DATA THROUGH THE INSERTING SYSTEM, BECAUSE IT IS NOT A MANUAL PROCESS.
            string pattern = "u";
            DateTime date = getDateSelected();
            string datestring = date.ToString(pattern).Split(' ')[0];
            string mediumid = " and medium_id<>1"; // ELIMINATE COLOR FOR NON COLOR DAYS
            if (rb.Contains("Color")) mediumid = " and medium_id=1"; //IF THIS IS COLOR, SHOW ONLY COLOR
            DTI_CTPAKMAN_BundleSizes = new Dictionary<string, Dictionary<string, pakman>>();
            using (SqlConnection con = new SqlConnection())
            {



                //
                // Open the SqlConnection.
                //
                con.Open();
                //
                // The following code uses an SqlCommand based on the SqlConnection.
                //
                // CREATE LOOKUP DICTIONARY.
                using (SqlCommand command = new SqlCommand("select * from DTI_CTPAKMAN where publicationdate='" + datestring + "'" + mediumid, con))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string routeID = reader["routeId"].ToString().Trim();
                        string product_number = "0" + reader["product_number"].ToString().Trim();
                        Debug.WriteLine(routeID + " " + product_number);
                        if (!DTI_CTPAKMAN_BundleSizes.ContainsKey(routeID))
                        {

                            DTI_CTPAKMAN_BundleSizes.Add(routeID, new Dictionary<string, pakman>());

                        }
                        if (!DTI_CTPAKMAN_BundleSizes[routeID].ContainsKey(product_number))
                        {
                            pakman newpakman = new pakman();
                            newpakman.bundlesize = Int32.Parse(reader["bundle_size"].ToString());
                            newpakman.draw = Int32.Parse(reader["quantity"].ToString());
                            DTI_CTPAKMAN_BundleSizes[routeID].Add(product_number, newpakman);
                        }
                    }
                }
            }



            // What are depot trucks
            // Mon through Sat // FD Delievery Schedule
            // depot trucks  501-000  through 518-999
            // Sun Late Main // SD Delievery Schedule.
            //depot trucks  751-000 through  768-999
            //color 
            //depot trucks  011-000 through  185-999



            if (dayofweek != "Sunday" && !rb.Contains("Color"))
            {
                translationHash = FDtranslationHash;
                lowrange = 501000;
                highrange = 518999;
            }
            else
            {
                lowrange = 751000;
                highrange = 768999;
                translationHash = SDtranslationHash;
            }

            List<manifestfile> CombinedColorList = (List<manifestfile>)Session["CombinedColorList"];
            // LETS CHECK AND SEE IF WE HAVE A NIGHTMARE TO DEAL WITH.
            // LATE MAIN 770s need Saturday Advance 770s subtracted from them.

            if (rb.Contains("Late Main") && CombinedColorList != null)
            {
                // WE ARE DEALING WITH COLOR. WE HAVE TO LOAD AND PARSE ONE ADDITIONAL FILE
                string combined770 = "";

                Debug.WriteLine("CombinedColor:");
                foreach (manifestfile i in CombinedColorList)
                {
                    Debug.WriteLine(i.file);
                    if (i.file.Contains("770"))
                    {
                        try
                        {
                            using (StreamReader streamReader = new StreamReader(i.file))
                            {

                                combined770 += streamReader.ReadToEnd();
                            }

                        }
                        catch (Exception exception)
                        {
                            //unc.NetUseDelete();
                            Response.Write(exception.Message);
                        }
                    }

                }


                /*}
                unc.Dispose();
            }*/

                // FIRE UP ENGINE AND CREATE OBJECTS.
                LinkedList<object> tocombine770Records = new LinkedList<object>(engine.ReadString(combined770));
                if (engine.ErrorManager.HasErrors)
                    engine.ErrorManager.SaveErrors(AppDomain.CurrentDomain.BaseDirectory + "CSVManifestError.log");


                ProductsDrawTotalsCarrierHash260 = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>();
                SortedDictionary<string, Truck> TruckHash260 = CreateTruckHash(tocombine770Records, "Saturday Advance", 1);
                foreach (KeyValuePair<string, Truck> t in TruckHash260)
                {
                    foreach (KeyValuePair<string, WholeTruck> wt in t.Value.WholeTruckHash)
                    {
                        wt.Value.calculateProductsby("routeid", ref removeR2s, ref ProductsDrawTotalsCarrierHash260);

                    }

                }

                // END Saturday Advance 770 subtract from late main crap.
            }

            // LETS CHECK AND SEE IF WE HAVE A NIGHTMARE TO DEAL WITH.
            // COMBINE COLOR FILE DRAW WITH DRAW OF 260 and 660.

            if (rb.Contains("Color") && CombinedColorList != null)
            {
                // WE ARE DEALING WITH COLOR. WE HAVE TO LOAD AND PARSE ONE ADDITIONAL FILE
                string combined260 = "";

                Debug.WriteLine("CombinedColor:");
                foreach (manifestfile i in CombinedColorList)
                {
                    Debug.WriteLine(i.file);
                    if (i.file.Contains("260"))
                    {
                        Debug.WriteLine(i.file);
                        try
                        {
                            using (StreamReader streamReader = new StreamReader(i.file))
                            {

                                combined260 += streamReader.ReadToEnd();
                            }

                        }
                        catch (Exception exception)
                        {
                            //unc.NetUseDelete();
                            Response.Write(exception.Message);
                        }
                    }

                }

                // FIRE UP ENGINE AND CREATE OBJECTS.
                
                LinkedList<object> tocombine260Records = new LinkedList<object>(engine.ReadString(combined260));
                if (engine.ErrorManager.HasErrors)
                    engine.ErrorManager.SaveErrors(AppDomain.CurrentDomain.BaseDirectory + "CSVManifestError.log");


                ProductsDrawTotalsCarrierHash260 = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>();
                SortedDictionary<string, Truck> TruckHash260 = CreateTruckHash(tocombine260Records, "Sunday Five Star (Bulldog)", 1);
                foreach (KeyValuePair<string, Truck> t in TruckHash260)
                {
                    foreach (KeyValuePair<string, WholeTruck> wt in t.Value.WholeTruckHash)
                    {
                        wt.Value.calculateProductsby("carrier", ref removeR2s, ref ProductsDrawTotalsCarrierHash260);

                    }

                }

                // PRODUCTSDRAWTOTALSCARRIERHASH260 AND PRODUCTSDRAWTOTALSCARRIERHASH660 MUST BE ADDED AGAINST COLOR FILE LATER
                // IT CANNOT BE ADDED IN NOW BECAUSE OF THE STUPID D1 RECORDS NOT BEING TALLIED TO REVEAL THE REAL DRAW.
                // WE'RE GOING TO ADD IT ON AT THE VERY END INSIDE WHOLETRUCK.CS.
            }

            

            if (engine.ErrorManager.HasErrors)
                engine.ErrorManager.SaveErrors(AppDomain.CurrentDomain.BaseDirectory + "CSVManifestError.log");

            TruckHash = CreateTruckHash(CSVManifestRecords, (string)Session["RBText"], 0);


            if (loadkeydropdowns)
            {
                List<String> toFromList = toFrom.OrderBy(o => o).ToList();
                dropdownlistFrom.DataSource = toFromList;
                dropdownlistFrom.DataBind();
                dropdownlistTo.DataSource = toFromList;
                dropdownlistTo.DataBind();
                dropdownlistTo.SelectedValue = toFromList.ElementAt(toFromList.Count - 1);
            }

            Debug.WriteLine("-----------------------------");

        }

        SortedDictionary<string, Truck> CreateTruckHash(LinkedList<object> CSVManifestRecords, string rbtext, int truckhashid)
        {


            // SPLIT THE OBJECTS UP ACCORDING TO THE FIRST 3 DIGITS OF THEIR TRUCK IDS AND LOAD THEM INTO TRUCK OBJECTS ASSOCIATED WITH THAT ID.
            // WITHIN THE TRUCK THE DATA IS FUTHER NESTED BY THE COMPLETE TRUCK ID.

            // SPLIT THE OBJECTS UP ACCORDING TO THE FIRST 3 DIGITS OF THEIR TRUCK IDS AND LOAD THEM INTO TRUCK OBJECTS ASSOCIATED WITH THAT ID.
            SortedDictionary<string, Truck> TruckHash = new SortedDictionary<string, Truck>();
            var AllR1sForKeyList = new List<R1RouteDetail>();
            var AllR2sForKeyList = new List<R2AdvanceSectionDetail>();
            String truckIDshort;

            toFrom = new SortedSet<String>();

            foreach (object record in CSVManifestRecords)
            {
                BaseRecordExtended item = (BaseRecordExtended)record;
                truckIDshort = item.TruckID.Substring(0, 3);


                // Make sure a 3 digit truck number exists for this record.
                if (!TruckHash.ContainsKey(truckIDshort))
                {
                    TruckHash.Add(truckIDshort, new Truck(truckIDshort, item.TruckName, lowrange, highrange, translationHash, truckhashid));
                    toFrom.Add(truckIDshort);
                }

                if (item.TruckID.Length == 3)
                {

                    if (record is R1RouteDetail)
                    {

                        R1RouteDetail rd = (R1RouteDetail)record;
                        //   Debug.WriteLine (rd.DropInstructions + " " + rd.DropLocation);

                        TruckHash[truckIDshort].headerRecord = rd;
                    }


                    continue;
                }


                // Make sure a full length truck number exists for this record.
                if (!TruckHash[truckIDshort].WholeTruckHash.ContainsKey(item.TruckID))
                {
                    TruckHash[truckIDshort].WholeTruckHash.Add(item.TruckID, new WholeTruck(item.TruckID, item.TruckName, lowrange, highrange, translationHash, truckhashid));
                }

                WholeTruck wholeTruck = (WholeTruck)TruckHash[truckIDshort].WholeTruckHash[item.TruckID];

                if (record is A1TruckTimeData)
                {
                    wholeTruck.A1TruckTimeDataList.Add((A1TruckTimeData)record);
                }
                else if (record is S1SubscriptionData)
                {
                    wholeTruck.S1SubscriptionDataList.Add((S1SubscriptionData)record);
                }
                else if (record is R1RouteDetail)
                {

                    R1RouteDetail rd = (R1RouteDetail)record;


                    // Change 776s back to 770s.

                    if (rd.Edition == "776" || rd.Edition == "772") { rd.Edition = "770"; }


                    // Make sure a TruckDropOrder / Drop# exists for this record.
                    if (!wholeTruck.R1RouteDetailDictionary.ContainsKey(rd.TruckDropOrder))
                    {
                        wholeTruck.R1RouteDetailDictionary.Add(rd.TruckDropOrder, new List<R1RouteDetail>());
                    }

                    wholeTruck.R1RouteDetailDictionary[rd.TruckDropOrder].Add(rd);
                    AllR1sForKeyList.Add(rd);

                }
                else if (record is D1DrawTotals)
                {
                    D1DrawTotals d1 = (D1DrawTotals)record;
                    // Make sure a TruckDropOrder / Drop# exists for this record.
                    if (!wholeTruck.D1DrawTotalsDictionaryUnfiltered.ContainsKey(d1.TruckDropOrder))
                    {
                        wholeTruck.D1DrawTotalsDictionaryUnfiltered.Add(d1.TruckDropOrder, new List<D1DrawTotals>());
                    }

                    wholeTruck.D1DrawTotalsDictionaryUnfiltered[d1.TruckDropOrder].Add(d1);
                }
                else if (record is P1PreviousDrawTotals)
                {
                    wholeTruck.P1PreviousDrawTotalsList.Add((P1PreviousDrawTotals)record);
                }
                else if (record is P2PrevDrawTotalsByType)
                {
                    wholeTruck.P2PrevDrawTotalsByTypeList.Add((P2PrevDrawTotalsByType)record);
                }
                else if (record is R2AdvanceSectionDetail)
                {
                    R2AdvanceSectionDetail r2 = (R2AdvanceSectionDetail)record;

                    // R2s MUST WAIT TO GO INTO THE WHOLE TRUCKS. WE NEED TO ARTIFICALLY CREATE NEW ONES 
                    // FOR THOSE THAT DTI IS NOT GIVING US. THIS WILL BE DONE AS A SEPARATE PROCESS.
                    // 776 is a 770. 201 is 200.
                    if (r2.PaperSection == "776" || r2.PaperSection == "772") r2.PaperSection = "770";
                    //if (r2.PaperSection == "201") r2.PaperSection = "200";
                    AllR2sForKeyList.Add(r2);

                }
                else if (record is R3TMProductDetail)
                {
                    R3TMProductDetail r3 = (R3TMProductDetail)record;
                    if (!wholeTruck.R3TMProductDetailDictionary.ContainsKey(r3.TruckDropOrder))
                    {
                        wholeTruck.R3TMProductDetailDictionary.Add(r3.TruckDropOrder, new List<R3TMProductDetail>());
                    }
                    wholeTruck.R3TMProductDetailDictionary[r3.TruckDropOrder].Add(r3);
                }
                else if (record is W1TruckWeightDetail)
                {
                    wholeTruck.W1TruckWeightDetailList.Add((W1TruckWeightDetail)record);
                }
                else if (record is T1TruckTotals)
                {
                    wholeTruck.T1TruckTotalsList.Add((T1TruckTotals)record);
                }
                else if (record is T2AdvanceSectionTruckTotals)
                {
                    wholeTruck.T2AdvanceSectionTruckTotalsList.Add((T2AdvanceSectionTruckTotals)record);
                }
                else if (record is T3TMProductTotals)
                {
                    wholeTruck.T3TMProductTotalsList.Add((T3TMProductTotals)record);
                }
                //    Debug.WriteLine ();
                //	   Debug.WriteLine (record.ToString ());
                //	   Debug.WriteLine ();

            }


            switch (rbtext)
            {
                case ("Saturday Advance"):
                case ("DC Advance"):
                    r2filterdefault(ref AllR1sForKeyList, ref AllR2sForKeyList, 1);
                    break;

                case ("Sunday Five Star (Bulldog)"):
                    r2filterdefault(ref AllR1sForKeyList, ref AllR2sForKeyList, 8);
                    break;


                case ("Sunday Three Star (Late Main)"):
                    r2filterdefault(ref AllR1sForKeyList, ref AllR2sForKeyList, 0);
                    break;

                case ("Tuesday Color"):
                case ("Wednesday Color"):
                case ("Thursday Color"):
                case ("Friday Color"):
                    r2filterdefault(ref AllR1sForKeyList, ref AllR2sForKeyList, 2);

                    break;
                default:
                    r2filterdefault(ref AllR1sForKeyList, ref AllR2sForKeyList, 4);
                    break;

            }




            // NOW THAT ARTIFICAL R2S HAVE BEEN CREATED, WE INSERT THEM INTO THE TRUCK TREE STRUCTURE.

            foreach (R2AdvanceSectionDetail item in AllR2sForKeyList)
            {

                truckIDshort = item.TruckID.Substring(0, 3);

                // Make sure a full length truck number exists for this record.
                if (!TruckHash[truckIDshort].WholeTruckHash.ContainsKey(item.TruckID))
                {

                    TruckHash[truckIDshort].WholeTruckHash.Add(item.TruckID, new WholeTruck(item.TruckID, item.TruckName, lowrange, highrange, translationHash, truckhashid));
                }
                WholeTruck wholeTruck = (WholeTruck)TruckHash[truckIDshort].WholeTruckHash[item.TruckID];

                // Make sure a TruckDropOrder / Drop# exists for this record.
                if (!wholeTruck.R2AdvanceSectionDetailDictionaryUnfiltered.ContainsKey(item.TruckDropOrder))
                {
                    wholeTruck.R2AdvanceSectionDetailDictionaryUnfiltered.Add(item.TruckDropOrder, new List<R2AdvanceSectionDetail>());
                }
                wholeTruck.R2AdvanceSectionDetailDictionaryUnfiltered[item.TruckDropOrder].Add(item);
            }

            return TruckHash;
        }

        void r2filterdefault(ref List<R1RouteDetail> AllR1sForKeyList, ref List<R2AdvanceSectionDetail> AllR2sForKeyList, int correctionType)
        {
            string package = "package--";

            foreach (R1RouteDetail r1 in AllR1sForKeyList)
            {

                if (r1.ProductID == "NYT" && r1.RouteTypeIndicator == "HD")
                {
                    createR2Record(r1, "970", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "TRB" && r1.RouteTypeIndicator == "HD")
                {
                    createR2Record(r1, "971", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "WSJ" && r1.RouteTypeIndicator == "HD")
                {
                    createR2Record(r1, "972", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "USA" && r1.RouteTypeIndicator == "HD")
                {
                    createR2Record(r1, "973", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "FTU" && r1.RouteTypeIndicator == "HD")
                {
                    createR2Record(r1, "974", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "IBD" && r1.RouteTypeIndicator == "HD")
                {
                    createR2Record(r1, "975", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "NYT" && r1.RouteTypeIndicator == "SC")
                {
                    createR2Record(r1, "960", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "WSJ" && r1.RouteTypeIndicator == "SC")
                {
                    createR2Record(r1, "962", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "USA" && r1.RouteTypeIndicator == "SC")
                {
                    createR2Record(r1, "963", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "USW" && r1.RouteTypeIndicator == "SC")
                {
                    createR2Record(r1, "964", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "IBD" && r1.RouteTypeIndicator == "SC")
                {
                    createR2Record(r1, "965", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "BAR" && r1.RouteTypeIndicator == "SC")
                {
                    createR2Record(r1, "966", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "USE" && r1.RouteTypeIndicator == "SC")
                {
                    createR2Record(r1, "967", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "TWP" && r1.RouteTypeIndicator == "SC")
                {
                    createR2Record(r1, "968", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "STL" && r1.RouteTypeIndicator == "SC")
                {
                    createR2Record(r1, "969", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "PPG" && r1.Edition == "999")
                {
                    createR2Record(r1, "200", ref AllR2sForKeyList, true);
                }
                else if (r1.ProductID == "PPG" && r1.Edition == "201")
                {
                    createR2Record(r1, "201", ref AllR2sForKeyList, true);
                }

            }

            List<R2AdvanceSectionDetail> dupR2list = new List<R2AdvanceSectionDetail>(AllR2sForKeyList);


            foreach (R1RouteDetail r1 in AllR1sForKeyList)
            {

                if (correctionType == 1)
                {
                    // THIS CORRECTION TYPE WILL LOOK FOR PAPER SECTION (PRODUCT CODE) IN EDITION FIELD ON R1 RECORD.
                    // USED FOR: 260, 261, 770, 776, 966 files
                    if (r1.ProductID == "PPG" && r1.Edition.Length > 0 && r1.Edition != "FINAL")
                    {
                        createR2Record(r1, r1.Edition, ref AllR2sForKeyList, true);
                    }
                }
                else if (correctionType == 8)
                {
                    // THIS CORRECTION TYPE WILL LOOK FOR PAPER SECTION (PRODUCT CODE) IN EDITION FIELD ON R1 RECORD.
                    // USED FOR: 260, 261, 770, 776, 966 files
                    if (r1.ProductID == "PPG" && r1.Edition.Length > 0)
                    {
                        createR2Record(r1, r1.Edition, ref AllR2sForKeyList, false);
                        if (r1.Edition == "260") createR2Record(r1, "770", ref AllR2sForKeyList, false);
                    }
                }


                else if (correctionType == 2)
                {
                    List<string> keys = new List<string>();


                    // CREATE R2s using DTI_PAKMAN FOR INSERTS. 
                    if (DTI_CTPAKMAN_BundleSizes.ContainsKey(r1.RouteID) && r1.ProductID == "PPG")
                    {
                        foreach (KeyValuePair<string, pakman> PaperSection in DTI_CTPAKMAN_BundleSizes[r1.RouteID])
                        {
                            createR2Record(r1, PaperSection.Key, ref AllR2sForKeyList, false);
                            keys.Add(PaperSection.Key);
                        }

                    }

                    // THIS CORRECTION TYPE LOOKS FOR PAPER SECTION (PRODUCT CODE) IN INSERTMIX COMBINATION.
                    // USED FOR: INSERT MIX IS USED IN COLOR FILES 

                    if (r1.InsertMixCombination.Contains(package) && r1.Edition != "770")
                    {
                        string prodcode = r1.InsertMixCombination.Remove(r1.InsertMixCombination.IndexOf(package), package.Length);
                        if (!keys.Contains(prodcode))
                        {

                            createR2Record(r1, prodcode, ref AllR2sForKeyList, false);
                        }

                    }

                }
                else if (correctionType == 3)
                {
                    if (r1.ProductID == "PPG" && r1.RunType.ToUpper() == "ADVANCE" && r1.Edition.Contains(package))
                    {
                        string prodcode = r1.Edition.Remove(r1.Edition.IndexOf(package), package.Length);
                        createR2Record(r1, r1.Edition, ref AllR2sForKeyList, true);
                    }
                }
                else if (correctionType == 4)
                {
                    List<string> keys = new List<string>();
                    bool create200 = true;

                    // CREATE R2s using DTI_PAKMAN FOR INSERTS. IF WE ONLY HAVE THESE, CREATE A 200.
                    if (DTI_CTPAKMAN_BundleSizes.ContainsKey(r1.RouteID) && r1.ProductID == "PPG")
                    {
                        foreach (KeyValuePair<string, pakman> PaperSection in DTI_CTPAKMAN_BundleSizes[r1.RouteID])
                        {
                            createR2Record(r1, PaperSection.Key, ref AllR2sForKeyList, false);
                            keys.Add(PaperSection.Key);
                        }

                    }

                    // CHECK AND SEE IF THE PACKAGES ARE ON THE DTI RECORDS.
                    if (r1.InsertMixCombination.Contains(package))
                    {
                        foreach (R2AdvanceSectionDetail r2detail in dupR2list)
                        {
                            if (r2detail.ProductID == r1.ProductID && r2detail.RouteID == r1.RouteID && r2detail.InsertMix == r1.InsertMixCombination)
                            {

                                // We've found a package. Make sure we already didn't add it from ctpackman and make sure we don't make a 200 record.
                                string prodcode = r1.InsertMixCombination.Remove(r1.InsertMixCombination.IndexOf(package), package.Length);

                                if (!keys.Contains(prodcode))
                                {
                                    // didn't find a ctpakman created package, so create it
                                    createR2Record(r1, prodcode, ref AllR2sForKeyList, false);
                                }
                                // we found a package so no 200
                                create200 = false;
                                break;

                            }

                        }


                    }

                    foreach (R2AdvanceSectionDetail r2detail in dupR2list)
                    {

                        if (r2detail.MainProductID == "PPG" && r2detail.RouteID == r1.RouteID)
                        {
                            create200 = false;
                            break;
                        }
                    }



                    if (create200 && r1.ProductID == "PPG" && r1.Edition.ToUpper() == "FINAL")
                    {
                        createR2Record(r1, "200", ref AllR2sForKeyList, false);
                    }

                }


            }
        }

        public void createR2Record(R1RouteDetail r1, string productcode, ref List<R2AdvanceSectionDetail> AllR2sForKeyList, bool checkfordups)
        {

            bool r2found = false;

            if (checkfordups)
            {
                foreach (R2AdvanceSectionDetail r2detail in AllR2sForKeyList)
                {

                    if (r2detail.MainProductID == r1.ProductID && r2detail.RouteID == r1.RouteID)
                    {
                        r2found = true;
                    }
                }

            }

            if (!r2found)
            {
                R2AdvanceSectionDetail r2 = new R2AdvanceSectionDetail();
                r2.RecordType = "R2";
                r2.MainProductID = r1.ProductID;
                r2.ProductID = r1.MainProductID;
                r2.RunType = r1.RunType;
                r2.RunDate = r1.RunDate;
                r2.TruckID = r1.TruckID;
                r2.TruckName = r1.TruckName;
                r2.TruckDropOrder = r1.TruckDropOrder;
                r2.RouteID = r1.RouteID;
                r2.PaperSection = productcode;
                r2.Updraw = "0";
                r2.InsertMix = r1.InsertMixCombination;
                r2.TruckID = r1.TruckID;
                AllR2sForKeyList.Add(r2);
            }

        }

        private List<manifestfile> getManifestFileNames(DateTime selectedDate, List<string> products)
        {
            List<manifestfile> result = new List<manifestfile>(dateList);

            /*
            	   Debug.WriteLine (selectedDate.ToShortDateString ());
                foreach(manifestfile a in result){
                       Debug.WriteLine (a.contentnumber+" "+a.date.ToShortDateString());
                }
            */

            result.Reverse(); // Files were ordered the wrong way for us. We need not order the list by age of the file, as it is already ordered (want to use the newest files only).
            result.RemoveAll(o => (o.date.CompareTo(selectedDate) != 0 || !products.Contains(o.contentnumber)));
            return result.GroupBy(product => product.contentnumber).ToDictionary(group => group.Key, group => group.First()).Values.ToList();
            /*	
            foreach (KeyValuePair<string,manifestfile> r in groupedCustomerList){
                   Debug.WriteLine("-");

                       Debug.WriteLine (r.Value.contentnumber+" "+r.Value.fileCreatedDate.ToShortDateString());
            }*/
        }
        private CancellationTokenSource _cts;

        private void RadioButtonListDaily_CheckChanged(Object sender, EventArgs e)
        {
            DateTime selectedDate = getDateSelected();
            string dayofweek = selectedDate.DayOfWeek.ToString();

            RadioButtonList rb = (RadioButtonList)sender;
            removeR2s = new List<string>();

            manifestList = (List<manifestfile>)Session["ManifestList"];
            advanceList = (List<manifestfile>)Session["AdvanceList"];

            List<manifestfile> files = new List<manifestfile>();
            manifestfile manifestfile;
            correctRadioButtonVisiblity();

            switch (rb.SelectedItem.Text)
            {

                case ("Daily Advance"):
                    //ADV file for the day.
                 
                    manifestfile = advanceList.FindLast(x => x.date == selectedDate);
                    if (manifestfile != null) files.Add(manifestfile);


                    load_datafile_populate(dayofweek, files);
                    break;
                case ("Daily"):
                    // Mainfest Only

                    manifestfile = manifestList.FindLast(x => x.date == selectedDate);
                    if (manifestfile != null) files.Add(manifestfile);
                    load_datafile_populate(dayofweek, files);
                    break;
            }

            Session["RBText"] = rb.SelectedItem.Text;
            Session["removeR2s"] = removeR2s;

            // KEY SELECTIONS WILL NEED TO CHANGE TO REFLECT WHAT WILL BE IN THE MANIFEST SO WE HAVE TO 
            // PERFORM SOME MORE COMPLICATED OPERATIONS, PRE FILTERING, PRIOR TO ACTUALLY GENERATE THE MANIFEST.

            var AllR1sForKeyList = (List<R1RouteDetail>)Session["AllR1sForKeyList"];
            var AllR2sForKeyList = (List<R2AdvanceSectionDetail>)Session["AllR2sForKeyList"];
            // WE NEED TO GENERATE ADDITIONAL R2S WHICH ARE NOT IN THE LIST SO THAT WE CAN KNOCK OUT ALL THE UNWANTED KEYS


        }

        private void RadioButtonList_CheckChanged(Object sender, EventArgs e)
        {
            DateTime selectedDate = getDateSelected();
            RadioButtonList rb = (RadioButtonList)sender;
            removeR2s = new List<string>();



            List<manifestfile> combinedcolorlist = new List<manifestfile>();
            manifestfile manifestfile;
            correctRadioButtonVisiblity();
            switch (rb.SelectedItem.Text)
            {

                case ("Saturday Advance"):
                    // WE MUST FIND THE FILES OF THE SAME DATE WHICH CONTAIN 260, 261, 996, 770 USING NUMBER IN THE FILE NAME AND COMBINE THEM. WHATEVEVER UNWANTED CRAP CAN BE FILTERED OUT.
                    load_datafile_populate(selectedDate.DayOfWeek.ToString(), getManifestFileNames(selectedDate, new List<string>(new String[] { "770" })));
                    break;
                case ("DC Advance"):
                    // (770) 776 ONLY
                    load_datafile_populate(selectedDate.DayOfWeek.ToString(), getManifestFileNames(selectedDate, new List<string>(new String[] { "776" })));
                    break;
                case ("Sunday Five Star (Bulldog)"):
                    // 261 260 770(772) 966 BARRONS
                    List<manifestfile> fivestarlist = getManifestFileNames(selectedDate, new List<string>(new String[] { "966", "261", "260" }));
                    load_datafile_populate(selectedDate.DayOfWeek.ToString(), fivestarlist);
                    
                    break;
                case ("Sunday Three Star (Late Main)"):
                    // 661 660 776/770 NYT-960 
                    // MAIN SUNDAY FILE MINUS COLOR CRAP.
                    List<manifestfile> threestarlist = new List<manifestfile>();
                    manifestfile = manifestList.FindLast(x => x.date == getDateSelected());
                    if (manifestfile != null) threestarlist.Add(manifestfile);
                    load_datafile_populate(selectedDate.DayOfWeek.ToString(), threestarlist);
                    // REMOVE ALL COLOR.
                    removeR2s.Add("CO-TUE");
                    removeR2s.Add("CO-WED");
                    removeR2s.Add("CO-THU");
                    removeR2s.Add("CO-FRI");
                    combinedcolorlist = getManifestFileNames(selectedDate, new List<string>(new String[] { "770" }));
                    break;


                // COLOR FILES ARE COMPOSED OF THEIR RESPECTIVE FILE, THE SUNDAY 660 and the BULLDOG 260. THESE ROUTES MUST BE MERGED BASED UPON THE CARRIER ID.
                // WHAT A BITCH.
                case ("Tuesday Color"):
                    load_datafile_populate(selectedDate.DayOfWeek.ToString(), getManifestFileNames(selectedDate, new List<string>(new String[] { "CO-TUE" })));
                    combinedcolorlist = getManifestFileNames(selectedDate, new List<string>(new String[] { "260" }));
                    break;
                case ("Wednesday Color"):
                    load_datafile_populate(selectedDate.DayOfWeek.ToString(), getManifestFileNames(selectedDate, new List<string>(new String[] { "CO-WED" })));
                    combinedcolorlist = getManifestFileNames(selectedDate, new List<string>(new String[] { "260" }));
                    break;
                case ("Thursday Color"):
                    load_datafile_populate(selectedDate.DayOfWeek.ToString(), getManifestFileNames(selectedDate, new List<string>(new String[] { "CO-THU" })));
                    combinedcolorlist = getManifestFileNames(selectedDate, new List<string>(new String[] { "260" }));
                    break;
                case ("Friday Color"):
                    load_datafile_populate(selectedDate.DayOfWeek.ToString(), getManifestFileNames(selectedDate, new List<string>(new String[] { "CO-FRI" })));
                    combinedcolorlist = getManifestFileNames(selectedDate, new List<string>(new String[] { "260" }));
                    break;
            }

            Session["CombinedColorList"] = combinedcolorlist;
            Session["RBText"] = rb.SelectedItem.Text;
            Session["removeR2s"] = removeR2s;

            // KEY SELECTIONS WILL NEED TO CHANGE TO REFLECT WHAT WILL BE IN THE MANIFEST SO WE HAVE TO 
            // PERFORM SOME MORE COMPLICATED OPERATIONS, PRE FILTERING, PRIOR TO ACTUALLY GENERATE THE MANIFEST.

            var AllR1sForKeyList = (List<R1RouteDetail>)Session["AllR1sForKeyList"];
            var AllR2sForKeyList = (List<R2AdvanceSectionDetail>)Session["AllR2sForKeyList"];
            // WE NEED TO GENERATE ADDITIONAL R2S WHICH ARE NOT IN THE LIST SO THAT WE CAN KNOCK OUT ALL THE UNWANTED KEYS

        }

        private static string GetNumbers(string input)
        {

            // CHECK FOR COLOR FILES.
            int index;
            if ((index = input.ToUpper().IndexOf("CO-")) > -1)
            {
                return input.Substring(index, 6).ToUpper();
            }
            else return new string(input.Where(c => char.IsDigit(c)).ToArray()).Substring(0, 3);
        }
        protected async void Page_Load(object sender, EventArgs e)
        {

            engine = new MultiRecordEngine(typeof(A1TruckTimeData), typeof(S1SubscriptionData), typeof(R1RouteDetail), typeof(D1DrawTotals),
                typeof(P1PreviousDrawTotals), typeof(P2PrevDrawTotalsByType), typeof(R2AdvanceSectionDetail), typeof(R3TMProductDetail),
                typeof(W1TruckWeightDetail), typeof(T1TruckTotals), typeof(locationtranslation));
            engine.ErrorManager.ErrorMode = ErrorMode.IgnoreAndContinue;
            engine.RecordSelector = new RecordTypeSelector(TruckManifestSelector);

            if (Page.IsPostBack)
            {
                Debug.WriteLine("Postback " + getDateSelected().DayOfWeek.ToString());


                SDtranslationHash = (SortedDictionary<string, locationtranslation>)Session["SDTranslationHash"];
                FDtranslationHash = (SortedDictionary<string, locationtranslation>)Session["FDTranslationHash"];
                dateList = (List<manifestfile>)Session["DateList"];
                DateHashSet = (HashSet<DateTime>)Session["DateHashSet"];
                manifestList = (List<manifestfile>)Session["ManifestList"];
                advanceList = (List<manifestfile>)Session["AdvanceList"];
                createRequiredRadioButtons();
                correctRadioButtonVisiblity();
            }
            else
            {
                Debug.WriteLine("not IsPostBack");
                FDtranslationHash = new SortedDictionary<string, locationtranslation>();
                SDtranslationHash = new SortedDictionary<string, locationtranslation>();
                FileHelperEngine translationEngine = new FileHelperEngine(typeof(locationtranslation));
                locationtranslation[] locationrecords;
                try
                {
                    locationrecords = (locationtranslation[])translationEngine.ReadFile(AppDomain.CurrentDomain.BaseDirectory + "FDtranslation.csv");
                    foreach (locationtranslation item in locationrecords)
                    {
                        if (!FDtranslationHash.ContainsKey(item.TruckID))
                        {
                            FDtranslationHash.Add(item.TruckID, item);
                        }
                    }
                    locationrecords = (locationtranslation[])translationEngine.ReadFile(AppDomain.CurrentDomain.BaseDirectory + "SDtranslation.csv");
                    foreach (locationtranslation item in locationrecords)
                    {
                        if (!SDtranslationHash.ContainsKey(item.TruckID))
                        {
                            SDtranslationHash.Add(item.TruckID, item);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.Write(exception.Message + "\n\nWarning: Problem with FDtranslation.csv or SDtranslation.csv");
                }
                Session["SDTranslationHash"] = SDtranslationHash;
                Session["FDTranslationHash"] = FDtranslationHash;
                Session["RBText"] = "";
               
                // OPENING EACH FILE AND GRABBING THE DATE TO DETERMINE IF IT SHOULD STAY OR if it's junk
                ManifestOpenResult manifestOpenResult = await openAllTheManifestFiles(fileslocation);
                DateHashSet = manifestOpenResult.DateHashSet;

                dateList = manifestOpenResult.dateList.OrderBy(o => o.date).ThenBy(n => n.fileCreatedDate).ToList();
                advanceList = new List<manifestfile>(dateList);
                manifestList = new List<manifestfile>(dateList);

                if (dateList.Count == 0)
                {
                    Response.Write("No data files present in " + fileslocation);
                    return;

                }

                // REMOVE ADVANCE MANIFESTS FROM MANIFESTLIST AND MANIFESTS FROM ADVANCELIST.
                manifestList.RemoveAll(o => o.RunType == "Advance");
                advanceList.RemoveAll(o => (o.RunType == "Actual" || o.file == ""));

                Session["DateList"] = dateList;
                Session["DateHashSet"] = DateHashSet;
                Session["ManifestList"] = manifestList;
                Session["AdvanceList"] = advanceList;
                if (!Page.IsPostBack)
                {
                    Calendar Calendar1 = (Calendar)this.FindControl("Calendar1");
                    Calendar1.SelectedDate = Calendar1.VisibleDate = manifestList[manifestList.Count - 1].date;

                }

                createRequiredRadioButtons();
                correctRadioButtonVisiblity();
            }



        }
        protected string renameFile(FileData file, string rename)
        {
            string extension = Path.GetExtension(file.Name);
            string fileNameOnly = rename + file.Name;
            string path = Path.GetDirectoryName(file.Path);
            string fullPath = path + "\\" + fileNameOnly;

            int count = 1;
            
           
            string newFullPath = fullPath;

            Debug.WriteLine(newFullPath);
            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            try
            {
                File.Move(file.Path, newFullPath);
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex);
            }

            return newFullPath;
        }
        private void badfile(FileData f)
        {
            ((Label)FindControl("Label2")).Text += "Warning: File " + f.Name + " contains an unexpected format and is unusable, this file has been renamed as bad.</br>";

            Task task = new Task(() => renameFile(f, "bad"));
            task.Start();
        }

        private manifestfile returnFileAndFirstLine(FileData f)
        {
            manifestfile m = new manifestfile();
            StreamReader sr = new StreamReader(f.Path);
            string line = sr.ReadLine();
            sr.Close();
            Debug.WriteLine("File Needs Renamed:" + f.Name);

            if (line != null)
            {

                object[] records = engine.ReadString(line);

                // IF NO RECORD WAS PARSED OUT OF THE FILE, OR THE FIRST RECORD IS NOT AN R1.
                if (records.Length == 0 || ((BaseRecord)records[0]).RecordType != "R1")
                {
                    badfile(f);
                }
                else
                {
                    // Dates matched with filenames array for dropdown.
                    R1RouteDetail r1 = ((R1RouteDetail)records[0]);
                    string RunType = "Actual";
                    if (f.Name.Contains("advmanifest")) RunType = "Advance";

                    string path = renameFile(f, r1.RunDate.ToString("yyyy_MM_dd_"));
                    string file = Path.GetFileName(path);
                    m = new manifestfile(path, GetNumbers(file.Substring(10,file.Length-10)), f.CreationTime, r1.RunDate.ToShortDateString(), r1.RunDate, RunType);


                }
            }
            else
            {

                badfile(f);
            }

            return m;
        }

        public async Task<ManifestOpenResult>openAllTheManifestFiles(string fileslocation)
        {

            List<Task<manifestfile>> mytaskslist = new List<Task<manifestfile>>();
            List<manifestfile> dateList = new List<manifestfile>();
            HashSet<DateTime> DateHashSet = new HashSet<DateTime>();
            DateTime retDate = DateTime.Now.AddDays(-2);
            DateTime dateValue;
            System.Globalization.CultureInfo enUS = new System.Globalization.CultureInfo("en-US");

            foreach (FileData f in FastDirectoryEnumerator.EnumerateFiles(fileslocation, "*manifest*.*", SearchOption.TopDirectoryOnly))
            {
                if (!f.Name.Contains("bad"))
                {
                    try
                    {
                        dateValue = DateTime.ParseExact(f.Name.Substring(0, 11), "yyyy_MM_dd_", enUS, System.Globalization.DateTimeStyles.None);
                        string RunType = "Actual";
                        if (f.Name.Contains("advmanifest")) RunType = "Advance";

                        manifestfile man = new manifestfile(f.Path, GetNumbers(f.Name.Substring(10, f.Name.Length-10)), f.CreationTime, dateValue.ToShortDateString(), dateValue, RunType);
                        DateHashSet.Add(man.date);
                        dateList.Add(man);

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        Task<manifestfile> task = new Task<manifestfile>(() => returnFileAndFirstLine(f));
                        task.Start();
                        mytaskslist.Add(task);
                    }


                }
            }

            while (mytaskslist.Count > 0)
            {

                Task<manifestfile> firstFinishedTask = await Task.WhenAny(mytaskslist);
                mytaskslist.Remove(firstFinishedTask);
                manifestfile m = await firstFinishedTask;
                DateHashSet.Add(m.date);
                dateList.Add(m);
            }
            /*foreach (manifestfile a in dateList)
            {
                Debug.WriteLine(a.contentnumber + " " + a.date.ToShortDateString());
            }*/
            return new ManifestOpenResult(DateHashSet, dateList);
        }


        private Dictionary<string, Dictionary<string, Dictionary<string, int>>> ProductsMasterTotalsHash;
        private Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawMasterTotalsDepotHash;

        Type TruckManifestSelector(MultiRecordEngine engine, string record)
        {


            if (record.StartsWith("D1"))
            {
                return typeof(D1DrawTotals);
            }
            else if (record.StartsWith("R1"))
            {
                return typeof(R1RouteDetail);
            }
            else if (record.StartsWith("R2"))
            {
                return typeof(R2AdvanceSectionDetail);
            }
            else if (record.StartsWith("R3"))
            {
                return typeof(R3TMProductDetail);
            }
            else if (record.StartsWith("T1"))
            {
                return typeof(T1TruckTotals);
            }
            else if (record.StartsWith("P1"))
            {
                return typeof(P1PreviousDrawTotals);
            }
            else if (record.StartsWith("P2"))
            {
                return typeof(P2PrevDrawTotalsByType);
            }
            else if (record.StartsWith("W1"))
            {
                return typeof(W1TruckWeightDetail);
            }
            else if (record.StartsWith("T2"))
            {
                return typeof(T2AdvanceSectionTruckTotals);
            }
            else if (record.StartsWith("T3"))
            {
                return typeof(T3TMProductTotals);
            }
            else if (record.StartsWith("A1"))
            {
                return typeof(A1TruckTimeData);
            }
            else if (record.StartsWith("S1"))
            {
                return typeof(S1SubscriptionData);
            }
            else
                Debug.WriteLine("WARNING: Unknown Data found in file: " + record);

            return null; // will cause multirecord engine to record/throw an exception.
        }

        public void writeToDisk(string filename, string content)
        {
            try
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\" + filename);
            }
            catch (System.Exception o_aException)
            {
                Debug.WriteLine("Exception deleting file.");
                Debug.WriteLine(o_aException.ToString());
            }

            try
            {
                FileStream o_fs_aFileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\" + filename, FileMode.OpenOrCreate);
                StreamWriter o_streamWriter_aWriter = new StreamWriter(o_fs_aFileStream);
                o_streamWriter_aWriter.WriteLine(content);
                o_streamWriter_aWriter.Close();

            }
            catch (System.Exception o_aException)
            {
                Response.Write("Exception writing file.");
                Response.Write(o_aException.ToString());
            }



        }

        public void LinkButton_Command(Object sender, CommandEventArgs e)
        {
            correctRadioButtonVisiblity();
            LinkedList<object> lastCSV = (LinkedList<object>)Session["lastCSV"];

            String title = (String)Session["RBtext"];
            if (title == "" || title == null)
            {
                ((Label)FindControl("Label2")).Text = "<font color='red'>A radio button must be selected.</font>";
                return;
            }
            else if (lastCSV.First == null && lastCSV.Last == null)
            {
                ((Label)FindControl("Label2")).Text = "CSV Data Length is zero. You might have requested a report for which there is no data file available.</br>Data files need to be placed in the following location: " + fileslocation;
                return;
            }


            bool color = false;
            if (title.Contains("Color")) color = true;

            removeR2s = (List<string>)Session["removeR2s"];
            parsedatafile(getDateSelected().DayOfWeek.ToString(), false, lastCSV);


            if (removeR2s == null)
            {
                removeR2s = new List<string>();
            }


            foreach (string r in removeR2s)
            {
                Debug.Write(r + " ");
            }


            ProductsMasterTotalsHash = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
            ProductsDrawMasterTotalsDepotHash = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>();

            var ManifestTemplate = new MasterManifestT4Template() { };
            var DistributionTemplate = new MasterDistributionT4Template() { };
            var DispatchTemplate = new MasterDispatchT4Template() { };
            var MasterPalletSheetTemplate = new MasterPalletSheetT4Template() { };
            var MasterDepotTemplate = new MasterDepotT4Template()
            {
                ProductsDrawMasterTotalsDepotHash = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>(),
            };

            var PalletTemplate = new MasterPalletT4Template()
            {
                MasterTruckDictionary = new StringDictionary(),
                ProductsMasterTotalsHash = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>(),
            };


            string report = "";
            int lineCountTotal = 0;
            string reportType = e.CommandArgument.ToString();
            switch (reportType)
            {
                case "Depot":

                    foreach (KeyValuePair<string, Truck> kvp in TruckHash)
                    {
                        Truck currentTruck = kvp.Value;
                        report += currentTruck.GetReport("Depot", ProductsMasterTotalsHash, ProductsDrawMasterTotalsDepotHash, new StringDictionary(), ref removeR2s, title, ref ProductsDrawTotalsCarrierHash260, ref DTI_CTPAKMAN_BundleSizes);
                    }
                    MasterDepotTemplate.report = report;
                    MasterDepotTemplate.ProductsDrawMasterTotalsDepotHash = ProductsDrawMasterTotalsDepotHash;

                    writeToDisk(reportType + ".html", MasterDepotTemplate.TransformText());
                    Response.Redirect(reportType + ".html", false);
                    break;
                case "Debug":
                    foreach (KeyValuePair<string, Truck> kvp in TruckHash)
                    {
                        Truck currentTruck = kvp.Value;
                        report += currentTruck.GetDebug();
                    }
                    writeToDisk(reportType + ".html", report);
                    Response.Redirect(reportType + ".html", false);
                    break;

                case "Pallet":
                    string RunDate = "";
                    foreach (KeyValuePair<string, Truck> kvp in TruckHash)
                    {


                        Truck currentTruck = kvp.Value;
                        if (lineCountTotal == 0)
                        {
                            currentTruck.setUseHeader();
                        }

                        if ((Int32.Parse(currentTruck.ShortTruckID) >= currentTruck.lowrange && Int32.Parse(currentTruck.ShortTruckID) <= currentTruck.highrange) || color)
                        {
                            report += currentTruck.GetReport("Pallet", ProductsMasterTotalsHash, ProductsDrawMasterTotalsDepotHash, new StringDictionary(), ref removeR2s, title, ref ProductsDrawTotalsCarrierHash260, ref DTI_CTPAKMAN_BundleSizes);
                            lineCountTotal += currentTruck.linecounter;
                            RunDate = currentTruck.RunDate.ToLongDateString();
                        }

                        if (lineCountTotal > 10)
                        {
                            lineCountTotal = 0;
                        }

                    }
                    PalletTemplate.report = report;
                    PalletTemplate.MasterTruckDictionary.Add("RunDate", RunDate);
                    PalletTemplate.MasterTruckDictionary.Add("title", title);

                    PalletTemplate.ProductsMasterTotalsHash = this.ProductsMasterTotalsHash;
                    writeToDisk(reportType + ".html", PalletTemplate.TransformText());
                    Response.Redirect(reportType + ".html", false);
                    break;

                case "AltDispatch":
                case "Dispatch":
                    // DISPATCH



                    foreach (KeyValuePair<string, Truck> kvp in TruckHash)
                    {


                        Truck currentTruck = kvp.Value;
                        //   Debug.WriteLine (currentTruck.headerRecord.RouteID);

                        if (lineCountTotal == 0)
                        {
                            currentTruck.setUseHeader();
                        }
                        report += currentTruck.GetReport(reportType, ProductsMasterTotalsHash, ProductsDrawMasterTotalsDepotHash, new StringDictionary(), ref removeR2s, title, ref ProductsDrawTotalsCarrierHash260, ref DTI_CTPAKMAN_BundleSizes);
                        lineCountTotal += currentTruck.linecounter;

                        if (lineCountTotal > 10)
                        {
                            report += "<tr style='break-after:always;'></tr>";
                            lineCountTotal = 0;
                        }
                    }
                    DispatchTemplate.report = report;
                    DispatchTemplate.ProductsMasterTotalsHash = this.ProductsMasterTotalsHash;
                    DispatchTemplate.altprod = (reportType == "AltDispatch");
                    writeToDisk(reportType + ".html", DispatchTemplate.TransformText());
                    Response.Redirect(reportType + ".html", false);


                    break;

                case "AltDistribution":

                    foreach (KeyValuePair<string, Truck> kvp in TruckHash)
                    {


                        Truck currentTruck = kvp.Value;

                        if (lineCountTotal == 0)
                        {
                            currentTruck.setUseHeader();
                        }
                        report += currentTruck.GetReport(reportType, ProductsMasterTotalsHash, ProductsDrawMasterTotalsDepotHash, new StringDictionary(), ref removeR2s, title, ref ProductsDrawTotalsCarrierHash260, ref DTI_CTPAKMAN_BundleSizes);
                        lineCountTotal += currentTruck.linecounter;

                        if (lineCountTotal >= 20)
                        {
                            report += "<tr style='break-after:always;'></tr>";
                            lineCountTotal = 0;
                        }
                    }
                    DistributionTemplate.report = report;
                    DistributionTemplate.ProductsMasterTotalsHash = this.ProductsMasterTotalsHash;
                    DistributionTemplate.altprod = (reportType == "AltDistribution");
                    writeToDisk(reportType + ".html", DistributionTemplate.TransformText());
                    Response.Redirect(reportType + ".html", false);


                    break;

                case "PalletSheet":
                    // PALLET SHEET
                    int i = 0;
                    foreach (KeyValuePair<string, Truck> kvp in TruckHash)
                    {
                        Truck currentTruck = kvp.Value;

                        if ((Int32.Parse(currentTruck.ShortTruckID) >= currentTruck.lowrange && Int32.Parse(currentTruck.ShortTruckID) <= currentTruck.highrange) || color)
                        {
                            i++;
                            StringDictionary TruckDictionary = new StringDictionary();
                            TruckDictionary.Add("Seq", i.ToString());

                            report += currentTruck.GetReport("PalletSheets", ProductsMasterTotalsHash, ProductsDrawMasterTotalsDepotHash, TruckDictionary, ref removeR2s, title, ref ProductsDrawTotalsCarrierHash260, ref DTI_CTPAKMAN_BundleSizes);
                        }
                    }
                    MasterPalletSheetTemplate.report = report;
                    writeToDisk(reportType + ".html", MasterPalletSheetTemplate.TransformText());

                    Response.Redirect(reportType + ".html", false);
                    break;
                case "Manifest":

                    // MANIFEST
                    int dropdowntoselection = Int32.Parse(dropdownlistTo.SelectedValue);
                    int dropdownfromselection = Int32.Parse(dropdownlistFrom.SelectedValue);

                    if (dropdowntoselection >= dropdownfromselection)
                    {
                        foreach (KeyValuePair<string, Truck> kvp in TruckHash)
                        {
                            Truck currentTruck = kvp.Value;
                            int ShortTruckID = Int32.Parse(kvp.Value.ShortTruckID);
                            if (ShortTruckID >= dropdownfromselection && ShortTruckID <= dropdowntoselection)
                            {
                                report += currentTruck.GetReport("Manifest", ProductsMasterTotalsHash, ProductsDrawMasterTotalsDepotHash, new StringDictionary(), ref removeR2s, title, ref ProductsDrawTotalsCarrierHash260, ref DTI_CTPAKMAN_BundleSizes);
                            }
                        }
                        ManifestTemplate.report = report;
                        writeToDisk(reportType + ".html", ManifestTemplate.TransformText());
                        Response.Redirect(reportType + ".html", false);
                    }
                    else
                    {
                        ((Label)FindControl("Label2")).Text = "To dropdown value must not be less than from dropdown value.";
                    }
                    break;

            }

        }
    }
}

