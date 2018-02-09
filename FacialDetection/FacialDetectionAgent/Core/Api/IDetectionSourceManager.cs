//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using System;
using System.Collections.Generic;

namespace FacialDetectionAgent.Core.Api
{
    public interface IDetectionSourceManager
    {
        /// <summary>
        /// Creates and registers a new facial detection source.
        /// </summary>
        /// <param name="datasourceId">The videoXpert datasource to process</param>
        /// <param name="uri">The endpoint for connecting to the video source</param>
        /// <returns></returns>
        bool RegisterSource(string datasourceId, Uri uri);

        /// <summary>
        /// Determines if a souce exists with the following id.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        bool ContainsSource(string sourceId);

        /// <summary>
        /// Gets a list of all the available source ids.
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetSourceIds();

        /// <summary>
        /// Gets the online state for a <see cref="IDetectionSource"/>.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        bool IsSourceOnline(string sourceId);

        /// <summary>
        /// Gets <see cref="IDetectionSource"/> a source by id.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        IDetectionSource GetSource(string sourceId);

        /// <summary>
        /// Attempts to get a <see cref="IDetectionSource"/> by id.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        bool TryGetSource(string sourceId, out IDetectionSource source);

        /// <summary>
        /// Shutsdown the detection source manager.  This can be used to clean up
        /// managed and created resources.
        /// </summary>
        void Shutdown();
    }
}
