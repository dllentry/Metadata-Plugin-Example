//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using FacialDetectionAgent.Core;
using FacialDetectionAgent.Core.Api;
using FacialDetectionAgent.HTTP;
using FacialDetectionAgent.RTSP;
using NLog;
using Pelco.Media.RTSP.Server;
using System;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Unity;

namespace FacialDetectionAgent
{
    public class Bootstrapper : UnityBootstrapper
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        public Bootstrapper() : base()
        {
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // Register the IUnityContainer so that we can access services from the
            // HTTP server's module handlers.
            ServiceRegistry.RegisterContainer(Container);

            var serializer = new ConfigurationFileSerializer();

            var config = serializer.Read();
            var sourceMgr = new FacialDetectionSourceManager(config);
            var sessionMgr = new RtspSessionManager();
            var rtspHandler = new RtspRequestHandler(sourceMgr, sessionMgr);

            var dispatcher = new DefaultRequestDispatcher();
            dispatcher.RegisterHandler("/stream", rtspHandler);

            var rtspServer = new RtspServer(config.RtspPort, dispatcher);
            var httpServer = new HttpServer(config.HttpPort);

            Container.RegisterInstance(config);
            Container.RegisterInstance(sessionMgr);
            Container.RegisterInstance(rtspServer);
            Container.RegisterInstance(httpServer);
            Container.RegisterType<IDataSourcesManager, DataSourcesManager>(new ContainerControlledLifetimeManager());
        }

        protected override DependencyObject CreateShell()
        {
            LoadResource();
            return null;
            //return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            App.Current.MainWindow = (Window)Shell;
            App.Current.MainWindow.Show();
        }

        private void LoadResource()
        {
            /// This is required instead of using the App.xaml when loaded as a plug-in
            /// App.xaml(.cs) is not ran and merged dictionaries will not be loaded.
            var resources = new string[]
            {
                "/FacialDetectionCommon;component/Styles/styles.xaml",
            };

            foreach (var resource in resources)
            {
                var dict = new ResourceDictionary();
                dict.Source = new Uri(resource, UriKind.Relative);
                App.Current.Resources.MergedDictionaries.Add(dict);
            }
        }
    }
}
