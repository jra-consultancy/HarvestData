using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestDataService.Model
{
    public class A_AssetHarvestResults
    {
        public int HarvestResultID { get; set; }
        public int HarvestID { get; set; }
        public string HarvestCollectionType { get; set; }
        public string HarvestValue { get; set; }
        public DateTime HarvestDate { get; set; }
    }
}
