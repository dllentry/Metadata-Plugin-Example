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
    public class SerenityError
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum CodeEnum
        {
            [EnumMember(Value = "Conflict")]
            Conflict,

            [EnumMember(Value = "InsufficientResource")]
            InsufficientResource,

            [EnumMember(Value = "NotReady")]
            NotReady,

            [EnumMember(Value = "OperationFailed")]
            OperationFailed,
        }

        [JsonProperty(PropertyName = "code")]
        public CodeEnum Code { get; set; }
        
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
