//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using FacialDetection.Core.Api;
using FacialDetection.Core.Metadata;
using FacialDetection.Events;
using Microsoft.Practices.Unity;
using NLog;
using Pelco.Media.Metadata.Api;
using Pelco.Phoenix.PluginHostInterfaces;
using Prism.Events;
using System;
using System.Windows;

namespace FacialDetection
{
    public class Plugin : PluginBase, IOCCPluginPlaybackNotifications, IOCCPluginVideoOverlay
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private Bootstrapper _bootstrapper;
        private IEventAggregator _eventAgg;
        private MetadataStreamManager _streamManager;

        public Plugin(IHost host)
        {
            _bootstrapper = new Bootstrapper(host);
        }

        public override string Name => "OpenCV Facial Recognition";

        public override string Description => "OpenCV Facial Recognition plugin";

        public override string Version => "1.0.0";

        public override bool IsOverlay => true;

        public override string PluginID => "d48759ee-4333-4806-aa4e-670e05613553";

        public override FrameworkElement CreateControl()
        {
            LOG.Info("Creating framework control");

            RunBootstrapper();
            return _bootstrapper.Container.Resolve<MainUserControl>();
        }

        public override string GetPluginKey()
        {
            return "eSY/QPsQvRvJOnyl2rqWTcLTs1q5UQJfy6fNHVux00ol+MRAgXhrAANJLDyBbHGg1BpAGKSdGHjESshEmrYMUg==";
        }

        public override void Shutdown()
        {
            LOG.Info("Received plugin shutdown request");

            _bootstrapper.Container.Resolve<IHostService>().RequestShutdown();
        }

        #region IOCCPluginPlaybackNotifications

        public async void OnNewVideoSourcePlaying(PlayingState state)
        {
            LOG.Info($"Received new video playing event vdsId={state.VideoDataSourceId}");

            await _streamManager?.TryRegisterStream(state.VideoDataSourceId);
        }

        public void OnPlayUpdate(bool isLive, DateTime? anchorTime, DateTime? initiationTime, double speed)
        {
            LOG.Info($"Received video update event live={isLive}, acnchor={anchorTime}, initiation={initiationTime}, speed={speed}");
        }

        public void OnVideoPaused()
        {
            LOG.Info("Received paused event");
        }

        public void OnVideoRemoved()
        {
            LOG.Info("Received video removed event");
        }

        #endregion

        #region IOCCPluginVideoOverlay

        public FrameworkElement CreateVideoOverlay()
        {
            // We must create a drawing canvas on the UI thread. The following code
            // will create the canvas on the UI thread send an event to pass it along
            // to any active metadata pipelines so that metadata can be annotated ontop
            // of the video stream.
            //return Application.Current.Dispatcher.Invoke(() =>
            //{
                var canvas = new DrawingCanvas(_eventAgg);

                // Send to active metadata pipelines.
                _eventAgg.GetEvent<CanvasSinkCreatedEvent>().Publish(canvas);

                // Return the actual FrameworkElement to the OpsCenter.
                return canvas.GetVisualOverlay();
            //});
        }

        public void OnVideoViewStreamAspectRatio(double aspectRatio)
        {
            _eventAgg.GetEvent<AspectRatioChangeEvent>().Publish(aspectRatio);
        }

        public void OnVideoViewDigitalPtzInfo(Rect normalizedDPTZWindow)
        {
            _eventAgg.GetEvent<DigitalPtzViewChangeEvent>().Publish(normalizedDPTZWindow);
        }

        public void OnVideoViewWindow(Rect normalizedVideoWindow, double rotation)
        {
            _eventAgg.GetEvent<VideoViewChangeEvent>().Publish(new VideoView
            {
                Rotation = rotation,
                NormalizedView = normalizedVideoWindow
            });
        }

        #endregion
        
        private void RunBootstrapper()
        {
            if (!_bootstrapper.HasRun)
            {
                LOG.Info("Running bootstrapper");

                _bootstrapper.Run();

                _eventAgg = _bootstrapper.Container.Resolve<IEventAggregator>();
                _streamManager = _bootstrapper.Container.Resolve<IMetadataStreamManager>() as MetadataStreamManager;
                _bootstrapper.Container.Resolve<IHostService>().RegisterForVideoVideoPlaybackEvents();

                LOG.Info("Plugin bootstrapper initialized");
            }
        }
    }
}
