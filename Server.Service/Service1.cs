using System;
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
            AutoLog = true;

            SetupProcess();
        }

        protected override void OnStart(string[] args)
        {
            Debugger.Launch();
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
            var serverRelativePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @".\..\..\..\Server\bin\Debug\net6.0\Server.exe");
            var serverProcessPath = Path.GetFullPath(serverRelativePath);

            _processInfo = new ProcessStartInfo(serverProcessPath)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                ErrorDialog = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
        }
    }
}
