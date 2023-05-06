using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestDataService.Model
{
    public class AD_HarvesterResult
    {

        public string Item { get; set; }
        public bool IsPingSuccess { get; set; }
        public bool IsWMISuccess { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }
    }
}
