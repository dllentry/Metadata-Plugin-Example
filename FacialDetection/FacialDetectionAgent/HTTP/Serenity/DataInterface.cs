//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace FacialDetectionAgent.HTTP.Serenity
{
    public class DataInterface : SerenityResource
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum StreamFormatEnum
        {
            [EnumMember(Value = "h264")]
            H264,

            [EnumMember(Value = "h265")]
            H265,

            [EnumMember(Value = "mpeg4")]
            MPEG4,

            [EnumMember(Value = "jpeg")]
            JPEG,

            [EnumMember(Value = "g711")]
            G711,

            [EnumMember(Value = "metadata")]
            Metadata
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum StreamProtocolEnum
        {
            [EnumMember(Value = "mjpeg-pull")]
            MJPEG_PULL,

            [EnumMember(Value = "rtsp/rtp")]
            RTSP_RTP
        }

        public DataInterface() : base("DataInterface")
        {
        }

        [JsonProperty(PropertyName = "endpoint", Required = Required.Always)]
        public string Endpoint { get; set; }

        [JsonProperty(PropertyName = "format", NullValueHandling = NullValueHandling.Ignore)]
        public StreamFormatEnum Format { get; set; }

        [JsonProperty(PropertyName = "multicast", Required = Required.Always)]
        public bool Multicast { get; set; }

        [JsonProperty(PropertyName = "protocol", Required = Required.Always)]
        public StreamProtocolEnum Protocol { get; set; }

        public void SetEndpoint(Uri uri)
        {
            Links.Add(Constants.Rels.ENDPOINT, uri);
        }
    }
}
