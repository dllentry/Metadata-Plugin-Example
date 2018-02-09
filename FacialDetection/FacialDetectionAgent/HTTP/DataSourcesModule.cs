//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using FacialDetectionAgent.Core;
using FacialDetectionAgent.Core.Api;
using FacialDetectionAgent.HTTP.Serenity;
using FacialDetectionCommon.Utils;
using Nancy;
using System;

namespace FacialDetectionAgent.HTTP
{
    public class DataSourcesModule : NancyModule
    {
        private static readonly string MIME_TYPE = "application/vnd.opencv.facial_detection";

        public DataSourcesModule()
        {
            var config = ServiceRegistry.Get<Configuration>();
            var manager = ServiceRegistry.Get<IDetectionSourceManager>();
            var systemId = GuidUtil.GetDeterministicGuid(NetworkUtil.GetSystemMacAddress()).ToString();

            Get["/system/datasources"] = _ =>
            {
                var datasources = new DataSources() { SystemId = systemId };

                foreach (var srcId in manager.GetSourceIds())
                {
                    var ds = new DataSource()
                    {
                        Id = srcId,
                        SystemId = systemId,
                        Enabled = true,
                        IsLive = true,
                        IpAddress = NetworkUtil.GetPrimaryAddress().ToString(),
                        State = manager.IsSourceOnline(srcId) ? DeviceStateEnum.Online : DeviceStateEnum.Offline,
                        Encoding = MIME_TYPE,
                        DataSourceType = DataSource.DataSourceTypeEnum.Metadata,
                    };

                    ds.Links.Add(Constants.Rels.SELF, new Uri($"/system/datasources/{srcId}", UriKind.Relative));
                    ds.Links.Add(Constants.Rels.DEVICE, new Uri($"/system/devices/{DeviceModules.ID}", UriKind.Relative));

                    string uri = $"rtsp://{NetworkUtil.GetPrimaryAddress()}:{config.RtspPort}/stream?sourceId={srcId}";
                    var di = new DataInterface()
                    {
                        Multicast = false,
                        Endpoint = uri,
                        Format = DataInterface.StreamFormatEnum.Metadata,
                        Protocol = DataInterface.StreamProtocolEnum.RTSP_RTP,
                    };
                    di.SetEndpoint(new Uri(uri));

                    ds.DataInterfaces.Add(di);

                    datasources.List.Add(ds);
                }

                var response = Response.AsJson(datasources);
                response.ContentType = Constants.ContentTypes.SERENITY_RESOURCE;

                return response;
            };

            Get["/system/datasources/{id}"] = parameters =>
            {
                if (!manager.ContainsSource(parameters.id))
                {
                    var res = new Response();
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.ReasonPhrase = "Not Found";

                    return res;
                }

                var srcId = parameters.id;

                var ds = new DataSource()
                {
                    Id = srcId,
                    SystemId = systemId,
                    Enabled = true,
                    IsLive = true,
                    IpAddress = NetworkUtil.GetPrimaryAddress().ToString(),
                    State = manager.IsSourceOnline(srcId) ? DeviceStateEnum.Online : DeviceStateEnum.Offline,
                    Encoding = MIME_TYPE,
                    DataSourceType = DataSource.DataSourceTypeEnum.Metadata,
                };

                ds.Links.Add(Constants.Rels.SELF, new Uri($"/system/datasources/{srcId}"));
                ds.Links.Add(Constants.Rels.DEVICE, new Uri($"/system/devices/{DeviceModules.ID}", UriKind.Relative));

                var uri = $"rtsp://{NetworkUtil.GetPrimaryAddress()}:{config.RtspPort}/stream?sourceId={srcId}";
                var di = new DataInterface()
                {
                    Multicast = false,
                    Endpoint = uri,
                    Format = DataInterface.StreamFormatEnum.Metadata,
                    Protocol = DataInterface.StreamProtocolEnum.RTSP_RTP,
                };
                di.SetEndpoint(new Uri(uri));

                ds.DataInterfaces.Add(di);

                var response = Response.AsJson(ds);
                response.ContentType = Constants.ContentTypes.SERENITY_RESOURCE;

                return response;
            };
        }
    }
}
