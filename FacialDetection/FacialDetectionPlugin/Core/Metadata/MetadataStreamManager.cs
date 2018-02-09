//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using CPPCli;
using FacialDetection.Events;
using FacialDetectionCommon.Serenity;
using NLog;
using Pelco.Media.Metadata;
using Pelco.Media.Metadata.Api;
using Pelco.UI.VideoOverlay;
using Prism.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FacialDetection.Core.Metadata
{
    public class MetadataStreamManager : MetadataStreamManagerBase
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private bool _disposed;
        private ISerenityService _serenity;
        private IEventAggregator _eventAgg;
        private PipelineFactory _pipelineFactory;

        public MetadataStreamManager(ISerenityService serenity, IEventAggregator eventAgg)
        {
            _disposed = false;
            _eventAgg = eventAgg;
            _serenity = serenity;
            _pipelineFactory = new PipelineFactory();

            _eventAgg.GetEvent<CanvasSinkCreatedEvent>().Subscribe(OnCanvasCreated);
        }

        ~MetadataStreamManager()
        {
            Dispose(false);
        }

        private void OnCanvasCreated(IVideoOverlayCanvas<FacialMetadata> sink)
        {
            // Now that the sink is available lets attach it to the
            // metadata pipeline so that we can draw rectangles for
            // detected faces.
            _pipelineFactory.CanvasSink = sink;
        }

        protected override Task Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _serenity?.Dispose();
                _disposed = true;

                _eventAgg.GetEvent<CanvasSinkCreatedEvent>().Unsubscribe(OnCanvasCreated);
            }

            return base.Dispose(disposing);
        }
        
        public async Task<bool> TryRegisterStream(string datasourceId)
        {
            try
            {
                var vds = await _serenity.GetDataSourceByIdAsync(datasourceId);
                if (vds == null)
                {
                    LOG.Error($"Could not retrieve datasource '{datasourceId}' from VideoXpert, metadata streams will not be started");
                    return false;
                }

                var stream = await DiscoverMetadataStreamAsync(vds);
                if (stream != null)
                {
                    await base.RegisterStream(stream);
                    return true;
                }
                else
                {
                    LOG.Debug($"No supported metadata assocations found for datasource '{datasourceId}'");
                }

            }
            catch (Exception e)
            {
                LOG.Error($"Failed to start metadata streams for datasource '{datasourceId}', reason: {e.Message}");
            }

            return false;
        }

        private async Task<IMetadataStream> DiscoverMetadataStreamAsync(DataSource vds)
        {
            var mds = await _serenity.GetAssociatedMetadataSourceByTypeAsync(vds, Constants.MIME_TYPE);

            var dInterface = mds?.DataInterfaces
                                 .Where(di => !di.SupportsMulticast && di.Protocol == DataInterface.StreamProtocols.RtspRtp)
                                 .FirstOrDefault();

            return dInterface != null ? new MetadataStream(_pipelineFactory, new Uri(dInterface.DataEndpoint)) : null;
        }
    }
}
