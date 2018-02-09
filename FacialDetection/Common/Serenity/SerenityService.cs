//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using CPPCli;
using NLog;
using Pelco.Media.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FacialDetectionCommon.Serenity
{
    public sealed class SerenityService : ISerenityService, IDisposable
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private const string kFormat = "yyyy-MM-dd'T'HH:mm:ss.fffK";

        public bool _initialized;
        private VXSystem _system;
        private ManualResetEvent _started;

        private delegate Results.Value Login(VXSystem system);

        public SerenityService()
        {
            _initialized = false;
            _started = new ManualResetEvent(false);
        }

        ~SerenityService()
        {
            Dispose();
        }
        
        public async Task<bool> LoginAsync(string ip, string username, string password)
        {
            return await Task.Run(() => Initialize(ip, (sys) => sys.Login(username, password)));
        }

        public async Task<bool> LoginAsync(string ip, string authToken)
        {
            return await Task.Run(() => Initialize(ip, (sys) => sys.Login(authToken)));
        }

        public async Task<DataSource> GetDataSourceByIdAsync(string id)
        {
            return await Task.Run(() =>
            {
                try
                {
                    WaitForInitialization();

                    return _system.GetDataSourceById(id);
                }
                catch (Exception e)
                {
                    LOG.Error($"Failed to retrieve datasource by id '{id}', reason: {e.Message}");
                }

                return null;
            });
        }

        public async Task<List<DataSource>> GetDataSourcesByTypeAsync(DataSource.Types type)
        {
            return await Task.Run(() =>
            {
                try
                {
                    WaitForInitialization();

                    return _system.GetDataSourcesByType(type);
                }
                catch (Exception e)
                {
                    LOG.Error($"Failed to retrieve data sources of type '{type}', reason: {e.Message}");
                }

                return new List<DataSource>();
            });
        }

        public async Task<List<Clip>> GetClipsAsync(string datasourceId, DateTime startSearch, DateTime endSearchTime)
        {
            return await GetClipsAsync(await GetDataSourceByIdAsync(datasourceId), startSearch, endSearchTime);
        }

        public async Task<List<Clip>> GetClipsAsync(DataSource ds, DateTime startSearch, DateTime endSearchTime)
        {
            return await Task.Run(() =>
            {
                try
                {
                    WaitForInitialization();

                    if (ds == null)
                    {
                        throw new ArgumentNullException("Cannot retrieve clips from a null DataSource instance");
                    }

                    if (startSearch == null)
                    {
                        throw new ArgumentNullException("Cannot retrieve clips with a null 'startSearchTime'");
                    }

                    if (endSearchTime == null)
                    {
                        throw new ArgumentNullException("Cannot retrieve clips with a null 'endSearchTime'");
                    }

                    return ds.GetClips(ToRfc3339DateString(startSearch), ToRfc3339DateString(endSearchTime));
                }
                catch (Exception e)
                {
                    LOG.Error($"Failed to retrieve clips for datasource '{ds.Id}', reason: {e.Message}");
                }

                return new List<Clip>();
            });
        }

        public async Task<List<DataSource>> GetAssociatedMetadataSourcesAsync(DataSource ds)
        {
            return await Task.Run(() =>
            {
                try
                {
                    WaitForInitialization();

                    return ds.LinkedMetadataRelations.Select(rel => rel.Resource).ToList();
                }
                catch (Exception e)
                {
                    LOG.Error($"Caught exception while retrieving metadata associations,  msg={e.Message}");
                }

                return new List<DataSource>();
            });
        }

        public async Task<DataSource> GetAssociatedMetadataSourceByTypeAsync(DataSource vds, MimeType type)
        {
            return (await GetAssociatedMetadataSourcesAsync(vds)).Where(ds => type.Is(MimeType.Parse(ds.Encoding))).FirstOrDefault();
        }

        public void Dispose()
        {
            LOG.Info("Serenity service has been disposed");
            _system?.Dispose();
            GC.SuppressFinalize(this);
        }

        private bool Initialize(string ip, Login login)
        {
            _system = new VXSystem(ip);
            var result = login.Invoke(_system);
            if (result != Results.Value.OK)
            {
                LOG.Error($"Failed to log into VideoXpert system at {ip}");
                _started.Set();
                return false;
            }

            LOG.Info($"Successfully logged into VideoXpert system {ip}");
            _initialized = true;
            _started.Set();

            return true;
        }

        private void WaitForInitialization()
        {
            if (_started.WaitOne() && !_initialized)
            {
                throw new Exception("Serenity service is not initialized");
            }
        }

        private string ToRfc3339DateString(DateTime dt)
        {
            if (dt.Kind != DateTimeKind.Utc)
            {
                dt = dt.ToUniversalTime();
            }

            return dt.ToString(kFormat, DateTimeFormatInfo.InvariantInfo);
        }
    }
}
