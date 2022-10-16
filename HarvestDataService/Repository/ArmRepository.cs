using HarvestDataService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HarvestDataService.Model.Keys;

namespace HarvestDataService
{


    public class ArmRepository : IArmRepository
    {
        private ConnectionDb _connectionDB;
        private readonly ILogger _logger;
        private string UploadLogFile = "";
        public ArmRepository()
        {
            _logger = Logger.GetInstance;

            _connectionDB = new ConnectionDb();
            UploadLogFile = GetFileLocation(0);
        }


        public string GetFileLocation(int Key)
        {
            //use condition for key and set property 
            string propertyName = "";
            if (Key == 0)
            {
                propertyName = Enum.GetName(typeof(KeyNames), 0);
            }

           

            string location = "";
            //string sourceTableQuery = "Select PropertyValue from [SystemGlobalProperties] WHERE [PropertyName] = @propertyName";
            string sourceTableQuery = "select [dbo].[fnGlobalProperty](@propertyName) AS PropertyValue";

            try
            {
                _connectionDB.con.Open();
                using (SqlCommand cmd = new SqlCommand(sourceTableQuery, _connectionDB.con))
                {
                    cmd.Parameters.AddWithValue("@propertyName", propertyName);

                    //var dr = cmd.ExecuteReader();
                    location = (string)cmd.ExecuteScalar();

                    //if (dr.Read()) // Read() returns TRUE if there are records to read, or FALSE if there is nothing
                    //{
                    //    location = dr["PropertyValue"].ToString();

                    //}

                }
                _connectionDB.con.Close();
                return location;
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("CV Upload Service Error Messege: " + ex.Message, EventLogEntryType.Error, 999, 1);
                }
                _logger.Log("GetFileLocation Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

                throw ex;
            }
            finally
            {
                if (_connectionDB.con.State == System.Data.ConnectionState.Open)
                {
                    _connectionDB.con.Close();
                }
            }

        }


        public DataTable GetAssetData()
        {
            DataTable dtSource = new DataTable();
            string sourceTableQuery = "Select HarvestID,AssetID from A_AssetHarvest WHERE Status = 0 AND HarvestType = @Ping";
            using (SqlCommand cmd = new SqlCommand(sourceTableQuery, _connectionDB.con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Ping", "Ping");
                    da.Fill(dtSource);
                }
            }
            return dtSource;
        }

        public void InsertBulkAssetData(DataTable dt)
        {
            DataTable dtSource = new DataTable();
            string sourceTableQuery = "Select top 1 * from A_AssetHarvestResults";
            using (SqlCommand cmd = new SqlCommand(sourceTableQuery, _connectionDB.con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtSource);
                }
            }
            using (SqlBulkCopy bulk = new SqlBulkCopy(_connectionDB.con) { DestinationTableName = "A_AssetHarvestResults", BatchSize = 500000000, BulkCopyTimeout = 0 })
            {

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string destinationColumnName = dt.Columns[i].ToString();

                    // check if destination column exists in source table 
                    // Contains method is not case sensitive    
                    if (dtSource.Columns.Contains(destinationColumnName))
                    {
                        //Once column matched get its index
                        int sourceColumnIndex = dtSource.Columns.IndexOf(destinationColumnName);

                        string sourceColumnName = dtSource.Columns[sourceColumnIndex].ToString();

                        // give column name of source table rather then destination table 
                        // so that it would avoid case sensitivity
                        bulk.ColumnMappings.Add(sourceColumnName, sourceColumnName);
                    }
                }
                _connectionDB.con.Open();
                bulk.WriteToServer(dt);
                dt.Clear();
                dt.Dispose();
                _connectionDB.con.Close();
            }
        }

        public void UpdateAssetStatus()
        {
            string query = "UPDATE A_AssetHarvest SET STATUS = 1 WHERE STATUS = 0 AND HarvestType = @Ping";


            try
            {
                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    cmd.Parameters.AddWithValue("@Ping", "Ping");
                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Log("UpdateAssetStatus Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                //throw ex;
            }
            finally
            {
                if (_connectionDB.con.State == System.Data.ConnectionState.Open)
                {
                    _connectionDB.con.Close();
                }
            }
        }
    }
}
