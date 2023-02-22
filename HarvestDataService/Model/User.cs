using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestDataService.Model
{
    public class User
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Department { get; set; }
        public string BusinessUnit { get; set; }
        public string Location { get; set; }
        public string JobTitle { get; set; }
        public bool VIP { get; set; } = true;
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public bool Live { get; set; } = true;
        public DateTime? Heartbeat { get; set; }
        public string Notes { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public string Custom05 { get; set; }
        public string FirstName { get; set; }
        public string Domain { get; set; }
        public string DirectManager { get; set; }
        public string CountryCode { get; set; }
        public string Language { get; set; }
        public long? BitStatus { get; set; }
    }
}
