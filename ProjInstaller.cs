using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace FileMonitoringService
{
    [RunInstaller(true)]
    public partial class ProjInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjInstaller()
        {
            InitializeComponent();

            // Configure the Service Process Installer
            serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalService // Adjust as needed (e.g., NetworkService ,LocalService)
            };
            // Configure the Service Installer
            serviceInstaller = new ServiceInstaller
            {
                ServiceName = "FileMonitoringService", // Must match the ServiceName in your ServiceBase class
                DisplayName = "File Monitoring Service",
                StartType = ServiceStartMode.Automatic ,
                Description = "This is my file monitoring service.",
                ServicesDependedOn = new string[] { "RpcSs", "EventLog", "LanmanWorkstation" }
            };
         
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);

        }
    }
}
