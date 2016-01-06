namespace truck_manifest
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using T4Templates;

	public class WholeTruck
	{
        
        //////////////////////////////////////////////////////////////////////////////////////
        // INJUNCTION BUNDLE SIZES TABLE. 
        //
        // A T T E N T I O N: MANY BUNDLE SIZES ARE OVERRIDEN USING THIS TABLE.
        //
        // DTI NEWS SHITTLE SOLUTIONS BUNDLE SIZES: SOME ARE NOT DELIEVERED IN THE TRUCK EXPORT FILES.
        // SINCE THE TRUCK EXPORT FILE FORMAT CANNOT BE ALTERED WE HAVE TO GET THE SIZES FOR THESE PRODUCTS ELSWHERE.
        Dictionary<string, int> bundleInjunction;

		// INPUT
		public List<A1TruckTimeData> A1TruckTimeDataList;
		public List<S1SubscriptionData> S1SubscriptionDataList;
		public Dictionary<string, List<R1RouteDetail>> R1RouteDetailDictionary;
		public Dictionary<string, List<D1DrawTotals>> D1DrawTotalsDictionaryUnfiltered;
		public List<P1PreviousDrawTotals> P1PreviousDrawTotalsList;
		public List<P2PrevDrawTotalsByType> P2PrevDrawTotalsByTypeList;
		public Dictionary<string, List<R2AdvanceSectionDetail>> R2AdvanceSectionDetailDictionaryUnfiltered;
		public Dictionary<string, List<R3TMProductDetail>> R3TMProductDetailDictionary;
		public List<W1TruckWeightDetail> W1TruckWeightDetailList;
		public List<T1TruckTotals> T1TruckTotalsList;
		public List<T2AdvanceSectionTruckTotals> T2AdvanceSectionTruckTotalsList;
		public List<T3TMProductTotals> T3TMProductTotalsList;

		// INPUT THEN ARRANGED.
		private string TruckID;
		private string TruckName;
		private Dictionary<string, List<D1DrawTotals>> D1DrawTotalsDictionary;
		private Dictionary<string, List<R2AdvanceSectionDetail>> R2AdvanceSectionDetailDictionary;
		private Dictionary<string, List<R2AdvanceSectionDetail>> R2AdvanceSectionDetailDictionaryBad;
		private SortedDictionary<string, locationtranslation> translationHash;

		// OUTPUT TO TRUCK LEVEL.
		public DateTime RunDate;
		public int numberofdrops;
		public int altnumberofdrops;
		private int lowrange;
		private int highrange;
		public Dictionary <string, Dictionary<string, TotalTypes>> ProductsDrawTotalsHash;	
		public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawTotalsDepotHash;
        
   
        public int truckhashid;
       
		public WholeTruck (string TruckID, string TruckName, int lowrange, int highrange, SortedDictionary<string, locationtranslation> translationHash, int truckhashid)
		{
            this.truckhashid = truckhashid;
			this.lowrange = lowrange;
			this.highrange = highrange; 
			this.TruckID = TruckID;
			this.TruckName = TruckName;
			this.translationHash = translationHash;

			A1TruckTimeDataList = new List<A1TruckTimeData> (); 
			S1SubscriptionDataList = new List<S1SubscriptionData> (); 
			R1RouteDetailDictionary = new Dictionary<string, List<R1RouteDetail>> ();

			D1DrawTotalsDictionary = new Dictionary<string, List<D1DrawTotals>> (); 
			D1DrawTotalsDictionaryUnfiltered = new Dictionary<string, List<D1DrawTotals>> (); 
			P1PreviousDrawTotalsList = new List<P1PreviousDrawTotals> (); 
			P2PrevDrawTotalsByTypeList = new List<P2PrevDrawTotalsByType> (); 
			R2AdvanceSectionDetailDictionaryUnfiltered = new Dictionary<string, List<R2AdvanceSectionDetail>> ();
			R2AdvanceSectionDetailDictionary = new Dictionary<string, List<R2AdvanceSectionDetail>> (); 
			R2AdvanceSectionDetailDictionaryBad = new Dictionary<string, List<R2AdvanceSectionDetail>> ();
			R3TMProductDetailDictionary = new Dictionary<string, List<R3TMProductDetail>> (); 
			W1TruckWeightDetailList = new List<W1TruckWeightDetail> (); 
			T1TruckTotalsList = new List<T1TruckTotals> (); 
			T2AdvanceSectionTruckTotalsList = new List<T2AdvanceSectionTruckTotals> (); 
			T3TMProductTotalsList = new List<T3TMProductTotals> (); 



			this.ProductsDrawTotalsHash = new Dictionary <string, Dictionary<string, TotalTypes>> ();
			this.ProductsDrawTotalsDepotHash = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> (); 
		}



        public void calculateProductsby(string type, ref List<string> removeR2s, ref Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawTotalsCarrierHash)
        {
            Dictionary<string, Dictionary<string, pakman>> dummy = new Dictionary<string, Dictionary<string, pakman>>();

            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> R2AdvanceSectionDetailSequences;
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> DrawTotalsHashByProduct;
            Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>> DrawTotalsHashByProductNoRouteID;
            Dictionary<string, Dictionary<string, TotalTypes>> DrawTotalsHashByProductTotals;
            Dictionary<string, Dictionary<string, TotalTypes>> DrawTotalsHashByProductDrawTypeTotals;
            CommonCalculate(removeR2s, out R2AdvanceSectionDetailSequences, out DrawTotalsHashByProduct, out DrawTotalsHashByProductNoRouteID, out DrawTotalsHashByProductTotals, out DrawTotalsHashByProductDrawTypeTotals, ref dummy);
            string id = "";
            foreach (KeyValuePair<string, List<R1RouteDetail>> R1RouteDetailListPair in R1RouteDetailDictionary)
            {
                
                
                List<R1RouteDetail> recordList = R1RouteDetailListPair.Value;
                this.RunDate = recordList[0].RunDate;
                int truckid;
                foreach (R1RouteDetail product in recordList)
                {
                    truckid = Int32.Parse(product.TruckID);
                    if (type == "carrier")
                    {
                        id = product.CarrierID;

                    }
                    else
                    {
                        id = product.RouteID;
                    }



                    // R1 records will exist that do not have d1 records to go along with them. These are empty records showing zero draw or are r1s whos d1s were 
                    // filtered out, we're supposed to ignore those as part of the filtering process. They are not required, but 
                    // we need to watch out for them because they will cause exception
                    if (DrawTotalsHashByProductTotals.ContainsKey(product.ProductID) &&
                        DrawTotalsHashByProductNoRouteID.ContainsKey(product.TruckDropOrder) &&
                        DrawTotalsHashByProductNoRouteID[product.TruckDropOrder].ContainsKey(product.ProductID))
                    {
                        // CARRIER -- FOR COLOR PREPRINTS
                        if (!ProductsDrawTotalsCarrierHash.ContainsKey(id))
                        {
                            var carrierlevel = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
                            ProductsDrawTotalsCarrierHash.Add(id, carrierlevel);
                        }

                        // PRODUCT ID
                        if (!ProductsDrawTotalsCarrierHash[id].ContainsKey(product.ProductID))
                        {
                            var ProductsHash = new Dictionary<string, Dictionary<string, int>>();
                            ProductsDrawTotalsCarrierHash[id].Add(product.ProductID, ProductsHash);
                        }
                        // PAPER SECTIONS
                        if (R2AdvanceSectionDetailSequences.ContainsKey(product.TruckDropOrder) && R2AdvanceSectionDetailSequences[product.TruckDropOrder].ContainsKey(product.RouteID) && R2AdvanceSectionDetailSequences[product.TruckDropOrder][product.RouteID].ContainsKey(product.ProductID))
                        {
                            foreach (KeyValuePair<string, TotalTypes> PaperSection in R2AdvanceSectionDetailSequences[product.TruckDropOrder][product.RouteID][product.ProductID])
                            {
                                if (DrawTotalsHashByProductNoRouteID[product.TruckDropOrder][product.ProductID].ContainsKey(PaperSection.Key))
                                {
                                 string Key = PaperSection.Key;
                                 int draw = PaperSection.Value.Draw;
                            if (!ProductsDrawTotalsCarrierHash[id][product.ProductID].ContainsKey(Key))
                            {
                                var ProductsCalcTotalsHash = new Dictionary<string, int>();
                                ProductsCalcTotalsHash.Add("Draw", 0);
                                ProductsCalcTotalsHash.Add("SingleCopyDraw", 0);
                                ProductsCalcTotalsHash.Add("HomeDeliveryDraw", 0);
                                ProductsCalcTotalsHash.Add("HomeDeliveryNumberOfDrops", 0);
                                ProductsCalcTotalsHash.Add("SingleCopyNumberOfDrops", 0);
                                ProductsCalcTotalsHash.Add("StandardBundleSize", Int32.Parse(product.StandardBundleSize));
                                ProductsCalcTotalsHash.Add("Updraw", draw);
                                ProductsDrawTotalsCarrierHash[id][product.ProductID].Add(Key, ProductsCalcTotalsHash);
                            }
                            ProductsDrawTotalsCarrierHash[id][product.ProductID][Key]["Draw"] += Int32.Parse(product.DrawTotal);

                                }
                                else
                                {
                                    Debug.WriteLine("0 " + product.TruckID + " " + product.TruckDropOrder + " " + product.ProductID);
                                }
                            }
                        }
                        else if (DrawTotalsHashByProductNoRouteID[product.TruckDropOrder][product.ProductID].ContainsKey("none"))
                        {

                            string Key = "none";
                            int draw = 0;
                            if (!ProductsDrawTotalsCarrierHash[id][product.ProductID].ContainsKey(Key))
                            {
                                var ProductsCalcTotalsHash = new Dictionary<string, int>();
                                ProductsCalcTotalsHash.Add("Draw", 0);
                                ProductsCalcTotalsHash.Add("SingleCopyDraw", 0);
                                ProductsCalcTotalsHash.Add("HomeDeliveryDraw", 0);
                                ProductsCalcTotalsHash.Add("HomeDeliveryNumberOfDrops", 0);
                                ProductsCalcTotalsHash.Add("SingleCopyNumberOfDrops", 0);
                                ProductsCalcTotalsHash.Add("StandardBundleSize", Int32.Parse(product.StandardBundleSize));
                                ProductsCalcTotalsHash.Add("Updraw", draw);
                                ProductsDrawTotalsCarrierHash[id][product.ProductID].Add(Key, ProductsCalcTotalsHash);
                            }
                            ProductsDrawTotalsCarrierHash[id][product.ProductID][Key]["Draw"] += Int32.Parse(product.DrawTotal);

                        
                        }
                        else
                        {
                            Debug.WriteLine("0 " + product.TruckID + " " + product.TruckDropOrder + " " + product.ProductID);
                        }

                    }
                    else
                    {
                        //	Console.WriteLine (ObjectDumper.Dump (product));
                    }
                }
            }
        }
       

        public string GetReport(string reportType, ref List<string> removeR2s, string title, ref Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawTotalsCarrierHash260, ref Dictionary<string, Dictionary<string, pakman>> DTI_CTPAKMAN_BundleSizes)
		{
            bundleInjunction  = new Dictionary<string, int>
    {
	{"260", 60},
	{"261", 10},
	{"770", 60},
	{"660", 60},
	{"661", 10},
	{"200", 60},
	{"600", 60},
	{"700", 60},
    {"800", 60},
	{"900", 60},
    {"602", 25},
    {"702", 25},
    {"802", 25},
    {"902", 25}
    };




            int tallyColor = 0;
            if (title.Contains("Color"))
            {
                tallyColor = 1;
            } else if (title.Contains("Late Main")){
                tallyColor = 2;

            }




			var ManifestTemplate = new WholeTruckManifestT4Template () {
				TruckDictionary = new StringDictionary (),
				DrawTotalsHashByProductTotals = new Dictionary <string, Dictionary<string, TotalTypes>> (),
				DrawTotalsHashByProduct = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> (),
				DrawTotalsHashByProductNoRouteID = new Dictionary<string, Dictionary<string, Dictionary<string,TotalTypes>>> (),
			};

			ManifestTemplate.TruckDictionary.Add ("TruckID", TruckID);
			ManifestTemplate.TruckDictionary.Add ("TruckName", TruckName);

			if (translationHash.ContainsKey (TruckID)) {
				ManifestTemplate.TruckDictionary.Add ("DeliveryArea", translationHash [TruckID].DeliveryArea);
				ManifestTemplate.TruckDictionary.Add ("GenInstructions", translationHash [TruckID].GenInstructions);
				ManifestTemplate.TruckDictionary.Add ("SpecInstructions", translationHash [TruckID].SpecInstructions);

			} else {
				ManifestTemplate.TruckDictionary.Add ("DeliveryArea", "No Information For This Truck ID");
				ManifestTemplate.TruckDictionary.Add ("GenInstructions", "");
				ManifestTemplate.TruckDictionary.Add ("SpecInstructions", "");
			}
		


			numberofdrops = 0;
			altnumberofdrops = 0;

            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> R2AdvanceSectionDetailSequences;
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> DrawTotalsHashByProduct;
            Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>> DrawTotalsHashByProductNoRouteID;
            Dictionary<string, Dictionary<string, TotalTypes>> DrawTotalsHashByProductTotals;
            Dictionary<string, Dictionary<string, TotalTypes>> DrawTotalsHashByProductDrawTypeTotals;
            CommonCalculate(removeR2s, out R2AdvanceSectionDetailSequences, out DrawTotalsHashByProduct, out DrawTotalsHashByProductNoRouteID, out DrawTotalsHashByProductTotals, out DrawTotalsHashByProductDrawTypeTotals, ref DTI_CTPAKMAN_BundleSizes);
            
			////////////////////////////// R1 RECORDS totals.
			/// 
			/// ORGANIZE BY THE FOLLOWING
			/// 
			//////////////////////////////           sequence, dummy number
			var ProductsDrawSeqHash = new Dictionary<string, int> ();
			////////////////////////////// R1 RECORDS totals.
			/// 
			/// ORGANIZE BY THE FOLLOWING
			/// 
			//////////////////////////////           sequence, dummy number
			var AltProductsDrawSeqHash = new Dictionary<string, int> ();

			/// 
			/// ORGANIZE BY THE FOLLOWING
			/// 
			//////////////////////////////              product            paper section      draw type
			var ProductsDrawTotalsHash = new Dictionary<string, Dictionary<string, Dictionary<string, int>>> ();
			/// 
			/// ORGANIZE BY THE FOLLOWING
			/// 
			//////////////////////////////          depot    product            paper section      draw type
			var ProductsDrawTotalsDepotHash = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ();

            /// 
            /// ORGANIZE BY THE FOLLOWING
            /// 
            //////////////////////////////          carrier    product            paper section      draw type


			foreach (KeyValuePair<string, List<R1RouteDetail>> R1RouteDetailListPair in R1RouteDetailDictionary) {
				List<R1RouteDetail> recordList = R1RouteDetailListPair.Value;
				this.RunDate = recordList [0].RunDate;
				int truckid;
				foreach (R1RouteDetail product in recordList) {
					truckid = Int32.Parse (product.TruckID);
				
					// R1 records will exist that do not have d1 records to go along with them. These are empty records showing zero draw or are r1s whos d1s were 
					// filtered out, we're supposed to ignore those as part of the filtering process. They are not required, but 
					// we need to watch out for them because they will cause exception
					if (DrawTotalsHashByProductTotals.ContainsKey (product.ProductID) &&
					    DrawTotalsHashByProductNoRouteID.ContainsKey (product.TruckDropOrder) &&
					    DrawTotalsHashByProductNoRouteID [product.TruckDropOrder].ContainsKey (product.ProductID)) {


						// DEPOT -- For depot reports.

						if ((truckid >= lowrange && truckid <= highrange) && (!ProductsDrawTotalsDepotHash.ContainsKey (product.DepotID))) {
							var depotlevel = new Dictionary<string, Dictionary<string, Dictionary<string, int>>> ();
							ProductsDrawTotalsDepotHash.Add (product.DepotID, depotlevel);
						}

						// SEQUENCE
						// FOR COUNT
						if (!ProductsDrawSeqHash.ContainsKey (product.TruckDropOrder)) {
							ProductsDrawSeqHash.Add (product.TruckDropOrder, 0);
						}

						// SEQUENCE
						// FOR COUNT
						if (!AltProductsDrawSeqHash.ContainsKey (product.TruckDropOrder)) {
							if (product.ProductID != "PPG") {
								AltProductsDrawSeqHash.Add (product.TruckDropOrder, 0);
							}
						}

						// PRODUCT ID
						if (!ProductsDrawTotalsHash.ContainsKey (product.ProductID)) {
							var ProductsHash = new Dictionary<string,Dictionary<string, int>> ();
							ProductsDrawTotalsHash.Add (product.ProductID, ProductsHash);
						}






                        int stdbund = Int32.Parse(product.StandardBundleSize);
                       
                        string package = "package--";

                        // WARNING: SOME PAPERSECTIONS SUCH AS COLOR PREPRINTS USE R1 RECORDS OF THE SAME PRODUCT ID, BUT HAVE DIFFERENT BUNDLE SIZES
                        foreach (KeyValuePair<string, TotalTypes> papersection in DrawTotalsHashByProductTotals[product.ProductID])
                        {

                            
                            // WARNING: SOME PAPERSECTIONS SUCH AS R3 RECORD GENERATED PAPERSECTIONS WILL ALREADY HAVE BUNDLESIZES WHICH WE DON'T WANT TO OVERWRITE.
                            if (papersection.Value.StandardBundleSize == 0 && !papersection.Value.DoNotOverWriteStandardBundleSize)
                            {
                                papersection.Value.StandardBundleSize = stdbund;
                            }

                            // BUNDLE SIZE OVERRIDE TABLE.
                            // BUNDER SIZE OVERRIDE IS FOR HARD CODED OR USER SET BUNDLE SIZES IN THE UI (TO DO). WE WILL OVERWRITE THESE HERE.
                            if (bundleInjunction.ContainsKey(papersection.Key))
                            {
                                papersection.Value.StandardBundleSize = bundleInjunction[papersection.Key];

                            }

                        }


                        // WARNING: SOME PAPERSECTIONS SUCH AS COLOR PREPRINTS USE R1 RECORDS OF THE SAME PRODUCT ID (PPG), BUT HAVE DIFFERENT BUNDLE SIZES
                        if (product.InsertMixCombination.Contains(package))
                        {

                            
                            string prodcode = product.InsertMixCombination.Remove(product.InsertMixCombination.IndexOf(package), package.Length);
                           

                            foreach (KeyValuePair<string, TotalTypes> papersection in DrawTotalsHashByProductTotals[product.ProductID])
                            {
                                // R1 RECORDS specifying their product codes in insertmix: their bundle sizes are ultimately superior.
                                if (papersection.Key == prodcode && !papersection.Value.DoNotOverWriteStandardBundleSize)
                                {
                                    papersection.Value.StandardBundleSize = stdbund;
                                    papersection.Value.DoNotOverWriteStandardBundleSize = true;
                                }
                            }
                        }
                        

						if ((truckid >= lowrange && truckid <= highrange) && (!ProductsDrawTotalsDepotHash [product.DepotID].ContainsKey (product.ProductID))) {
							//

							var ProductsHash = new Dictionary<string,Dictionary<string, int>> ();
							ProductsDrawTotalsDepotHash [product.DepotID].Add (product.ProductID, ProductsHash);
						} 

						
							
						//
						// PAPER SECTIONS
						//

						if (R2AdvanceSectionDetailSequences.ContainsKey (product.TruckDropOrder) && R2AdvanceSectionDetailSequences [product.TruckDropOrder].ContainsKey (product.RouteID) && R2AdvanceSectionDetailSequences [product.TruckDropOrder] [product.RouteID].ContainsKey (product.ProductID)) {
							foreach (KeyValuePair<string,TotalTypes> PaperSection in R2AdvanceSectionDetailSequences[product.TruckDropOrder] [product.RouteID][product.ProductID]) {
								if (DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID].ContainsKey (PaperSection.Key)) {
                                    createpapersections(PaperSection.Key, PaperSection.Value.Draw, product, DrawTotalsHashByProductNoRouteID, ProductsDrawTotalsHash, ProductsDrawSeqHash.Count, ProductsDrawTotalsDepotHash, truckid, tallyColor, ref ProductsDrawTotalsCarrierHash260, ref DrawTotalsHashByProductTotals, ref DrawTotalsHashByProduct, ref DrawTotalsHashByProductDrawTypeTotals);
								} else { 
									Debug.WriteLine ("-0 " + product.TruckID + " " + product.TruckDropOrder + " " + product.ProductID);
								}
							}
						} else if (DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID].ContainsKey ("none")) {
                            createpapersections("none", 0, product, DrawTotalsHashByProductNoRouteID, ProductsDrawTotalsHash, ProductsDrawSeqHash.Count, ProductsDrawTotalsDepotHash, truckid, tallyColor, ref ProductsDrawTotalsCarrierHash260, ref DrawTotalsHashByProductTotals, ref DrawTotalsHashByProduct, ref DrawTotalsHashByProductDrawTypeTotals);
						} else { 
							Debug.WriteLine ("+0 " + product.TruckID + " " + product.TruckDropOrder + " " + product.ProductID);
						}

                            // WE NEED TO CORRECT AN ISSUE ON COLOR MANIFESTS HERE.
                            // COLOR MANIFESTS CONTAIN LETTER PREFIXED ROUTES, THESE LETTER PREFIXED ROUTES HAVE DRAW ON THEM THAT IS ACTUALLY SUPPOSED TO BE
                            // ON THE 0 ROUTE (A ROUTE WITH THE SAME NUMBERS AFTER THE FIRST CHARACTER). WE NEED TO CORRECT THIS.
                            // 0 and J cannot but used to identify what to combine, because these fields are not limited. The user can put whatever they want in the field.
                            // Instead we will use carrier ids.






					} else {
					//	Console.WriteLine (ObjectDumper.Dump (product));
					}
				} 
			}
			// Make sure we have something to display, if not, return.
		/*	if (ProductsDrawSeqHash.Count == 0) {
				return "";
			}*/
            foreach (KeyValuePair<string, Dictionary<string, TotalTypes>> prod in DrawTotalsHashByProductTotals)
            {
                foreach (KeyValuePair<string, TotalTypes> entry in prod.Value)
                {
                }
            }





			// CALCULATE BUNDLES AND ODDS 
			foreach (KeyValuePair<string, Dictionary<string, TotalTypes>> prod in DrawTotalsHashByProductTotals) {
				foreach (KeyValuePair<string, TotalTypes> entry in prod.Value) {
					if ((entry.Value.Draw >= entry.Value.StandardBundleSize) && (entry.Value.StandardBundleSize > 0)) {
						entry.Value.Bundles = entry.Value.Draw / entry.Value.StandardBundleSize;
						entry.Value.Odds = entry.Value.Draw % entry.Value.StandardBundleSize;
					} else {
						entry.Value.Bundles = 0;
						entry.Value.Odds = entry.Value.Draw;
					}
					if (ProductsDrawTotalsHash.ContainsKey (prod.Key) && ProductsDrawTotalsHash [prod.Key].ContainsKey (entry.Key)) {
						entry.Value.NumberOfDrops += ProductsDrawTotalsHash [prod.Key] [entry.Key] ["NumberOfDrops"];
						entry.Value.SingleCopyNumberOfDrops += ProductsDrawTotalsHash [prod.Key] [entry.Key] ["SingleCopyNumberOfDrops"];
						entry.Value.HomeDeliveryNumberOfDrops += ProductsDrawTotalsHash [prod.Key] [entry.Key] ["HomeDeliveryNumberOfDrops"];
						entry.Value.SingleCopyDraw += ProductsDrawTotalsHash [prod.Key] [entry.Key] ["SingleCopyDraw"];
						entry.Value.HomeDeliveryDraw += ProductsDrawTotalsHash [prod.Key] [entry.Key] ["HomeDeliveryDraw"];
					}
				}
			}
			this.ProductsDrawTotalsHash = DrawTotalsHashByProductTotals;
			this.ProductsDrawTotalsDepotHash = ProductsDrawTotalsDepotHash;
            ManifestTemplate.TruckDictionary.Add ("title", title);
			ManifestTemplate.DrawTotalsHashByProductTotals = DrawTotalsHashByProductTotals;
			ManifestTemplate.DrawTotalsHashByProduct = DrawTotalsHashByProduct;
			ManifestTemplate.DrawTotalsHashByProductNoRouteID = DrawTotalsHashByProductNoRouteID;
            ManifestTemplate.TruckDictionary.Add ("RunDate", this.RunDate.ToLongDateString ());




			ManifestTemplate.TruckDictionary.Add ("TotalNumberOfDrops", ProductsDrawSeqHash.Count.ToString ());
			numberofdrops += ProductsDrawSeqHash.Count;
			altnumberofdrops += AltProductsDrawSeqHash.Count;
			//Console.WriteLine (ManifestTemplate.TruckDictionary["TruckID"].Insert(3,"-")+" "+ProductsDrawSeqHash.Count.ToString ());

      
                return ManifestTemplate.TransformText ();
		}

        private void CommonCalculate(List<string> removeR2s, out Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> R2AdvanceSectionDetailSequences, out Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> DrawTotalsHashByProduct, out Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>> DrawTotalsHashByProductNoRouteID, out Dictionary<string, Dictionary<string, TotalTypes>> DrawTotalsHashByProductTotals, out Dictionary<string, Dictionary<string, TotalTypes>> DrawTotalsHashByProductDrawTypeTotals, ref Dictionary<string, Dictionary<string, pakman>> DTI_CTPAKMAN_BundleSizes)
        {


            if (removeR2s.Count > 0)
            {

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// F I L T E R I N G 
                /// 
                // SCRUB UNWANTED RECORDS FROM UNFILTERED DICTIONARIES. THIS IS BASED ON THE FILTER SET BY THE RADIO BUTTONS.

                // Go Through R2s ARRANGE GOOD LIST AND BAD LIST, BAD LIST IS TO DELETE TRASH RECORDS FROM D1s and R1s.
                foreach (KeyValuePair<string, List<R2AdvanceSectionDetail>> R2AdvanceSectionDetailDictionaryPair in R2AdvanceSectionDetailDictionaryUnfiltered)
                {
                    List<R2AdvanceSectionDetail> recordList = R2AdvanceSectionDetailDictionaryPair.Value;
                    // LOOP THROUGH ALL R2 RECORDS
                    foreach (R2AdvanceSectionDetail product in recordList)
                    {
                        // Check Paper Section Against Delete R2 list. If it is in the list we must add it to the list of bad r2s, if its not in the list it goes in the list of good r2s.


                        if (removeR2s.Contains(product.PaperSection))
                        {
                            //Debug.WriteLine ("bad found:" + product.PaperSection);
                            if (!R2AdvanceSectionDetailDictionaryBad.ContainsKey(product.TruckDropOrder))
                            {
                                R2AdvanceSectionDetailDictionaryBad.Add(product.TruckDropOrder, new List<R2AdvanceSectionDetail>());
                            }
                            R2AdvanceSectionDetailDictionaryBad[product.TruckDropOrder].Add(product);
                        }
                        else
                        {
                            //Debug.WriteLine ("good found:" + product.PaperSection);
                            if (!R2AdvanceSectionDetailDictionary.ContainsKey(product.TruckDropOrder))
                            {
                                R2AdvanceSectionDetailDictionary.Add(product.TruckDropOrder, new List<R2AdvanceSectionDetail>());
                            }

                            R2AdvanceSectionDetailDictionary[product.TruckDropOrder].Add(product);
                        }

                    }
                }

                /*
            if( Int32.Parse(TruckID.Substring (0, 3)) > 300 && Int32.Parse(TruckID.Substring (0, 3)) < 400 ){

                Console.WriteLine (TruckID);
            Console.WriteLine ("Good");
            foreach (KeyValuePair<string,List<R2AdvanceSectionDetail>> t in R2AdvanceSectionDetailDictionary) {
                foreach (R2AdvanceSectionDetail a in t.Value) {
                Console.Write (a.PaperSection + " ");
                }
            }
            Console.WriteLine ("");
            Console.WriteLine ("Bad");

            foreach (KeyValuePair<string,List<R2AdvanceSectionDetail>> t in R2AdvanceSectionDetailDictionaryBad) {
                foreach (R2AdvanceSectionDetail a in t.Value) {
                    Console.Write (a.PaperSection + " ");
                }
            }
            Console.WriteLine ("");
            }*/

                ////////////////////////////// R2 RECORDS BAD // ARRANGE TO ALLOW DELETION OF D1s and R1s that are trash.
                ///
                /// ORGANIZE BY THE FOLLOWING
                /// 
                //////////////////////////////                 sequence           route id                 product            Paper Section    total types (Draw / bundles / etc)
                var R2AdvanceSectionDetailSequencesBad = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>>();

                foreach (KeyValuePair<string, List<R2AdvanceSectionDetail>> R2AdvanceSectionDetailDictionaryPair in R2AdvanceSectionDetailDictionaryBad)
                {
                    List<R2AdvanceSectionDetail> recordList = R2AdvanceSectionDetailDictionaryPair.Value;
                    string Sequence = R2AdvanceSectionDetailDictionaryPair.Key;
                    // SEQUENCE
                    if (!R2AdvanceSectionDetailSequencesBad.ContainsKey(Sequence))
                    {
                        var SequenceHash = new Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>();
                        R2AdvanceSectionDetailSequencesBad.Add(Sequence, SequenceHash);
                    }

                    // LOOP THROUGH ALL R2 RECORDS
                    foreach (R2AdvanceSectionDetail product in recordList)
                    {
                        // ROUTE ID 
                        if (!R2AdvanceSectionDetailSequencesBad[Sequence].ContainsKey(product.RouteID))
                        {
                            var ProductIDHash = new Dictionary<string, Dictionary<string, TotalTypes>>();
                            R2AdvanceSectionDetailSequencesBad[Sequence].Add(product.RouteID, ProductIDHash);
                        }

                        // PRODUCT ID
                        if (!R2AdvanceSectionDetailSequencesBad[Sequence][product.RouteID].ContainsKey(product.MainProductID))
                        {
                            var ProductDrawTypeHash = new Dictionary<string, TotalTypes>();
                            R2AdvanceSectionDetailSequencesBad[Sequence][product.RouteID].Add(product.MainProductID, ProductDrawTypeHash);
                        }

                        // PAPER SECTION ID
                        if (!R2AdvanceSectionDetailSequencesBad[Sequence][product.RouteID][product.MainProductID].ContainsKey(product.PaperSection))
                        {
                            TotalTypes PaperSectionTotalTypes = new TotalTypes();
                            R2AdvanceSectionDetailSequencesBad[Sequence][product.RouteID][product.MainProductID].Add(product.PaperSection, PaperSectionTotalTypes);
                        }

                    }
                }

                R2AdvanceSectionDetailSequences = R2sort();
                //////////////////////////////////
                // Go Through D1s D1DrawTotalsDictionaryUnfiltered & CROSS OVER WITH R2AdvanceSectionDetailSequencesBad to create D1DrawTotalsDictionary
                foreach (KeyValuePair<string, List<D1DrawTotals>> D1DrawTotalsDictionaryPair in D1DrawTotalsDictionaryUnfiltered)
                {
                    List<D1DrawTotals> recordList = D1DrawTotalsDictionaryPair.Value;
                    string Sequence = D1DrawTotalsDictionaryPair.Key;
                    // LOOP THROUGH ALL D1 RECORDS
                    foreach (D1DrawTotals product in recordList)
                    {


                        // IF THIS HAS AN ENTRY IN THE LIST OF BAD R2S...
                        if (R2AdvanceSectionDetailSequencesBad.ContainsKey(product.TruckDropOrder) && R2AdvanceSectionDetailSequencesBad[product.TruckDropOrder].ContainsKey(product.RouteID) && R2AdvanceSectionDetailSequencesBad[product.TruckDropOrder][product.RouteID].ContainsKey(product.ProductID))
                        {
                            // IF IT HAS AND ENTRY IN THE LIST OF GOOD R2s...
                            if (R2AdvanceSectionDetailSequences.ContainsKey(product.TruckDropOrder) && R2AdvanceSectionDetailSequences[product.TruckDropOrder].ContainsKey(product.RouteID) && R2AdvanceSectionDetailSequences[product.TruckDropOrder][product.RouteID].ContainsKey(product.ProductID))
                            {
                                //Debug.WriteLine ("Good D1 found:" + ObjectDumper.Dump (product));
                                if (!D1DrawTotalsDictionary.ContainsKey(product.TruckDropOrder))
                                {
                                    D1DrawTotalsDictionary.Add(product.TruckDropOrder, new List<D1DrawTotals>());
                                }
                                D1DrawTotalsDictionary[product.TruckDropOrder].Add(product);

                            }
                            else
                            {
                                // IT DOES NOT HAVE AN ENTRY IN THE GOOD R2s.
                                // KICK IT OUT.
                                //Debug.WriteLine ("Filtered out D1 found:" + ObjectDumper.Dump (product));
                            }

                        }
                        else
                        {
                            // IT DOES NOT HAVE AN ENTRY IN THE LIST OF BAD R2s.
                            //Debug.WriteLine ("Good D1 found:" + ObjectDumper.Dump (product));
                            if (!D1DrawTotalsDictionary.ContainsKey(product.TruckDropOrder))
                            {
                                D1DrawTotalsDictionary.Add(product.TruckDropOrder, new List<D1DrawTotals>());
                            }
                            D1DrawTotalsDictionary[product.TruckDropOrder].Add(product);


                        }

                    }

                }

                /// END FILTERING, R1s ARE KICKED OUT LATER AUTOMATICALLY BECAUSE THEY DON'T HAVE D1s anymore.
            }
            else
            {
                // NO FILTER CLOUTURE.
                R2AdvanceSectionDetailDictionary = R2AdvanceSectionDetailDictionaryUnfiltered;
                D1DrawTotalsDictionary = D1DrawTotalsDictionaryUnfiltered;
                R2AdvanceSectionDetailSequences = R2sort();
            }
            /// CRAP
            ///  
            /*

            if (R2AdvanceSectionDetailSequencesBad.ContainsKey (product.TruckDropOrder) && R2AdvanceSectionDetailSequencesBad [product.TruckDropOrder].ContainsKey (product.RouteID) && R2AdvanceSectionDetailSequencesBad [product.TruckDropOrder] [product.RouteID].ContainsKey (product.ProductID)) {
                //	Console.WriteLine ("Filtered out D1 found:" + ObjectDumper.Dump (product));

            } else {

                if (R2AdvanceSectionDetailSequencesBad.ContainsKey (product.TruckDropOrder) && R2AdvanceSectionDetailSequencesBad [product.TruckDropOrder].ContainsKey ("*") && R2AdvanceSectionDetailSequencesBad [product.TruckDropOrder] ["*"].ContainsKey (product.ProductID)) {
                                //Console.WriteLine ("Filtered out starred D1 found:" + ObjectDumper.Dump (product));
                            } else {
                //Console.WriteLine ("Good D1 found:" + ObjectDumper.Dump (product));
                if (!D1DrawTotalsDictionary.ContainsKey (product.TruckDropOrder)) {
                    D1DrawTotalsDictionary.Add (product.TruckDropOrder, new List<D1DrawTotals> ());
                }
                D1DrawTotalsDictionary [product.TruckDropOrder].Add (product);
                }
        }
        */


            ////////////////////////////// D1 RECORDS totals. // WE WILL SET A FOUNDATION OF THE R3 RECORDS, WHICH MUST BE INCLUDED NOW BECAUSE THEY ACCOUNT FOR EVERYTHING THEY REQUIRE.
            ////////////////////////////// 
            /// 
            /// ORGANIZE BY THE FOLLOWING
            /// 
            //////////////////////////////                 sequence           route id          paper section     product  total types
            DrawTotalsHashByProduct = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>>();
            ////////////////////////////// 
            /// 
            /// ORGANIZE BY THE FOLLOWING
            /// 
            //////////////////////////////                        sequence           product           paper section  total types
            DrawTotalsHashByProductNoRouteID = new Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>();
            ////////////////////////////// 
            /// 
            /// ORGANIZE BY THE FOLLOWING
            /// 
            //////////////////////////////                      product          paper section   total types
            DrawTotalsHashByProductTotals = new Dictionary<string, Dictionary<string, TotalTypes>>();
            ////////////////////////////// 
            /// ORGANIZE BY THE FOLLOWING
            ///   
            //////////////////////////////                      product            draw type         total types (Draw / bundles / etc)
            DrawTotalsHashByProductDrawTypeTotals = new Dictionary<string, Dictionary<string, TotalTypes>>();
            loadDrawTotalsR3(R3TMProductDetailDictionary, R2AdvanceSectionDetailSequences, ref DrawTotalsHashByProduct, ref DrawTotalsHashByProductNoRouteID, ref  DrawTotalsHashByProductTotals, ref DrawTotalsHashByProductDrawTypeTotals);
            loadDrawTotals(D1DrawTotalsDictionary, R2AdvanceSectionDetailSequences, ref DrawTotalsHashByProduct, ref DrawTotalsHashByProductNoRouteID, ref  DrawTotalsHashByProductTotals, ref DrawTotalsHashByProductDrawTypeTotals, ref DTI_CTPAKMAN_BundleSizes);
        
        }


        
		/// THIS FUNCTION LOADS UP THE DRAW TOTALS FROM SOURCES R3 RECORDS
		void loadDrawTotalsR3 (Dictionary<string, List<R3TMProductDetail>> inputDictionary, 
			Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> R2AdvanceSectionDetailSequences, 
			ref Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> DrawTotalsHashByProduct,
			ref Dictionary<string, Dictionary <string, Dictionary<string,TotalTypes>>> DrawTotalsHashByProductNoRouteID,
			ref Dictionary<string, Dictionary<string, TotalTypes>>  DrawTotalsHashByProductTotals, 
			ref Dictionary<string, Dictionary<string, TotalTypes>>  DrawTotalsHashByProductDrawTypeTotals){

			////////////////////////////////////////////
			// R3 RECORDS
			foreach (KeyValuePair<string, List<R3TMProductDetail>> DictionaryPair in inputDictionary) {
				List<R3TMProductDetail> recordList = DictionaryPair.Value;
				string Sequence = DictionaryPair.Key;

				// SEQUENCE

				if (!DrawTotalsHashByProduct.ContainsKey (Sequence)) {
					var SequenceHash = new Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>> ();
					DrawTotalsHashByProduct.Add (Sequence, SequenceHash);
				}

				if (!DrawTotalsHashByProductNoRouteID.ContainsKey (Sequence)) {
					var SequenceHash = new Dictionary<string, Dictionary<string, TotalTypes>> ();
					DrawTotalsHashByProductNoRouteID.Add (Sequence, SequenceHash);
				}
				// LOOP THROUGH ALL R3 RECORDS
				foreach (R3TMProductDetail product in recordList) {

					// Totals, so not by sequence.
					// Draw drilled down to product with disregard for sequence and routeid (totals).
					if (!DrawTotalsHashByProductTotals.ContainsKey (product.ProductID)) {
						var ProductsCalcTotalsHash = new Dictionary<string, TotalTypes> ();
						DrawTotalsHashByProductTotals.Add (product.ProductID, ProductsCalcTotalsHash);
					}

					// Draw drilled down to product with drawtype, with disregard for sequence and routeid (totals).
					if (!DrawTotalsHashByProductDrawTypeTotals.ContainsKey (product.ProductID)) {
						var DrawTypeHash = new Dictionary<string, TotalTypes> ();
						DrawTotalsHashByProductDrawTypeTotals.Add (product.ProductID, DrawTypeHash);

					}


					// ROUTE ID 

					if (!DrawTotalsHashByProduct [Sequence].ContainsKey (product.RouteID)) {
						var ProductIDHash = new Dictionary<string, Dictionary<string,TotalTypes>> ();
						DrawTotalsHashByProduct [Sequence].Add (product.RouteID, ProductIDHash);
					}

					// PRODUCT ID

					if (!DrawTotalsHashByProduct [Sequence] [product.RouteID].ContainsKey (product.ProductID)) {
						var ProductsCalcTotalsHash = new Dictionary <string, TotalTypes> ();
						DrawTotalsHashByProduct [Sequence] [product.RouteID].Add (product.ProductID, ProductsCalcTotalsHash);
					}

					if (!DrawTotalsHashByProductNoRouteID [Sequence].ContainsKey (product.ProductID)) {
                        var ProductsCalcTotalsHash = new Dictionary<string, TotalTypes>(StringComparer.InvariantCultureIgnoreCase);
						DrawTotalsHashByProductNoRouteID [Sequence].Add (product.ProductID, ProductsCalcTotalsHash);
					}
					// PAPER SECTION
					if (!DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID].ContainsKey (product.TMProductID)) {
						TotalTypes tt = new TotalTypes ();
						DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID].Add (product.TMProductID, tt);
					}
					DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID] [product.TMProductID].Draw += Int32.Parse (product.TMDrawTotal);

					if (!DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID].ContainsKey (product.TMProductID)) {
						TotalTypes tt = new TotalTypes ();
						DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID].Add (product.TMProductID, tt);
					}
					// Draw drilled down to product with disregard for route id.
					DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID] [product.TMProductID].Draw += Int32.Parse (product.TMDrawTotal);

					if (!DrawTotalsHashByProductTotals [product.ProductID].ContainsKey (product.TMProductID)) {
						TotalTypes ProductsCalcTotalsHash = new TotalTypes ();
						DrawTotalsHashByProductTotals [product.ProductID].Add (product.TMProductID, ProductsCalcTotalsHash);
					}

					DrawTotalsHashByProductTotals [product.ProductID] [product.TMProductID].Draw += Int32.Parse (product.TMDrawTotal);
					DrawTotalsHashByProductTotals [product.ProductID] [product.TMProductID].StandardBundleSize = Int32.Parse (product.StandardBundleSize);

					// DRAW TYPE

					if (!DrawTotalsHashByProductDrawTypeTotals [product.ProductID].ContainsKey ("None")) {
						TotalTypes ProductsCalcTotalsHash = new TotalTypes ();
						DrawTotalsHashByProductDrawTypeTotals [product.ProductID].Add ("None", ProductsCalcTotalsHash);
					}
					// Draw drilled down to draw type.
					DrawTotalsHashByProductDrawTypeTotals [product.ProductID] ["None"].Draw += Int32.Parse (product.TMDrawTotal);

				}
			}

		}

		/// THIS FUNCTION LOADS UP THE DRAW TOTALS FROM SOURCES D1 RECORDS
		void loadDrawTotals (Dictionary<string, List<D1DrawTotals>> inputDictionary, 
			Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> R2AdvanceSectionDetailSequences,
			ref Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> DrawTotalsHashByProduct,
			ref Dictionary<string, Dictionary <string, Dictionary<string,TotalTypes>>> DrawTotalsHashByProductNoRouteID,
			ref Dictionary<string, Dictionary<string, TotalTypes>>  DrawTotalsHashByProductTotals, 
			ref Dictionary<string, Dictionary<string, TotalTypes>>  DrawTotalsHashByProductDrawTypeTotals, ref Dictionary<string,Dictionary<string,pakman>> DTI_CTPAKMAN_BundleSizes){




			////////////////////////////////////////////
			// D1 RECORDS

			foreach (KeyValuePair<string, List<D1DrawTotals>> D1DrawTotalsDictionaryPair in D1DrawTotalsDictionary) {
				List<D1DrawTotals> recordList = D1DrawTotalsDictionaryPair.Value;
				string Sequence = D1DrawTotalsDictionaryPair.Key;

				// SEQUENCE


				if (!DrawTotalsHashByProduct.ContainsKey (Sequence)) {
					var SequenceHash = new Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>> ();
					DrawTotalsHashByProduct.Add (Sequence, SequenceHash);
				}

				if (!DrawTotalsHashByProductNoRouteID.ContainsKey (Sequence)) {
					var SequenceHash = new Dictionary<string, Dictionary<string, TotalTypes>> ();
					DrawTotalsHashByProductNoRouteID.Add (Sequence, SequenceHash);
				}
				// LOOP THROUGH ALL D1 RECORDS
				foreach (D1DrawTotals product in recordList) {

					// Totals, so not by sequence.
					// Draw drilled down to product with disregard for sequence and routeid (totals).
					if (!DrawTotalsHashByProductTotals.ContainsKey (product.ProductID)) {
						var ProductsCalcTotalsHash = new Dictionary<string, TotalTypes> ();
						DrawTotalsHashByProductTotals.Add (product.ProductID, ProductsCalcTotalsHash);
					}

					// Draw drilled down to product with drawtype, with disregard for sequence and routeid (totals).
					if (!DrawTotalsHashByProductDrawTypeTotals.ContainsKey (product.ProductID)) {
						var DrawTypeHash = new Dictionary<string, TotalTypes> ();
						DrawTotalsHashByProductDrawTypeTotals.Add (product.ProductID, DrawTypeHash);

					}


					// ROUTE ID 
					if (!DrawTotalsHashByProduct [Sequence].ContainsKey (product.RouteID)) {
						var ProductIDHash = new Dictionary<string, Dictionary<string,TotalTypes>> ();
						DrawTotalsHashByProduct [Sequence].Add (product.RouteID, ProductIDHash);
					}

					// PRODUCT ID

					if (!DrawTotalsHashByProduct [Sequence] [product.RouteID].ContainsKey (product.ProductID)) {
						var ProductsCalcTotalsHash = new Dictionary <string, TotalTypes> ();
						DrawTotalsHashByProduct [Sequence] [product.RouteID].Add (product.ProductID, ProductsCalcTotalsHash);
					}

					if (!DrawTotalsHashByProductNoRouteID [Sequence].ContainsKey (product.ProductID)) {
						var ProductsCalcTotalsHash = new Dictionary<string, TotalTypes> (StringComparer.InvariantCultureIgnoreCase);
						DrawTotalsHashByProductNoRouteID [Sequence].Add (product.ProductID, ProductsCalcTotalsHash);
					}
					// PAPER SECTION
                   

                        
                    
                  
                    if (DTI_CTPAKMAN_BundleSizes.ContainsKey(product.RouteID) && product.ProductID=="PPG")
                    {
                        foreach (KeyValuePair<string, pakman> PaperSection in DTI_CTPAKMAN_BundleSizes[product.RouteID]){
                        // IT LOOKS LIKE THERE ARE PACKAGES ON THIS. 
                        if (!DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID].ContainsKey (PaperSection.Key)) {
								TotalTypes tt = new TotalTypes ();
								DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID].Add (PaperSection.Key, tt);
							}
                        DrawTotalsHashByProduct[Sequence][product.RouteID][product.ProductID][PaperSection.Key].StandardBundleSize = PaperSection.Value.bundlesize;


                     		DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID] [PaperSection.Key].Draw += Int32.Parse (product.DrawTotal);

							if (!DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID].ContainsKey (PaperSection.Key.ToString())) {
								TotalTypes tt = new TotalTypes ();
                                DrawTotalsHashByProductNoRouteID[Sequence][product.ProductID].Add(PaperSection.Key.ToString(), tt);
                                
                    		}

							// Draw drilled down to product with disregard for route id.
				            DrawTotalsHashByProductNoRouteID[Sequence][product.ProductID][PaperSection.Key].StandardBundleSize = PaperSection.Value.bundlesize;

							if (!DrawTotalsHashByProductTotals [product.ProductID].ContainsKey (PaperSection.Key)) {
								TotalTypes ProductsCalcTotalsHash = new TotalTypes ();
								DrawTotalsHashByProductTotals [product.ProductID].Add (PaperSection.Key, ProductsCalcTotalsHash);
							}

				            DrawTotalsHashByProductTotals[product.ProductID][PaperSection.Key].StandardBundleSize = PaperSection.Value.bundlesize;
                            DrawTotalsHashByProductTotals[product.ProductID][PaperSection.Key].DoNotOverWriteStandardBundleSize = true;
                    }

                    }
                    
                    if (R2AdvanceSectionDetailSequences.ContainsKey (product.TruckDropOrder) && R2AdvanceSectionDetailSequences [product.TruckDropOrder].ContainsKey (product.RouteID) && R2AdvanceSectionDetailSequences [product.TruckDropOrder] [product.RouteID].ContainsKey (product.ProductID)) {
						foreach (KeyValuePair<string,TotalTypes> PaperSection in R2AdvanceSectionDetailSequences[product.TruckDropOrder] [product.RouteID][product.ProductID]) {
            
                            if (!DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID].ContainsKey (PaperSection.Key)) {
								TotalTypes tt = new TotalTypes ();
								DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID].Add (PaperSection.Key, tt);
							}
							DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID] [PaperSection.Key].Draw += Int32.Parse (product.DrawTotal);

							if (!DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID].ContainsKey (PaperSection.Key.ToString())) {
								TotalTypes tt = new TotalTypes ();
								DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID].Add (PaperSection.Key.ToString(), tt);
                            }
							// Draw drilled down to product with disregard for route id.
							DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID] [PaperSection.Key].Draw += Int32.Parse (product.DrawTotal);
            
							if (!DrawTotalsHashByProductTotals [product.ProductID].ContainsKey (PaperSection.Key)) {
								TotalTypes ProductsCalcTotalsHash = new TotalTypes ();
								DrawTotalsHashByProductTotals [product.ProductID].Add (PaperSection.Key, ProductsCalcTotalsHash);
							}

							DrawTotalsHashByProductTotals [product.ProductID] [PaperSection.Key].Draw += Int32.Parse (product.DrawTotal);
						}
                    }
                    else if (R2AdvanceSectionDetailSequences.ContainsKey (product.TruckDropOrder) && R2AdvanceSectionDetailSequences [product.TruckDropOrder].ContainsKey ("*") && R2AdvanceSectionDetailSequences [product.TruckDropOrder] ["*"].ContainsKey (product.ProductID)) {
						foreach (KeyValuePair<string,TotalTypes> PaperSection in R2AdvanceSectionDetailSequences[product.TruckDropOrder] ["*"][product.ProductID]) {
            				if (!DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID].ContainsKey (PaperSection.Key)) {
								TotalTypes tt = new TotalTypes ();
								DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID].Add (PaperSection.Key, tt);
							}
							DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID] [PaperSection.Key].Draw += Int32.Parse (product.DrawTotal);


							if (!DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID].ContainsKey (PaperSection.Key)) {
								TotalTypes tt = new TotalTypes ();
								DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID].Add (PaperSection.Key, tt);
							}
							// Draw drilled down to product with disregard for route id.
							DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID] [PaperSection.Key].Draw += Int32.Parse (product.DrawTotal);

							if (!DrawTotalsHashByProductTotals [product.ProductID].ContainsKey (PaperSection.Key)) {
								TotalTypes ProductsCalcTotalsHash = new TotalTypes ();
								DrawTotalsHashByProductTotals [product.ProductID].Add (PaperSection.Key, ProductsCalcTotalsHash);
							}

							DrawTotalsHashByProductTotals [product.ProductID] [PaperSection.Key].Draw += Int32.Parse (product.DrawTotal);
						}
					} else {
						if (!DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID].ContainsKey ("none")) {
							TotalTypes tt = new TotalTypes ();
							DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID].Add ("none", tt);
						}
						// Draw drilled down to product with disregard for route id.
						DrawTotalsHashByProduct [Sequence] [product.RouteID] [product.ProductID] ["none"].Draw += Int32.Parse (product.DrawTotal);


						if (!DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID].ContainsKey ("none")) {
							TotalTypes tt = new TotalTypes ();
							DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID].Add ("none", tt);
						}
						// Draw drilled down to product with disregard for route id.
						DrawTotalsHashByProductNoRouteID [Sequence] [product.ProductID] ["none"].Draw += Int32.Parse (product.DrawTotal);

						if (!DrawTotalsHashByProductTotals [product.ProductID].ContainsKey ("none")) {
							TotalTypes ProductsCalcTotalsHash = new TotalTypes ();
							DrawTotalsHashByProductTotals [product.ProductID].Add ("none", ProductsCalcTotalsHash);
						}

						DrawTotalsHashByProductTotals [product.ProductID] ["none"].Draw += Int32.Parse (product.DrawTotal);
					}


					// DRAW TYPE

					if (!DrawTotalsHashByProductDrawTypeTotals [product.ProductID].ContainsKey (product.DrawType)) {
						TotalTypes ProductsCalcTotalsHash = new TotalTypes ();
						DrawTotalsHashByProductDrawTypeTotals [product.ProductID].Add (product.DrawType, ProductsCalcTotalsHash);
					}
					// Draw drilled down to draw type.
					DrawTotalsHashByProductDrawTypeTotals [product.ProductID] [product.DrawType].Draw += Int32.Parse (product.DrawTotal);

				}
			}

		}

        			

        void createpapersections(string Key, int draw, R1RouteDetail product, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>> DrawTotalsHashByProductNoRouteID, Dictionary<string, Dictionary<string, Dictionary<string, int>>> ProductsDrawTotalsHash, int count, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawTotalsDepotHash, int truckid, int tallyColor, ref Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawTotalsCarrierHash260, ref Dictionary<string, Dictionary<string, TotalTypes>> DrawTotalsHashByProductTotals, ref Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> DrawTotalsHashByProduct, ref Dictionary<string, Dictionary<string, TotalTypes>> DrawTotalsHashByProductDrawTypeTotals)
		{

            Int32 bundleSizeForKey = DrawTotalsHashByProductTotals[product.ProductID][Key].StandardBundleSize;
			if (!ProductsDrawTotalsHash [product.ProductID].ContainsKey (Key)) {
				var ProductsCalcTotalsHash = new Dictionary<string, int> ();
				ProductsCalcTotalsHash.Add ("Draw", 0);
				ProductsCalcTotalsHash.Add ("NumberOfDrops", 0);
				ProductsCalcTotalsHash.Add ("SingleCopyDraw", 0);
				ProductsCalcTotalsHash.Add ("HomeDeliveryDraw", 0);
				ProductsCalcTotalsHash.Add ("HomeDeliveryNumberOfDrops", 0);
				ProductsCalcTotalsHash.Add ("SingleCopyNumberOfDrops", 0);
                ProductsCalcTotalsHash.Add("StandardBundleSize", bundleSizeForKey);
				ProductsCalcTotalsHash.Add ("Updraw", draw);
				ProductsDrawTotalsHash [product.ProductID].Add (Key, ProductsCalcTotalsHash);
			}
			/*if (Key == "200" && product.ProductID == "PPG")
				Console.WriteLine ("currently:" + ProductsDrawTotalsHash [product.ProductID] [Key] ["Draw"] + " adding on:" + product.DrawTotal+ " truckID " +product.TruckID + " seq: " + product.TruckDropOrder + " routeid: " + product.RouteID );

			ProductsDrawTotalsHash [product.ProductID] [Key] ["Draw"] += Int32.Parse (product.DrawTotal);
			if (Key == "200" && product.ProductID == "PPG")
				Console.WriteLine (ProductsDrawTotalsHash [product.ProductID] [Key] ["Draw"]);
*/

			if (truckid >= lowrange && truckid <= highrange) {
				if (!ProductsDrawTotalsDepotHash [product.DepotID] [product.ProductID].ContainsKey (Key)) {
					var ProductsCalcTotalsHash = new Dictionary<string, int> ();
					ProductsCalcTotalsHash.Add ("Draw", 0);
					ProductsCalcTotalsHash.Add ("SingleCopyDraw", 0);
					ProductsCalcTotalsHash.Add ("HomeDeliveryDraw", 0);
					ProductsCalcTotalsHash.Add ("HomeDeliveryNumberOfDrops", 0);
					ProductsCalcTotalsHash.Add ("SingleCopyNumberOfDrops", 0);
                    ProductsCalcTotalsHash.Add("StandardBundleSize", bundleSizeForKey);
					ProductsCalcTotalsHash.Add ("Updraw", draw);
					ProductsDrawTotalsDepotHash [product.DepotID] [product.ProductID].Add (Key, ProductsCalcTotalsHash);
				}
				ProductsDrawTotalsDepotHash [product.DepotID] [product.ProductID] [Key] ["Draw"] += Int32.Parse (product.DrawTotal);

			}

			if (product.RouteTypeIndicator == "SC") {
				ProductsDrawTotalsHash [product.ProductID] [Key] ["SingleCopyNumberOfDrops"]++;
				ProductsDrawTotalsHash [product.ProductID] [Key] ["SingleCopyDraw"] += Int32.Parse (product.DrawTotal);
			} else if (product.RouteTypeIndicator == "HD") {
				ProductsDrawTotalsHash [product.ProductID] [Key] ["HomeDeliveryNumberOfDrops"]++;
				ProductsDrawTotalsHash [product.ProductID] [Key] ["HomeDeliveryDraw"] += Int32.Parse (product.DrawTotal);
			}
			ProductsDrawTotalsHash [product.ProductID] [Key] ["NumberOfDrops"]++;

            // THIS IS WHERE WE WILL HANDLE COLOR MANIFEST BULLSHIT, GIANT HEADACHE. THE DRAW FROM 260 NEEDS ADDDED ON TO THE DRAW FOR THE COLOR FILE USING CARRIER ID AS THE COMMON LINK.
            // WE HAVE TO AUGMENT ALL DRAW TOTALS AFTER THE TOTALING. IT'S STUPID, BUT IT SEEMED TO BE THE MOST EASY WAY TO IMPLIMENT IT SINCE D1s DO NOT KNOW THEIR CARRIER IDS.
            //
            // IDENTIFY IF THIS R1 RECORD IS ONE WHICH INDICATES A NEED TO ALTER DRAW USING 260

           
            /*
            ref Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> DrawTotalsHashByProduct,
			ref Dictionary<string, Dictionary <string, Dictionary<string,TotalTypes>>> DrawTotalsHashByProductNoRouteID,
			ref Dictionary<string, Dictionary<string, TotalTypes>>  DrawTotalsHashByProductTotals, 
			ref Dictionary<string, Dictionary<string, TotalTypes>>  DrawTotalsHashByProductDrawTypeTotals)
            
                         //////////////////////////////                 sequence           route id          paper section     product  total types
            DrawTotalsHashByProduct = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>>();
            ////////////////////////////// 
            /// 
            /// ORGANIZE BY THE FOLLOWING
            /// 
            //////////////////////////////                        sequence           product           paper section  total types
            DrawTotalsHashByProductNoRouteID = new Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>();
            ////////////////////////////// 
            /// 
            /// ORGANIZE BY THE FOLLOWING
            /// 
            //////////////////////////////                      product          paper section   total types
            DrawTotalsHashByProductTotals = new Dictionary<string, Dictionary<string, TotalTypes>>();
            
             */

            // COLOR 
            if (tallyColor == 1 && product.RouteID.Substring(0, 3) != "999" && product.MainProductID == "PPG" && ProductsDrawTotalsCarrierHash260.ContainsKey(product.CarrierID) && ProductsDrawTotalsCarrierHash260[product.CarrierID].ContainsKey(product.MainProductID) && ProductsDrawTotalsCarrierHash260[product.CarrierID][product.MainProductID].ContainsKey("260"))
            {
               // Debug.WriteLine(product.DropLocation + " " + product.CarrierID + " " + product.DrawTotal + " " + DrawTotalsHashByProductNoRouteID[product.TruckDropOrder][product.ProductID][Key].Draw + " " + ProductsDrawTotalsCarrierHash260[product.CarrierID][product.MainProductID]["260"]["Draw"]);

               // Debug.WriteLine(DrawTotalsHashByProductNoRouteID[product.TruckDropOrder][product.ProductID][Key].Draw);
               DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Draw += ProductsDrawTotalsCarrierHash260[product.CarrierID][product.MainProductID]["260"]["Draw"];
               DrawTotalsHashByProductTotals[product.ProductID][Key].Draw += ProductsDrawTotalsCarrierHash260[product.CarrierID][product.MainProductID]["260"]["Draw"];
                
               
               // Debug.WriteLine(DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Draw);
            }
            
            if (tallyColor == 2 && product.ProductID == "PPG" && Key == "770" && ProductsDrawTotalsCarrierHash260.ContainsKey(product.RouteID) && ProductsDrawTotalsCarrierHash260[product.RouteID].ContainsKey(product.ProductID) && ProductsDrawTotalsCarrierHash260[product.RouteID][product.ProductID].ContainsKey("770"))
            
            {
                Debug.WriteLine("subtracting");
                Debug.WriteLine(product.DropLocation + " " + product.RouteID + " " + product.DrawTotal + " " + DrawTotalsHashByProductNoRouteID[product.TruckDropOrder][product.ProductID][Key].Draw + " " + ProductsDrawTotalsCarrierHash260[product.RouteID][product.ProductID]["770"]["Draw"]);

               // Debug.WriteLine(DrawTotalsHashByProductNoRouteID[product.TruckDropOrder][product.ProductID][Key].Draw);
                DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Draw -= ProductsDrawTotalsCarrierHash260[product.RouteID][product.ProductID]["770"]["Draw"];
                DrawTotalsHashByProductTotals[product.ProductID][Key].Draw -= ProductsDrawTotalsCarrierHash260[product.RouteID][product.ProductID]["770"]["Draw"];
                
               
                Debug.WriteLine(DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Draw);
            }



            
           
			if (product.RouteID.Length == 8) {
				DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].RouteID = product.RouteID.Insert (4, "-");
			} else {
				DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].RouteID = product.RouteID;
			}

			DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].DistributionFor = "--";
			DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Prod = product.ProductID;
            DrawTotalsHashByProductNoRouteID[product.TruckDropOrder][product.ProductID][Key].StandardBundleSize = bundleSizeForKey;
			DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].numberinlist = count;

			int StandardBundleSize = DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].StandardBundleSize;
			int Draw = DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Draw;
			// recalc bundles and odds because R1 records are not correct.

			if ((Draw >= StandardBundleSize) && (StandardBundleSize > 0)) {
				DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Bundles = Draw / StandardBundleSize;
				DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Odds = Draw % StandardBundleSize;
			} else {
				DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Bundles = 0;
				DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Odds = Draw;
			}
			//Console.WriteLine (product.TruckID + " " + product.TruckDropOrder + " " + product.RouteID + " " + product.DropLocation+ " " + Key);
			DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].RouteType = product.RouteType;
            DrawTotalsHashByProductNoRouteID[product.TruckDropOrder][product.ProductID][Key].CarrierID = product.CarrierID;
			DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Name = product.CarrierName;
			DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Address = product.DropLocation;
			DrawTotalsHashByProductNoRouteID [product.TruckDropOrder] [product.ProductID] [Key].Instructions = product.DropInstructions;

        }




		private Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> R2sort ()
		{ 

			////////////////////////////// R2 RECORDS. What sequences have special products.
			///
			/// ORGANIZE BY THE FOLLOWING
			/// 
			//////////////////////////////                 sequence           route id                 product            Paper Section    total types (Draw / bundles / etc)
			var R2AdvanceSectionDetailSequences = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>>> ();

			foreach (KeyValuePair<string, List<R2AdvanceSectionDetail>>R2AdvanceSectionDetailDictionaryPair in R2AdvanceSectionDetailDictionary) {
				List<R2AdvanceSectionDetail> recordList = R2AdvanceSectionDetailDictionaryPair.Value;
				string Sequence = R2AdvanceSectionDetailDictionaryPair.Key;
				// SEQUENCE
				if (!R2AdvanceSectionDetailSequences.ContainsKey (Sequence)) {
					var SequenceHash = new Dictionary<string, Dictionary<string, Dictionary<string, TotalTypes>>> ();
					R2AdvanceSectionDetailSequences.Add (Sequence, SequenceHash);
				}

				// LOOP THROUGH ALL R2 RECORDS
				foreach (R2AdvanceSectionDetail product in recordList) {
					// ROUTE ID 
					if (!R2AdvanceSectionDetailSequences [Sequence].ContainsKey (product.RouteID)) {
						var ProductIDHash = new Dictionary<string, Dictionary<string, TotalTypes>> ();
						R2AdvanceSectionDetailSequences [Sequence].Add (product.RouteID, ProductIDHash);
					}

					// PRODUCT ID
					if (!R2AdvanceSectionDetailSequences [Sequence] [product.RouteID].ContainsKey (product.MainProductID)) {
						var ProductDrawTypeHash = new Dictionary<string,TotalTypes> ();
						R2AdvanceSectionDetailSequences [Sequence] [product.RouteID].Add (product.MainProductID, ProductDrawTypeHash);
					}

					// PAPER SECTION ID
					if (!R2AdvanceSectionDetailSequences [Sequence] [product.RouteID] [product.MainProductID].ContainsKey (product.PaperSection)) {
						TotalTypes PaperSectionTotalTypes = new TotalTypes ();
						R2AdvanceSectionDetailSequences [Sequence] [product.RouteID] [product.MainProductID].Add (product.PaperSection, PaperSectionTotalTypes);
					}

					R2AdvanceSectionDetailSequences [Sequence] [product.RouteID] [product.MainProductID] [product.PaperSection].Draw += Int32.Parse (product.Updraw);
				}
			}	
			return R2AdvanceSectionDetailSequences;
		}
		public string GetDebug ()
		{
			System.Text.StringBuilder text = new System.Text.StringBuilder ();
			/*
			text.Append ("</br>");
			text.Append ("</br>");
			text.Append ("</br>");

			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small; font-family: san-serif;\">");
			foreach( A1TruckTimeData record in A1TruckTimeDataList)
			{
				String dump = ObjectDumper.Dump(record);
				text.Append ("<tr>"+dump+"</tr>");
			}
			text.Append("</table>");
			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small;\">");
			foreach( S1SubscriptionData record in S1SubscriptionDataList)
			{
				String dump = ObjectDumper.Dump(record);
				text.Append ("<tr>"+dump+"</tr>");
			}
			text.Append("</table>");
			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small;\">");
			foreach( D1DrawTotals record in D1DrawTotalsList)
			{
				String dump = ObjectDumper.Dump(record);
				text.Append ("<tr>"+dump+"</tr>");
			}
			text.Append("</table>");
			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small;\">");
			foreach( P1PreviousDrawTotals record in P1PreviousDrawTotalsList)
			{
				String dump = ObjectDumper.Dump(record);
				text.Append ("<tr>"+dump+"</tr>");
			}
			text.Append("</table>");
			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small;\">");
			foreach( P2PrevDrawTotalsByType record in P2PrevDrawTotalsByTypeList)
			{
				String dump = ObjectDumper.Dump(record);
				text.Append ("<tr>"+dump+"</tr>");
			}
			text.Append("</table>");*/
			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small;\">");
			foreach (KeyValuePair<string, List<R1RouteDetail>> R1RouteDetailListPair in R1RouteDetailDictionary) {
				List<R1RouteDetail> recordList = R1RouteDetailListPair.Value;
				foreach (R1RouteDetail product in recordList) {

					String dump = ObjectDumper.Dump (product);
					text.Append ("<tr>" + dump + "</tr>");
				}
			}
			text.Append ("</table>");
			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small;\">");/*
			foreach( R2AdvanceSectionDetail record in R2AdvanceSectionDetailList)
			{
				String dump = ObjectDumper.Dump(record);
				text.Append ("<tr>"+dump+"</tr>");
			}
			text.Append("</table>");
			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small;\">");
			foreach( R3TMProductDetail record in R3TMProductDetailList)
			{
				String dump = ObjectDumper.Dump(record);
				text.Append ("<tr>"+dump+"</tr>");
			}
			text.Append("</table>");
			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small;\">");
			foreach( W1TruckWeightDetail record in  W1TruckWeightDetailList)
			{
				String dump = ObjectDumper.Dump(record);
				text.Append ("<tr>"+dump+"</tr>");
			}

			text.Append("</table>");
			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small;\">");
			foreach( T1TruckTotals record in T1TruckTotalsList)
			{
				String dump = ObjectDumper.Dump(record);
				text.Append ("<tr>"+dump+"</tr>");
			}
			text.Append("</table>");
			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small;\">");
			foreach( T2AdvanceSectionTruckTotals record in T2AdvanceSectionTruckTotalsList)
			{
				String dump = ObjectDumper.Dump(record);
				text.Append ("<tr>"+dump+"</tr>");
			}
			text.Append("</table>");
			text.Append ("<table style=\"padding-left: 5px; font-size:xx-small;\">");
			foreach( T3TMProductTotals record in T3TMProductTotalsList)
			{
				String dump = ObjectDumper.Dump(record);
				text.Append ("<tr>"+dump+"</tr>");
			}
*/
			text.Append ("</table>");
			return text.ToString ();
		}




	}




}

