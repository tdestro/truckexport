using FileHelpers;

namespace truck_manifest
{ 
	[DelimitedRecord("|")]
	public class P1PreviousDrawTotals : BaseRecordExtended
	{
		public string TruckDropOrder;
		public string RouteID;
		public string TotalDraw;
		public string TotalDrawMinusPrevDate;
	}

}
