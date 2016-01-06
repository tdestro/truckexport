using System.Collections.Generic;
using System.Linq;

namespace truck_manifest
{
	public static class outputsort
	{



        public static List<string> OrderSectionKeysPPG(List<string> Keys)
        {
            List<string> sortedKeyList = new List<string>();
            List<string> keyList = Keys;

            IOrderedEnumerable<string> sortedProductKeys = from key in keyList
                                                           orderby key.Length ascending, key ascending                 
                                                           select key;
            sortedKeyList.AddRange(sortedProductKeys.ToList());

            return sortedKeyList;
        }
		public static List<string> OrderKeysPPGFirst(List<string> Keys){
			List<string> sortedKeyList = new List<string>();
			List<string> keyList = Keys;
			if(keyList.Contains("PPG")){
				sortedKeyList.Add("PPG");
				keyList.Remove("PPG");
			}
			IOrderedEnumerable<string> sortedProductKeys = from key in keyList
				orderby key ascending
				select key;
			sortedKeyList.AddRange(sortedProductKeys.ToList());

			return sortedKeyList;
		}
        public static List<string> OrderKeysNumericalSeq(List<string> Keys)
        {
            List<string> sortedKeyList = new List<string>();

            List<int> KeyList = Keys.Select(s => int.Parse(s)).ToList();

            IOrderedEnumerable<int> sortedProductKeys = from key in KeyList
                                                        orderby key ascending
                                                        select key;
            sortedKeyList.AddRange(sortedProductKeys.ToList().ConvertAll<string>(delegate(int i) { return i.ToString(); }));

            return sortedKeyList;

        }



        public static List<string> OrderKeysNumerical(List<string> Keys)
        {
            List<string> sortedKeyList = new List<string>();
            List<string> keyList = Keys;

            IOrderedEnumerable<string> sortedProductKeys = from key in keyList
                                                           orderby key ascending
                                                           select key;
            sortedKeyList.AddRange(sortedProductKeys.ToList());

            return sortedKeyList;

        }
	}
}

