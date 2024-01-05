using HarvestDataService.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestDataService
{
    public interface IArmRepository
    {

        //string GetFileLocation(int Key);
        DataTable GetAssetData(string type, int Cadence);
        //void InsertBulkAssetData(DataTable pingResult);
        void UpdateAssetStatus(string type);

        void InsertBulkAssetsADData(DataTable assets);
        void InsertBulkUsersADData(DataTable users);

        string GetAD_Domain();
        void InsertVersionNoIfNotFound(string versionNo);
        void InsertAD_DomainName(string domainname);

        void UpdateHarvestResult(DataTable Res, string Type);
        void ResetHarvestResult(string Type);
        string GetGlobalProperties(string propertyName);
        void InsertBulkAD(DataTable Data, string TableName);

    }
}
