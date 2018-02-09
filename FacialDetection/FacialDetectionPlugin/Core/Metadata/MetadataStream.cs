//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using NLog;
using Pelco.Media.Metadata;
using Pelco.Media.Metadata.Api;
using System;

namespace FacialDetection.Core.Metadata
{
    public class MetadataStream : MetadataStreamBase
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private IPipelineFactory _pipelineFactory;

		public MetadataStream(IPipelineFactory factory, Uri rtspUri) : base(Constants.MIME_TYPE, rtspUri)
        {
            _pipelineFactory = factory;
        }

        public override IPipelineFactory GetPipelineFactory()
        {
            return _pipelineFactory;
        }
    }
}
