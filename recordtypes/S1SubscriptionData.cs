using FileHelpers;

namespace truck_manifest
{
	[DelimitedRecord("|")]
	public class S1SubscriptionData : BaseRecordExtended
	{
		public string DropOrder;
		public string RouteID;
		public string Throwoff;
		public string Edition;
		public string NumberOfCopies;
		public string SubscriptionID;
		public string AddressLine1;
		public string AddressLine2;
		public string AddressLine3;
		public string AddressLine4;
		public string AddressLine5;
		public string AddressLine6;
		public string MapReference;
		public string MapNumber;

	}

}
