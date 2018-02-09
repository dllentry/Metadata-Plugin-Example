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
using Microsoft.Practices.Unity;
using NLog;
using Pelco.Media.RTSP.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VxEventAgent;

namespace FacialDetectionAgent
{
    public class FacialDetectionEventAgent : EventAgentBase
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private IHost _host;
        private bool _running;
        private bool _configured;
        private RtspServer _rtspServer;
        private HttpServer _httpServer;
        private Bootstrapper _bootstrapper;
        private IDataSourcesManager _dataSourcesManager;
        private List<NewSituation> _situations;
        private IDetectionSourceManager _sourcesManager;

        public FacialDetectionEventAgent(IHost host)
        {
            _host = host;
            _running = false;
            _configured = true;
            _bootstrapper = new Bootstrapper();

            _situations = new List<NewSituation>();

            _situations.Add(new NewSituation()
            {
                AckNeeded = false,
                Audible = false,
                AutoAcknowledgeTimeout = 60 * 5,
                Log = true,
                Notify = true,
                Severity = 3,
                SnoozeIntervals = null,
                Type = "external/facial_detection/match"
            });

            _bootstrapper.Run();
        }

        public override string Id => "ed7f7d51-16c7-4577-be4c-91e584514cc2";

        public override string Name => "OpenCV Facial Detection Agent";

        public override string Version => "1.0.0";

        public override string Manufacturer => "Pelco";

        public override string Author => "Pelco";

        public override string Description => "Example Facial Recognition Metadata Event Agent";

        public override List<NewSituation> Situations => _situations;

        public override bool IsRunning => _running;

        public override bool IsConfigured => _configured;

        public override bool RequiresControl => true;

        public override FrameworkElement CreateControl()
        {
            var userControl = _bootstrapper.Container.Resolve<MainUserControl>();

            return userControl;
        }

        public override bool Run()
        {
            LOG.Info("Starting facial detection agent");

            try
            {
                _rtspServer = _bootstrapper.Container.Resolve<RtspServer>();
                _httpServer = _bootstrapper.Container.Resolve<HttpServer>();
                _dataSourcesManager = _bootstrapper.Container.Resolve<IDataSourcesManager>();
                _sourcesManager = _bootstrapper.Container.Resolve<IDetectionSourceManager>();

                _rtspServer.Start();
                _httpServer.Start();

                InitializeSources();
            }
            catch (Exception e)
            {
                LOG.Error(e, "Caught exception starting facial detection agent");
                return false;
            }

            return true;
        }

        public override void Stop()
        {
            try
            {
                if (_rtspServer != null)
                {
                    _rtspServer.Stop();
                }

                if (_httpServer != null)
                {
                    _httpServer.Stop();
                }

                if (_dataSourcesManager != null)
                {
                    _dataSourcesManager.Close();
                }

                if (_sourcesManager != null)
                {
                    _sourcesManager.Shutdown();
                }
            }
            catch (Exception e)
            {
                LOG.Error(e, "Caught exception while stopping facial detection agent");
            }
        }

        private Uri SelectDataInterface(CPPCli.DataSource ds)
        {
            var interfaces = ds.DataInterfaces
                               .Where(di => !di.SupportsMulticast && di.Protocol == CPPCli.DataInterface.StreamProtocols.RtspRtp)
                               .ToList();

            if (interfaces.Count == 0)
            {
                throw new Exception($"No RTSP DataInterfaces for datasource '{ds.Id}'");
            }

            // If we can select the second stream then we will, otherwise the primary is returned.
            // We prefer the secondary stream because it will usually be a lower quality and will take
            // much less resources to performe facial recognition on it.
            return new Uri(interfaces.Count > 1 ? interfaces[1].DataEndpoint : interfaces[0].DataEndpoint); 
        }

        private async void InitializeSources()
        {
            await Task.Run(async () =>
            {
                var config = _bootstrapper.Container.Resolve<Configuration>();

                if (!await _dataSourcesManager.Init())
                {
                    LOG.Error("Unable to initialized datasource manager");
                    return;
                }

                Parallel.ForEach(config.SelectedDatasources, sds =>
                {
                    try
                    {
                        if (_dataSourcesManager.TryGetDataSource(sds.Id, out CPPCli.DataSource ds))
                        {
                            if (!_sourcesManager.RegisterSource(ds.Id, SelectDataInterface(ds)))
                            {
                                LOG.Error($"Failed to create facial recongnition source with  datasource '{ds.Name}'");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LOG.Error($"Unable to initialize datasource '{sds.Id}', reason={e.Message}");
                    }
                });
            });
        }
    }
}
