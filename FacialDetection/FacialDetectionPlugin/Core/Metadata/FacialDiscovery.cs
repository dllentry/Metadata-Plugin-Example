//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FacialDetection.Core.Metadata
{
    [XmlRoot(ElementName = "FaceDiscovery")]
    public class FacialDiscovery
    {
        [XmlElement(ElementName = "Faces")]
        public Faces faces { get; set; }
    }

    public class Faces
    {
        [XmlElement(ElementName = "Face")]
        public List<Face> Items { get; set; }
    }

    public class Face
    {
        [XmlAttribute(AttributeName = "x-upper-left")]
        public double UpperLeftx { get; set; }

        [XmlAttribute(AttributeName = "y-upper-left")]
        public double UpperLefty { get; set; }

        [XmlAttribute(AttributeName = "x-bottom-right")]
        public double BottomRightx { get; set; }

        [XmlAttribute(AttributeName = "y-bottom-right")]
        public double BottomRighty { get; set; }
    }
}
