//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using FacialDetectionAgent.HTTP.Serenity;
using FacialDetectionCommon.Utils;
using Nancy;
using System;

namespace FacialDetectionAgent.HTTP
{
    public class DeviceModules : NancyModule
    {
        public static readonly string ID = "0f92cfaa-3326-4a3c-9833-3d57b6fea582";
        public DeviceModules()
        {

            var systemId = GuidUtil.GetDeterministicGuid(NetworkUtil.GetSystemMacAddress()).ToString();

            Get["/system/local"] = _ =>
            {
                var response = Response.AsJson(CreateDevice(systemId));
                response.ContentType = Constants.ContentTypes.SERENITY_RESOURCE;

                return response;
            };

            Get["/system/devices"] = _ =>
            {
                var devices = new Devices() { SystemId = systemId };
                devices.List.Add(CreateDevice(systemId));

                var response = Response.AsJson(devices);
                response.ContentType = Constants.ContentTypes.SERENITY_RESOURCE;

                return response;
            };

            Get["/system/devices/{id}"] = parameters =>
            {
                if (parameters.id != ID)
                {
                    var res = new Response();
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.ReasonPhrase = "Not Found";

                    return res;
                }

                var response = Response.AsJson(CreateDevice(systemId));
                response.ContentType = Constants.ContentTypes.SERENITY_RESOURCE;

                return response;
            };
        }

        private Device CreateDevice(string systemId)
        {
            var device = new Device()
            {
                Id = ID,
                SystemId = systemId,
                Name = "OpenCV Facial Recognition Example Device",
                Vendor = "Pelco",
                Version = "1.0.0",
                Commissioned = true,
                IpAddress = NetworkUtil.GetPrimaryAddress().ToString(),
                State = DeviceStateEnum.Online,
                DeviceType = Device.DeviceTypeEnum.Generic,
            };

            device.Links.Add(Constants.Rels.SELF, new Uri($"/system/devices/{ID}", UriKind.Relative));
            device.Links.Add(Constants.Rels.DATASOURCES, new Uri("/system/datasources", UriKind.Relative));

            return device;
        }
    }
}
