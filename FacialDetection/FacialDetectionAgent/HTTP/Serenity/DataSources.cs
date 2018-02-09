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
    public class DataSources : SerenityResource
    {
        public DataSources() : base("DataSources")
        {
            List = new List<DataSource>();
        }

        [JsonProperty(PropertyName = "data_sources")]
        public List<DataSource> List { get; set; }
    }
}
