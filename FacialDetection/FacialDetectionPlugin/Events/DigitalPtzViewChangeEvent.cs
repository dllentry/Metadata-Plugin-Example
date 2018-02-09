﻿//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Prism.Events;
using System.Windows;

namespace FacialDetection.Events
{
    /// <summary>
    /// Event sent when the digital PTZ view is changed, due to a
    /// digital ptz action.
    /// </summary>
    public class DigitalPtzViewChangeEvent : PubSubEvent<Rect>
    {
    }
}
