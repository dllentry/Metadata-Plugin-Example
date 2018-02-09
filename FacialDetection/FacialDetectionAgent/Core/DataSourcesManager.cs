//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using CPPCli;
using FacialDetectionCommon.Serenity;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacialDetectionAgent.Core.Api
{
    public class DataSourcesManager : IDataSourcesManager
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private readonly object DatasourcesLock = new object();

        private string _ip;
        private string _user;
        private string _password;
        private ISerenityService _serenity;
        private ConcurrentBag<DataSource> _datasources;

        public DataSourcesManager(ISerenityService serenity, Configuration config)
        {
            _serenity = serenity;
            _ip = config.SerenityAddress;
            _user = config.SerenityUser;
            _password = config.SerenityPassword;
            _datasources = new ConcurrentBag<DataSource>();
        }

        public bool TryGetDataSource(string id, out DataSource source)
        {
            lock (DatasourcesLock)
            {
                source = _datasources.FirstOrDefault(ds => ds.Id == id);

                return source != null;
            }
        }

        public async Task<bool> Init()
        {

            if (!await _serenity.LoginAsync(_ip, _user, _password))
            {
                LOG.Error($"Failed to log into VideoXpert system at {_ip}");
                return false;
            }

            LOG.Info($"Successfully logged into VideoXpert system at {_ip}");

            var datasources = await GetDataSourcesAsync();

            lock (DatasourcesLock)
            {
                if (datasources != null)
                {
                    _datasources = new ConcurrentBag<DataSource>(datasources);

                    return true;
                }

                return false;
            }
        }

        public async Task<bool> ReInit(string ip, string username, string password)
        {
            if (_ip != ip || _user != username || _password != password)
            {
                _ip = ip;
                _user = username;
                _password = password;

                Close();
                return await Init();
            }

            return false;
        }

        public void Close()
        {
                
                lock (DatasourcesLock)
                {
                    // Clear the datasources.
                    while (!_datasources.IsEmpty)
                    {
                        DataSource ds;
                        _datasources.TryTake(out ds);
                    }
                }
            }
        

        public async Task<bool> Refresh()
        {
            var datasources = await GetDataSourcesAsync();
            lock (DatasourcesLock)
            {
                if (datasources != null)
                {
                    _datasources = new ConcurrentBag<DataSource>(datasources);

                    return true;
                }

                return false;
            }
        }

        public IReadOnlyCollection<DataSource> GetVideoDataSources()
        {
            lock (DatasourcesLock)
            {
                return _datasources;
            }
        }

        private async Task<List<DataSource>> GetDataSourcesAsync()
        {
            return await _serenity.GetDataSourcesByTypeAsync(DataSource.Types.Video);
        }
    }
}
