using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace Server.Service
{
    public partial class Service1 : ServiceBase
    {
        private ProcessStartInfo _processInfo;
        private Process _process;

        public Service1()
        {
            InitializeComponent();
            CanStop = true;
            CanPauseAndContinue = true;

            SetupProcess();
        }

        protected override void OnStart(string[] args)
        {
            _process = Process.Start(_processInfo);
        }

        protected override void OnStop()
        {
            _process.Kill();
            _process = null;
            _processInfo = null;
        }

        private void SetupProcess()
        {
            var serverProcessPath = Path.Combine(Directory.GetCurrentDirectory(), "Server.exe");
            _processInfo = new ProcessStartInfo(serverProcessPath);
            _processInfo.UseShellExecute = false;
            _processInfo.RedirectStandardOutput = true;
            _processInfo.RedirectStandardError = true;
            _processInfo.RedirectStandardInput = true;
            _processInfo.CreateNoWindow = true;
            _processInfo.ErrorDialog = false;
            _processInfo.WindowStyle = ProcessWindowStyle.Hidden;
        }
    }
}
