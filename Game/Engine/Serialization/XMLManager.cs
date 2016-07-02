using OurGame.Engine.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OurGame.Engine.Serialization
{
    public class XMLManager<T>
    {
        public XMLManager()
        {
            Type = typeof(T);
        }
        public Type Type { set; get; }
        public T LoadFromFile(Stream reader)
        {
            T result;
            XmlSerializer serializer = new XmlSerializer(Type);
            result = (T)serializer.Deserialize(reader);
            return result;
        }

        public T LoadFromFile(string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(Type);
                return (T)serializer.Deserialize(stream);
            }
        }


        public void SaveToFile(Stream stream, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(Type);
            serializer.Serialize(stream, obj);
        }
        public void SaveToFile(string path, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(Type);
            using(Stream stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, obj);
            }
        }
    }
}
