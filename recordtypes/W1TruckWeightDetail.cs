using FileHelpers;
using System;

namespace truck_manifest
{ 
	[DelimitedRecord("|")]
	public class W1TruckWeightDetail : BaseRecord
	{
		public string Edition;
		public string TruckID;
		[FieldConverter(ConverterKind.Date, "d/M/yyyy" )] 
		public DateTime RunDate;
		public string RunType;
		public string TotalWeight;
	}

}
