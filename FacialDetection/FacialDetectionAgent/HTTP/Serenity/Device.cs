//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace FacialDetectionAgent.HTTP.Serenity
{
    public class Device : SerenityResource
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum DeviceTypeEnum
        {
            [EnumMember(Value = "encoder")]
            Encoder,

            [EnumMember(Value = "external")]
            External,

            [EnumMember(Value = "generic")]
            Generic
        }

        public Device() : base("Device")
        {
        }

        [JsonProperty(PropertyName = "base_version", NullValueHandling = NullValueHandling.Ignore)]
        public string BaseVersion { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "vendor", NullValueHandling = NullValueHandling.Ignore)]
        public string Vendor { get; set; }

        [JsonProperty(PropertyName = "version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "commissioned", Required = Required.Always)]
        public bool Commissioned { get; set; }

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "ip", Required = Required.Always)]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "state", Required = Required.Always)]
        public DeviceStateEnum State { get; set; }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public DeviceTypeEnum DeviceType { get; set; }
    }
}
