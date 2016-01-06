namespace truck_manifest
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using T4Templates;

    public class Truck
	{
		public string ShortTruckID;
		public string TruckName;
		public R1RouteDetail headerRecord;

		public Dictionary<string, Dictionary<string,Dictionary<string, int>>> ProductsDrawGrandTotalsHash;
		Dictionary<string, Dictionary<string, Dictionary<string, int>>> ProductsMasterTotalsHash;
		Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawMasterTotalsDepotHash;
		Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawGrandTotalsDepotHash;
		public SortedDictionary<string, WholeTruck> WholeTruckHash;
		public int linecounter;
		private bool UseHeader;
		public DateTime RunDate;
		public int lowrange;
		public int highrange;
        public int truckhashid;

		public SortedDictionary<string, locationtranslation> translationHash;
		public Truck ()
		{
			WholeTruckHash = new SortedDictionary<string, WholeTruck> (); 
		}

		public Truck (string ShortTruckID, string TruckName,int lowrange, int highrange,SortedDictionary<string, locationtranslation> translationHash, int truckhashid)
		{
            this.truckhashid = truckhashid;
            this.translationHash = translationHash;
			this.TruckName = TruckName;
			this.ShortTruckID = ShortTruckID;
			RunDate = new DateTime (1800, 1, 18);
			WholeTruckHash = new SortedDictionary<string, WholeTruck> (); 
			linecounter = 0;
			UseHeader = false;
			this.lowrange = Int32.Parse(lowrange.ToString().Substring (0, 3));
			this.highrange = Int32.Parse(highrange.ToString().Substring (0, 3));

		}


		public void setUseHeader ()
		{
			UseHeader = true;
		}

		public string GetDebug ()
		{
			string response = "";
			foreach (KeyValuePair<string, WholeTruck> kvp in WholeTruckHash) {
				WholeTruck currentTruck = kvp.Value;
				response += currentTruck.GetDebug ();
			}
			return response;
		}



        private void prepareToCalculateGrandTotals(string reportType, ref string response, ref int numberofdrops, ref int altnumberofdrops, ref List<string> removeR2s, string title, ref Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawTotalsCarrierHash260, ref Dictionary<string, Dictionary<string, pakman>>DTI_CTPAKMAN_BundleSizes)
		{
			int numberOfProducts = 0;
			ProductsDrawGrandTotalsDepotHash = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>();
			ProductsDrawGrandTotalsHash = new Dictionary<string, Dictionary<string,Dictionary<string, int>>> ();

			foreach (KeyValuePair<string, WholeTruck> kvp in WholeTruckHash) {
				WholeTruck currentTruck = kvp.Value;
				response += currentTruck.GetReport (reportType, ref removeR2s, title, ref ProductsDrawTotalsCarrierHash260, ref DTI_CTPAKMAN_BundleSizes);
				numberofdrops += currentTruck.numberofdrops;
				altnumberofdrops += currentTruck.altnumberofdrops;
				this.RunDate = currentTruck.RunDate;

				// ACCUMULATE DRAW TOTALS FOR DEPOTS
				foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> depot in currentTruck.ProductsDrawTotalsDepotHash) {
					if (! ProductsDrawMasterTotalsDepotHash.ContainsKey (depot.Key)) {
						var calc = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
						ProductsDrawMasterTotalsDepotHash.Add (depot.Key, calc);
					}

					if (! ProductsDrawGrandTotalsDepotHash.ContainsKey (depot.Key)) {
						var calc = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
						ProductsDrawGrandTotalsDepotHash.Add (depot.Key, calc);
					}

					foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, int>>> product in depot.Value) {
						if (! ProductsDrawMasterTotalsDepotHash[depot.Key].ContainsKey (product.Key)) {
							var calc = new Dictionary<string, Dictionary<string, int>>();
							ProductsDrawMasterTotalsDepotHash[depot.Key].Add (product.Key, calc);
						}

						if (! ProductsDrawGrandTotalsDepotHash[depot.Key].ContainsKey (product.Key)) {
							var calc = new Dictionary<string, Dictionary<string, int>>();
							ProductsDrawGrandTotalsDepotHash[depot.Key].Add (product.Key, calc);
						}


						foreach (KeyValuePair<string, Dictionary<string, int>> papersection in product.Value) {
							if (!ProductsDrawMasterTotalsDepotHash [depot.Key] [product.Key].ContainsKey (papersection.Key)) {
								var calc = new Dictionary<string, int> ();
								calc.Add ("Draw", 0);
								calc.Add ("SingleCopyDraw", 0);
								calc.Add ("HomeDeliveryDraw", 0);
								calc.Add ("HomeDeliveryNumberOfDrops", 0);
								calc.Add ("SingleCopyNumberOfDrops", 0);
								calc.Add ("StandardBundleSize", papersection.Value["StandardBundleSize"]);
								ProductsDrawMasterTotalsDepotHash [depot.Key] [product.Key].Add (papersection.Key, calc);
							}
							ProductsDrawMasterTotalsDepotHash [depot.Key] [product.Key] [papersection.Key] ["Draw"] += papersection.Value ["Draw"];

							if (!ProductsDrawGrandTotalsDepotHash [depot.Key] [product.Key].ContainsKey (papersection.Key)) {
								var calc = new Dictionary<string, int> ();
								calc.Add ("Draw", 0);
								calc.Add ("SingleCopyDraw", 0);
								calc.Add ("HomeDeliveryDraw", 0);
								calc.Add ("HomeDeliveryNumberOfDrops", 0);
								calc.Add ("SingleCopyNumberOfDrops", 0);
								calc.Add ("StandardBundleSize", papersection.Value["StandardBundleSize"]);
								ProductsDrawGrandTotalsDepotHash [depot.Key] [product.Key].Add (papersection.Key, calc);
							}
							ProductsDrawGrandTotalsDepotHash [depot.Key] [product.Key] [papersection.Key] ["Draw"] += papersection.Value ["Draw"];

						
						}
					}
				}


				// ACCUMULATE DRAW TOTALS FOR GRAND TOTALS AND MASTER TOTALS
				foreach (KeyValuePair<string, Dictionary<string, TotalTypes>> product in currentTruck.ProductsDrawTotalsHash) {
					foreach (KeyValuePair<string, TotalTypes> papersection in product.Value) {

					if (!ProductsMasterTotalsHash.ContainsKey (product.Key)) {
						var ProductsCalcMasterTotalsHash = new Dictionary<string,Dictionary<string, int>> ();
					
							ProductsMasterTotalsHash.Add (product.Key, ProductsCalcMasterTotalsHash);
						}

						if (!ProductsMasterTotalsHash[product.Key].ContainsKey (papersection.Key)) {
							var ProductsCalcMasterTotalsHash = new Dictionary<string, int> ();
							ProductsCalcMasterTotalsHash.Add ("Draw", 0);
							ProductsCalcMasterTotalsHash.Add ("NumberOfDrops", 0);

							ProductsCalcMasterTotalsHash.Add ("StandardBundleSize", papersection.Value.StandardBundleSize);
							ProductsCalcMasterTotalsHash.Add ("Bundles",0);
							ProductsCalcMasterTotalsHash.Add ("Odds",0);
							ProductsCalcMasterTotalsHash.Add ("OddPalletContains",0);
							ProductsCalcMasterTotalsHash.Add ("OddPalletCount",0);
							ProductsCalcMasterTotalsHash.Add ("SingleCopyDraw",0);
							ProductsCalcMasterTotalsHash.Add ("HomeDeliveryDraw",0);
							ProductsCalcMasterTotalsHash.Add ("SingleCopyDrawDepot",0);
							ProductsCalcMasterTotalsHash.Add ("HomeDeliveryDrawDepot",0);		
							ProductsCalcMasterTotalsHash.Add ("SingleCopyDrawBulk",0);
							ProductsCalcMasterTotalsHash.Add ("HomeDeliveryDrawBulk",0);

							ProductsCalcMasterTotalsHash.Add ("FullPallets",0);
							ProductsCalcMasterTotalsHash.Add ("Pallets",0);
							ProductsCalcMasterTotalsHash.Add ("HomeDeliveryNumberOfDropsDepot",0);
							ProductsCalcMasterTotalsHash.Add ("HomeDeliveryNumberOfDropsBulk",0);
							ProductsCalcMasterTotalsHash.Add ("HomeDeliveryOddsBulk",0);
							ProductsCalcMasterTotalsHash.Add ("HomeDeliveryNumberOfDrops",0);

							ProductsCalcMasterTotalsHash.Add ("SingleCopyNumberOfDrops",0);
							ProductsCalcMasterTotalsHash.Add ("SingleCopyNumberOfDropsDepot",0);
							ProductsCalcMasterTotalsHash.Add ("SingleCopyNumberOfDropsBulk",0);
							ProductsCalcMasterTotalsHash.Add ("SingleCopyOddsBulk",0);

							ProductsMasterTotalsHash[product.Key].Add (papersection.Key, ProductsCalcMasterTotalsHash);
						}

					ProductsMasterTotalsHash [product.Key][papersection.Key] ["Draw"] +=  papersection.Value.Draw;
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["NumberOfDrops"] += papersection.Value.NumberOfDrops;
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["SingleCopyDraw"] += papersection.Value.SingleCopyDraw;
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["HomeDeliveryDraw"] +=  papersection.Value.HomeDeliveryDraw;
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["HomeDeliveryNumberOfDrops"] += papersection.Value.HomeDeliveryNumberOfDrops;
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["SingleCopyNumberOfDrops"] += papersection.Value.SingleCopyNumberOfDrops;

					// BULK AND DEPOT TOTALS FOR DISPATCH REPORT.
					if (Int32.Parse(this.ShortTruckID) >= lowrange && Int32.Parse(this.ShortTruckID) <= highrange) {
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["HomeDeliveryNumberOfDropsDepot"] += papersection.Value.HomeDeliveryNumberOfDrops;
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["HomeDeliveryDrawDepot"] += papersection.Value.HomeDeliveryDraw;
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["SingleCopyNumberOfDropsDepot"] += papersection.Value.SingleCopyNumberOfDrops;
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["SingleCopyDrawDepot"] += papersection.Value.SingleCopyDraw;
					} else { // BULK
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["HomeDeliveryNumberOfDropsBulk"] += papersection.Value.HomeDeliveryNumberOfDrops;
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["HomeDeliveryDrawBulk"] += papersection.Value.HomeDeliveryDraw;
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["SingleCopyNumberOfDropsBulk"] += papersection.Value.SingleCopyNumberOfDrops;
					ProductsMasterTotalsHash [product.Key][papersection.Key] ["SingleCopyDrawBulk"] += papersection.Value.SingleCopyDraw;
					}
					
						if (!ProductsDrawGrandTotalsHash.ContainsKey (product.Key)) {
							ProductsDrawGrandTotalsHash.Add (product.Key, new Dictionary<string,Dictionary<string, int>> ());
						}

					if (!ProductsDrawGrandTotalsHash[product.Key].ContainsKey (papersection.Key)) {
						var ProductsCalcGrandTotalsHash = new Dictionary<string, int> ();
						ProductsCalcGrandTotalsHash.Add ("Draw",papersection.Value.Draw);
						ProductsCalcGrandTotalsHash.Add ("StandardBundleSize", papersection.Value.StandardBundleSize);
						ProductsDrawGrandTotalsHash[product.Key].Add (papersection.Key, ProductsCalcGrandTotalsHash);
						numberOfProducts++;
					} else {
						ProductsDrawGrandTotalsHash [product.Key][papersection.Key] ["Draw"] += papersection.Value.Draw;
					}


					}

				}
			}
			linecounter = numberOfProducts / 3; 
			if (numberOfProducts % 3 > 0)
				linecounter++;

			// USE PREVIOUSLY ACCUMULATED DRAW TO CALCULATE BUNDLES, ODDS, PALLETS AND ODD PALLETS FOR GRAND TOTALS AND MASTER TOTALS.
			foreach (KeyValuePair<string,Dictionary<string, Dictionary<string, int>>> product in ProductsDrawGrandTotalsHash) {
			foreach (KeyValuePair<string, Dictionary<string, int>> entry in product.Value) {
				if ((entry.Value ["Draw"] >= entry.Value ["StandardBundleSize"]) && entry.Value ["StandardBundleSize"] > 0) {
					entry.Value.Add ("Bundles", entry.Value ["Draw"] / entry.Value ["StandardBundleSize"]);
					entry.Value.Add ("Odds", entry.Value ["Draw"] % entry.Value ["StandardBundleSize"]);
				} else {
					entry.Value.Add ("Bundles", 0);
					entry.Value.Add ("Odds", entry.Value ["Draw"]);
				}

				int Pallets = entry.Value ["Bundles"] / 60;
				entry.Value.Add ("OddPalletContains", entry.Value ["Bundles"] % 60);
				entry.Value.Add ("FullPallets", Pallets);

				if (entry.Value ["OddPalletContains"] > 0) {
						ProductsMasterTotalsHash[product.Key][entry.Key] ["OddPalletCount"] += 1;
					Pallets++;
				}
				entry.Value.Add ("Pallets", Pallets);

				// MASTER TOTALS, no aggregation.
				if (reportType != "Dispatch" && reportType != "AltDispatch") {
				ProductsMasterTotalsHash [product.Key] [entry.Key] ["Bundles"] += entry.Value ["Bundles"];
				ProductsMasterTotalsHash [product.Key] [entry.Key] ["Odds"] += entry.Value ["Odds"];
				ProductsMasterTotalsHash [product.Key] [entry.Key] ["OddPalletContains"] += entry.Value ["OddPalletContains"];
				ProductsMasterTotalsHash [product.Key] [entry.Key] ["FullPallets"] += entry.Value ["FullPallets"];
				ProductsMasterTotalsHash [product.Key] [entry.Key] ["Pallets"] += entry.Value ["Pallets"];
				}
				}
			}
			// IF THIS IS THE DISPATCH REPORT, CALCULATE BUNDLES AND ODDS AGGREGATED TO MAXIMUM.
			if (reportType == "Dispatch" || reportType == "AltDispatch") {
				foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, int>>> product in ProductsMasterTotalsHash) {
					foreach (KeyValuePair<string, Dictionary<string, int>> entry in product.Value) {


						if ((entry.Value ["Draw"] >= entry.Value ["StandardBundleSize"]) && entry.Value ["StandardBundleSize"] > 0) {
							entry.Value["Bundles"] = entry.Value ["Draw"] / entry.Value ["StandardBundleSize"];
							entry.Value["Odds"] = entry.Value ["Draw"] % entry.Value ["StandardBundleSize"];
						} else {
							entry.Value["Bundles"] = 0;
							entry.Value["Odds"] = entry.Value ["Draw"];
						}


						if ((entry.Value ["HomeDeliveryDrawDepot"] >= entry.Value ["StandardBundleSize"]) && entry.Value ["StandardBundleSize"] > 0) {
							entry.Value ["HomeDeliveryBundlesDepot"] = entry.Value ["HomeDeliveryDrawDepot"] / entry.Value ["StandardBundleSize"];
							entry.Value ["HomeDeliveryOddsDepot"] = entry.Value ["HomeDeliveryDrawDepot"] % entry.Value ["StandardBundleSize"];
						} else {
							entry.Value ["HomeDeliveryBundlesDepot"] = 0;
							entry.Value ["HomeDeliveryOddsDepot"] = entry.Value ["HomeDeliveryDrawDepot"];
						}
						if ((entry.Value ["SingleCopyDrawDepot"] >= entry.Value ["StandardBundleSize"]) && entry.Value ["StandardBundleSize"] > 0) {
							entry.Value ["SingleCopyBundlesDepot"] = entry.Value ["SingleCopyDrawDepot"] / entry.Value ["StandardBundleSize"];
							entry.Value ["SingleCopyOddsDepot"] = entry.Value ["SingleCopyDrawDepot"] % entry.Value ["StandardBundleSize"];
						} else {
							entry.Value ["SingleCopyBundlesDepot"] = 0;
							entry.Value ["SingleCopyOddsDepot"] = entry.Value ["SingleCopyDrawDepot"];
						}
						///
						///
						///
						if ((entry.Value ["HomeDeliveryDrawBulk"] >= entry.Value ["StandardBundleSize"]) && entry.Value ["StandardBundleSize"] > 0) {
							entry.Value ["HomeDeliveryBundlesBulk"] = entry.Value ["HomeDeliveryDrawBulk"] / entry.Value ["StandardBundleSize"];
							entry.Value ["HomeDeliveryOddsBulk"] = entry.Value ["HomeDeliveryDrawBulk"] % entry.Value ["StandardBundleSize"];
						} else {
							entry.Value ["HomeDeliveryBundlesBulk"] = 0;
							entry.Value ["HomeDeliveryOddsBulk"] = entry.Value ["HomeDeliveryDrawBulk"];
						}
						if ((entry.Value ["SingleCopyDrawBulk"] >= entry.Value ["StandardBundleSize"]) && entry.Value ["StandardBundleSize"] > 0) {
							entry.Value ["SingleCopyBundlesBulk"] = entry.Value ["SingleCopyDrawBulk"] / entry.Value ["StandardBundleSize"];
							entry.Value ["SingleCopyOddsBulk"] = entry.Value ["SingleCopyDrawBulk"] % entry.Value ["StandardBundleSize"];
						} else {
							entry.Value ["SingleCopyBundlesBulk"] = 0;
							entry.Value ["SingleCopyOddsDepot"] = entry.Value ["SingleCopyDrawDepot"];
						}
						///
						///
						///
						if ((entry.Value ["HomeDeliveryDraw"] >= entry.Value ["StandardBundleSize"]) && entry.Value ["StandardBundleSize"] > 0) {
							entry.Value ["HomeDeliveryBundles"] = entry.Value ["HomeDeliveryDraw"] / entry.Value ["StandardBundleSize"];
							entry.Value ["HomeDeliveryOdds"] = entry.Value ["HomeDeliveryDraw"] % entry.Value ["StandardBundleSize"];
						} else {
							entry.Value ["HomeDeliveryBundles"] = 0;
							entry.Value ["HomeDeliveryOdds"] = entry.Value ["HomeDeliveryDraw"];
						}
						if ((entry.Value ["SingleCopyDraw"] >= entry.Value ["StandardBundleSize"]) && entry.Value ["StandardBundleSize"] > 0) {
							entry.Value ["SingleCopyBundles"] = entry.Value ["SingleCopyDraw"] / entry.Value ["StandardBundleSize"];
							entry.Value ["SingleCopyOdds"] = entry.Value ["SingleCopyDraw"] % entry.Value ["StandardBundleSize"];
						} else {
							entry.Value ["SingleCopyBundles"] = 0;
							entry.Value ["SingleCopyOdds"] = entry.Value ["SingleCopyDraw"];
						}
					}
				}

			}


		}

        public string GetReport(string reportType, Dictionary<string, Dictionary<string, Dictionary<string, int>>> ProductsMasterTotalsHash, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawMasterTotalsDepotHash, StringDictionary TruckDictionary, ref List<string> removeR2s, string title, ref Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> ProductsDrawTotalsCarrierHash260, ref Dictionary<string, Dictionary<string,pakman>> DTI_CTPAKMAN_BundleSizes)
		{
			this.ProductsMasterTotalsHash = ProductsMasterTotalsHash;
			this.ProductsDrawMasterTotalsDepotHash = ProductsDrawMasterTotalsDepotHash;
			string response = "";
			int numberofdrops = 0;
			int altnumberofdrops = 0;
        
			prepareToCalculateGrandTotals (reportType, ref response, ref numberofdrops, ref altnumberofdrops, ref removeR2s, title, ref ProductsDrawTotalsCarrierHash260, ref DTI_CTPAKMAN_BundleSizes); 
			if(numberofdrops == 0) return "";

			TruckDictionary.Add ("ShortTruckID", ShortTruckID);
			TruckDictionary.Add ("TruckName", TruckName);
			TruckDictionary.Add ("RunDate", this.RunDate.ToLongDateString ());
			TruckDictionary.Add ("TotalNumberOfDrops", numberofdrops.ToString ());
			TruckDictionary.Add ("AltTotalNumberOfDrops", altnumberofdrops.ToString ());
			TruckDictionary.Add ("UseHeader", UseHeader.ToString ());
            TruckDictionary.Add ("title", title);

			if (headerRecord != null) {
				TruckDictionary.Add ("DeliveryArea", headerRecord.DropLocation);
				TruckDictionary.Add ("GenInstructions", headerRecord.DropInstructions);

			} else {
				TruckDictionary.Add ("DeliveryArea", "No Information For This Truck ID");
				TruckDictionary.Add ("GenInstructions", "");
			}

			if (reportType == "Depot") {

				var Template = new DepotT4Template () {
					TruckDictionary = TruckDictionary, ProductsDrawGrandTotalsDepotHash = this.ProductsDrawGrandTotalsDepotHash,};
				if (Int32.Parse(this.ShortTruckID) >= lowrange && Int32.Parse(this.ShortTruckID) <= highrange) {
					return Template.TransformText ();
				} else {
					return "";
				}
			}
			else if (reportType == "Dispatch") {
                

				var Template = new DispatchT4Template () {
					TruckDictionary = TruckDictionary, ProductsDrawGrandTotalsHash = this.ProductsDrawGrandTotalsHash,
				};


				return Template.TransformText ();
            }
            else if (reportType == "AltDistribution")
            {


                var Template = new DistributionT4Template()
                {
                    TruckDictionary = TruckDictionary,
                    ProductsDrawGrandTotalsHash = this.ProductsDrawGrandTotalsHash,
                    altprod = true,
                };
                linecounter = 1;
                return Template.TransformText();
            }
            else if (reportType == "AltDispatch")
            {

				var Template = new DispatchT4Template () {
					TruckDictionary = TruckDictionary, ProductsDrawGrandTotalsHash = this.ProductsDrawGrandTotalsHash, altprod = true,
				};
				return Template.TransformText ();
            }
            else if (reportType == "Pallet")
            {
				var Template = new PalletT4Template () {
					TruckDictionary = TruckDictionary, ProductsDrawGrandTotalsHash = this.ProductsDrawGrandTotalsHash,
				};
				return Template.TransformText ();
			} else if (reportType == "PalletSheets") {

				var Template = new PalletSheetsT4Template () {
					TruckDictionary = TruckDictionary, ProductsDrawGrandTotalsHash = this.ProductsDrawGrandTotalsHash,
				};
				return Template.TransformText ();
			} else {
				var Template = new TruckManifestT4Template () {
					TruckDictionary = TruckDictionary, ProductsDrawGrandTotalsHash = this.ProductsDrawGrandTotalsHash,
				};
				return Template.TransformText () + response;
			}
		}
	}
}
