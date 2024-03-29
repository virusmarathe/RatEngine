﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace RatEditor.Utils
{
    public static class Serializer
    {
        public static void ToFile<T>(T instance, string path)
        {
            try
            {
//                using var fs = new FileStream(path, FileMode.Create);
                var serializer = new DataContractSerializer(typeof(T));
                var settings = new XmlWriterSettings()
                {
                    Indent = true
                };
                using (var writer = XmlWriter.Create(path, settings))
                {
                    serializer.WriteObject(writer, instance);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to serialize {instance} to {path}");
                throw;
            }
        }

        public static T FromFile<T>(string path)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Open);
                var serializer = new DataContractSerializer(typeof(T));
                T instance = (T)serializer.ReadObject(fs);
                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to deserialize {path}");
                throw;
            }
        }
    }
}
