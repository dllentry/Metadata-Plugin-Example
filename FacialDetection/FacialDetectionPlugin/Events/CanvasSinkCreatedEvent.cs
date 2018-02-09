//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using FacialDetection.Core.Metadata;
using Pelco.UI.VideoOverlay;
using Prism.Events;

namespace FacialDetection.Events
{
    /// <summary>
    /// Event sent when the video canvas is created. The canvas must be created
    /// on the UI thread and will be created on the Plugin.CreateVideoOverlay
    /// call and will be passed to the MetadataStreamManager.
    /// </summary>
    public class CanvasSinkCreatedEvent : PubSubEvent<IVideoOverlayCanvas<FacialMetadata>>
    {
    }
}
