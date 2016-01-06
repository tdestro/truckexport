using System;
using System.Collections.Generic; 
namespace truck_manifest
{
    public static class productdesc
    {

        public static Dictionary<string, string> DailyProductTranslationHash = new Dictionary<string, string>(){
	            {"200", "PG"},
	            {"970", "NYT (HD)"},
	            {"960", "NYT (SC)"},
                {"972", "WSJ (HD)"},
	            {"962", "WSJ (SC)"},
	            {"965", "IBD"},
                {"974", "FT"},
	            {"963", "USA (HD)"},
	            {"973", "USA (SC)"},
                {"967", "Sport Weekly Extra"},
                {"600", "East Zone"},
	            {"700", "North Zone"},
                {"800", "South Zone"},
                {"900", "West Zone"},
	            {"602", "East Zone Complete"},
                {"702", "North Zone Complete"},
                {"802", "South Zone Complete"},
	            {"902", "West Zone Complete"},
                {"260", "5 Star"},
                {"261", "5 Star Complete"},
                {"770", "Advance"},
                {"966", "Barron"},
                {"660", "3 Star"},
                {"661", "3 Star Complete"}
	            };

         public static string translate(String s, String title){
             if (title.Contains("Color")) {
                 return "Sunday Color Preprints";
             } 
             else if (DailyProductTranslationHash.ContainsKey(s))
             {
                 return DailyProductTranslationHash[s];
             }
             else return "";
}


        }
   



    }
