using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using Client.Services;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IMessageService, MessageService>();
            containerRegistry.Register<IConnectionService, ConnectionService>();
        }

        protected override Window CreateShell()
        {
            var window = Container.Resolve<Views.MainWindow>();
            return window;
        }
    }
}