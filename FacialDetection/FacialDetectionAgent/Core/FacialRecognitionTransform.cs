//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using Pelco.Media.Pipeline;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace FacialDetectionAgent.Core
{
    class FacialRecognitionTransform : ObjectTypeToBufferTransformBase<ImageFrame>
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private CascadeClassifier _classifier;

        public FacialRecognitionTransform()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

            _classifier = new CascadeClassifier(Path.Combine(dir.FullName, "haarcascade_frontalface_default.xml"));

            ScaleFactor = 1.2;
            MinimumNeighbors = 5;
        }

        #region Properties

        public double ScaleFactor { get; set; }

        public int MinimumNeighbors { get; set; }

        public Size MinimumSize { get; set; }

        #endregion

        public override bool HandleObject(ImageFrame frame)
        {
            using (var grayImage = frame.Image.Convert<Gray, byte>())
            {
                var faces = _classifier.DetectMultiScale(grayImage,
                                                         ScaleFactor,
                                                         MinimumNeighbors,
                                                         MinimumSize,
                                                         new Size(frame.Image.Width, frame.Image.Height));

                if (faces.Length > 0)
                {
                    // We have detected some faces lets pass them along.
                    return PushBuffer(ToXmlBuffer(frame.TimeStamp, faces, frame.Image.Width, frame.Image.Height));
                }
            }

            return true;
        }

        public override bool WriteBuffer(ByteBuffer buffer)
        {
            throw new NotImplementedException();
        }

        private ByteBuffer ToXmlBuffer(DateTime ts, Rectangle[] rects, int width, int height)
        {
              var xml = new XElement("FaceDiscovery",
                        new XElement("Faces",
                            rects.Select(r => new XElement("Face", new XAttribute("x-upper-left", (float)(r.X / (float)width)),
                                                                   new XAttribute("y-upper-left", (float)(r.Y / (float)height)),
                                                                   new XAttribute("x-bottom-right", (float)((r.X + r.Width) / (float)width)),
                                                                   new XAttribute("y-bottom-right", (float)((r.Y + r.Height) / (float)height))))
                        )
                      );

            var buffer = new ByteBuffer(Encoding.UTF8.GetBytes(xml.ToString(SaveOptions.DisableFormatting)));
            buffer.TimeReference = ts;

            return buffer;
        }
    }
}
