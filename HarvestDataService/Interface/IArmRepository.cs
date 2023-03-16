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
        
        string GetFileLocation(int Key);
        DataTable GetAssetData(string type);
        void InsertBulkAssetData(DataTable pingResult);
        void UpdateAssetStatus(string type);

        void InsertBulkAssetsADData(DataTable assets);
        void InsertBulkUsersADData(DataTable users);

        string GetAD_Domain();
        void InsertVersionNoIfNotFound(string versionNo);




    }
}
