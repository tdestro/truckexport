using FileHelpers;

namespace truck_manifest
{
	[DelimitedRecord("|")]
	public class R2AdvanceSectionDetail : BaseRecordExtended
    {
        [FieldTrim(TrimMode.Both)]
		public string TruckDropOrder;
        [FieldTrim(TrimMode.Both)]
		public string RouteID;
        [FieldTrim(TrimMode.Both)]
        public string PaperSection;
        [FieldTrim(TrimMode.Both)]
		public string Updraw;
        [FieldTrim(TrimMode.Both)]
        public string InsertMix;
        
	}
}
