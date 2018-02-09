//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using CPPCli;
using FacialDetectionAgent.Core;
using FacialDetectionAgent.Core.Api;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FacialDetectionAgent
{
    public class MainUserControlViewModel : BindableBase
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private int _rtspPort;
        private int _httpPort;
        private int _minNeighbors;
        private double _scaleFactor;
        private string _serenityIp;
        private string _serenityUser;
        private string _serenityPass;
        private IDataSourcesManager _datasourcesMgr;
        private ConfigurationFileSerializer _serializer;

        public MainUserControlViewModel(Core.Configuration config, IDataSourcesManager dsMgr)
        {
            _rtspPort = config.RtspPort;
            _httpPort = config.HttpPort;
            _scaleFactor = config.ScaleFactor;
            _minNeighbors = config.MinimumNeighbors;
            _serenityIp = config.SerenityAddress;
            _serenityUser = config.SerenityUser;
            _serenityPass = config.SerenityPassword;
            _serializer = new ConfigurationFileSerializer();
            _datasourcesMgr = dsMgr;

            SelectedDataSources = new ObservableCollection<DataSource>();
            AvailableDataSources = new ObservableCollection<DataSource>();
            SaveCommand = new DelegateCommand(Save);
            AddDataSourcesCommand = new DelegateCommand<IList>(selected => AddDataSources(selected));
            RemoveDataSourcesCommand = new DelegateCommand<IList>(selected => RemoveDataSources(selected));

            InitDatasources(config.SelectedDatasources);
        }

        #region Properties

        public int RtspPort
        {
            get
            {
                return _rtspPort;
            }

            set
            {
                SetProperty(ref _rtspPort, value);
            }
        }

        public int HttpPort
        {
            get
            {
                return _httpPort;
            }

            set
            {
                SetProperty(ref _httpPort, value);
            }
        }

        public string SerenityAddress
        {
            get
            {
                return _serenityIp;
            }

            set
            {
                SetProperty(ref _serenityIp, value);
            }
        }

        public string SerenityUser
        {
            get
            {
                return _serenityUser;
            }

            set
            {
                SetProperty(ref _serenityUser, value);
            }
        }

        public string SerenityPassword
        {
            get
            {
                return _serenityPass;
            }

            set
            {
                SetProperty(ref _serenityPass, value);
            }
        }

        public double ScaleFactor
        {
            get
            {
                return _scaleFactor;
            }

            set
            {
                SetProperty(ref _scaleFactor, value);
            }
        }

        public int MinimumNeighbors
        {
            get
            {
                return _minNeighbors;
            }

            set
            {
                SetProperty(ref _minNeighbors, value);
            }
        }

        public ObservableCollection<DataSource> AvailableDataSources { get; private set; }

        public ObservableCollection<DataSource> SelectedDataSources { get; private set; }

        #endregion

        #region Commands

        public ICommand AddDataSourcesCommand { get; private set; }

        public ICommand RemoveDataSourcesCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        private void AddDataSources(IList selectedItems)
        {
            var toRemove = new List<DataSource>();

            foreach (object item in selectedItems)
            {
                var ds = item as DataSource;
                toRemove.Add(ds);
                App.Current.Dispatcher.Invoke(() => SelectedDataSources.Add(ds));
            }

            App.Current.Dispatcher.Invoke(() => toRemove.ForEach(ds => AvailableDataSources.Remove(ds)));
        }

        private void RemoveDataSources(IList selectedItems)
        {
            var toRemove = new List<DataSource>();

            foreach (object item in selectedItems)
            {
                var ds = item as DataSource;
                toRemove.Add(ds);
                App.Current.Dispatcher.Invoke(() => AvailableDataSources.Add(ds));
            }

            App.Current.Dispatcher.Invoke(() => toRemove.ForEach(ds => SelectedDataSources.Remove(ds)));
        }

        public async void Save()
        {
            await Task.Run(async () =>
             {
                 var config = new Core.Configuration()
                 {
                     RtspPort = _rtspPort,
                     HttpPort = _httpPort,
                     MinimumNeighbors = _minNeighbors,
                     ScaleFactor = _scaleFactor,
                     SerenityAddress = _serenityIp,
                     SerenityUser = _serenityUser,
                     SerenityPassword = _serenityPass
                 };

                 config.SelectedDatasources.AddRange(SelectedDataSources.Select(ds =>
                 {
                     return new DataSourceEntry() { Name = ds.Name, Id = ds.Id };
                 }));

                 if (await _datasourcesMgr.ReInit(_serenityIp, _serenityUser, _serenityPass))
                 {
                     InitDatasources(new List<DataSourceEntry>());
                 }

                 if (!_serializer.Save(config))
                 {
                     LOG.Error("Failed to apply(save) configuration file");
                 }
             });
        }

        #endregion

        private void InitDatasources(List<DataSourceEntry> configuredSources)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (! await _datasourcesMgr.Init())
                    {
                        LOG.Error("Unable to retrieve datasources, failed in initalize serenity datasources manager");
                        return;
                    }

                    var datasources = _datasourcesMgr.GetVideoDataSources();

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        AvailableDataSources.Clear();
                        SelectedDataSources.Clear();
                        AvailableDataSources.AddRange(datasources.Where(ds => !IsDataSourceSelected(ds, configuredSources)));
                        SelectedDataSources.AddRange(datasources.Where(ds => IsDataSourceSelected(ds, configuredSources)));
                    });
                }
                catch (Exception e)
                {
                    LOG.Error(e, "Failed to initialize datasources");
                }
            });
        }

        private bool IsDataSourceSelected(DataSource ds, List<DataSourceEntry> configuredSources)
        {
            return configuredSources.Where(s => s.Id == ds.Id).Count() > 0;
        }
    }
}
