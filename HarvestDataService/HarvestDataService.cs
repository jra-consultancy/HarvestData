using HarvestDataService.Service;
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
       

        readonly System.Timers.Timer _timer = new System.Timers.Timer();
        readonly System.Timers.Timer _timerPing = new System.Timers.Timer();
        readonly System.Timers.Timer _timerPingReset = new System.Timers.Timer();
        readonly System.Timers.Timer _timerWMI = new System.Timers.Timer();

        Logger4net log;

        HarvestData harv = new HarvestData();
        ArmRepository _iArmRepo = new ArmRepository();
        public HarvestDataService()
        {
            log = new Logger4net();
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
   
        }

        protected override void OnStart(string[] args)
        {

            log.PushLog("Service started", "Service started");

            InitializeComponents();
        }


        private void InitializeComponents()
        {
            DateTime startTime = DateTime.Today.AddHours(4).AddMinutes(30);

            // Calculate the time interval until the scheduled time
            TimeSpan timeUntilStart = startTime - DateTime.Now;

            // If the scheduled time has already passed today, adjust the start time to tomorrow
            if (timeUntilStart <= TimeSpan.Zero)
            {
                startTime = startTime.AddDays(1);
                timeUntilStart = startTime - DateTime.Now;
            }

            // Start the service immediately on the first run
            (new HarvestData()).Harvest();

            // Set up the timer to run at the scheduled time every 24 hours
            _timer.AutoReset = true;
            _timer.Interval = timeUntilStart.TotalMilliseconds;
            _timer.Enabled = true;
            _timer.Start();
            _timer.Elapsed += (sender, e) => (new HarvestData()).Harvest();
            //_timer.Elapsed += (new HarvestData()).Harvest;

            int intInterval = Convert.ToInt32(1);
            log.PushLog("_timerPing Interval " + TimeSpan.FromMinutes(intInterval).TotalMilliseconds.ToString(), "");
            // Set up the timer to run at the scheduled time every 24 hours
            _timerPing.AutoReset = true;
            _timerPing.Interval = TimeSpan.FromMinutes(intInterval).TotalMilliseconds;
            _timerPing.Enabled = true;
            _timerPing.Elapsed += (sender1, e1) => harv.PingAssetAsync(sender1, e1);
            _timerPing.Start();



            Thread.Sleep(10000);
            log.PushLog("_timerWMI Interval " + TimeSpan.FromMinutes(intInterval).TotalMilliseconds.ToString(), "");
            // Set up the timer to run at the scheduled time every 24 hours
            _timerWMI.AutoReset = true;
            _timerWMI.Interval = TimeSpan.FromMinutes(intInterval).TotalMilliseconds;
            _timerWMI.Enabled = true;
            _timerWMI.Elapsed += (sender2, e2) => harv.WMIAssetAsync(sender2, e2);
            _timerWMI.Start();



            log.PushLog("Service Running", "");
        }


        protected override void OnStop()
        {
            _timer.Enabled = false;
            _timerPing.Enabled = false;
            _timerWMI.Enabled = false;
            _timerWMI.Stop();
            _timer.Stop();
            _timerPing.Stop();
            //_logger.Log("Service stopped", @"" + UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
            log.PushLog("Service stopped", "Service stopped");

        }

    }
}
