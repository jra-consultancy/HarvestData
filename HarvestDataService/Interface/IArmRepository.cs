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
        DataTable GetAssetData();
        void InsertBulkAssetData(DataTable pingResult);
        void UpdateAssetStatus();
    }
}
