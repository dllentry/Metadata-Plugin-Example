//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using FacialDetection.Core.Api;
using FacialDetectionCommon.Serenity;
using NLog;
using Prism.Commands;
using System.Windows.Input;

namespace FacialDetection
{
    public class MainUserControlViewModel
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private IHostService _host;
        private ISerenityService _serenity;

        public MainUserControlViewModel(IHostService host, ISerenityService serenity)
        {
            _host = host;
            _serenity = serenity;

            CloseCommand = new DelegateCommand(ClosePlugin);
            LoadCommand = new DelegateCommand(UserControlLoaded);
        }

        public ICommand CloseCommand { get; private set; }

        public ICommand LoadCommand { get; private set; }

        private void ClosePlugin()
        {
            _host?.RequestShutdown();
        }

        private async void UserControlLoaded()
        {
            var endpoint = _host.VideoXpertEndpoint;
            if (! await _serenity.LoginAsync(endpoint.Host, _host.SerenityAuthToken))
            {
                LOG.Error($"Failed to log into VideoXpert system at {endpoint}");
            }
            else
            {
                LOG.Info($"Logged into VideoXpert system at '{endpoint}'");
            }
        }
    }
}
