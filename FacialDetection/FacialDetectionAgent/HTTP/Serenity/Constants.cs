//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
namespace FacialDetectionAgent.HTTP.Serenity
{
    public sealed class Constants
    {
        public sealed class ContentTypes
        {
            public static readonly string SERENITY_RESOURCE = "application/vnd.pelco.resource+json";
        }

        public sealed class Rels
        {
            public static readonly string SELF = "self";
            public static readonly string ENDPOINT = "/pelco/rel/endpoint";
            public static readonly string DEVICE = "/pelco/rel/device";
            public static readonly string DEVICES = "/pelco/rel/devices";
            public static readonly string DATASOURCES = "/pelco/rel/data_sources";
        }
    }
}