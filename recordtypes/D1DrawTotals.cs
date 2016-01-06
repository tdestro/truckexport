using FileHelpers;

namespace truck_manifest
{
	[DelimitedRecord("|")]
	public class D1DrawTotals : BaseRecordExtended
    {
        [FieldTrim(TrimMode.Both)]
		public string TruckDropOrder;
        [FieldTrim(TrimMode.Both)]
		public string RouteID;
        [FieldTrim(TrimMode.Both)]
		public string DrawType;
        [FieldTrim(TrimMode.Both)]
		public string DrawTotal;
        [FieldTrim(TrimMode.Both)]
		public string UndocumentedString1;
        [FieldTrim(TrimMode.Both)]
		public string UndocumentedString2;
	}

}

