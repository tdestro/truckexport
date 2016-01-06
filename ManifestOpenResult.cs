using System;
using System.Collections.Generic;
using System.Collections;

namespace truck_manifest
{
    public class ManifestOpenResult
    {
        public ManifestOpenResult(HashSet<DateTime> DateHashSet, List<manifestfile> dateList)
        {
            this.DateHashSet = DateHashSet;
            this.dateList = dateList;
        }
        /*
        public ManifestOpenResult(HashSet<DateTime> DateHashSet, Hashtable<DateTime, manifestfile> advanceList, HashTable<DateTime, manifestfile> manifestList, List<manifestfile> dateList)
        {
            this.DateHashSet = DateHashSet;
            this.advanceList = advanceList;
            this.manifestList = manifestList;
            this.dateList = dateList;
        }*/
        public HashSet<DateTime> DateHashSet;
        //Hashtable<DateTime, manifestfile> advanceList;
        //Hashtable<DateTime, manifestfile> manifestList;
        public List<manifestfile> dateList;
    }
}