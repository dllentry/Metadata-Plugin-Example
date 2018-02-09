//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Pelco.Media.Pipeline;

namespace FacialDetectionAgent.Core.Api
{
    /// <summary>
    /// A Facial recognition and/or detection source.  The source
    /// will provide metadata related to facial analytics data.
    /// </summary>
    public interface IDetectionSource
    {
        bool IsOnline { get; }

        /// <summary>
        /// Starts the detection source
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the detection source.
        /// </summary>
        void Stop();

        /// <summary>
        /// Retrieves the <see cref="MediaPipeline"/> source
        /// to be used as the source for another pipeline.
        /// </summary>
        /// <returns></returns>
        ISource GetPipelineSource();
    }
}
