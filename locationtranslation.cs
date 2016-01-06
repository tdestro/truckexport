using FileHelpers;

namespace truck_manifest
{[DelimitedRecord(";")]
	public class locationtranslation
	{
		public string TruckID;
		public string DeliveryArea;
		[FieldOptional]
		public string GenInstructions; 
		[FieldOptional]
		public string SpecInstructions;
	}
}
