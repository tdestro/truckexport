namespace truck_manifest
{
	public class TotalTypes
	{
		public TotalTypes ()
		{
			this.Draw = 0;
			this.Instructions = "";
			this.DistributionFor = "";
			this.Prod = "";
			this.StandardBundleSize = 0;
			this.Bundles = 0;
			this.Odds = 0;
			this.RouteType = "";
			this.Name = "";
			this.Address = "";
			this.Instructions = "";
			this.RouteID = "";
			this.SingleCopyDraw = 0;
			this.HomeDeliveryDraw = 0;
			this.HomeDeliveryNumberOfDrops = 0;
			this.NumberOfDrops = 0;
			this.SingleCopyNumberOfDrops = 0;
			this.numberinlist = 0;
            this.DoNotOverWriteStandardBundleSize = false;
		}
		public int Draw;
		public string DistributionFor;
		public string Prod;
		public int StandardBundleSize;
		public int Bundles;
		public int Odds;
		public string RouteType;
		public string Name;
		public string Address;
		public string Instructions;
		public string RouteID;
        public string CarrierID;

		public int SingleCopyDraw;
		public int HomeDeliveryDraw;
		public int HomeDeliveryNumberOfDrops;
		public int SingleCopyNumberOfDrops;
		public int NumberOfDrops;
        public bool DoNotOverWriteStandardBundleSize;
       
		public int numberinlist;
	}

}

