using FileHelpers;

namespace truck_manifest
{
	[DelimitedRecord("|")]
	public class A1TruckTimeData : BaseRecord
	{
		public string TruckID;
		public string TruckTime;
	}

}