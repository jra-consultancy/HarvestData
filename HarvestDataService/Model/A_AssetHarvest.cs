using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestDataService.Model
{
    public class A_AssetHarvest
    {
        public int HarvestID { get; set; }
        public string AssetID { get; set; }
        public int EventID { get; set; }
        public string HarvestType { get; set; }
        public int Status { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
