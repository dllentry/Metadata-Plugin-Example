//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FacialDetectionAgent.HTTP.Serenity
{
    public class DataSource : SerenityResource
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum DataSourceTypeEnum
        {
            [EnumMember(Value = "audio")]
            Audio,

            [EnumMember(Value = "metadata")]
            Metadata,

            [EnumMember(Value = "video")]
            Video
        }

        public DataSource() : base("DataSource")
        {
            Recorded = false;
            DataInterfaces = new List<DataInterface>();
        }

        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { get; set; }

        [JsonProperty(PropertyName = "recorded")]
        public bool Recorded { get; set; }

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "ip", Required =  Required.Always)]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "live", Required = Required.Always)]
        public bool IsLive { get; set; }

        [JsonProperty(PropertyName = "state", Required = Required.Always)]
        public DeviceStateEnum State { get; set; }

        [JsonProperty(PropertyName = "type")]
        public DataSourceTypeEnum DataSourceType { get; set; }

        [JsonProperty(PropertyName = "encoding", Required = Required.AllowNull)]
        public string Encoding { get; set; }

        [JsonProperty(PropertyName = "data_interfaces")]
        public List<DataInterface> DataInterfaces { get; set; }
    }
}
