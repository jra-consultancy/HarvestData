using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestDataService.Model
{
    public class Asset
    {
        public string AssetID { get; set; }
        public string AssetTag { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string OS { get; set; }
        public string UserID { get; set; }
        public DateTime? WarrantyDate { get; set; }
        public bool? Live { get; set; }
        public DateTime? Heartbeat { get; set; }
        public string Notes { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public string Custom05 { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string BArea { get; set; }
        public string Owner { get; set; }
        public string Email { get; set; }
        public string OU { get; set; }
        public string Make { get; set; }
        public long? BitStatus { get; set; }
        public string IP { get; set; }
        public string BuildNumber { get; set; }
    }
}
