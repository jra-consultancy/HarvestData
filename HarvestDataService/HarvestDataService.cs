using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace HarvestDataService
{
    public partial class HarvestDataService : ServiceBase
    {
        private readonly ILogger _logger;

        readonly System.Timers.Timer _timer = new System.Timers.Timer();
        private string UploadLogFile = "";
        private System.Timers.Timer timer;


        ArmRepository _iArmRepo = new ArmRepository();
        public HarvestDataService()
        {

            UploadLogFile = _iArmRepo.GetFileLocation(0);
            _logger = Logger.GetInstance;
            CreateLogDirectory(UploadLogFile);

            //InitializeComponents();
            

        }

        private void CreateLogDirectory(string uploadLogFile)
        {

            int index = uploadLogFile.LastIndexOf("Logs") ;
            if(index > 0)
            {
                uploadLogFile = uploadLogFile.Substring(0, index+5);
            }
            if (!Directory.Exists(uploadLogFile))
                Directory.CreateDirectory(uploadLogFile);
            _logger.Log("Creating Log Directory", @"" + UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

        }

        protected override void OnStart(string[] args)
        {
            _logger.Log("Service started", @"" + UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

            InitializeComponents();
        }


        private void InitializeComponents()
        {
            //HarvestData parser = new HarvestData();
            //parser.Harvest();
            DateTime startTime = DateTime.Today.AddHours(9);

            // If the start time has already passed today, schedule for tomorrow
            if (startTime < DateTime.Now)
            {
                startTime = startTime.AddDays(1);
            }

            // Calculate the time interval until the scheduled time
            TimeSpan timeUntilStart = startTime - DateTime.Now;

            // Set up the timer to run at the scheduled time every 24 hours
            _timer.AutoReset = true;
            _timer.Interval = timeUntilStart.TotalMilliseconds;
            _timer.Enabled = true;
            _timer.Start();
            _timer.Elapsed += (sender, e) => (new HarvestData()).Harvest();

            //_timer.Elapsed += (new HarvestData()).Harvest;
        }


        protected override void OnStop()
        {
            _timer.Enabled = false;
            _timer.Stop();
            _logger.Log("Service stopped", @"" + UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

        }

    }
}
