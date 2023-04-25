using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HarvestDataService
{
    internal static class Program
    {
     
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //var fileParser = new HarvestData();
            //fileParser.Harvest();
            //Console.ReadLine();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new HarvestDataService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

