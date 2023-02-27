using HarvestDataService.Model;
using Aspose.Cells;
using CsvHelper;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Management;
using ADODB;
using System.DirectoryServices;
using System.ComponentModel;

namespace HarvestDataService
{
    public class HarvestData
    {

        private IArmRepository _iArmRepo;
        private readonly ILogger _logger;
        private string UploadLogFile = "";
        private string type = "";

        public HarvestData()
        {
            _iArmRepo = new ArmRepository();
            _logger = Logger.GetInstance;
            UploadLogFile = _iArmRepo.GetFileLocation(0);
        }
        private static readonly object Mylock = new object();
        public void Harvest(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Monitor.TryEnter(Mylock, 0)) return;
            try
            {
                ExecutePing(type = "Ping");
                ExecuteWmiData(type = "WMI");
            }
            catch (Exception ex)
            {
                _logger.Log("Harvest :" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

            }
            finally
            {
                Monitor.Exit(Mylock);
            }


        }
        public void Harvest()
        {
            try
            {
                //ExecutePing(type = "Ping");
                //ExecuteWmiData(type = "WMI");
                ExecuteADData();
            }
            catch (Exception ex)
            {
                _logger.Log("Harvest :" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

            }

        }

        private void ExecuteADData()
        {
            List<User> users = GetUserADData();
            if (users.Count() > 0)
            {
                _iArmRepo.InsertBulkUsersADData(ToDataTable(users));

            }

            List<Asset> assets = GetComputerADData();
            if(assets.Count() > 0)
            {
                _iArmRepo.InsertBulkAssetsADData(ToDataTable(assets));

            }
        }

        private List<Asset> GetComputerADData()
        {
            try
            {
                List<Asset> assets = new List<Asset>();
                DirectoryEntry objRootDSE = new DirectoryEntry("LDAP://RootDSE");
                string strDNSDomain = objRootDSE.Properties["defaultNamingContext"].Value.ToString();
                string strTarget = "LDAP://" + strDNSDomain;

                string domainPath = strTarget;
                string searchFilter = "(&(objectCategory=computer))";
                string[] propertiesToLoad = new string[] {
                    "cn", "whenCreated", "description", "displayName", "dNSHostName",
                    "userAccountControl", "eucDeviceType", "ipv4Address", "ipv6Address",
                    "isDeleted", "lastLogon", "location", "lockoutTime",
                    "logonCount", "managedBy", "name", "operatingSystem",
                    "operatingSystemVersion", "pwdLastSet","objectGUID","distinguishedName",
                    "operatingSystemServicePack","whenChanged","servicePrincipalName","memberOf"
                };

                DirectoryEntry entry = new DirectoryEntry(domainPath);
                DirectorySearcher searcher = new DirectorySearcher(entry, searchFilter, propertiesToLoad);
                SearchResultCollection results = searcher.FindAll();


                foreach (SearchResult result in results)
                {
                    Asset asset = new Asset();
                    asset.AssetID = result.Properties["cn"].Count > 0 ? result.Properties["cn"][0].ToString() : "";
                    asset.WhenCreated = result.Properties["whenCreated"].Count > 0 ? (DateTime)result.Properties["whenCreated"][0] : DateTime.MinValue;
                    asset.Description = result.Properties["description"].Count > 0 ? result.Properties["description"][0].ToString() : "";
                    asset.DisplayName = result.Properties["displayName"].Count > 0 ? result.Properties["displayName"][0].ToString() : "";
                    asset.DNSHostName = result.Properties["dNSHostName"].Count > 0 ? result.Properties["dNSHostName"][0].ToString() : "";
                    asset.Enabled = result.Properties["userAccountControl"].Count > 0 ? Convert.ToInt32(result.Properties["userAccountControl"][0]) != 0x0002 : false;
                    asset.EduDeviceType = result.Properties["eucDeviceType"].Count > 0 ? result.Properties["eucDeviceType"][0].ToString() : "";
                    asset.IPv4Address = result.Properties["ipv4Address"].Count > 0 ? result.Properties["ipv4Address"][0].ToString() : "";
                    asset.IPv6Address = result.Properties["ipv6Address"].Count > 0 ? result.Properties["ipv6Address"][0].ToString() : "";
                    asset.isDeleted = result.Properties["isDeleted"].Count > 0 ? (bool)result.Properties["isDeleted"][0] : false;
                    asset.LastLogonDate = result.Properties["lastLogon"].Count > 0 ? DateTime.FromFileTime((long)result.Properties["lastLogon"][0]) : DateTime.MinValue;
                    asset.Location = result.Properties["location"].Count > 0 ? result.Properties["location"][0].ToString() : "";
                    asset.LockedOut = result.Properties["lockoutTime"].Count > 0 ? Convert.ToInt64(result.Properties["lockoutTime"][0]) != 0 : false;
                    asset.logonCount = result.Properties["logonCount"].Count > 0 ? Convert.ToInt32(result.Properties["logonCount"][0]) : 0;
                    asset.ManagedBy = result.Properties["managedBy"].Count > 0 ? result.Properties["managedBy"][0].ToString() : "";
                    asset.Name = result.Properties["name"].Count > 0 ? result.Properties["name"][0].ToString() : "";
                    asset.OperatingSystem = result.Properties["operatingSystem"].Count > 0 ? result.Properties["operatingSystem"][0].ToString() : "";
                    asset.OperatingSystemVersion = result.Properties["operatingSystemVersion"].Count > 0 ? result.Properties["operatingSystemVersion"][0].ToString() : "";
                    asset.PasswordExpired = result.Properties["PasswordExpired"].Count > 0 ? result.Properties["PasswordExpired"][0].ToString() : "";
                    asset.ObjectGUID = result.Properties["objectGUID"].Count > 0 ? result.Properties["objectGUID"][0].ToString() : "";
                    asset.DistinguishedName = result.Properties["distinguishedName"].Count > 0 ? result.Properties["distinguishedName"][0].ToString() : "";
                    asset.OperatingSystemServicePack = result.Properties["operatingSystemServicePack"].Count > 0 ? result.Properties["operatingSystemServicePack"][0].ToString() : "";
                    asset.WhenChanged = result.Properties["whenChanged"].Count > 0 ? DateTime.FromFileTime((long)result.Properties["whenChanged"][0]) : DateTime.MinValue;
                    asset.ServicePrincipalName = result.Properties["servicePrincipalName"].Count > 0 ? result.Properties["servicePrincipalName"][0].ToString() : "";
                    asset.MemberOf = result.Properties["memberOf"].Count > 0 ? result.Properties["memberOf"][0].ToString() : "";

                    assets.Add(asset);

                }
                results.Dispose();
                searcher.Dispose();
                entry.Dispose();
                return assets;
            }
            catch(Exception ex)
            {
                _logger.Log("Harvest GetComputerADData:" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                throw ex;
            }
           
        }

        private List<User> GetUserADData()
        {
            try
            {
                List<User> users = new List<User>();
                DirectoryEntry objRootDSE = new DirectoryEntry("LDAP://RootDSE");
                string strDNSDomain = objRootDSE.Properties["defaultNamingContext"].Value.ToString();
                string strTarget = "LDAP://" + strDNSDomain;

                string domainPath = strTarget;//"LDAP://yourdomain.com"; // Replace with your domain name
                string searchFilter = "(&(objectCategory=person)(objectClass=user))";
                string[] propertiesToLoad = new string[] { "userPrincipalName", "accountExpires", "givenName", "company", "uSNCreated",
                    "department", "description", "displayName", "mail","employeeID","enabled","uSNCreated","logonCount","mailNickname",
                    "manager","PasswordExpired","physicalDeliveryOfficeName","postalCode","sn","telephoneNumber","title","userAccountControl",
                    "sAMAccountName","streetAddress","countryCode"
                };

                DirectoryEntry entry = new DirectoryEntry(domainPath);
                DirectorySearcher searcher = new DirectorySearcher(entry, searchFilter, propertiesToLoad);
                SearchResultCollection results = searcher.FindAll();

                foreach (SearchResult result in results)
                {
                    User user = new User();
                    user.UserId = result.Properties["userPrincipalName"].Count > 0 ? result.Properties["userPrincipalName"][0].ToString() : Convert.ToString(Guid.NewGuid());
                    user.AccountExpirationDate = result.Properties["accountExpires"].Count > 0 ? (DateTime)result.Properties["accountExpires"][0] : DateTime.MinValue;
                    user.GivenName = result.Properties["givenName"].Count > 0 ? result.Properties["givenName"][0].ToString() : "";
                    user.CO = result.Properties["countryCode"].Count > 0 ? result.Properties["countryCode"][0].ToString() : "";
                    user.Company = result.Properties["company"].Count > 0 ? result.Properties["company"][0].ToString() : "";
                    user.CreateTimeStamp= result.Properties["uSNCreated"].Count > 0 ? (DateTime)result.Properties["uSNCreated"][0] : DateTime.MinValue;
                    user.Department = result.Properties["department"].Count > 0 ? result.Properties["department"][0].ToString() : "";
                    user.Description = result.Properties["description"].Count > 0 ? result.Properties["description"][0].ToString() : "";
                    user.DisplayName = result.Properties["displayName"].Count > 0 ? result.Properties["displayName"][0].ToString() : "";
                    user.EmailAddress = result.Properties["mail"].Count > 0 ? result.Properties["mail"][0].ToString() : "";
                    user.EmployeeID = result.Properties["employeeID"].Count > 0 ? result.Properties["employeeID"][0].ToString() : "";
                    user.Enabled = result.Properties["enabled"].Count > 0 ? Convert.ToInt64(result.Properties["enabled"][0]) != 0 : false;
                    user.LastLogonDate = result.Properties["uSNCreated"].Count > 0 ? (DateTime)result.Properties["uSNCreated"][0] : DateTime.MinValue;
                    user.logonCount = result.Properties["logonCount"].Count > 0 ? Convert.ToInt32(result.Properties["logonCount"][0]) : 0;
                    user.mailNickname = result.Properties["sAMAccountName"].Count > 0 ? result.Properties["sAMAccountName"][0].ToString() : "";
                    user.manager = result.Properties["manager"].Count > 0 ? result.Properties["manager"][0].ToString() : "";
                    user.PasswordExpired = result.Properties["PasswordExpired"].Count > 0 ? Convert.ToInt64(result.Properties["PasswordExpired"][0]) != 0 : false;
                    user.PhysicalDeliveryOfficeName = result.Properties["physicalDeliveryOfficeName"].Count > 0 ? result.Properties["physicalDeliveryOfficeName"][0].ToString() : "";
                    user.postalCode = result.Properties["postalCode"].Count > 0 ? result.Properties["postalCode"][0].ToString() : "";
                    user.Surname = result.Properties["sn"].Count > 0 ? result.Properties["sn"][0].ToString() : "";
                    user.TelephoneNumber = result.Properties["telephoneNumber"].Count > 0 ? result.Properties["telephoneNumber"][0].ToString() : "";
                    user.Title = result.Properties["title"].Count > 0 ? result.Properties["title"][0].ToString() : "";
                    user.UserAccountControl = result.Properties["userAccountControl"].Count > 0 ? result.Properties["userAccountControl"][0].ToString() : "";
                    user.sam_account_name = result.Properties["sAMAccountName"].Count > 0 ? result.Properties["sAMAccountName"][0].ToString() : "";
                    user.street_address = result.Properties["streetAddress"].Count > 0 ? result.Properties["streetAddress"][0].ToString() : "";
                    user.country_Code = result.Properties["countryCode"].Count > 0 ? result.Properties["countryCode"][0].ToString() : "";


                    users.Add(user);

                }

                results.Dispose();
                searcher.Dispose();
                entry.Dispose();

                return users;
            }
            catch(Exception ex)
            {
                _logger.Log("Harvest GetComputerADData:" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                throw ex;
            }
        }

        private void ExecutePing(string type)
        {
            DataTable pingResult = new DataTable();

            DataTable dt = _iArmRepo.GetAssetData(type);

            if (dt.Rows.Count > 0)
            {
                pingResult = GetAllMachinePingData(dt);
                _iArmRepo.InsertBulkAssetData(pingResult);
                _iArmRepo.UpdateAssetStatus(type);
            }
            else
            {
                _logger.Log("Harvest : A_AssetHarvest Table has no Ping data to process", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

            }
        }
        private void ExecuteWmiData(string type)
        {
            DataTable wmiResult = new DataTable();

            DataTable dt = _iArmRepo.GetAssetData(type);

            if (dt != null)
            {
                wmiResult = GetAllMachineWmiData(dt);
                _iArmRepo.InsertBulkAssetData(wmiResult);
                _iArmRepo.UpdateAssetStatus(type);
            }
            else
            {
                _logger.Log("Harvest : A_AssetHarvest Table has no WMI data to process", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

            }
        }

        private DataTable GetAllMachinePingData(DataTable dt)
        {
            try
            {
                DataTable allMachinePingData = new DataTable();
                allMachinePingData.Columns.Add("HarvestID");
                allMachinePingData.Columns.Add("HarvestCollectionType");
                allMachinePingData.Columns.Add("HarvestValue");
                allMachinePingData.Columns.Add("HarvestDate");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow data = allMachinePingData.NewRow();

                    string pingStatus = GetPingStatus(dt.Rows[i]["AssetID"].ToString());

                    data["HarvestID"] = dt.Rows[i]["HarvestID"].ToString();
                    data["HarvestCollectionType"] = "Ping";
                    data["HarvestValue"] = pingStatus;
                    data["HarvestDate"] = DateTime.Now;
                    allMachinePingData.Rows.Add(data);

                }
                return allMachinePingData;
            }
            catch (Exception ex)
            {
                _logger.Log("Harvest GetAllMachinePingData:" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                throw ex;

            }
        }
        private DataTable GetAllMachineWmiData(DataTable dt)
        {
            try
            {
                DataTable allMachinePingData = new DataTable();
                allMachinePingData.Columns.Add("HarvestID");
                allMachinePingData.Columns.Add("HarvestCollectionType");
                allMachinePingData.Columns.Add("HarvestValue");
                allMachinePingData.Columns.Add("HarvestDate");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow data = allMachinePingData.NewRow();

                    allMachinePingData.Rows.Add(GetSerialNumber(dt.Rows[i]["AssetID"].ToString(), dt.Rows[i]["HarvestID"].ToString(), allMachinePingData.NewRow()));

                    allMachinePingData.Rows.Add(GetBiosVersion(dt.Rows[i]["AssetID"].ToString(), dt.Rows[i]["HarvestID"].ToString(), allMachinePingData.NewRow()));

                    allMachinePingData.Rows.Add(GetChessisType(dt.Rows[i]["AssetID"].ToString(), dt.Rows[i]["HarvestID"].ToString(), allMachinePingData.NewRow()));

                    allMachinePingData.Rows.Add(GetMemorycapacity(dt.Rows[i]["AssetID"].ToString(), dt.Rows[i]["HarvestID"].ToString(), allMachinePingData.NewRow()));

                    allMachinePingData.Rows.Add(GetHDSize(dt.Rows[i]["AssetID"].ToString(), dt.Rows[i]["HarvestID"].ToString(), allMachinePingData.NewRow()));

                    /*Add free space for HD*/

                    //allMachinePingData.Rows.Add(data);

                }
                return allMachinePingData;
            }
            catch (Exception ex)
            {
                _logger.Log("Harvest GetAllMachineWmiData:" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                throw ex;
            }

        }

        private string GetPingStatus(string machineName)
        {

            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(machineName);

            if (reply.Status == IPStatus.Success)
            {
                return "True";
            }
            else
            {
                return "False";
            }
        }

        private DataRow GetHDSize(string machineName, string harvestID, DataRow data)
        {
            string hdSize = "";
            string connectingMachineName = "\\\\" + machineName + "\\root\\cimv2";
            ConnectionOptions options = new ConnectionOptions();
            ManagementScope scope = new ManagementScope(connectingMachineName, options);
            scope.Connect();
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_DiskDrive");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (var disk in searcher.Get())
            {
                hdSize = disk.GetPropertyValue("Size").ToString();
            }

            data["HarvestID"] = harvestID;
            data["HarvestCollectionType"] = "HDS";
            data["HarvestValue"] = hdSize;
            data["HarvestDate"] = DateTime.Now;
            return data;
        }

        private DataRow GetMemorycapacity(string machineName, string harvestID, DataRow data)
        {
            string memoryCapacity = "";
            string connectingMachineName = "\\\\" + machineName + "\\root\\cimv2";
            ConnectionOptions options = new ConnectionOptions();
            ManagementScope scope = new ManagementScope(connectingMachineName, options);
            scope.Connect();
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (var disk in searcher.Get())
            {
                memoryCapacity = disk.GetPropertyValue("Capacity").ToString();
            }

            data["HarvestID"] = harvestID;
            data["HarvestCollectionType"] = "MC";
            data["HarvestValue"] = memoryCapacity;
            data["HarvestDate"] = DateTime.Now;
            return data;
        }

        private DataRow GetChessisType(string machineName, string harvestID, DataRow data)
        {
            string chassisType = "";
            string connectingMachineName = "\\\\" + machineName + "\\root\\cimv2";
            ConnectionOptions options = new ConnectionOptions();
            ManagementScope scope = new ManagementScope(connectingMachineName, options);
            scope.Connect();
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SystemEnclosure");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (var disk in searcher.Get())
            {
                Array x = (Array)disk.GetPropertyValue("ChassisTypes");
                foreach (var d in x)
                {
                    chassisType = d.ToString();
                }
            }

            data["HarvestID"] = harvestID;
            data["HarvestCollectionType"] = "CT";
            data["HarvestValue"] = chassisType;
            data["HarvestDate"] = DateTime.Now;
            return data;
        }

        private DataRow GetBiosVersion(string machineName, string harvestID, DataRow data)
        {
            string biosNameVersion = "";
            string connectingMachineName = "\\\\" + machineName + "\\root\\cimv2";
            ConnectionOptions options = new ConnectionOptions();
            ManagementScope scope = new ManagementScope(connectingMachineName, options);
            scope.Connect();
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_BIOS");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (var disk in searcher.Get())
            {
                biosNameVersion = disk.GetPropertyValue("Name").ToString();
            }

            data["HarvestID"] = harvestID;
            data["HarvestCollectionType"] = "BV";
            data["HarvestValue"] = biosNameVersion;
            data["HarvestDate"] = DateTime.Now;
            return data;
        }

        private DataRow GetSerialNumber(string machineName, string harvestID, DataRow data)
        {
            string serialNumber = "";
            string connectingMachineName = "\\\\" + machineName + "\\root\\cimv2";
            ConnectionOptions options = new ConnectionOptions();
            ManagementScope scope = new ManagementScope(connectingMachineName, options);
            scope.Connect();
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (var disk in searcher.Get())
            {
                serialNumber = disk.GetPropertyValue("SerialNumber").ToString();
            }

            data["HarvestID"] = harvestID;
            data["HarvestCollectionType"] = "SN";
            data["HarvestValue"] = serialNumber;
            data["HarvestDate"] = DateTime.Now;
            return data;
        }

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

    }
}
