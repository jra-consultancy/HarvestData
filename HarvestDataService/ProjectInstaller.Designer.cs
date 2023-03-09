namespace HarvestDataService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.HarvestProcess = new System.ServiceProcess.ServiceProcessInstaller();
            this.HarvestData = new System.ServiceProcess.ServiceInstaller();
            // 
            // HarvestProcess
            // 
            this.HarvestProcess.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.HarvestProcess.Password = null;
            this.HarvestProcess.Username = null;
            this.HarvestProcess.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.CVUploadProcess_AfterInstall);
            // 
            // HarvestData
            // 
            this.HarvestData.ServiceName = "CV Harvest Data Service";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.HarvestProcess,
            this.HarvestData});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller HarvestProcess;
        private System.ServiceProcess.ServiceInstaller HarvestData;
    }
}