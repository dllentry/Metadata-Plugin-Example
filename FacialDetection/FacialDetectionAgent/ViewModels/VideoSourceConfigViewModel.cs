//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using CPPCli;
using Emgu.CV;
using Emgu.CV.Structure;
using FacialDetectionAgent.Core;
using Pelco.Media.Pipeline;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace FacialDetectionAgent.ViewModels
{
    public class VideoSourceConfigViewModel : BindableBase, IObjectTypeSink<Image<Bgr, byte>>
    {
        private DataSource _dataSource;
        private MediaPipeline _pipeline;
        private BitmapSource _imageSource;
        private FacialRecognitionImageTransform _transform;

        public VideoSourceConfigViewModel(DataSource dataSource)
        {
            _dataSource = dataSource;
            _transform = new FacialRecognitionImageTransform();
        }

        public BitmapSource VideoImage
        {
            get
            {
                return _imageSource;
            }

            set
            {
                SetProperty(ref _imageSource, value);
            }
        }

        public double ScaleFactor
        {
            get
            {
                return _transform.ScaleFactor;
            }

            set
            {
                if (_transform.ScaleFactor != value)
                {
                    _transform.ScaleFactor = value;
                    RaisePropertyChanged(nameof(ScaleFactor));
                }
            }
        }

        public int MinimumNeighbors
        {
            get
            {
                return _transform.MinimumNeighbors;
            }

            set
            {
                if (_transform.MinimumNeighbors != value)
                {
                    _transform.MinimumNeighbors = value;
                    RaisePropertyChanged(nameof(MinimumNeighbors));
                }
            }
        }

        public ISource UpstreamLink { get; set; }

        public async Task StartStream()
        {
            await Task.Run(() =>
            {
                _pipeline = MediaPipeline.CreateBuilder()
                                         .Source(new OpenCVRtspSource(SelectDataInterface(_dataSource), TimeSpan.FromMilliseconds(250)))
                                         .Transform(_transform)
                                         .Sink(this)
                                         .Build();

                _pipeline.Start();
            });
        }

        public async Task StopStream()
        {
            if (_pipeline != null)
            {
                await Task.Run(() => _pipeline.Stop());
            }
        }

        public bool HandleObject(Image<Bgr, byte> image)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                VideoImage = ToBitmapSource(image);
            });

            return true;
        }

        public bool WriteBuffer(ByteBuffer buffer)
        {
            return true;
        }

        public void Stop()
        {
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr ptr);

        private static BitmapSource ToBitmapSource(IImage image)
        {
            using (var source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap();

                BitmapSource bms = Imaging.CreateBitmapSourceFromHBitmap(ptr,
                                                                         IntPtr.Zero,
                                                                         Int32Rect.Empty,
                                                                         BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr);
                return bms;
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

        public void PushEvent(Pelco.Media.Pipeline.MediaEvent e)
        {
            UpstreamLink?.OnMediaEvent(e);
        }
    }
}
