//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FacialDetectionAgent.HTTP.Serenity
{
    public class Devices : SerenityResource
    {
        public Devices() : base("Devices")
        {
            List = new List<Device>();
        }

        [JsonProperty(PropertyName = "devices")]
        public List<Device> List { get; set; }
    }
}
