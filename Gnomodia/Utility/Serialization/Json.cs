using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Gnomodia.Utility.Serialization
{
    public static class Json
    {
        public static void ToJson<T>(T value, Stream targetStream, params Type[] knownTypes)
        {
            if (targetStream == null)
            {
                throw new ArgumentNullException("targetStream");
            }
            var jsonSerializer = new DataContractJsonSerializer(typeof(T), knownTypes);
            jsonSerializer.WriteObject(targetStream, value);
        }

        public static string ToJson<T>(T objectToSerialize, params Type[] knownTypes)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ToJson(objectToSerialize, stream, knownTypes);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static T FromJson<T>(Stream sourceStream, params Type[] knownTypes)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(T), knownTypes);
            return (T)jsonSerializer.ReadObject(sourceStream);
        }

        public static T FromJson<T>(string jsonText, params Type[] knownTypes)
        {
            return FromJson<T>(new MemoryStream(Encoding.UTF8.GetBytes(jsonText)), knownTypes);
        }
    }
}