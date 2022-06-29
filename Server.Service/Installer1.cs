using System.ComponentModel;
using System.ServiceProcess;

namespace Server.Service
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();

            var serviceInstaller = new ServiceInstaller();
            var processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalService;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "Run_Telemessage_server_service";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
