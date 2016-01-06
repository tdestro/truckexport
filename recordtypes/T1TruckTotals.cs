using FileHelpers;

namespace truck_manifest
{
	[DelimitedRecord("|")]
	public class T1TruckTotals : BaseRecordExtended
	{
		public string InsertMixCombination;
		public string NumberOfBundles;
		public string TotalDraw;
		public string KeyDraw;
		public string NumberOfStandardBundles;
		public string NumberOfKeyBundles;
		public string BulkDrawTotal;
		public string BulkKeyDrawTotal;
		public string NumberOfBulkStandardBundleTops;
		public string NumberOfBulkKeyBundleTops;
		public string NumberOfHandTies;
		public string NumberOfThrowoffs;
		public string RoundedDraw;
		public string TotalWeight;
		public string ChuteNumber;
		public string DriversName;
		public string DepartureOrder;
		public string PublicationNameMain;
		public string PublicationNameProductID;
	}

}
