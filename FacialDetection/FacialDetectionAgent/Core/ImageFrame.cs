//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace FacialDetectionAgent.Core
{
    public class ImageFrame
    {
        public Image<Bgr, byte> Image { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
