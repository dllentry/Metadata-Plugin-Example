//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Nancy.Hosting.Self;
using NLog;
using System;

namespace FacialDetectionAgent.HTTP
{
    public class HttpServer
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private readonly object HostLock = new object();

        private Uri _uri;
        private int _port;
        private NancyHost _host;

        public HttpServer(int port)
        {
            _port = port;
            _uri = new Uri($"http://localhost:{_port}");
        }

        public void Start()
        {
            lock (HostLock)
            {
                if (_host == null)
                {
                    var hostConf = new HostConfiguration();
                    hostConf.RewriteLocalhost = true;

                    _host = new NancyHost(hostConf, _uri);
                    _host.Start();

                    LOG.Info($"Started HTTP Server listening at '{_uri}'");
                }
            }
            
        }

        public void Stop()
        {
            lock (HostLock)
            {
                if (_host != null)
                {
                    _host.Stop();
                    _host.Dispose();
                    _host = null;

                    LOG.Info("HTTP Server successfully stopped");
                }
            }
        }
    }
}
