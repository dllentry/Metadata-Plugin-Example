//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using NLog;
using Pelco.Media.Pipeline;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace FacialDetection.Core.Metadata
{
    public class FacialDiscoveryTransform : BufferToObjectTypeTransformBase<FacialMetadata>
    {
        private readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private XmlSerializer _serializer;

        public FacialDiscoveryTransform()
        {
            _serializer = new XmlSerializer(typeof(FacialDiscovery));
        }

        public override bool WriteBuffer(ByteBuffer buffer)
        {
            try
            {
                using (var reader = new StringReader(buffer.ToString(Encoding.UTF8)))
                {
                    return PushObject(new FacialMetadata()
                    {
                        TimeReference = buffer.TimeReference,
                        DiscoveredFaces = (FacialDiscovery)_serializer.Deserialize(reader)
                    });
                }
            }
            catch (Exception e)
            {
                LOG.Error($"Unable to process facial detection metadata, reason={e.Message}");
                return true;
            }
        }
    }
}
