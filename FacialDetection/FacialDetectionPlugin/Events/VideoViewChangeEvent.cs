//
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
    public sealed class VideoView
    {
        public double Rotation { get; set; }
        public Rect NormalizedView { get; set; }
    }

    /// <summary>
    /// Event sent when the video view has changed. This occurs when
    /// the viewable video area is changed due to a video cell resize.
    /// </summary>
    public class VideoViewChangeEvent : PubSubEvent<VideoView>
    {
    }
}
