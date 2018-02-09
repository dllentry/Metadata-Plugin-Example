//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using FacialDetection.Events;
using NLog;
using Pelco.UI.VideoOverlay;
using Pelco.UI.VideoOverlay.Overlays;
using Prism.Events;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace FacialDetection.Core.Metadata
{
    public class DrawingCanvas : VideoOverlayCanvasBase<FacialMetadata>
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private Timer _timer;
        private bool _disposed;
        private IEventAggregator _eventAgg;

        public DrawingCanvas(IEventAggregator eventAgg) : base()
        {
            _disposed = false;
            _eventAgg = eventAgg;

            _timer = new Timer
            {
                AutoReset = true,
                Interval = 1000,
                Enabled = true,

            };
            _timer.Elapsed += Timer_Elapsed;

            _eventAgg.GetEvent<VideoViewChangeEvent>().Subscribe(OnVideoViewWindowChange);
            _eventAgg.GetEvent<DigitalPtzViewChangeEvent>().Subscribe(OnDigitalPtzViewChange);
            _eventAgg.GetEvent<AspectRatioChangeEvent>().Subscribe(OnOverlayStreamAspectRatioChange);
        }

        ~DrawingCanvas()
        {
            Dispose(false);
        }

        public override bool HandleObject(FacialMetadata md)
        {
            try
            {
                ResetTimer();
                ClearOverlays();
                foreach (var face in md.DiscoveredFaces.faces.Items)
                {
                    DrawOverlay(new RectangleOverlay()
                    {
                        BorderColor = Colors.Red,
                        UpperLeft = new Point(face.UpperLeftx, face.UpperLefty),
                        BottomRight = new Point(face.BottomRightx, face.BottomRighty),
                        TimeReference = md.TimeReference
                    });
                }
            }
            catch (Exception e)
            {
                LOG.Error($"Failed to draw overlay, reason: {e.Message}");
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _eventAgg.GetEvent<VideoViewChangeEvent>().Unsubscribe(OnVideoViewWindowChange);
                _eventAgg.GetEvent<DigitalPtzViewChangeEvent>().Unsubscribe(OnDigitalPtzViewChange);
                _eventAgg.GetEvent<AspectRatioChangeEvent>().Unsubscribe(OnStreamAspectRatioChange);

                base.Dispose(disposing);

                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Called every 500 ms to remove overlays that are no longer valid.
            ClearOverlays();
        }

        private void ResetTimer()
        {
            _timer.Interval = 1000;
        }

        private void OnStreamAspectRatioChange(double aspectRatio)
        {
            OnOverlayStreamAspectRatioChange(aspectRatio);
        }

        private void OnVideoViewWindowChange(VideoView view)
        {
            OnOverlayWindowChange(view.NormalizedView, view.Rotation);
        }

        private void OnDigitalPtzViewChange(Rect rect)
        {
            OnDigitalPtzViewChange(rect);
        }
    }
}
