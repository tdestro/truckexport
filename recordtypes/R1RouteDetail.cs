using FileHelpers;

namespace truck_manifest
{
	[DelimitedRecord("|")]
	public class R1RouteDetail : BaseRecordExtended
	{
        [FieldTrim(TrimMode.Both)]
		public string TruckDropOrder;
        [FieldTrim(TrimMode.Both)]
		public string RouteID;
        [FieldTrim(TrimMode.Both)]
		public string RouteType;
        [FieldTrim(TrimMode.Both)]
		public string RouteTypeIndicator;
        [FieldTrim(TrimMode.Both)]
		public string DepotID;
        [FieldTrim(TrimMode.Both)]
		public string DepotDropOrder;
        [FieldTrim(TrimMode.Both)]
		public string Edition;
        [FieldTrim(TrimMode.Both)]
		public string DrawTotal;
        [FieldTrim(TrimMode.Both)]
		public string NumberOfStandardBundles;
        [FieldTrim(TrimMode.Both)]
		public string NumberOfKeyBundles;
        [FieldTrim(TrimMode.Both)]
		public string KeyBundleSize;
        [FieldTrim(TrimMode.Both)]
		public string CarrierName;
        [FieldTrim(TrimMode.Both)]
		public string CarrierPhoneNumber;
        [FieldTrim(TrimMode.Both)]
		public string InsertMixCombination;
        [FieldTrim(TrimMode.Both)]
		public string DropLocation;
        [FieldTrim(TrimMode.Both)]
		public string DropInstructions;
        [FieldTrim(TrimMode.Both)]
		public string AdZone;
        [FieldTrim(TrimMode.Both)]
		public string PreprintDemographic;
        [FieldTrim(TrimMode.Both)]
		public string InsertExceptionIndicator;
        [FieldTrim(TrimMode.Both)]
		public string BulkIndicator;
        [FieldTrim(TrimMode.Both)]
		public string HandTieIndicator;
        [FieldTrim(TrimMode.Both)]
		public string MinimumBundleSize;
        [FieldTrim(TrimMode.Both)]
		public string MaximumBundleSize;
        [FieldTrim(TrimMode.Both)]
		public string StandardBundleSize;
        [FieldTrim(TrimMode.Both)]
		public string RouteName;
        [FieldTrim(TrimMode.Both)]
		public string MapReference;
        [FieldTrim(TrimMode.Both)]
		public string MapNumber;
        [FieldTrim(TrimMode.Both)]
		public string MultipackID;
        [FieldTrim(TrimMode.Both)]
		public string WeightOfProduct;
        [FieldTrim(TrimMode.Both)]
		public string TotalDropWeight;
        [FieldTrim(TrimMode.Both)]
		public string StandardBundleWeight;
        [FieldTrim(TrimMode.Both)]
        public string CarrierID;
        [FieldTrim(TrimMode.Both)]
		public string SupplyTruckID;
        [FieldTrim(TrimMode.Both)]
        public string SingleCopyRate;
		//public string ChuteNumber;
		//public string DepartureOrder;

	}

}
