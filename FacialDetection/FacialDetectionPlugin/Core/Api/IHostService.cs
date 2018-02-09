//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using System;

namespace FacialDetection.Core.Api
{
    /// <summary>
    /// Provides access to plugin host services.
    /// </summary>
    public interface IHostService
    {
        /// <summary>
        /// Gets the Serenity authorization token used for authorizing calls
        /// to the connected videoXpert system.
        /// </summary>
        string SerenityAuthToken { get; }

        /// <summary>
        /// Gets the VideoXpert (Serenity) system uri used to communicate with the
        /// connected VideoXpert system.
        /// </summary>
        Uri VideoXpertEndpoint { get; }

        /// <summary>
        /// Issues a pause command to the video playing in this plugin's cell.
        /// </summary>
        void PauseVideo();

        /// <summary>
        /// Issues an un-pause command to the paused video in this plugin's cell.
        /// </summary>
        void ResumePausedVideo();

        /// <summary>
        /// Issues a seek command to the video playing in the plugin's cell.
        /// </summary>
        /// <param name="seekTime">The date and time to seek to.</param>
        void SeekVideo(DateTime seekTime);

        /// <summary>
        /// Issues a command to jump to live video.  If the video is already live this
        /// has no affect, otherwise the video stream will be jumped to live.
        /// </summary>
        void JumpToLiveVideo();

        /// <summary>
        /// Issues a command to change the currently playing datasource in this plugin's cell.
        /// </summary>
        /// <param name="datasourceId">The datasource to play</param>
        /// <param name="playAt">The time to start playing at.</param>
        void ShowDataSource(string datasourceId, DateTime? playAt);

        /// <summary>
        /// Issues a request to the host to shutdown this plugin instance.
        /// </summary>
        void RequestShutdown();

        /// <summary>
        /// Request to receive video playback events.
        /// </summary>
        void RegisterForVideoVideoPlaybackEvents();
    }
}
