using FileHelpers;

namespace truck_manifest
{
	[DelimitedRecord("|")]
	public class T3TMProductTotals : BaseRecordExtended
	{
		public string TMProductID;
		public string TotalDrops;
		public string TotalBundles;
		public string TMDrawTotal;
		public string TotalStandardBundles;
		public string TotalKeyBundles;
		public string TMTotalBulkDraw;
		public string TotalBulkStandardBundles;
		public string TotalBulkKeyBundles;
		public string BulkKeySize;
		public string TotalWeight;
		public string ChuteNumber;
		public string DriversName;
		public string DepartureOrder;
	}

}
