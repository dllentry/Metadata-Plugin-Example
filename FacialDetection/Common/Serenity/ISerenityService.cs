//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using CPPCli;
using Pelco.Media.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FacialDetectionCommon.Serenity
{
    public interface ISerenityService : IDisposable
    {
        Task<bool> LoginAsync(string ip, string username, string password);

        Task<bool> LoginAsync(string ip, string authToken);

        Task<DataSource> GetDataSourceByIdAsync(string id);

        Task<List<DataSource>> GetDataSourcesByTypeAsync(DataSource.Types type);

        Task<List<Clip>> GetClipsAsync(string datasourceId, DateTime startSearch, DateTime endSearchTime);

        Task<List<Clip>> GetClipsAsync(DataSource ds, DateTime startSearch, DateTime endSearchTime);

        Task<List<DataSource>> GetAssociatedMetadataSourcesAsync(DataSource ds);

        Task<DataSource> GetAssociatedMetadataSourceByTypeAsync(DataSource vds, MimeType type);
    }
}
