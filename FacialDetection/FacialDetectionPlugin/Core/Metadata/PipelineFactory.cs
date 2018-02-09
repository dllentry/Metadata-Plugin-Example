//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using NLog;
using Pelco.Media.Metadata.Api;
using Pelco.Media.Pipeline;

namespace FacialDetection.Core.Metadata
{
    /// <summary>
    /// A pipeline factor used to build a custom pipeline for processing
    /// facial recognition metadata.
    /// </summary>
    public class PipelineFactory : IPipelineFactory
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private DelayedInitializationSink _sink;

        public PipelineFactory()
        {
            _sink = new DelayedInitializationSink();
        }

        public MediaPipeline CreatePipeline(ISource src, bool isLive)
        {
            return MediaPipeline.CreateBuilder()
                                .Source(src)
                                .Transform(new FacialDiscoveryTransform())
                                .Sink(_sink)
                                .Build();
        }

        /// <summary>
        /// Allows for setting the UI canvas sink
        /// </summary>
        public IObjectTypeSink<FacialMetadata> CanvasSink
        {
            set
            {
                LOG.Info($"Setting delayed sink to {value.GetType().Name}");
                _sink.CanvasSink = value;
            }
        }
    }
}
