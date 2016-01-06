using FileHelpers;

namespace truck_manifest
{
	[DelimitedRecord("|")]
	public class R3TMProductDetail : BaseRecordExtended
	{
        [FieldTrim(TrimMode.Both)]
		public string TruckDropOrder;
        [FieldTrim(TrimMode.Both)]
		public string RouteID;
        [FieldTrim(TrimMode.Both)]
		public string TMProductID;
        [FieldTrim(TrimMode.Both)]
		public string TMDrawTotal;
        [FieldTrim(TrimMode.Both)]
		public string NumberOfStandardBundles;
        [FieldTrim(TrimMode.Both)]
		public string NumberOfKeyBundles;
        [FieldTrim(TrimMode.Both)]
		public string TotalKeyDraw;
        [FieldTrim(TrimMode.Both)]
		public string MinimumBundleSize;
        [FieldTrim(TrimMode.Both)]
		public string MaximumBundleSize;
        [FieldTrim(TrimMode.Both)]
		public string StandardBundleSize;
        [FieldTrim(TrimMode.Both)]
		public string Weight;
        [FieldTrim(TrimMode.Both)]
		public string StandardBundleWeight;
        [FieldTrim(TrimMode.Both)]
		public string SupplyTruckID;
        [FieldTrim(TrimMode.Both)]
		public string CarrierID;
        [FieldTrim(TrimMode.Both)]
		public string ChuteNumber;
	}

}
