﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism.Ioc;
using Prism.Unity;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<Services.IMessagesStore, Services.MessagesStore>();
        }

        protected override Window CreateShell()
        {
            var window = Container.Resolve<Views.MainWindow>();
            return window;
        }
    }
}