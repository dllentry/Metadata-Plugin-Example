//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Pelco.Media.Pipeline;

namespace FacialDetection.Core.Metadata
{
    /// <summary>
    /// A delayed initialization sink is used to allow us to create a pipeline
    /// that requires a UI sink that requires creation in the UI thread. The UI
    /// sink will be created at some later time and this sink will allows us to
    /// handle this case.
    /// </summary>
    public class DelayedInitializationSink : ObjectTypeSinkBase<FacialMetadata>
    {
        private IObjectTypeSink<FacialMetadata> _sink;

        public DelayedInitializationSink()
        {
            _sink = null;
        }
        
        /// <summary>
        /// <see cref="ObjectTypeSinkBase{T}.HandleObject(T)"/>
        /// </summary>
        /// <param name="faces"></param>
        /// <returns></returns>
        public override bool HandleObject(FacialMetadata faces)
        {
            lock (this)
            {
                return _sink != null ? _sink.HandleObject(faces) : true;
            }
        }
        
        /// <summary>
        /// Set the actual UI sink when it is available
        /// </summary>
        public IObjectTypeSink<FacialMetadata> CanvasSink
        {
            set
            {
                lock (this)
                {
                    _sink = value;
                }
            }
        }
    }
}
