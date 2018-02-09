//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using FacialDetectionAgent.Core.Api;
using Pelco.Media.Pipeline;
using Pelco.Media.Pipeline.Sinks;
using System;

namespace FacialDetectionAgent.Core
{
    public class FacialDetectionSource : IDetectionSource
    {
        private TeeSink _teeSink;
        private OpenCVRtspSource _src;
        private MediaPipeline _pipeline;

        public FacialDetectionSource(Uri rtspUri, Configuration config)
        {
            var transform = new FacialRecognitionTransform();
            transform.ScaleFactor = config.ScaleFactor;
            transform.MinimumNeighbors = config.MinimumNeighbors;

            _teeSink = new TeeSink();
            _src = new OpenCVRtspSource(rtspUri, TimeSpan.FromMilliseconds(250));

            _pipeline = MediaPipeline.CreateBuilder()
                                     .Source(_src)
                                     .Transform(transform)
                                     .Sink(_teeSink)
                                     .Build();
        }

        public bool IsOnline
        {
            get
            {
                return _src.IsOnline;
            }
        }

        public void Start()
        {
            _pipeline.Start();
        }

        public void Stop()
        {
            _pipeline.Stop();
            _teeSink.Stop();
        }

        public ISource GetPipelineSource()
        {
            return _teeSink.CreateSource();
        }
    }
}
