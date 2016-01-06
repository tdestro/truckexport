using FileHelpers;

namespace truck_manifest
{
	[DelimitedRecord("|")]
	public class T2AdvanceSectionTruckTotals : BaseRecordExtended
	{
		public string PaperSection;
		public string Updraw;
		public string ChuteNumber;
		public string DriversName;
		public string DepartureOrder;

	}

}
