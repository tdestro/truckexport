using FileHelpers;

namespace truck_manifest
{ 
	[DelimitedRecord("|")]
	public class P2PrevDrawTotalsByType : BaseRecordExtended
	{
		public string TruckDropOrder;
		public string RouteID;
		public string DrawType;
		public string TotalDraw;
		public string TotalDrawMinusPrevDate;
	}

}
