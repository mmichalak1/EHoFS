using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace OurGame.Engine.Serialization
{
    public static class ObjectCloner
    {
        public static T Clone<T>(T source)
        {
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            DataContractSerializer ser = new DataContractSerializer(typeof(T));
            Stream stream = new MemoryStream();
            using (stream)
            {
                ser.WriteObject(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)ser.ReadObject(stream);
            }
        }
    }
}

