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
    public class SystemModule : NancyModule
    {
        public SystemModule()
        {
            Get["/system"] = _ =>
            {
                var system = new SerenitySystem()
                {
                    Id = GuidUtil.GetDeterministicGuid(NetworkUtil.GetSystemMacAddress()).ToString(),
                };

                system.Links.Add(Constants.Rels.DEVICE, new Uri("/system/local", UriKind.Relative));
                system.Links.Add(Constants.Rels.DEVICES, new Uri("/system/devices", UriKind.Relative));
                system.Links.Add(Constants.Rels.DATASOURCES, new Uri("/system/datasources", UriKind.Relative));

                var response = Response.AsJson(system);
                response.ContentType = Constants.ContentTypes.SERENITY_RESOURCE;

                return response;
            };
        }
    }
}
