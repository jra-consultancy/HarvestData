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
using System.Reflection;
using System.Web.Hosting;
using System.ServiceProcess;

namespace HarvestDataService
{
    public class HarvestData
    {

        private IArmRepository _iArmRepo;
        private readonly ILogger _logger;
        private string UploadLogFile = "";
        private string type = "";
        private string serviceName = "CV Harvest Data Service";

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
                string version = GetServiceVersion(serviceName);
                _iArmRepo.InsertVersionNoIfNotFound(version);
                //ExecutePing(type = "Ping");
                //ExecuteWmiData(type = "WMI");
                ExecuteADData();
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
                string version = GetServiceVersion(serviceName);
                _iArmRepo.InsertVersionNoIfNotFound(version);
                ExecutePing(type = "Ping");
                //ExecuteWmiData(type = "WMI");
                ExecuteADData();
            }
            catch (Exception ex)
            {
                _logger.Log("Harvest :" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

            }

        }

        private string GetServiceVersion(string serviceName)
        {

            // Get the service controller for the specified service name
            Version serviceVersion = Assembly.GetExecutingAssembly().GetName().Version;

            return serviceVersion.ToString();
        }

        private void ExecuteADData()
        {
            List<Asset> assets = GetComputerADData();
            if (assets.Count() > 0)
            {
                _iArmRepo.InsertBulkAssetsADData(ConvertToDataTable(assets));

            }
            List<User> users = GetUserADData();
            if (users.Count() > 0)
            {
                _iArmRepo.InsertBulkUsersADData(ConvertToDataTable(users));

            }

        }

        private List<Asset> GetComputerADData()
        {
            try
            {
                string tempDomainValue = "";
                List<Asset> assets = new List<Asset>();

                DirectoryEntry objRootDSE = new DirectoryEntry("LDAP://RootDSE");
                string strDNSDomain = "";
                string tempstrDNSDomain = "";
                strDNSDomain = tempstrDNSDomain  = _iArmRepo.GetAD_Domain();
                if (String.IsNullOrEmpty(strDNSDomain))
                {
                    //strDNSDomain = objRootDSE.Properties["defaultNamingContext"].Value.ToString();
                    strDNSDomain = objRootDSE.Properties["rootDomainNamingContext"].Value.ToString();

                }
                string[] ldapPathComponents = strDNSDomain.Split(',');
                string domainName = "";
                string dc1 = "";
                string dc2 = "";
                string domainPath = "";

                string firstDomain = ldapPathComponents[0].Substring(3);

                for (int i = 0; i < ldapPathComponents.Length; i++)
                {
                    domainPath = null;
                    domainName = "";
                    dc1 = "";
                    dc1 = ldapPathComponents[i].Substring(3);
                    if (i + 1 == ldapPathComponents.Length)
                    {
                        if (ldapPathComponents.Length > 2)
                        {
                            domainName = firstDomain + "." + dc1;
                            domainPath = "LDAP://DC=" + firstDomain + ",DC=" + dc2;
                        }
                    }
                    else
                    {
                        dc2 = "";
                        dc2 = ldapPathComponents[i + 1].Substring(3);
                        domainName = dc1 + "." + dc2;
                        domainPath = "LDAP://DC=" + dc1 + ",DC=" + dc2;
                    }
                    //string domainPath = "LDAP://" + ldapPathComponents[i];
                    _logger.Log("Harvest GetComputerADData: Domain Path is " + domainPath, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

                    if (domainPath == null)
                    {
                        break;
                    }


                    //string domainPath = strTarget;
                    string searchFilter = "(&(objectCategory=computer))";
                    try
                    {
                        DirectoryEntry entry = new DirectoryEntry(domainPath);
                        DirectorySearcher searcher = new DirectorySearcher(entry, searchFilter);
                        searcher.PageSize = 1;
                        searcher.SizeLimit = 0;
                        searcher.PropertiesToLoad.Clear();
                        searcher.PropertiesToLoad.Add("cn");
                        SearchResultCollection results = searcher.FindAll();
                        int totalRecords = results.Count;

                        searcher.PropertiesToLoad.Clear();
                        searcher.PropertiesToLoad.AddRange(new string[] {
                        "cn", "whenCreated", "description", "displayName", "dNSHostName",
                        "userAccountControl", "eucDeviceType", "ipv4Address", "ipv6Address",
                        "isDeleted", "lastLogonTimestamp", "location", "lockoutTime",
                        "logonCount", "managedBy", "name", "operatingSystem",
                        "operatingSystemVersion", "pwdLastSet","objectGUID","distinguishedName",
                        "operatingSystemServicePack","whenChanged","servicePrincipalName","memberOf"
                                     });
                        searcher.PageSize = 6000;
                        int count = 0;
                        while (count < totalRecords)
                        {
                            results = searcher.FindAll();
                            if (results == null || results.Count == 0)
                            {
                                _logger.Log("Harvest GetComputerADData: No Result Found ", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                                break;
                            }
                            _logger.Log("Harvest GetComputerADData: Connection Established ", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

                            foreach (SearchResult result in results)
                            {
                                Asset asset = new Asset();
                                asset.AssetID = result.Properties["cn"].Count > 0 ? result.Properties["cn"][0].ToString() : "";
                                asset.WhenCreated = result.Properties["whenCreated"].Count > 0 ? (DateTime?)result.Properties["whenCreated"][0] : null;
                                asset.Description = result.Properties["description"].Count > 0 ? result.Properties["description"][0].ToString() : "";
                                asset.DisplayName = result.Properties["displayName"].Count > 0 ? result.Properties["displayName"][0].ToString() : "";
                                asset.DNSHostName = result.Properties["dNSHostName"].Count > 0 ? result.Properties["dNSHostName"][0].ToString() : "";
                                asset.Enabled = result.Properties["userAccountControl"].Count > 0 ? Convert.ToInt32(result.Properties["userAccountControl"][0]) != 0x0002 : false;
                                asset.EduDeviceType = result.Properties["eucDeviceType"].Count > 0 ? result.Properties["eucDeviceType"][0].ToString() : "";
                                asset.IPv4Address = result.Properties["ipv4Address"].Count > 0 ? result.Properties["ipv4Address"][0].ToString() : "";
                                asset.IPv6Address = result.Properties["ipv6Address"].Count > 0 ? result.Properties["ipv6Address"][0].ToString() : "";
                                asset.isDeleted = result.Properties["isDeleted"].Count > 0 ? (bool)result.Properties["isDeleted"][0] : false;
                                asset.LastLogonDate = result.Properties["lastLogonTimestamp"].Count > 0 ? DateTime.FromFileTime((long)result.Properties["lastLogonTimestamp"][0]) : (DateTime?)null;
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
                                asset.WhenChanged = result.Properties["whenChanged"].Count > 0 ? (DateTime?)result.Properties["whenChanged"][0] : null;
                                asset.ServicePrincipalName = result.Properties["servicePrincipalName"].Count > 0 ? result.Properties["servicePrincipalName"][0].ToString() : "";
                                asset.MemberOf = result.Properties["memberOf"].Count > 0 ? result.Properties["memberOf"][0].ToString() : "";
                                string distinguishedName = result.Properties["distinguishedName"][0].ToString();

                                string[] parts = distinguishedName.Split(',');

                                // iterate over the parts in reverse order and look for the "OU=" component
                                foreach (string part in parts.Reverse())
                                {
                                    if (part.StartsWith("OU="))
                                    {
                                        // if an OU component is found, extract the name and return it
                                        asset.OU = part.Substring(3);
                                        break;
                                    }
                                }

                                assets.Add(asset);
                                count++;

                            }
                            tempDomainValue = domainPath.Substring(7);
                            results.Dispose();
                        }

                        searcher.Dispose();
                        entry.Dispose();
                    }
                    catch (DirectoryServicesCOMException e)
                    {
                        _logger.Log("Harvest GetComputerADData: Domain Path is Invalid" + domainPath, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                    }
                }
                if (String.IsNullOrEmpty(tempstrDNSDomain))
                {
                    _iArmRepo.InsertAD_DomainName(tempDomainValue);
                }
                return assets;
            }
            catch (Exception ex)
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
                string strDNSDomain = "";
                strDNSDomain = _iArmRepo.GetAD_Domain();
                if (String.IsNullOrEmpty(strDNSDomain))
                {
                    //strDNSDomain = objRootDSE.Properties["defaultNamingContext"].Value.ToString();
                    strDNSDomain = objRootDSE.Properties["rootDomainNamingContext"].Value.ToString();

                }

                //string strDNSDomain = "DC=manufacture,DC=astellas,DC=net";
                string domainName = "";
                string domainPath = "";
                string dc1 = "";
                string dc2 = "";

                string[] ldapPathComponents = strDNSDomain.Split(',');
                string firstDomain = ldapPathComponents[0].Substring(3);

                for (int i = 0; i < ldapPathComponents.Length; i++)
                {
                    domainPath = null;
                    dc1 = "";
                    dc1 = ldapPathComponents[i].Substring(3);
                    if (i + 1 == ldapPathComponents.Length)
                    {
                        if (ldapPathComponents.Length > 2)
                        {
                            domainName = firstDomain + "." + dc1;
                            domainPath = "LDAP://DC=" + firstDomain + ",DC=" + dc2;

                        }

                    }
                    else
                    {
                        dc2 = "";
                        dc2 = ldapPathComponents[i + 1].Substring(3);
                        domainName = dc1 + "." + dc2;
                        domainPath = "LDAP://DC=" + dc1 + ",DC=" + dc2;

                    }
                    if (domainPath == null)
                    {
                        break;
                    }

                    _logger.Log("Harvest GetUserADData: Domain Path is " + domainPath, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                    string searchFilter = "(&(objectCategory=person)(objectClass=user)(!userAccountControl:1.2.840.113556.1.4.803:=2)(!samaccountname=Administrator)(!samaccountname=SYSTEM)(!description=Built-in account for administering the computer/domain))";
                    try
                    {
                        DirectoryEntry entry = new DirectoryEntry(domainPath);
                        DirectorySearcher searcher = new DirectorySearcher(entry, searchFilter);
                        // Count the total number of records in the Active Directory
                        searcher.PageSize = 1;
                        searcher.SizeLimit = 0;
                        searcher.PropertiesToLoad.Clear();
                        searcher.PropertiesToLoad.Add("cn");
                        SearchResultCollection results = searcher.FindAll();
                        int totalRecords = results.Count;

                        searcher.PropertiesToLoad.Clear();
                        searcher.PropertiesToLoad.AddRange(new string[] { "userPrincipalName", "AccountExpirationDate", "givenName", "company", "lastLogonTimestamp",
                    "department", "description", "displayName", "mail","employeeID","enabled","uSNCreated","logonCount","mailNickname",
                    "manager","PasswordExpired","physicalDeliveryOfficeName","postalCode","sn","telephoneNumber","title","userAccountControl",
                    "sAMAccountName","streetAddress","countryCode","distinguishedName"
                     });
                        searcher.PageSize = 6000;
                        int count = 0;


                        while (count < totalRecords)
                        {
                            results = searcher.FindAll();
                            if (results == null)
                            {
                                _logger.Log("Harvest GetUserADData: No Result Found ", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                                break;
                            }
                            _logger.Log("Harvest GetUserADData: Connection Established ", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

                            foreach (SearchResult result in results)
                            {
                                User user = new User();
                                user.UserId = result.Properties["userPrincipalName"].Count > 0 ? result.Properties["userPrincipalName"][0].ToString() : Convert.ToString(Guid.NewGuid());
                                user.AccountExpirationDate = result.Properties["AccountExpirationDate"].Count > 0 ? (DateTime?)result.Properties["AccountExpirationDate"][0] : null;
                                user.GivenName = result.Properties["givenName"].Count > 0 ? result.Properties["givenName"][0].ToString() : "";
                                user.CO = result.Properties["countryCode"].Count > 0 ? result.Properties["countryCode"][0].ToString() : "";
                                user.Company = result.Properties["company"].Count > 0 ? result.Properties["company"][0].ToString() : "";
                                user.CreateTimeStamp = result.Properties["whenCreated"].Count > 0 ? (DateTime?)result.Properties["whenCreated"][0] : null;
                                user.Department = result.Properties["department"].Count > 0 ? result.Properties["department"][0].ToString() : "";
                                user.Description = result.Properties["description"].Count > 0 ? result.Properties["description"][0].ToString() : "";
                                user.DisplayName = result.Properties["displayName"].Count > 0 ? result.Properties["displayName"][0].ToString() : "";
                                user.EmailAddress = result.Properties["mail"].Count > 0 ? result.Properties["mail"][0].ToString() : "";
                                user.EmployeeID = result.Properties["employeeID"].Count > 0 ? result.Properties["employeeID"][0].ToString() : "";
                                user.Enabled = result.Properties["enabled"].Count > 0 ? Convert.ToInt64(result.Properties["enabled"][0]) != 0 : false;
                                user.LastLogonDate = result.Properties["lastLogonTimestamp"].Count > 0 ? DateTime.FromFileTime((long)result.Properties["lastLogonTimestamp"][0]) : (DateTime?)null;
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
                                user.SamAccountName = result.Properties["sAMAccountName"].Count > 0 ? result.Properties["sAMAccountName"][0].ToString() : "";
                                user.StreetAddress = result.Properties["streetAddress"].Count > 0 ? result.Properties["streetAddress"][0].ToString() : "";
                                user.CountryCode = result.Properties["countryCode"].Count > 0 ? result.Properties["countryCode"][0].ToString() : "";
                                string distinguishedName = result.Properties["distinguishedName"][0].ToString();

                                string[] parts = distinguishedName.Split(',');

                                // iterate over the parts in reverse order and look for the "OU=" component
                                foreach (string part in parts.Reverse())
                                {
                                    if (part.StartsWith("OU="))
                                    {
                                        // if an OU component is found, extract the name and return it
                                        user.OU = part.Substring(3);
                                        break;
                                    }
                                }



                                users.Add(user);
                                count++;
                            }
                            results.Dispose();
                        }

                        searcher.Dispose();
                        entry.Dispose();
                    }
                    catch (DirectoryServicesCOMException e)
                    {
                        _logger.Log("Harvest GetUserADData: Domain Path is Invalid" + domainPath, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

                    }

                    //string strTarget = "LDAP://" + strDNSDomain;

                    //string domainPath = strTarget;//"LDAP://yourdomain.com"; // Replace with your domain name
                    //string searchFilter = "(&(objectCategory=person)(objectClass=user))";
                    //string searchFilter = "(&(objectCategory=person)(objectClass=user)(!userAccountControl:1.2.840.113556.1.4.803:=2))";

                }
                return users;

            }
            catch (Exception ex)
            {
                _logger.Log("Harvest GetUserADData:" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
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

        public static DataTable ConvertToDataTable<T>(IEnumerable<T> list)
        {
            var dataTable = new DataTable();
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            // create columns in the data table based on the properties of the object
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                dataTable.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
            }

            // add rows to the data table
            foreach (T obj in list)
            {
                DataRow dataRow = dataTable.NewRow();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    dataRow[propertyInfo.Name] = propertyInfo.GetValue(obj, null) ?? DBNull.Value;
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

    }
}
