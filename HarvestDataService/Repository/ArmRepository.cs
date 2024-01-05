using HarvestDataService.Model;
using HarvestDataService.Service;
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
        private string value = "AD_Domain";
        private string HVersion = "Harvester_version";
        private string HVersionTime = "Harvester_InstalledDate";

        Logger4net log;
        public ArmRepository()
        {
            _connectionDB = new ConnectionDb();
            log = new Logger4net();
        }


        


        public DataTable GetAssetData(string type,int Cadence)
        {
            try
            {
                DataTable dtSource = new DataTable();
                string sourceTableQuery = GetGlobalProperties("HSpGetAHarvester");//"SP_GetAHarvester";
                using (SqlCommand cmd = new SqlCommand(sourceTableQuery, _connectionDB.con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.AddWithValue("@Type", type);
                        da.SelectCommand.Parameters.AddWithValue("@Cadence", Cadence);
                        da.Fill(dtSource);
                    }
                }
                return dtSource;
            }
            catch (Exception ex)
            {
                //_logger.Log("GetAssetData Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                log.PushLog("GetAssetData Exception: " + ex.Message + ex.InnerException, "ExecuteADData");
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

        //public void InsertBulkAssetData(DataTable dt)
        //{
        //    try
        //    {
        //        DataTable dtSource = new DataTable();
        //        string sourceTableQuery = "Select top 1 * from A_AssetHarvestResults";
        //        using (SqlCommand cmd = new SqlCommand(sourceTableQuery, _connectionDB.con))
        //        {
        //            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //            {
        //                da.Fill(dtSource);
        //            }
        //        }
        //        using (SqlBulkCopy bulk = new SqlBulkCopy(_connectionDB.con) { DestinationTableName = "A_AssetHarvestResults", BatchSize = 500000000, BulkCopyTimeout = 0 })
        //        {

        //            for (int i = 0; i < dt.Columns.Count; i++)
        //            {
        //                string destinationColumnName = dt.Columns[i].ToString();

        //                // check if destination column exists in source table 
        //                // Contains method is not case sensitive    
        //                if (dtSource.Columns.Contains(destinationColumnName))
        //                {
        //                    //Once column matched get its index
        //                    int sourceColumnIndex = dtSource.Columns.IndexOf(destinationColumnName);

        //                    string sourceColumnName = dtSource.Columns[sourceColumnIndex].ToString();

        //                    // give column name of source table rather then destination table 
        //                    // so that it would avoid case sensitivity
        //                    bulk.ColumnMappings.Add(sourceColumnName, sourceColumnName);
        //                }
        //            }
        //            _connectionDB.con.Open();
        //            bulk.WriteToServer(dt);
        //            dt.Clear();
        //            dt.Dispose();
        //            _connectionDB.con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //_logger.Log("InsertBulkAssetData Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
        //        log.PushLog("InsertBulkAssetData Exception: " + ex.Message + ex.InnerException, "InsertBulkAssetData");
        //        throw ex;
        //    }

        //    finally
        //    {
        //        if (_connectionDB.con.State == System.Data.ConnectionState.Open)
        //        {
        //            _connectionDB.con.Close();
        //        }
        //    }

        //}

        public void UpdateAssetStatus(string type)
        {
            string query = "UPDATE A_AssetHarvest SET STATUS = 1 WHERE STATUS = 0 AND HarvestType = @type";


            try
            {
                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    cmd.Parameters.AddWithValue("@type", type);
                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }
            }
            catch (Exception ex)
            {
                //_logger.Log("UpdateAssetStatus Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                log.PushLog("UpdateAssetStatus Exception: " + ex.Message + ex.InnerException, "UpdateAssetStatus");
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

        public void InsertBulkAssetsADData(DataTable assets)
        {
            try
            {
                string query = GetGlobalProperties("HSPForInsertAssetAD"); // "SP_Bulk_Insert_Update_Assets";

                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Data", assets);
                    //SqlParameter tvpParam = cmd.Parameters.AddWithValue("@Data", assets);
                    //tvpParam.SqlDbType = SqlDbType.Structured;
                    //tvpParam.TypeName = "dbo.AD_AssetTableType";
                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }
                //_logger.Log("InsertBulkAssetsADData Sucessfull", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                log.PushLog("InsertBulkAssetsADData Sucessfull", "");

            }
            catch (Exception ex)
            {
                //_logger.Log("InsertBulkAssetsADData Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                log.PushLog("InsertBulkAssetsADData Exception: " + ex.Message + ex.InnerException, "InsertBulkAssetsADData");
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

        public void InsertBulkUsersADData(DataTable users)
        {
            try
            {
                string query = GetGlobalProperties("HSpForInsertUserAD"); //"[dbo].[SP_Bulk_Insert_Update_Users]";

                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Data", users);
                    //SqlParameter tvpParam = cmd.Parameters.AddWithValue("@Data", users);
                    //tvpParam.SqlDbType = SqlDbType.Structured;
                    //tvpParam.TypeName = "dbo.AD_UserTableType";

                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }
                
                //_logger.Log("InsertBulkUsersADData Sucessfull", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                log.PushLog("InsertBulkUsersADData Sucessfull", "");

            }
            catch (Exception ex)
            {
                //_logger.Log("InsertBulkUsersADData Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                log.PushLog("InsertBulkUsersADData Exception: "+ ex.Message + ex.InnerException, "InsertBulkUsersADData");
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

        public void UpdateHarvestResult(DataTable Res, string Type)
        {
            try
            {
                string query = GetGlobalProperties("HSpHarvestResult"); //"[dbo].[SP_UpdateAHarvesterResult]";

                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Data", Res);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }

                log.PushLog("UpdateHarvestResult Sucessfull" + Type, "");

            }
            catch (Exception ex)
            {
                log.PushLog("UpdateHarvestResult Exception: " + Type + ex.Message + ex.InnerException, "UpdateHarvestResult");
            }
            finally
            {
                if (_connectionDB.con.State == System.Data.ConnectionState.Open)
                {
                    _connectionDB.con.Close();
                }
            }
        }


        public void ResetHarvestResult(string Type)
        {
            try
            {
                string query = "[dbo].[SP_ResetADHarvester]";

                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }

                log.PushLog("ResetHarvestResult Sucessfull" + Type, "");

            }
            catch (Exception ex)
            {
                log.PushLog("ResetHarvestResult Exception: " + Type + ex.Message + ex.InnerException, "ResetHarvestResult");
            }
            finally
            {
                if (_connectionDB.con.State == System.Data.ConnectionState.Open)
                {
                    _connectionDB.con.Close();
                }
            }
        }

        public string GetGlobalProperties(string propertyName)
        {

            string location = "";
            string sourceTableQuery = "Select top 1 PropertyValue from [SystemGlobalProperties] WHERE [PropertyName] = @propertyName";
            //string sourceTableQuery = "select [dbo].[fnGlobalProperty](@propertyName) AS PropertyValue";

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
                    eventLog.WriteEntry("CV Harvest Service Error Messege: " + ex.Message, EventLogEntryType.Error, 999, 1);
                }

                log.PushLog("GetGlobalProperties Exception: :" + ex.Message + ex.InnerException, "GetGlobalProperties");
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

        public string GetAD_Domain()
        {
            string sourceTableQuery = "select [dbo].[fnGlobalProperty](@propertyName) AS PropertyValue";

            string domainPath;
            try
            {
                _connectionDB.con.Open();
                using (SqlCommand cmd = new SqlCommand(sourceTableQuery, _connectionDB.con))
                {
                    cmd.Parameters.AddWithValue("@propertyName", value);

                    //var dr = cmd.ExecuteReader();
                    object result = cmd.ExecuteScalar();
                    if (result == DBNull.Value || result == "")
                    {
                        domainPath = null;
                    }
                    else
                    {
                        domainPath = (string)result;
                    }

                    //if (dr.Read()) // Read() returns TRUE if there are records to read, or FALSE if there is nothing
                    //{
                    //    location = dr["PropertyValue"].ToString();

                    //}

                }
                _connectionDB.con.Close();
                return domainPath;
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Harvest Service Error Messege: " + ex.Message, EventLogEntryType.Error, 999, 1);
                }
                //_logger.Log("GetFileLocation Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                log.PushLog("GetAdLocation Exception: " + ex.Message + ex.InnerException, "GetAdLocation");

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

        public void InsertVersionNoIfNotFound(string versionNo)
        {
            string sourceTableQuery = "select [dbo].[fnGlobalProperty](@propertyName) AS PropertyValue";

            string version;
            try
            {
                _connectionDB.con.Open();
                using (SqlCommand cmd = new SqlCommand(sourceTableQuery, _connectionDB.con))
                {
                    cmd.Parameters.AddWithValue("@propertyName", HVersion);
                    object result = cmd.ExecuteScalar();
                    if (result == DBNull.Value || result == "")
                    {
                        version = null;
                    }
                    else
                    {
                        version = (string)result;
                    }
                }
                _connectionDB.con.Close();
                if(String.IsNullOrEmpty(version) || version != versionNo)
                {
                    string sql = "UPDATE [dbo].[SystemGlobalProperties] SET PropertyValue = @versionNo WHERE PropertyName = @propertyName";
                    _connectionDB.con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, _connectionDB.con))
                    {
                        cmd.Parameters.AddWithValue("@versionNo", versionNo);
                        cmd.Parameters.AddWithValue("@propertyName", HVersion);
                        cmd.ExecuteNonQuery();
                    }
                    _connectionDB.con.Close();

                    string sql2 = "UPDATE [dbo].[SystemGlobalProperties] SET PropertyValue = @datetime WHERE PropertyName = @propertyName";
                    _connectionDB.con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql2, _connectionDB.con))
                    {
                        cmd.Parameters.AddWithValue("@datetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        cmd.Parameters.AddWithValue("@propertyName", HVersionTime);
                        cmd.ExecuteNonQuery();
                    }
                    _connectionDB.con.Close();
                }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Harvest Service Error Messege: " + ex.Message, EventLogEntryType.Error, 999, 1);
                }
                //_logger.Log("GetFileLocation Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                log.PushLog("InsertVersionNoIfNotFound :" + ex.Message + ex.InnerException, "InsertVersionNoIfNotFound");

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

        public void InsertAD_DomainName(string domainname)
        {
            string query = "UPDATE [dbo].[SystemGlobalProperties] SET [PropertyValue] = @domainName WHERE [PropertyName] = @value";


            try
            {
                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    cmd.Parameters.AddWithValue("@domainName", domainname);
                    cmd.Parameters.AddWithValue("@value", value);

                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }
            }
            catch (Exception ex)
            {
                //_logger.Log("UpdateAssetStatus Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                log.PushLog("InsertAD_DomainName :" + ex.Message + ex.InnerException, "InsertAD_DomainName");
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

        public void InsertBulkAD(DataTable Data, string TableName)
        {
            try
            {
                _connectionDB.con.Open();
                
                string query = BuildCreateTableScript(Data, TableName);
                
               // log.PushLog(query, "InsertAD_DomainName");
                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    cmd.ExecuteNonQuery();
                }

                //Open bulkcopy connection.
                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(_connectionDB.con))
                    {
                    bulkcopy.DestinationTableName = "dbo."+TableName;

                    foreach (DataColumn col in Data.Columns) {
                        bulkcopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkcopy.WriteToServer(Data);
                    }

                _connectionDB.con.Close();
                log.PushLog("InsertBulkAD Sucessfull", "");
            }
            catch (Exception ex)
            {
                log.PushLog("InsertBulkAD Exception: " + ex.Message + ex.InnerException, "InsertBulkAD");
            }
            finally
            {
                if (_connectionDB.con.State == System.Data.ConnectionState.Open)
                {
                    _connectionDB.con.Close();
                }
            }
        }


        public string BuildCreateTableScript(DataTable Table, string tableName)
        {

            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0} {1}", "IF OBJECT_ID('" + tableName + "', 'U') IS NOT NULL BEGIN DROP TABLE " + tableName + "; END ", Environment.NewLine);

            result.AppendFormat("CREATE TABLE [{0}] ( ", tableName);

            result.AppendFormat("[{0}] {1} {2} {3} {4}",
                    "ID", // 0
                    "[INT] ", // 1
                    "IDENTITY(1,1)",//2
                    "NOT NULL", // 3
                    Environment.NewLine // 4
                );
            result.Append("   ,");
            bool FirstTime = true;
            foreach (DataColumn column in Table.Columns.OfType<DataColumn>())
            {
                if (FirstTime) FirstTime = false;
                else
                    result.Append("   ,");

                result.AppendFormat("[{0}] {1} {2} {3}",
                    column.ColumnName.Trim(), // 0
                    GetSQLTypeAsString(column.DataType), // 1
                    column.AllowDBNull ? "NULL" : "NOT NULL", // 2
                    Environment.NewLine // 3
                );
            }
            result.AppendFormat(") ON [PRIMARY]{0}", Environment.NewLine);

            // Build an ALTER TABLE script that adds keys to a table that already exists.
            if (Table.PrimaryKey.Length > 0)
                result.Append(BuildKeysScript(Table));

            return result.ToString();
        }

        /// <summary>
        /// Builds an ALTER TABLE script that adds a primary or composite key to a table that already exists.
        /// </summary>
        private string BuildKeysScript(DataTable Table)
        {
            // Already checked by public method CreateTable. Un-comment if making the method public
            // if (Helper.IsValidDatatable(Table, IgnoreZeroRows: true)) return string.Empty;
            if (Table.PrimaryKey.Length < 1) return string.Empty;

            StringBuilder result = new StringBuilder();

            if (Table.PrimaryKey.Length == 1)
                result.AppendFormat("ALTER TABLE {1}{0}   ADD PRIMARY KEY ({2}){0}GO{0}{0}", Environment.NewLine, Table.TableName, Table.PrimaryKey[0].ColumnName);
            else
            {
                List<string> compositeKeys = Table.PrimaryKey.OfType<DataColumn>().Select(dc => dc.ColumnName).ToList();
                string keyName = compositeKeys.Aggregate((a, b) => a + b);
                string keys = compositeKeys.Aggregate((a, b) => string.Format("{0}, {1}", a, b));
                result.AppendFormat("ALTER TABLE {1}{0}ADD CONSTRAINT pk_{3} PRIMARY KEY ({2}){0}GO{0}{0}", Environment.NewLine, Table.TableName, keys, keyName);
            }

           
            return result.ToString();
        }

        /// <summary>
        /// Returns the SQL data type equivalent, as a string for use in SQL script generation methods.
        /// </summary>
        private  string GetSQLTypeAsString(Type DataType)
        {
            switch (DataType.Name)
            {
                case "Boolean": return "[bit]";
                case "Char": return "[char]";
                case "SByte": return "[tinyint]";
                case "Int16": return "[smallint]";
                case "Int32": return "[int]";
                case "Int64": return "[bigint]";
                case "Byte": return "[tinyint] UNSIGNED";
                case "UInt16": return "[smallint] UNSIGNED";
                case "UInt32": return "[int] UNSIGNED";
                case "UInt64": return "[bigint] UNSIGNED";
                case "Single": return "[float]";
                case "Double": return "[float]";
                case "Decimal": return "[decimal]";
                case "DateTime": return "[datetime]";
                case "Guid": return "[uniqueidentifier]";
                case "Object": return "[variant]";
                case "String": return "[nvarchar](max)";
                default: return "[nvarchar](MAX)";
            }
        }

    }
}
