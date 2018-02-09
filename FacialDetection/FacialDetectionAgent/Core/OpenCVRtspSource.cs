//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using Pelco.Media.Common;
using Pelco.Media.Pipeline;
using Pelco.Media.RTSP;
using Pelco.Media.RTSP.Client;
using System;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace FacialDetectionAgent.Core
{
    /// <summary>
    /// Source used to receive images from an RTSP video source.
    /// </summary>
    public class OpenCVRtspSource : ObjectTypeSource<ImageFrame>
    {
        private static readonly object TsLock = new object();
        private static readonly object SourceLock = new object();
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private Uri _uri;
        private VideoCapture _source;
        private DateTime _lastFrameTs;
        private ManualResetEvent _stop;
        private TimeSpan _queryFrequency;
        private ManualResetEvent _started;

        /// <summary>
        /// Creates a new RtspVideoSource instance.  
        /// </summary>
        /// <param name="rtspUri">The RTSP uri to pull video from.</param>
        public OpenCVRtspSource(Uri rtspUri, TimeSpan queryFrequency)
        {
            _uri = rtspUri;
            _queryFrequency = queryFrequency;
            _stop = new ManualResetEvent(false);
            _started = new ManualResetEvent(false);
        }

        public bool IsOnline
        {
            get
            {
                lock (TsLock)
                {
                    // If the last timestamp is more that 5 seconds old then we will assume
                    // that the source is not online.
                    return (_lastFrameTs - DateTime.Now).TotalSeconds < 5;
                }
            }
        }

        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        public override void Start()
        {
            lock (SourceLock)
            {
                if (_source == null)
                {
                    _stop.Reset();
                    _started.Reset();

                    // We run the creation of the VideoCapture in a thread because it will block
                    // if something goes wrong.
                    Task.Run(() =>
                    {
                        try
                        {
                            _source = new VideoCapture(DetermineRealUri().ToString());
                            _started.Set(); // Indicate to the processing thread that the source is started.

                        }
                        catch (AccessViolationException e)
                        {
                            LOG.Error($"Cauge access violation exception: msg={e.Message}");
                        }
                        catch (Exception e)
                        {
                            LOG.Error(e, $"Unable to start RTSP source at endpoint '{_uri}'");
                        }
                    });

                    Task.Run(() => ReceiveAndPushFrames());
                }

                LOG.Info($"Started RTSP video source with uri '{_uri}'");
            }
        }

        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        public override void Stop()
        {
            lock (SourceLock)
            {
                try
                {
                    if (_source != null)
                    {
                        _stop.Set();
                        _source.Dispose();

                        _source = null;

                        LOG.Info($"Successfully shutdown RTSP video source with uri '{_uri}'");
                    }
                }
                catch (AccessViolationException e)
                {
                    LOG.Error($"Cauge access violation exception: msg={e.Message}");
                }
            }
        }

        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        private void ReceiveAndPushFrames()
        {
            if (!_started.WaitOne(TimeSpan.FromSeconds(10)))
            {
                LOG.Error("Rtsp capture source did not start within 10 seconds. Stopping...");
                Stop();
                return;
            }

            lock (TsLock)
            {
                _lastFrameTs = DateTime.Now;
            }

            while (!_stop.WaitOne(0))
            {
                Image<Bgr, byte> image = null;

                try
                {
                    using (var mat = _source.QueryFrame())
                    {

                        if (mat != null)
                        {
                            lock (TsLock)
                            {
                                _lastFrameTs = DateTime.Now;

                                image = mat.ToImage<Bgr, byte>();
                                PushObject(new ImageFrame() { Image = image, TimeStamp = _lastFrameTs });
                                image = null;
                            }
                        }
                    }
                }
                catch (AccessViolationException e)
                {
                    LOG.Error($"Cauge access violation exception: msg={e.Message}");
                }
                catch (ObjectDisposedException)
                {
                    LOG.Info($"Rtsp source was disposed for '{_uri}");
                }
                catch (Exception e)
                {
                    LOG.Error(e, $"Caught exception while receiving video frame, message={e.Message}");
                }
                finally
                {
                    if (image != null)
                    {
                        image.Dispose();
                    }
                }
            }
        }

        private Uri DetermineRealUri()
        {
            // The VideoXpert system does some non-standard RTSP redirecting so that it can determine the intent of
            // the client (intent being live or playback).  It stats off by creating a spoofed SDP and then will
            // redirect on play.  What we will do it write a wrapper that does some of the RTSP calls to first
            // determine what the actual live RTSP uri is.

            using (RtspClient client = new RtspClient(_uri))
            {
                try
                {
                    var method = RtspRequest.RtspMethod.OPTIONS;
                    CheckResponse(client.Send(RtspRequest.CreateBuilder()
                                                         .Method(method)
                                                         .Uri(_uri)
                                                         .Build()), method);

                    method = RtspRequest.RtspMethod.DESCRIBE;
                    var res = CheckResponse(client.Send(RtspRequest.CreateBuilder()
                                                                   .Method(method)
                                                                   .Uri(_uri)
                                                                   .Build()), method);

                    var sdp = res.GetBodyAsSdp();
                    if (!sdp.SessionName.Contains("Spoofed session"))
                    {
                        // We are not working with a spoofed session just return
                        // the current uri.
                        return _uri;
                    }

                    Uri controlUri = null;
                    var tracks = MediaTracks.FromSdp(sdp, _uri);
                    foreach (var track in tracks)
                    {
                        if (track.Type.Is(MimeType.ANY_VIDEO))
                        {
                            if (GetRedirectUri(client, track, out controlUri))
                            {
                                return controlUri;
                            }
                        }
                    }

                    throw new RtspClientException($"Unable to retrieve usable uri from server at endpoint '{_uri}'");
                }
                catch (Exception e)
                {
                    LOG.Error(e, $"Failed while communicating with RTSP server at '{_uri}'");
                    throw e;
                }
            }
        }

        private bool GetRedirectUri(RtspClient client, MediaTrack track, out Uri uri)
        {
            var transport = TransportHeader.CreateBuilder()
                                           .Type(TransportType.RtspInterleaved)
                                           .InterleavedChannels(0, 1)
                                           .Build();

            var method = RtspRequest.RtspMethod.SETUP;
            var res = CheckResponse(client.Send(RtspRequest.CreateBuilder()
                                                           .Method(method)
                                                           .Uri(track.ControlUri)
                                                           .AddHeader(RtspHeaders.Names.TRANSPORT, transport.ToString())
                                                           .Build()), method);

            if (!res.Headers.ContainsKey(RtspHeaders.Names.SESSION))
            {
                uri = null;
                return false;
            }
            var rtspSession = Session.Parse(res.Headers[RtspHeaders.Names.SESSION]);

            method = RtspRequest.RtspMethod.PLAY;
            res = CheckResponse(client.Send(RtspRequest.CreateBuilder()
                                                       .Method(method)
                                                       .Uri(track.ControlUri)
                                                       .AddHeader(RtspHeaders.Names.SESSION, rtspSession.ID)
                                                       .Build()), method);

            var status = res.ResponseStatus;

            if (status.Is(RtspResponse.Status.MovedPermanently) || status.Is(RtspResponse.Status.MovedTemporarily))
            {
                // We received a redirect lets get the uri and return it.

                if (res.Headers.ContainsKey(RtspHeaders.Names.LOCATION))
                {
                    var value = res.Headers[RtspHeaders.Names.LOCATION];

                    return Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uri);
                }
            }

            uri = null;
            return false;
        }

        private RtspResponse CheckResponse(RtspResponse response, RtspRequest.RtspMethod method)
        {
            var status = response.ResponseStatus;

            if (status.Code >= RtspResponse.Status.BadRequest.Code)
            {
                throw new RtspClientException($"{method} received response status {status.Code} {status.ReasonPhrase}");
            }

            return response;
        }
    }
}
