//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Pelco.Media.Pipeline;
using Pelco.Media.Pipeline.Sinks;
using Pelco.Media.Pipeline.Transforms;
using Pelco.Media.RTP;
using Pelco.Media.RTSP.Server;
using System.Net;

namespace FacialDetectionAgent.RTSP
{
    public class RtspSession : RtspSessionBase
    {
        private MediaPipeline _pipeline;

        public RtspSession(ISource source, byte payloadType, IPEndPoint target)
        {
            _pipeline = MediaPipeline.CreateBuilder()
                                     .Source(source)
                                     .Transform(new RtpPacketizer(new DefaultRtpClock(90000), SSRC, payloadType))
                                     .Sink(new UdpSink(target))
                                     .Build();
        }

        public override void Start()
        {
            _pipeline.Start();
        }

        public override void Stop()
        {
            _pipeline.Stop();
        }
    }
}
