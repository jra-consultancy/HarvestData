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
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string DNSHostName { get; set; }
        public bool Enabled { get; set; }
        public string EduDeviceType { get; set; }
        public DateTime? Created { get; set; }
        public string IPv4Address { get; set; }
        public string IPv6Address { get; set; }
        public bool? isDeleted { get; set; }
        public DateTime? LastLogonDate { get; set; }
        public string Location { get; set; }
        public bool? LockedOut { get; set; }
        public int logonCount { get; set; }
        public string ManagedBy { get; set; }
        public string Name { get; set; }
        public string OperatingSystem { get; set; }
        public string OperatingSystemVersion { get; set; }
        public string PasswordExpired { get; set; }
    }
}
