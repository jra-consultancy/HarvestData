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
            Connection objConnection = new Connection();
            Command objCmd = new Command();
            Recordset objRecordSet = new Recordset();
            string outFile = "";
            bool Appending = false;

            //if (args.Length > 1)
            //{
            //    outFile = args[1] + ".txt";
            //}
            //else
            //{
            //    outFile = args[0] + ".txt";
            //}

            //if (File.Exists(outFile))
            //{
            //    Appending = true;
            //}

            using (StreamWriter objFile = new StreamWriter(outFile, Appending))
            {
                DirectoryEntry objRootDSE = new DirectoryEntry("LDAP://RootDSE");
                string strDNSDomain = objRootDSE.Properties["defaultNamingContext"].Value.ToString();
                string strTarget = "LDAP://" + strDNSDomain;
                Console.WriteLine("Starting search from " + strTarget);

                objConnection.Provider = "ADsDSOObject";
                objConnection.Open();
                objCmd.ActiveConnection = objConnection;
                objCmd.Properties["Page Size"].Value = 100;
                objCmd.Properties["Timeout"].Value = 30;
                objCmd.Properties["Searchscope"].Value = SearchScope.Subtree;
                objCmd.Properties["Cache Results"].Value = false;
                objCmd.CommandText = "SELECT c, userAccountControl, CN, givenName, SN, mail, whenCreated, department, displayname FROM '" + strTarget + "' WHERE objectClass = 'user' and objectCategory = 'person' and Name <> 'ExcelExcellence''";

                objRecordSet = objCmd.Execute(out object _, int.MaxValue, (int)CommandTypeEnum.adCmdText);
                if (Appending)
                {
                    Console.WriteLine("Appending output file " + outFile + ". Please wait....");
                }
                else
                {
                    Console.WriteLine("Creating output file " + outFile + ". Please wait....");
                    objFile.WriteLine("UserID\tEmail\tFirstName\tLastName\tCountryCode\tCountry\tDepartment\tCreatedOn\tManager\tuserAccountControl\tphysicalDeliveryOfficeName\ttitle\tDisplayName\ttelephone");
                }

                objRecordSet.MoveFirst();
                while (!objRecordSet.EOF)
                {
                    string SC = objRecordSet.Fields["c"].Value.ToString();
                    string SCN = objRecordSet.Fields["CN"].Value.ToString();
                    string SFN = objRecordSet.Fields["givenName"].Value.ToString();
                    string SSN = objRecordSet.Fields["SN"].Value.ToString();
                    string SMAIL = objRecordSet.Fields["mail"].Value.ToString();
                    string SWC = objRecordSet.Fields["WhenCreated"].Value.ToString();
                    string OFFICE = objRecordSet.Fields["physicalDeliveryOfficeName"].Value.ToString();
                    string DEPT = objRecordSet.Fields["department"].Value.ToString();
                    string CO = objRecordSet.Fields["co"].Value.ToString();
                    string userAccountControl = objRecordSet.Fields["userAccountControl"].Value.ToString();
                    string JobTitle = objRecordSet.Fields["title"].Value.ToString();
                    string accExp = objRecordSet.Fields["accountExpires"].Value.ToString();
                    string lastLogon = objRecordSet.Fields["lastLogon"].Value.ToString();
                    string Tel = objRecordSet.Fields["telephoneNumber"].Value.ToString();
                    string DisplayName = objRecordSet.Fields["DisplayName"].Value.ToString();
                    string PO = objRecordSet.Fields["postalCode"].Value.ToString();
                    string ManagerUserID = objRecordSet.Fields["manager"].Value.ToString();



                    objFile.Write("" + SCN);
                    objFile.Write("\t" + SMAIL);
                    objFile.Write("\t" + SFN);
                    objFile.Write("\t" + SSN);
                    objFile.Write("" + SC);
                    objFile.Write("\t" + CO);
                    objFile.Write("\t" + DEPT);
                    objFile.Write("\t" + SWC);
                    objFile.Write("" + ManagerUserID);
                    objFile.Write("\t" + userAccountControl);
                    objFile.Write("\t" + OFFICE);
                    objFile.Write("\t" + JobTitle);
                    objFile.Write("" + DisplayName);
                    objFile.WriteLine("\t" + Tel);

                    objRecordSet.MoveNext();
                }
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
