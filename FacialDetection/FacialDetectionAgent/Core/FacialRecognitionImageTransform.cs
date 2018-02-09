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
using System.Reflection;

namespace FacialDetectionAgent.Core
{
    public class FacialRecognitionImageTransform : ObjectTypeTransformBase<ImageFrame, Image<Bgr, byte>>
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private CascadeClassifier _classifier;

        public FacialRecognitionImageTransform()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

            _classifier = new CascadeClassifier(Path.Combine(dir.FullName, "haarcascade_frontalface_default.xml"));

            ScaleFactor = 1.2;
            MinimumNeighbors = 5;
        }

        public double ScaleFactor { get; set; }

        public int MinimumNeighbors { get; set; }

        public Size MinimumSize { get; set; }

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
                    foreach (var face in faces)
                    {
                        frame.Image.Draw(face, new Bgr(0, 0, 255.0), 2);
                    }
                }

                PushObject(frame.Image);
            }

            return true;
        }

        public override bool WriteBuffer(ByteBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
