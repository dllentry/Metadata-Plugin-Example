//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using CPPCli;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FacialDetectionAgent.Core.Api
{
    public interface IDataSourcesManager
    {
        /// <summary>
        /// Allows instances to created and initialize resources if needed.
        /// </summary>
        /// <returns>True if the initialization was successful, false otherwise</returns>
        Task<bool> Init();

        /// <summary>
        /// Allows instances to 
        /// </summary>
        /// <param name="ip">The ip address of the server hosting the datasources</param>
        /// <param param name="username">The username to authenicate with</param>
        /// <param name="password">The password to authenticate with</param>
        /// <returns></returns>
        Task<bool> ReInit(string ip, string username, string password);

        /// <summary>
        /// Allows instances to clean up any created resources.
        /// </summary>
        void Close();

        /// <summary>
        /// Refreshes the list of datasources if needed.
        /// </summary>
        Task<bool> Refresh();

        /// <summary>
        /// Attempts to get a <see cref="DataSource"/> by id.
        /// </summary>
        /// <param name="id">The id of the datasource to retreive</param>
        /// <param name="source">The matching datasource</param>
        /// <returns>True if a matching datasource was found, false otherwise</returns>
        bool TryGetDataSource(string id, out DataSource source);

        /// <summary>
        /// Returns a collection of all available <see cref="DataSource"/>s.
        /// </summary>
        /// <returns>Read only collection of the avilable datasources</returns>
        IReadOnlyCollection<DataSource> GetVideoDataSources();
    }
}
