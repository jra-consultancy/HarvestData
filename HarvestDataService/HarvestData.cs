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

namespace HarvestDataService
{
    public class HarvestData
    {

        private readonly IArmService _iArmService;
        private IArmRepository _iArmRepo;
        private readonly ILogger _logger;
        private string UploadLogFile = "";

        public HarvestData()
        {
            _iArmService = new ArmService();
            _iArmRepo = new ArmRepository();
            _logger = Logger.GetInstance;
            UploadLogFile = _iArmRepo.GetFileLocation(0);
        }
        private static readonly object Mylock = new object();
        public void Harvest(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Monitor.TryEnter(Mylock, 0)) return;
            DataTable pingResult = new DataTable();
            try
            {
                DataTable dt = _iArmRepo.GetAssetData();
                
                if (dt != null)
                {
                    pingResult = GetAllMachinePingData(dt);
                    _iArmRepo.InsertBulkAssetData(pingResult);
                }
                else
                {
                    _logger.Log("Harvest : A_AssetHarvest Table has no data to process", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

                }

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

        private DataTable GetAllMachinePingData(DataTable dt)
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

        public void Harvest()
        {
            DataTable pingResult = new DataTable();
            try
            {
                DataTable dt = _iArmRepo.GetAssetData();

                if (dt != null)
                {
                    pingResult = GetAllMachinePingData(dt);
                    _iArmRepo.InsertBulkAssetData(pingResult);
                    _iArmRepo.UpdateAssetStatus();
                }
                else
                {
                    _logger.Log("Harvest : A_AssetHarvest Table has no data to process", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

                }

            }
            catch (Exception ex)
            {
                _logger.Log("Harvest :" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

            }

        }


    }
}
