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
using System.DirectoryServices.ActiveDirectory;
using HarvestDataService.Service;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Support.UI;

namespace HarvestDataService
{
    public class HarvestData
    {

        private IArmRepository _iArmRepo;
        private string type = "";
        private string serviceName = "CV Harvest Data Service";
        Logger4net log;

        int CurrentHour = 1;

        private bool isPingAssetRun = false;
        private bool isWMIAssetRun = false;

        private bool IsWarrantyAssetRun = false;

        private bool isDevMode = false;

        public HarvestData()
        {
            _iArmRepo = new ArmRepository();
            log  = new Logger4net();
            
            isPingAssetRun = false;

            //log.PushLog("Komang Here", "TesLog");
        }
        private static readonly object Mylock = new object();

        #region Method to call in service 
        
        public void Harvest(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Monitor.TryEnter(Mylock, 0)) return;
            try
            {
                string version = GetServiceVersion(serviceName);
                _iArmRepo.InsertVersionNoIfNotFound(version);
                ExecuteADData();
            }
            catch (Exception ex)
            {
                log.PushLog("Harvest :" + ex.Message + ex.InnerException, "ExecuteADData");

            }
            finally
            {
                Monitor.Exit(Mylock);
            }


        }

        public void PingAssetAsync(object sender, System.Timers.ElapsedEventArgs e)
        {
  
            try
            {
                PingAsset();
            }
            catch (Exception ex)
            {
                log.PushLog("PingAsset :" + ex.Message + ex.InnerException, "PingAsset");
            }
            


        }

        public void WMIAssetAsync(object sender, System.Timers.ElapsedEventArgs e)
        {

            try
            {
                GetWMIAsset();
            }
            catch (Exception ex)
            {
                log.PushLog("WMIAssetAsync :" + ex.Message + ex.InnerException, "WMIAssetAsync");
            }
          


        }

        public void WarrantyAssetAsync(object sender, System.Timers.ElapsedEventArgs e)
        {

            try
            {
                GetWarrantyAsset();
            }
            catch (Exception ex)
            {
                log.PushLog("WarrantyAssetAsync :" + ex.Message + ex.InnerException, "WarrantyAssetAsync");
            }



        }

        public void ResetPingAssetAsync(object sender, System.Timers.ElapsedEventArgs e)
        {

            try
            {
                _iArmRepo.ResetHarvestResult("Ping");
            }
            catch (Exception ex)
            {
                log.PushLog("ResetPingAssetAsync :" + ex.Message + ex.InnerException, "ResetPingAssetAsync");
            }
     
        }

        #endregion

        /// <summary>
        ///  add by komang
        /// </summary>

        #region Controler Method
       
        public void PingAsset() {

            DateTime now = DateTime.Now;
            //if (now.Minute > 10 && now.Minute <= 15)
            if ((now.Minute > 30 && now.Minute <= 35) || isDevMode )
            {
                
            }
            else {

                return;
            }

            if (isPingAssetRun)
            {
                return;
            }

            try
            {
                isPingAssetRun = true;
                ExecutePing(type = "Ping", now.Hour+1);

                isPingAssetRun = false;
            }
            catch (Exception ex)
            {
                isPingAssetRun = false;
                log.PushLog("PingAsset :" + ex.Message + ex.InnerException, "PingAsset");

            }
        }
        public void GetWMIAsset() {

            DateTime now = DateTime.Now;
            //if (now.Minute > 10 && now.Minute <= 15)

            if ((now.Minute > 30 && now.Minute <= 35) || isDevMode)
            {
            }
            else
            {
                //log.PushLog("GetWMIAsset : Is Not Time Yet", "");
                return;
            }

            if (isWMIAssetRun)
            {
                //log.PushLog("GetWMIAsset : last process is running", "");
                return;
            }

            try
            {
                isWMIAssetRun = true;
                ExecuteWmiData(type = "WMI", now.Hour + 1);
                isWMIAssetRun = false;
            }
            catch (Exception ex)
            {
                isWMIAssetRun = false;
                log.PushLog("GetWMIAsset :" + ex.Message + ex.InnerException, "GetWMIAsset");

            }

        }

        public void GetWarrantyAsset()
        {
            DateTime now = DateTime.Now;
            //if (now.Minute > 10 && now.Minute <= 15)
            if ((now.Minute > 30 && now.Minute <= 35) || isDevMode)
            {

            }
            else
            {
                return;
            }

            if (IsWarrantyAssetRun)
            {
                return;
            }

            try
            {
                IsWarrantyAssetRun = true;
                ///komang aranty have bug ill be fix
                //ExecuteWarranty(type = "Warranty", now.Hour + 1);

                IsWarrantyAssetRun = false;
            }
            catch (Exception ex)
            {
                IsWarrantyAssetRun = false;
                log.PushLog("ExecuteWarranty :" + ex.Message + ex.InnerException, "ExecuteWarranty");

            }
        }

        public void Harvest()
        {
            try
            {
                string version = GetServiceVersion(serviceName);
                _iArmRepo.InsertVersionNoIfNotFound(version);
                ExecuteADData();
            }
            catch (Exception ex)
            {
                log.PushLog("Harvest :" + ex.Message + ex.InnerException, "ExecuteADData");

            }

        }
        
        #endregion


        private string GetServiceVersion(string serviceName)
        {

            // Get the service controller for the specified service name
            Version serviceVersion = Assembly.GetExecutingAssembly().GetName().Version;

            return serviceVersion.ToString();
        }

        #region AD
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
                List<string> LDomain = new List<string>();

                DirectoryEntry objRootDSE = new DirectoryEntry("LDAP://RootDSE");
                string strDNSDomain = "";
                string domainPath = "";
                string PathAD = "";
                strDNSDomain =  _iArmRepo.GetAD_Domain();

                // komang 04-23-2023 get root domain for full path AD
                Forest currentForest = Forest.GetCurrentForest();
                Domain RootDomain = currentForest.RootDomain;

                if (String.IsNullOrEmpty(strDNSDomain))
                {
                    strDNSDomain = objRootDSE.Properties["defaultNamingContext"].Value.ToString();
                    //strDNSDomain = objRootDSE.Properties["rootDomainNamingContext"].Value.ToString();

                    _iArmRepo.InsertAD_DomainName(strDNSDomain);

                    // komang 04-23-2023 get root domain for full path AD
                    //PathAD = "LDAP://" + RootDomain.Name + "/" + strDNSDomain;
                    //strDNSDomain = PathAD;

                    LDomain.Add(strDNSDomain);
                }
                else {
                    LDomain.Add(strDNSDomain);
                    //LDomain = SplitDomain(strDNSDomain);
                }

                //if (strDNSDomain.Contains("LDAP://"))
                //{
                //    domainPath = strDNSDomain;
                //}
                //else
                //{
                //    domainPath = "LDAP://" + strDNSDomain;

                //}

                //_logger.Log("Harvest GetComputerADData: Domain Path is " + domainPath, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy"))) // (|(!(userAccountControl:1.2.840.113556.1.4.803:=2))(whenChanged>=" + thirtyDaysAgo.ToString("yyyyMMddHHmmss.0Z") + "));
                
                DateTime thirtyDaysAgo = DateTime.Today.AddDays(-30);
                string searchFilter = "(&(objectCategory=computer)(!userAccountControl:1.2.840.113556.1.4.803:=2))";

                foreach (string DomainName in LDomain) {

                    if (DomainName == null || DomainName == "")
                    {
                        continue;
                    }

                    if (DomainName.Contains("LDAP://"))
                    {
                        domainPath = DomainName;
                    }
                    else
                    {
                        domainPath = "LDAP://" + DomainName;
                    }

                    log.PushLog("Harvest GetComputerADData: Domain Path is " + domainPath, "");

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
                        //_logger.Log("Harvest GetComputerADData: Records found: " + totalRecords, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                        log.PushLog("Harvest GetComputerADData: Records found: " + totalRecords, "");

                        while (count < totalRecords)
                        {
                            results = searcher.FindAll();
                            if (results == null || results.Count == 0)
                            {
                                //_logger.Log("Harvest GetComputerADData: No Result Found ", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                                log.PushLog("Harvest GetComputerADData: No Result Found ", "");
                                break;
                            }

                            //_logger.Log("Harvest GetComputerADData: Connection Established ", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                            log.PushLog("Harvest GetComputerADData: Connection Established ", "");
                            foreach (SearchResult result in results)
                            {
                                Asset asset = new Asset();
                                asset.AssetID = result.Properties["cn"].Count > 0 ? result.Properties["cn"][0].ToString() : "";
                                asset.WhenCreated = result.Properties["whenCreated"].Count > 0 ? (DateTime?)result.Properties["whenCreated"][0] : null;
                                asset.Description = result.Properties["description"].Count > 0 ? result.Properties["description"][0].ToString() : "";
                                asset.DisplayName = result.Properties["displayName"].Count > 0 ? result.Properties["displayName"][0].ToString() : "";
                                asset.DNSHostName = result.Properties["dNSHostName"].Count > 0 ? result.Properties["dNSHostName"][0].ToString() : "";
                                //asset.Enabled = result.Properties["userAccountControl"].Count > 0 ? Convert.ToInt32(result.Properties["userAccountControl"][0]) != 0x0002 : false;

                                if (result.Properties["userAccountControl"].Count > 0)
                                {
                                    int userAccountControl = (int)result.Properties["userAccountControl"][0];
                                    int AccountDisabled = 0x0002;
                                    asset.Enabled = (userAccountControl & AccountDisabled) != AccountDisabled;
                                }
                                else
                                {
                                    asset.Enabled = false;
                                }

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

                                try
                                {
                                    if (result.Properties["objectGUID"].Count > 0)
                                    {

                                        byte[] objbuf = (byte[])result.Properties["objectGUID"][0];
                                        asset.ObjectGUID = new Guid(objbuf).ToString();
                                    }
                                    else
                                    {
                                        asset.ObjectGUID = "";
                                    }
                                }
                                catch { asset.ObjectGUID = ""; }

                                asset.UserAccountControl = result.Properties["userAccountControl"].Count > 0 ? result.Properties["userAccountControl"][0].ToString() : "";
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
                    catch (DirectoryServicesCOMException asc)
                    {
                        //_logger.Log("Harvest GetComputerADData: Domain Path is Invalid" + domainPath, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                        log.PushLog("Harvest GetComputerADData: Domain Path is Invalid " + domainPath + " " + asc.Message + asc.InnerException, "GetComputerADData");

                    }

                }

                return assets;
            }
            catch (Exception ex)
            {
                //_logger.Log("Harvest GetComputerADData:" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                log.PushLog("Harvest :" + ex.Message + ex.InnerException, "GetComputerADData");
                throw ex;
            }

        }

        private List<User> GetUserADData()
        {
            try
            {
                List<User> users = new List<User>();
                
                List<string> LDomain = new List<string>();

                DirectoryEntry objRootDSE = new DirectoryEntry("LDAP://RootDSE");
                string strDNSDomain = "";
                string domainPath = "";
                strDNSDomain = _iArmRepo.GetAD_Domain();

                // komang 04-23-2023 get root domain for full path AD
                string PathAD = "";
                Forest currentForest = Forest.GetCurrentForest();
                Domain RootDomain = currentForest.RootDomain;

                if (String.IsNullOrEmpty(strDNSDomain))
                {
                    strDNSDomain = objRootDSE.Properties["defaultNamingContext"].Value.ToString();

                    //strDNSDomain = objRootDSE.Properties["rootDomainNamingContext"].Value.ToString();

                    // komang 04-23-2023 get root domain for full path AD
                    //PathAD = "LDAP://" + RootDomain.Name + "/" + strDNSDomain;
                    //strDNSDomain = PathAD;

                    LDomain.Add(strDNSDomain);
                }
                else {

                    LDomain.Add(strDNSDomain);

                    //LDomain = SplitDomain(strDNSDomain);
                }

                //_logger.Log("Harvest GetUserADData: Domain Path is " + domainPath, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

                
                
                DateTime thirtyDaysAgo = DateTime.Today.AddDays(-30);//(whenChanged>="+thirtyDaysAgo.ToString("yyyyMMddHHmmss.0Z")+") (|(userAccountControl:1.2.840.113556.1.4.803:=2)(whenChanged>=" + thirtyDaysAgo.ToString("yyyyMMddHHmmss.0Z") + "))
                string searchFilter = "(&(objectCategory=person)(objectClass=user)(!samaccountname=Administrator)(!samaccountname=SYSTEM)(!description=Built-in account for administering the computer/domain)(!userAccountControl:1.2.840.113556.1.4.803:=2))";

                foreach (string DomainName in LDomain) {

                    if (DomainName == null || DomainName == "") {
                        continue;
                    }

                    if (DomainName.Contains("LDAP://"))
                    {
                        domainPath = DomainName;
                    }
                    else
                    {
                        domainPath = "LDAP://" + DomainName;
                    }

                    log.PushLog("Harvest GetUserADData: Domain Path is " + domainPath, "");
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

                        //_logger.Log("Harvest GetUserADData: Records found: " + totalRecords, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                        log.PushLog("Harvest GetUserADData: Records found: " + totalRecords, "");

                        while (count < totalRecords)
                        {
                            results = searcher.FindAll();
                            if (results == null)
                            {
                                //_logger.Log("Harvest GetUserADData: No Result Found ", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                                log.PushLog("Harvest GetUserADData: No Result Found ", "");
                                break;
                            }
                            //_logger.Log("Harvest GetUserADData: Connection Established ", UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                            log.PushLog("Harvest GetUserADData: Connection Established ", "");

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
                                //user.Enabled = result.Properties["enabled"].Count > 0 ? Convert.ToInt64(result.Properties["enabled"][0]) != 0 : false;

                                if (result.Properties["userAccountControl"].Count > 0)
                                {
                                    int userAccountControl = (int)result.Properties["userAccountControl"][0];
                                    int AccountDisabled = 0x0002;
                                    user.Enabled = (userAccountControl & AccountDisabled) != AccountDisabled;
                                }
                                else
                                {
                                    user.Enabled = false;
                                }

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
                    catch (DirectoryServicesCOMException asc)
                    {
                        //_logger.Log("Harvest GetUserADData: Domain Path is Invalid" + domainPath, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                        log.PushLog("Harvest GetUserADData: Domain Path is Invalid " + domainPath + " " + asc.Message + asc.InnerException, "GetUserADData");
                    }
                }
                
                return users;

            }
            catch (Exception ex)
            {
                //_logger.Log("Harvest GetUserADData:" + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                log.PushLog("Harvest GetUserADData:" + ex.Message + ex.InnerException, "GetUserADData");
                throw ex;
            }
        }

        private List<Asset> GetComputerADDataOLD()
        {
            try
            {
                string tempDomainValue = "";
                List<Asset> assets = new List<Asset>();

                DirectoryEntry objRootDSE = new DirectoryEntry("LDAP://RootDSE");
                string strDNSDomain = "";
                string tempstrDNSDomain = "";
                strDNSDomain = tempstrDNSDomain = _iArmRepo.GetAD_Domain();
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
                                 break;
                            }
                            
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
                 throw ex;
            }

        }

        private static List<string> SplitDomain(string strDNSDomain)
        {
            List<string> Ldomain = new List<string>();

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
                Ldomain.Add(domainPath);
            }
            return Ldomain;
        }

        #endregion
        private void ExecutePing(string type,int Cadence)
        {
            List<AD_HarvesterResult> pingResult = new List<AD_HarvesterResult>();

            DataTable dt = _iArmRepo.GetAssetData(type, Cadence);


            if (dt.Rows.Count > 0)
            {
                log.PushLog("PingAsset : " + dt.Rows.Count.ToString() + " Assets , Cadence at " + Cadence.ToString(), "");

                pingResult =  GetAllMachinePingData(dt);

                _iArmRepo.UpdateHarvestResult(ConvertToDataTable(pingResult), type);

                List<AD_HarvesterResult> IsPingSuccessList = pingResult
                                        .Where(m => m.IsPingSuccess == true).ToList();

                if (IsPingSuccessList != null) {
                    log.PushLog("PingAsset success : " + IsPingSuccessList.Count.ToString() + " Assets", "");
                }

                
            }
            else
            {
                log.PushLog("Harvest : A_Harvester Table has no Ping data to process");
            }
        }
        private void ExecuteWmiData(string type, int Cadence)
        {
            List<AD_HarvesterResult> ListData = new List<AD_HarvesterResult>();

            DataTable dt = _iArmRepo.GetAssetData(type, Cadence);

            if (dt != null)
            {
                ListData = GetAllMachineWmiData(dt);
                _iArmRepo.UpdateHarvestResult(ConvertToDataTable(ListData), type);
            }
            else
            {
                log.PushLog("Harvest : A_Harvester Table has no WMI data to process");
            }
        }

        private void ExecuteWarranty(string type, int Cadence)
        {
            List<AD_HarvesterResult> ListData = new List<AD_HarvesterResult>();

            DataTable dt = _iArmRepo.GetAssetData(type, Cadence);

            if (dt != null)
            {
                ListData = GetAllMachineWarrantyData(dt);
                _iArmRepo.UpdateHarvestResult(ConvertToDataTable(ListData), type);
            }
            else
            {
                log.PushLog("Harvest : A_Harvester Table has no WMI data to process");
            }
        }


        private List<AD_HarvesterResult> GetAllMachinePingData(DataTable dt)
        {
            List<AD_HarvesterResult> ListData = new List<AD_HarvesterResult>();
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string Item = dt.Rows[i]["Item"].ToString();
                    AD_HarvesterResult allMachinePingData = new AD_HarvesterResult();

                    //bool pingStatus =  GetPingStatus(dt.Rows[i]["Item"].ToString());

                    string IpAddess = GetIpAddress(Item);
                    //log.PushLog("PingAsset : " + dt.Rows[i]["Item"].ToString() + " Status " + pingStatus.ToString(), " ");

                    // do WMI while get IpAddres or ping success
                    if (IpAddess != "TimeOut")
                    {
                        allMachinePingData.IsPingSuccess = true;
                        allMachinePingData.Property = "IpAddress";
                        allMachinePingData.Value = IpAddess;

                        List<AD_HarvesterResult> ListResultWMIDataDetail = new List<AD_HarvesterResult>();
                        ListResultWMIDataDetail = GetMachineWmiDataByItemId(Item);
                        ListData.AddRange(ListResultWMIDataDetail);
                    }
                    else {
                        allMachinePingData.IsPingSuccess = false;
                    }

                    allMachinePingData.Item = Item;
                    ListData.Add(allMachinePingData);

                }
                return ListData;
            }
            catch (Exception ex)
            {
                log.PushLog("Harvest GetAllMachinePingData:" + ex.Message + ex.InnerException, "GetAllMachinePingData");
                return ListData;
            }
        }
        private List<AD_HarvesterResult> GetAllMachineWmiData(DataTable dt)
        {
            List<AD_HarvesterResult> ListData = new List<AD_HarvesterResult>();
            try
            {
               
                DataTable  dtItem = dt.AsEnumerable()
                    .GroupBy(r => new { Item = r["Item"] })
                    .Select(g => g.OrderBy(r => r["Item"]).First())
                .CopyToDataTable();


                foreach (DataRow drItem in dtItem.Rows) {

                    string Item = drItem["Item"].ToString();
                    List<AD_HarvesterResult> ListResultDataDetail = new List<AD_HarvesterResult>();
                    ListResultDataDetail = GetMachineWmiDataByItemId(Item);
                    ListData.AddRange(ListResultDataDetail);

                }

                return ListData;
            }
            catch (Exception ex)
            {
                log.PushLog("Harvest GetAllMachineWmiData:" + ex.Message + ex.InnerException, "GetAllMachineWmiData");

                throw ex;
            }

        }


        private List<AD_HarvesterResult> GetMachineWmiDataByItemId(string Item)
        {
            List<AD_HarvesterResult> ListData = new List<AD_HarvesterResult>();
            try
            {
                    //string Username = _iArmRepo.GetGlobalProperties("DAUsername");
                    //string Password = _iArmRepo.GetGlobalProperties("DAPassword");

                    try
                    {
                        string connectingMachineName = "\\\\" + Item + "\\root\\cimv2";
                        ConnectionOptions options = new ConnectionOptions();

                        //if (Username != null && Username != "") {
                        //    options.Username = Username; // replace with the actual username
                        //    options.Password = Password; //  for test only 
                        //}

                        //options.Username = "general.cp"; // replace with the actual username
                        //options.Password = "AdIns2017"; //  for test only 

                        ManagementScope scope = new ManagementScope(connectingMachineName, options);
                        scope.Connect();

                        string Value = "";
                        string Property = "";
                        string sAction = "";

                        try
                        {

                            Value = "";
                            Property = "";

                            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SystemEnclosure");
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                            foreach (var disk in searcher.Get())
                            {
                                Value = disk.GetPropertyValue("SerialNumber").ToString();
                                Property = "SerialNumber";
                            }

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = true;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);

                        }
                        catch
                        {

                            Value = "Error!!";
                            Property = "SerialNumber";

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = false;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);
                        }

                        try
                        {
                            Value = "";
                            Property = "SystemSKUNumber";
                            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SystemEnclosure");
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                            foreach (var disk in searcher.Get())
                            {
                                Value = disk.GetPropertyValue("SystemSKUNumber").ToString();
                                Property = "SystemSKUNumber";
                            }

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = true;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);

                        }
                        catch
                        {

                            Value = "Error!!";
                            Property = "SystemSKUNumber";

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = false;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);
                        }


                        try
                        {
                            Value = "";
                            Property = "BIOS";

                            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_BIOS");
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                            foreach (var disk in searcher.Get())
                            {
                                Value = disk.GetPropertyValue("Name").ToString();
                                Property = "BIOS";
                            }

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = true;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);
                        }
                        catch
                        {

                            Value = "Error!!";
                            Property = "BIOS";

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = false;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);
                        }
                        try
                        {
                            Value = "";
                            Property = "ChassisTypes";

                            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SystemEnclosure");
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                            foreach (var disk in searcher.Get())
                            {
                                Array x = (Array)disk.GetPropertyValue("ChassisTypes");
                                foreach (var d in x)
                                {
                                    Value = d.ToString();
                                    Property = "ChassisTypes";
                                }
                            }

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = true;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);

                        }
                        catch
                        {

                            Value = "Error!!";
                            Property = "ChassisTypes";

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = false;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);
                        }
                        try
                        {

                            Value = "";
                            Property = "PhysicalMemory";

                            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                            foreach (var disk in searcher.Get())
                            {
                                Value = disk.GetPropertyValue("TotalPhysicalMemory").ToString();
                                Property = "PhysicalMemory";
                            }

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = true;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);
                        }
                        catch
                        {
                            Value = "Error!!";
                            Property = "PhysicalMemory";

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = false;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);

                        }


                        try
                        {
                            Value = "";
                            Property = "DiskDrive";

                            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_DiskDrive");
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                            foreach (var disk in searcher.Get())
                            {
                                Value = disk.GetPropertyValue("Size").ToString();
                                Property = "DiskDrive";
                            }

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = true;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);
                        }
                        catch
                        {
                            Value = "Error!!";
                            Property = "DiskDrive";

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = false;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);

                        }
                        try
                        {
                            Value = "";
                            Property = "Manufacturer";

                            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SystemEnclosure");
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                            foreach (var disk in searcher.Get())
                            {
                                Value = disk.GetPropertyValue("Manufacturer").ToString();
                                Property = "Manufacturer";
                            }

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = true;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);
                        }
                        catch
                        {
                            Value = "Error!!";
                            Property = "Manufacturer";

                            AD_HarvesterResult Rw = new AD_HarvesterResult();
                            Rw.IsWMISuccess = false;
                            Rw.Item = Item;
                            Rw.Property = Property;
                            Rw.Value = Value;
                            ListData.Add(Rw);

                        }
                    }
                    catch
                    {

                    }

                return ListData;
            }
            catch (Exception ex)
            {
                log.PushLog("Harvest GetAllMachineWmiData:" + ex.Message + ex.InnerException, "GetAllMachineWmiData");

                throw ex;
            }

        }

        private List<AD_HarvesterResult> GetAllMachineWarrantyData(DataTable dt)
        {
            List<AD_HarvesterResult> ListData = new List<AD_HarvesterResult>();
            try
            {
                var options = new ChromeOptions();
                options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36");

                IWebDriver driver = new ChromeDriver(options);

                IDevTools devTools = driver as IDevTools;
                var session = devTools.GetDevToolsSession();
                var network = session.Domains.Network;

                network.EnableNetwork();
                network.EnableNetworkCaching();

                // warm_up Dell
                driver.Navigate().GoToUrl("https://www.dell.com/support/home/en-id?app=warranty");
                IWebElement inputFieldSM1 = driver.FindElement(By.XPath("//*[@id=\"inpEntrySelection\"]"));
                inputFieldSM1.SendKeys("JW76D2W");
                IWebElement submitButton1 = driver.FindElement(By.XPath("//*[@id=\"btn-entry-select\"]"));
                submitButton1.Click();
                inputFieldSM1.Clear();

                Thread.Sleep(30000);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    AD_HarvesterResult allMachinePingData = new AD_HarvesterResult();

                    string WRdate = GetWarrantyDate(dt.Rows[i]["Action"].ToString(), driver);

                    //log.PushLog("PingAsset : " + dt.Rows[i]["Item"].ToString() + " Status " + pingStatus.ToString(), " ");

                    allMachinePingData.Property = "WarrantyDate";
                    allMachinePingData.Value = WRdate;
                    allMachinePingData.Item = dt.Rows[i]["Item"].ToString();
                    ListData.Add(allMachinePingData);

                }
                return ListData;
            }
            catch (Exception ex)
            {
                log.PushLog("Harvest GetAllMachinePingData:" + ex.Message + ex.InnerException, "GetAllMachinePingData");
                return ListData;
            }
        }



        private  bool GetPingStatus(string machineName)
        {
            try {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(machineName);
                
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false ;
                }

            } catch {
                return false;
            }
    
        }
        private string GetIpAddress(string machineName)
        {
            string IpAddres = "TimeOut";
            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(machineName);

                
                if (reply.Status == IPStatus.Success)
                {
                    IpAddres = reply.Address.ToString();
                    return IpAddres;
                }
                else
                {
                    return IpAddres;
                }
            }
            catch
            {
                return IpAddres;
            }

        }

        private string GetWarrantyDate(string Action, IWebDriver driver)
        {
            string WRDate = "";
            try
            {
                string[] Info = Action.Split('|');

                string company = Info[0];
                string sn = Info[1];
                string pn = Info[2];


                if (company == "HP") {
                    WRDate = GetHpwaratyhp(sn, driver);

                } else if (company == "DEL") {
                    WRDate = GetHpwaratyDell(sn, driver);
                }
                else if (company == "LENOVO")
                {
                    WRDate = GetHpwaratyLenovo(sn, driver);
                }

                return WRDate;
            }
            catch
            {
                return WRDate;
            }

        }

        #region Selenium Wr
        public static string GetHpwaratyhp(string sn, IWebDriver driver)
        {
            string expDate = "";

            driver.Navigate().GoToUrl("https://support.hp.com/gb-en/check-warranty");

            IWebElement inputFieldSM = driver.FindElement(By.XPath("//*[@id=\"inputtextpfinder\"]"));

            // Enter text into the input field
            inputFieldSM.SendKeys(sn);

            IWebElement submitButton = driver.FindElement(By.XPath("//*[@id=\"FindMyProduct\"]"));

            // Submit the form
            submitButton.Submit();

            for (int i = 0; i < 10; i++)
            {
                string CurrentUrl = driver.Url;

                try
                {

                    if (CurrentUrl.Contains("warrantyresult"))
                    {
                        IWebElement WDateElement = driver.FindElement(By.XPath("//*[@id=\"directionTracker\"]/app-layout/app-check-warranty/div/div/div[2]/app-warranty-details/div/div[2]/main/div[4]/div/div[2]/div/div/div[2]/div[5]/div[2]"));
                        expDate = WDateElement.Text;

                        string format = "MMMM d, yyyy";
                        CultureInfo provider = CultureInfo.InvariantCulture;

                        DateTime result = DateTime.ParseExact(expDate, format, provider);
                        expDate = result.ToString("yyyy-MM-dd");
                        
                        Console.WriteLine("expDate " + expDate);
                        break;
                    }
                    else
                    {
                        IWebElement ErrSN = driver.FindElement(By.XPath("//*[@id=\"directionTracker\"]/app-layout/app-check-warranty/div/div/div[2]/app-check-warranty-landing/div[2]/app-single-product/div/form/div/div[1]/p[1]"));
                        if (ErrSN.Displayed)
                        {
                            Console.WriteLine("sn Invalid " + sn);
                            break;
                        }
                    }

                }
                catch { }
                Thread.Sleep(1000);
            }

            return expDate;
        }

        public static string GetHpwaratyDell(string sn, IWebDriver driver)
        {
            string expDate = "";
            
            driver.Navigate().GoToUrl("https://www.dell.com/support/home/en-id?app=warranty");

            IWebElement inputFieldSM = driver.FindElement(By.XPath("//*[@id=\"inpEntrySelection\"]"));
            
            inputFieldSM.SendKeys(sn);
            
            IWebElement submitButton = driver.FindElement(By.XPath("//*[@id=\"btn-entry-select\"]"));
            
            submitButton.Click();

            for (int i = 0; i < 10; i++)
            {
                string CurrentUrl = driver.Url;

                try
                {

                    if (CurrentUrl.Contains("product-support/servicetag"))
                    {
                        IWebElement WDateElement = driver.FindElement(By.XPath("//*[@id=\"dsk-expirationDt\"]"));
                        expDate = WDateElement.Text;

                        string format = "dd MMM yyyy";
                        CultureInfo provider = CultureInfo.InvariantCulture;

                        DateTime result = DateTime.ParseExact(expDate, format, provider);
                        expDate = result.ToString("yyyy-MM-dd");
                        Console.WriteLine("expDate " + expDate);
                        break;
                    }
                    else
                    {
                        IWebElement ErrSN = driver.FindElement(By.XPath("//*[@id=\"divEntryError\"]"));
                        
                        if (ErrSN.Displayed)
                        {
                            Console.WriteLine("sn Invalid " + sn);
                            break;
                        }
                    }

                }
                catch { }

                Thread.Sleep(1000);
            }

            return expDate;
        }

        public static string GetHpwaratyLenovo(string sn, IWebDriver driver)
        {
            string expDate = "";

       
            driver.Navigate().GoToUrl("https://pcsupport.lenovo.com/id/id/warranty-lookup#/");
            IWebElement inputFieldSM = driver.FindElement(By.XPath("//*[@id=\"app-standalone-warrantylookup\"]/div[1]/div[2]/div/div/div[1]/div[1]/div[1]/div[1]/input"));
            inputFieldSM.SendKeys(sn);
            IWebElement submitButton = driver.FindElement(By.XPath("//*[@id=\"app-standalone-warrantylookup\"]/div[1]/div[2]/div/div/div[1]/button"));

            // Submit the form
            submitButton.Submit();

            for (int i = 0; i < 10; i++)
            {
                string CurrentUrl = driver.Url;
                Console.WriteLine("url " + CurrentUrl);
                Thread.Sleep(1000);
            }
            return expDate;
        }

        #endregion

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
