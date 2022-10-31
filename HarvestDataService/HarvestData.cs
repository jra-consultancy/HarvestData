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
                ExecutePing(type = "Ping");
                ExecuteWmiData(type = "WMI");
            }
            catch (Exception ex)
            {
                _logger.Log("Harvest :" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

            }

        }

        private void ExecutePing(string type)
        {
            DataTable pingResult = new DataTable();

            DataTable dt = _iArmRepo.GetAssetData(type);

            if (dt != null)
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
                    for (int j = 0; j < 5; j++)
                    {
                        DataRow data = allMachinePingData.NewRow();

                        allMachinePingData.Rows.Add(GetSerialNumber(dt.Rows[i]["AssetID"].ToString(), dt.Rows[i]["HarvestID"].ToString(), data));

                        allMachinePingData.Rows.Add(GetBiosVersion(dt.Rows[i]["AssetID"].ToString(), dt.Rows[i]["HarvestID"].ToString(), data));

                        allMachinePingData.Rows.Add(GetChessisType(dt.Rows[i]["AssetID"].ToString(), dt.Rows[i]["HarvestID"].ToString(), data));

                        allMachinePingData.Rows.Add(GetMemorycapacity(dt.Rows[i]["AssetID"].ToString(), dt.Rows[i]["HarvestID"].ToString(), data));

                        allMachinePingData.Rows.Add(GetHDSize(dt.Rows[i]["AssetID"].ToString(), dt.Rows[i]["HarvestID"].ToString(), data));


                        //allMachinePingData.Rows.Add(data);
                    }

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
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (var disk in searcher.Get())
            {
                hdSize = disk.GetPropertyValue("Size").ToString();
            }

            data["HarvestID"] = harvestID;
            data["HarvestCollectionType"] = "WMI";
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
            data["HarvestCollectionType"] = "WMI";
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
            data["HarvestCollectionType"] = "WMI";
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
            data["HarvestCollectionType"] = "WMI";
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
            data["HarvestCollectionType"] = "WMI";
            data["HarvestValue"] = serialNumber;
            data["HarvestDate"] = DateTime.Now;
            return data;
        }

    }
}
