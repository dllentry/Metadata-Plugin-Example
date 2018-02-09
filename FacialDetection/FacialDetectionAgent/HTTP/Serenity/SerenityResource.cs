//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FacialDetectionAgent.HTTP.Serenity
{
    public class SerenityResource
    {
        public SerenityResource(string type)
        {
            ResourceType = type;
            Links = new Dictionary<string, Uri>();
        }

        [JsonProperty(PropertyName = "_type", Required = Required.Always)]
        public string ResourceType { get; set; }

        [JsonProperty(PropertyName = "_system_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemId { get; set; }
        
        [JsonProperty(PropertyName = "_links", Required = Required.Always)]
        public Dictionary<string, Uri> Links;
    }
}
