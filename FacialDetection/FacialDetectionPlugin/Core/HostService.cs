//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using FacialDetection.Core.Api;
using NLog;
using Pelco.Phoenix.PluginHostInterfaces;
using System;

namespace FacialDetection.Core
{
    public class HostService : IHostService
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private IHost _host;
        private IOCCHostSerenity _serenity;
        private IOCCHostOverlay _hostOverlay;
        private IOCCHostPlaybackController _playbackController;

        public HostService(IHost host)
        {
            _host = host;

            _serenity = host.GetService<IOCCHostSerenity>();
            if (_serenity == null)
            {
                host.ReportFatalError("IOCCHostSerenity service is not available", "Unable to load plugin due to missing host service");
                return;
            }

            _playbackController = host.GetService<IOCCHostPlaybackController>();
            if (_playbackController == null)
            {
                host.ReportFatalError("IOCCHostPlaybackController service is not available", "Unable to load plugin due to missing host service");
                return;
            }

            _hostOverlay = host.GetService<IOCCHostOverlay>();
            if (_hostOverlay == null)
            {
                host.ReportFatalError("IOCCHostOverlay service is not available", "Unable to load plugin due to missing host service");
                return;
            }

            var videoOverlay = host.GetService<IOCCHostVideoOverlay>();
            if (videoOverlay == null)
            {
                host.ReportFatalError("IOCCHostVideoOverlay service is not available", "Unable to load plugin due to missing host service");
                return;
            }

            // Register for video view notifications
            videoOverlay.RegisterForVideoViewNotifications(true);

            // Make sure we register for playback notifications so that we can control
            // and manage our metadata streams.
            //_playbackController.RegisterForVideoPlaybackNotifications(true);

            _hostOverlay.SetOverlayAnchor(AnchorTypes.right, 20, 20, 500);

            LOG.Debug("Successfully created PluginModel instance");
        }

        /// <summary>
        /// Gets the Serenity authorization token used for authorizing calls
        /// to the connected videoXpert system.
        /// </summary>
        public string SerenityAuthToken
        {
            get
            {
                return _serenity.GetAuthToken();
            }
        }

        /// <summary>
        /// Gets the VideoXpert (Serenity) system uri used to communicate with the
        /// connected VideoXpert system.
        /// </summary>
        public Uri VideoXpertEndpoint
        {
            get
            {
                return new Uri(_serenity.GetBaseURI());
            }
        }

        /// <summary>
        /// Issues a pause command to the video playing in this plugin's cell.
        /// </summary>
        public void PauseVideo()
        {
            _playbackController.Pause();
        }

        /// <summary>
        /// Issues an un-pause command to the paused video in this plugin's cell.
        /// </summary>
        public void ResumePausedVideo()
        {
            _playbackController.Play();
        }

        /// <summary>
        /// Issues a seek command to the video playing in the plugin's cell.
        /// </summary>
        /// <param name="seekTime">The date and time to seek to.</param>
        public void SeekVideo(DateTime seekTime)
        {
            _playbackController.Seek(seekTime);
        }

        /// <summary>
        /// Issues a command to jump to live video.  If the video is already live this
        /// has no affect, otherwise the video stream will be jumped to live.
        /// </summary>
        public void JumpToLiveVideo()
        {
            _playbackController.JumpToLive();
        }

        /// <summary>
        /// Issues a command to change the currently playing datasource in this plugin's cell.
        /// </summary>
        /// <param name="datasourceId">The datasource to play</param>
        /// <param name="playAt">The time to start playing at.</param>
        public void ShowDataSource(string datasourceId, DateTime? playAt)
        {
            _playbackController.SetOnScreenDataSource(datasourceId, playAt);
        }

        /// <summary>
        /// <see cref="IHostService.RequestShutdown"/>
        /// </summary>
        public void RequestShutdown()
        {
            _host.RequestClose();
        }

        /// <summary>
        /// <see cref="IHostService.RegisterForVideoVideoPlaybackEvents"/>
        /// </summary>
        public void RegisterForVideoVideoPlaybackEvents()
        {
            _playbackController.RegisterForVideoPlaybackNotifications(true);
        }
    }
}
