﻿//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Prism.Events;

namespace FacialDetection.Events
{
    /// <summary>
    /// Event sent with the video stream's aspect ratio is updated.
    /// </summary>
    public class AspectRatioChangeEvent : PubSubEvent<double>
    {
    }
}
