//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using FacialDetection.Core;
using FacialDetection.Core.Api;
using FacialDetection.Core.Metadata;
using FacialDetectionCommon.Serenity;
using Microsoft.Practices.Unity;
using Pelco.Media.Metadata.Api;
using Pelco.Phoenix.PluginHostInterfaces;
using Prism.Unity;
using System.Windows;

namespace FacialDetection
{
    public class Bootstrapper : UnityBootstrapper
    {
        private bool _isPlugin;
        private IHostService _host;

        public Bootstrapper(IHost host, bool isPlugin = true) : base()
        {
            _isPlugin = isPlugin;
            _host = new HostService(host);
        }

        public bool HasRun
        {
            get
            {
                return Container != null;
            }
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterInstance<IHostService>(_host);
            Container.RegisterType<ISerenityService, SerenityService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IMetadataStreamManager, MetadataStreamManager>(new ContainerControlledLifetimeManager());
        }

        protected override DependencyObject CreateShell()
        {
            if (_isPlugin)
            {
                LoadResource();

                return null;
            }
            else
            {
                return Container.Resolve<MainWindow>();
            }
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            App.Current.MainWindow = Shell as Window;
            App.Current.MainWindow.Show();
        }

        private void LoadResource()
        {
        }
        }
}
