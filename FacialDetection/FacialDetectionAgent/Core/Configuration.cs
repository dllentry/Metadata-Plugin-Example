//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FacialDetectionAgent.Core
{
    /// <summary>
    /// The facial recognition agent's configuration.
    /// </summary>
    [DataContract(Name = "Configuration")]
    public class Configuration
    {
        public Configuration()
        {
            RtspPort = 7777;
            HttpPort = 43567;
            SerenityAddress = "10.220.232.229";
            SerenityUser = "admin";
            SerenityPassword = "admin123";
            ScaleFactor = 1.2;
            MinimumNeighbors = 5;
            SelectedDatasources = new List<DataSourceEntry>();
        }

        /// <summary>
        /// Gets and sets the port number to use for the embedded RTSP server.
        /// </summary>
        [DataMember]
        public int RtspPort { get; set; }

        /// <summary>
        /// Gets and sets the port number to use for the embedded HTTP server.
        /// </summary>
        [DataMember]
        public int HttpPort { get; set; }

        /// <summary>
        /// Gets and set the serenty server's
        /// </summary>
        [DataMember]
        public string SerenityAddress { get; set; }

        [DataMember]
        public string SerenityUser { get; set; }

        [DataMember]
        public string SerenityPassword { get; set; }

        [DataMember]
        public double ScaleFactor { get; set; }

        [DataMember]
        public int MinimumNeighbors { get; set; }

        [DataMember]
        public List<DataSourceEntry> SelectedDatasources { get; set; }
    }
}
