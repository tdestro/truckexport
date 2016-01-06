using System;

namespace truck_manifest
{
	public class manifestfile
	{
        public manifestfile()
        {

        }
		public manifestfile (string file, string contentnumber, DateTime fileCreatedDate, string datestring, DateTime date, string RunType, string edition)
		{
			this.file = file;
			this.datestring = datestring;
			this.date = date;
			this.RunType = RunType;
			this.edition = edition;
			this.fileCreatedDate = fileCreatedDate;
			this.contentnumber = contentnumber;
		}


        public manifestfile(string file, string contentnumber, DateTime fileCreatedDate, string datestring, DateTime date, string RunType)
        {
            this.file = file;
            this.datestring = datestring;
            this.date = date;
            this.RunType = RunType;
            this.fileCreatedDate = fileCreatedDate;
            this.contentnumber = contentnumber;
        }

		public manifestfile(string file){
			this.file = file;
		}

		public DateTime date;
		public string file;
		public string datestring;
		public string RunType;
		public string edition;
		public string description;
		public DateTime fileCreatedDate;
		public string contentnumber;
	}
}


		