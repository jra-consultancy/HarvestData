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
            GetUserADData();
            GetComputerADData();
        }

        private void GetComputerADData()
        {
            try
            {
                DirectoryEntry objRootDSE = new DirectoryEntry("LDAP://RootDSE");
                string strDNSDomain = objRootDSE.Properties["defaultNamingContext"].Value.ToString();
                string strTarget = "LDAP://" + strDNSDomain;

                string domainPath = strTarget;
                string searchFilter = "(&(objectCategory=computer))";
                string[] propertiesToLoad = new string[] {
                    "cn", "whenCreated", "description", "displayName", "dNSHostName",
                    "userAccountControl", "eucDeviceType", "ipv4Address", "ipv6Address",
                    "isDeleted", "lastLogonTimestamp", "location", "lockoutTime",
                    "logonCount", "managedBy", "name", "operatingSystem",
                    "operatingSystemVersion", "pwdLastSet"
                };

                DirectoryEntry entry = new DirectoryEntry(domainPath);
                DirectorySearcher searcher = new DirectorySearcher(entry, searchFilter, propertiesToLoad);
                SearchResultCollection results = searcher.FindAll();


                foreach (SearchResult result in results)
                {
                    // Get the specified properties for the computer object
                    string cn = result.Properties["cn"].Count > 0 ? result.Properties["cn"][0].ToString() : "";
                    DateTime created = result.Properties["whenCreated"].Count > 0 ? (DateTime)result.Properties["whenCreated"][0] : DateTime.MinValue;
                    string description = result.Properties["description"].Count > 0 ? result.Properties["description"][0].ToString() : "";
                    string displayName = result.Properties["displayName"].Count > 0 ? result.Properties["displayName"][0].ToString() : "";
                    string dnsHostName = result.Properties["dNSHostName"].Count > 0 ? result.Properties["dNSHostName"][0].ToString() : "";
                    bool enabled = result.Properties["userAccountControl"].Count > 0 ? Convert.ToInt32(result.Properties["userAccountControl"][0]) != 0x0002 : false;
                    string eucDeviceType = result.Properties["eucDeviceType"].Count > 0 ? result.Properties["eucDeviceType"][0].ToString() : "";
                    string ipv4Address = result.Properties["ipv4Address"].Count > 0 ? result.Properties["ipv4Address"][0].ToString() : "";
                    string ipv6Address = result.Properties["ipv6Address"].Count > 0 ? result.Properties["ipv6Address"][0].ToString() : "";
                    bool isDeleted = result.Properties["isDeleted"].Count > 0 ? (bool)result.Properties["isDeleted"][0] : false;
                    DateTime lastLogonDate = result.Properties["lastLogonTimestamp"].Count > 0 ? DateTime.FromFileTime((long)result.Properties["lastLogonTimestamp"][0]) : DateTime.MinValue;
                    string location = result.Properties["location"].Count > 0 ? result.Properties["location"][0].ToString() : "";
                    bool lockedOut = result.Properties["lockoutTime"].Count > 0 ? Convert.ToInt64(result.Properties["lockoutTime"][0]) != 0 : false;
                    int logonCount = result.Properties["logonCount"].Count > 0 ? Convert.ToInt32(result.Properties["logonCount"][0]) : 0;
                    string managedBy = result.Properties["managedBy"].Count > 0 ? result.Properties["managedBy"][0].ToString() : "";
                    string name = result.Properties["name"].Count > 0 ? result.Properties["name"][0].ToString() : "";
                    string operatingSystem = result.Properties["operatingSystem"].Count > 0 ? result.Properties["operatingSystem"][0].ToString() : "";
                    string operatingSystemVersion = result.Properties["operatingSystemVersion"].Count > 0 ? result.Properties["operatingSystemVersion"][0].ToString() : "";
                }
                results.Dispose();
                searcher.Dispose();
                entry.Dispose();
            }
            catch(Exception ex)
            {
                _logger.Log("Harvest GetComputerADData:" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                throw ex;
            }
           
        }

        private void GetUserADData()
        {
            try
            {
                DirectoryEntry objRootDSE = new DirectoryEntry("LDAP://RootDSE");
                string strDNSDomain = objRootDSE.Properties["defaultNamingContext"].Value.ToString();
                string strTarget = "LDAP://" + strDNSDomain;
                Console.WriteLine("Starting search from " + strTarget);

                string domainPath = strTarget;//"LDAP://yourdomain.com"; // Replace with your domain name
                string searchFilter = "(&(objectCategory=person)(objectClass=user))";
                string[] propertiesToLoad = new string[] { "c", "userAccountControl", "cn", "givenName", "sn", "mail", "whenCreated", "displayname", "lastname", "surname" };

                DirectoryEntry entry = new DirectoryEntry(domainPath);
                DirectorySearcher searcher = new DirectorySearcher(entry, searchFilter, propertiesToLoad);
                SearchResultCollection results = searcher.FindAll();

                foreach (SearchResult result in results)
                {
                    string givenName = result.Properties["givenName"].Count > 0 ? result.Properties["givenName"][0].ToString() : "";
                    string firstName = result.Properties["cn"].Count > 0 ? result.Properties["cn"][0].ToString() : "";
                    string sn = result.Properties["sn"].Count > 0 ? result.Properties["sn"][0].ToString() : "";
                    string mail = result.Properties["mail"].Count > 0 ? result.Properties["mail"][0].ToString() : "";
                    string c = result.Properties["c"].Count > 0 ? result.Properties["c"][0].ToString() : "";
                    string userAccountControl = result.Properties["userAccountControl"].Count > 0 ? result.Properties["userAccountControl"][0].ToString() : "";
                    string whenCreated = result.Properties["whenCreated"].Count > 0 ? result.Properties["whenCreated"][0].ToString() : "";
                    string displayname = result.Properties["displayname"].Count > 0 ? result.Properties["displayname"][0].ToString() : "";
                    string lastname = result.Properties["lastname"].Count > 0 ? result.Properties["lastname"][0].ToString() : "";
                    string surname = result.Properties["surname"].Count > 0 ? result.Properties["surname"][0].ToString() : "";

                }

                results.Dispose();
                searcher.Dispose();
                entry.Dispose();
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

    }
}
