using FileHelpers;
using System;

namespace truck_manifest
{
	public class BaseRecordExtended : BaseRecord
	{
		public  BaseRecordExtended(){
			this.RunDate = new DateTime();
		}
	    public string MainProductID;
		public string ProductID;
		public string RunType;

		[FieldConverter(ConverterKind.Date, "M/d/yyyy" )] 
		public DateTime RunDate;

		public string TruckID;
		public string TruckName;

	}
}