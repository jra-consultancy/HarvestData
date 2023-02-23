using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestDataService.Model
{
    public class User
    {
        
        public string UserId { get; set; }
        public DateTime? AccountExpirationDate { get; set; }
        public string CO { get; set; }
        public string Company { get; set; }
        public DateTime? CreateTimeStamp { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string EmployeeID { get; set; }
        public bool Enabled { get; set; } = false;
        public string GivenName { get; set; }
        public DateTime? LastLogonDate { get; set; }
        public int logonCount { get; set; }
        public string mailNickname { get; set; }
        public string manager { get; set; }
        public bool PasswordExpired { get; set; } = false;
        public string PhysicalDeliveryOfficeName { get; set; }
        public string postalCode { get; set; }
        public string Surname { get; set; }
        public string TelephoneNumber { get; set; }
        public string Title { get; set; }
        public string UserAccountControl { get; set; }
    }
}
