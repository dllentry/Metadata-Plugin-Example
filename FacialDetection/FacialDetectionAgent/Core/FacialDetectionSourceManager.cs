//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using FacialDetectionAgent.Core.Api;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FacialDetectionAgent.Core
{
    public class FacialDetectionSourceManager : IDetectionSourceManager
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private Configuration _config;
        private System.Timers.Timer _timer;
        private ConcurrentDictionary<string, FacialDetectionSource> _sources;

        public FacialDetectionSourceManager(Configuration config)
        {
            _config = config;
            _sources = new ConcurrentDictionary<string, FacialDetectionSource>();

            _timer = new System.Timers.Timer();
            _timer.AutoReset = true;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;

            _timer.Start();
        }

        public bool RegisterSource(string dataSourceId, Uri uri)
        {
            var id = GetTranslatedId(dataSourceId);

            if (!_sources.ContainsKey(id))
            {
                var source = new FacialDetectionSource(uri, _config);
                source.Start();

                return _sources.TryAdd(id, source);
            }

            return true;
        }

        public bool ContainsSource(string id)
        {
            return _sources.ContainsKey(id);
        }

        public ICollection<string> GetSourceIds()
        {
            return _sources.Keys;
        }

        public bool IsSourceOnline(string sourceId)
        {
            var src = GetSource(sourceId);

            return src != null ? src.IsOnline : false;
        }

        public IDetectionSource GetSource(string id)
        {
            FacialDetectionSource source = null;

            if (_sources.TryGetValue(id, out source))
            {
                return source;
            }

            return null;
        }

        public bool TryGetSource(string id, out IDetectionSource source)
        {
            source = GetSource(id);

            return source != null;
        }

        public void Shutdown()
        {
            _timer.Stop();

            foreach (var source in _sources)
            {
                try
                {
                    source.Value.Stop();
                }
                catch (Exception e)
                {
                    LOG.Error($"Caught exception while shutting down facial detection source, msg={e.Message}");
                }
            }

            _sources.Clear();
        }

        private string GetTranslatedId(string vxid)
        {
            // VideoXpert datasource ids end with :video. This just makes
            // the id end with :metadata
            return vxid.Replace("video", "metadata");
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Parallel.ForEach(_sources, (src) =>
            {
                if (!src.Value.IsOnline)
                {
                    try
                    {
                        LOG.Info($"Detected source '{src.Key}' is offline, attempting to reconnect...");
                        src.Value.Stop();
                        src.Value.Start();
                    }
                    catch (Exception ex)
                    {
                        LOG.Error($"Caught exception while attempting to reconnect source '{src.Key}', reason: {ex.Message}");
                    }
                }
            });
        }
    }
}
