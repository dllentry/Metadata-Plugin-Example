//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using System;
using System.Web;
using System.Net;
using Pelco.Media.RTSP.Server;
using Pelco.Media.RTSP;
using Pelco.Media.RTSP.SDP;
using FacialDetectionAgent.Core.Api;
using Nancy.Helpers;

namespace FacialDetectionAgent.RTSP
{
    public class RtspRequestHandler : RequestHandlerBase
    {
        private static readonly byte PAYLOAD_TYPE = 98;
        private static readonly string MIME_TYPE = "vnd.opencv.facial_detection";

        private RtspSessionManager _sessionMgr;
        private IDetectionSourceManager _sourceMgr;

        public RtspRequestHandler(IDetectionSourceManager sourceMgr, RtspSessionManager sessionMgr)
        {
            _sourceMgr = sourceMgr;
            _sessionMgr = sessionMgr;
        }

        public override void Init()
        {
            base.Init();

            _sessionMgr.Start();
        }

        public override void Close()
        {
            base.Close();

            _sessionMgr.Stop();
        }

        public override RtspResponse Describe(RtspRequest request)
        {
            var queryParams = HttpUtility.ParseQueryString(request.URI.Query);
            var sourceId = queryParams["sourceId"];

            var builder = RtspResponse.CreateBuilder().Status(RtspResponse.Status.Ok);

            if (sourceId == null && !_sourceMgr.ContainsSource(sourceId))
            {
                builder.Status(RtspResponse.Status.NotFound);
            }
            else
            {
                builder.Body(CreateSdp(request.URI));
            }

            return builder.Build();
        }

        public override RtspResponse GetParamater(RtspRequest request)
        {
            if (request.Headers.ContainsKey(RtspHeaders.Names.SESSION))
            {
                var session = request.Headers[RtspHeaders.Names.SESSION];

                var builder = RtspResponse.CreateBuilder().Status(RtspResponse.Status.Ok);

                if (!_sessionMgr.RefreshSession(session))
                {
                    builder.Status(RtspResponse.Status.SessionNotFound);
                }

                return builder.Build();
            }
            else
            {
                return RtspResponse.CreateBuilder().Status(RtspResponse.Status.BadRequest).Build();
            }
        }

        public override RtspResponse Play(RtspRequest request)
        {
            var builder = RtspResponse.CreateBuilder().Status(RtspResponse.Status.SessionNotFound);

            if (request.Headers.ContainsKey(RtspHeaders.Names.SESSION))
            {
                var session = request.Headers[RtspHeaders.Names.SESSION];

                if (_sessionMgr.PlaySession(session))
                {
                    builder.Status(RtspResponse.Status.Ok);
                }
            }

            return builder.Build();
        }

        public override RtspResponse SetUp(RtspRequest request)
        {
            var builder = RtspResponse.CreateBuilder();

            var queryParams = HttpUtility.ParseQueryString(request.URI.Query);
            var sourceId = queryParams["sourceId"];
            if (sourceId == null || !_sourceMgr.ContainsSource(sourceId))
            {
                return builder.Status(RtspResponse.Status.NotFound).Build();
            }
            
            var transport = request.Transport;
            if (transport == null)
            {
                return builder.Status(RtspResponse.Status.BadRequest).Build();
            }

            if (transport.Type != Pelco.Media.RTSP.TransportType.UdpUnicast)
            {
                return builder.Status(RtspResponse.Status.UnsupportedTransport).Build();
            }

            if (transport.ClientPorts == null || !transport.ClientPorts.IsSet)
            {
                return builder.Status(RtspResponse.Status.BadRequest).Build();
            }

            var rtpPort = transport.ClientPorts.RtpPort;
            var address = transport.Destination != null ? Dns.GetHostAddresses(transport.Destination)[0]
                                                        : request.RemoteEndpoint.Address;

            var rtspSession = new RtspSession(_sourceMgr.GetSource(sourceId).GetPipelineSource(),
                                              PAYLOAD_TYPE,
                                              new IPEndPoint(address, rtpPort));

            if (!_sessionMgr.RegisterSession(rtspSession))
            {
                return builder.Status(RtspResponse.Status.InternalServerError)
                              .AddHeader(RtspHeaders.Names.CONTENT_TYPE, "text/plain")
                              .Body("Unable to register Rtsp session with system")
                              .Build();
            }

            var session = Session.FromParts(rtspSession.Id, RtspSession.RTSP_SESSION_TIMEOUT);

            transport = TransportHeader.CreateBuilder()
                                       .Type(Pelco.Media.RTSP.TransportType.UdpUnicast)
                                       .ClientPorts(transport.ClientPorts)
                                       .ServerPorts(rtpPort + 3, rtpPort + 4) // Just create dummy ports.
                                       .Build();

            return builder.Status(RtspResponse.Status.Ok)
                          .AddHeader(RtspHeaders.Names.TRANSPORT, transport.ToString())
                          .AddHeader(RtspHeaders.Names.SESSION, session.ToString())
                          .Build();
        }

        public override RtspResponse TearDown(RtspRequest request)
        {
            var builder = RtspResponse.CreateBuilder().Status(RtspResponse.Status.SessionNotFound);

            if (request.Headers.ContainsKey(RtspHeaders.Names.SESSION))
            {
                var session = request.Headers[RtspHeaders.Names.SESSION];

                if (_sessionMgr.TearDownSession(session))
                {
                    builder.Status(RtspResponse.Status.Ok);
                }
            }

            return builder.Build();
        }

        private SessionDescription CreateSdp(Uri uri)
        {
            var sdp = new SessionDescription();

            var media = MediaDescription.CreateBuilder()
                                        .MediaType(MediaType.APPLICATION)
                                        .Port(0)
                                        .Protocol(TransportProtocol.RTP_AVP)
                                        .AddFormat(PAYLOAD_TYPE)
                                        .AddAttribute(new Pelco.Media.RTSP.SDP.Attribute("control", uri.ToString()))
                                        .AddAttribute(new Pelco.Media.RTSP.SDP.Attribute("rtpmap", $"{PAYLOAD_TYPE} {MIME_TYPE}/90000"))
                                        .Build();

            sdp.SessionInformation = "OpenCV Facial Detection RTSP Session";
            sdp.MediaDescriptions.Add(media);

            sdp.Connection = ConnectionInfo.CreateBuilder()
                                           .NetType(NetworkType.IN)
                                           .AddrType(AddressType.IP4)
                                           .Address("0.0.0.0")
                                           .Build();

            return sdp;
        }
    }
}
