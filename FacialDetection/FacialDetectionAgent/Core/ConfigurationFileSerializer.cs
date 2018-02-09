//
// Copyright (c) 2018 Pelco. All rights reserved.
//
// This file contains trade secrets of Pelco.  No part may be reproduced or
// transmitted in any form by any means or for any purpose without the express
// written permission of Pelco.
//
using NLog;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace FacialDetectionAgent.Core
{
    public class ConfigurationFileSerializer
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private string _path;

        public ConfigurationFileSerializer()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            _path = Path.Combine(dir.FullName, "Configuration.xml");
        }

        public Configuration Read()
        {
            try
            {
                if (File.Exists(_path))
                {
                    using (var fs = new FileStream(_path, FileMode.Open))
                    using (var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(Configuration));
                        return (Configuration)serializer.ReadObject(reader);
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Error(e, $"Failed to read configuration file '{_path}'");
            }

            return new Configuration();
        }

        public bool Save(Configuration config)
        {
            try
            {
                // Create the parent directory if it does not exist.
                if (CreateDirectory(Directory.GetParent(_path).FullName))
                {
                    using (XmlTextWriter writer = new XmlTextWriter(_path, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(Configuration));
                        dataContractSerializer.WriteObject(writer, config);
                    }

                    return true;
                }

            }
            catch (Exception e)
            {
                LOG.Error(e, $"Failed to save configuration to '{_path}'");
            }

            return false;
        }

        private bool CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return true;
            }
            catch (Exception e)
            {
                LOG.Error(e, $"Failed to create directory '{path}' while saving configuration");
                return false;
            }
        }
    }
}
