using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Velib.Common
{
   public static class SerializationExtensions
    {
        public static string ToJson<T>(this T obj) where T : class, new()
        {
            var ser = new DataContractJsonSerializer(obj.GetType());
            var result = String.Empty;

            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);

                var array = ms.ToArray();
                result = Encoding.UTF8.GetString(array, 0, array.Length);
            }
            return (result);
        }
        public static T FromXmlString<T>(this string xmlString, string defaultNamespaceName)
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
                return (mem.FromXmlStream<T>(defaultNamespaceName));
        }

        public static T FromXmlStream<T>(this Stream xmlStream, string defaultNamespaceName)
        {
            var c = new XmlSerializer(typeof(T), new XmlRootAttribute(defaultNamespaceName));
            var parsedObject = c.Deserialize(xmlStream);
            return ((T)parsedObject);
        }

        public static T FromJsonString<T>(this string jsonString)
        {
            return jsonString.FromJsonString<T>(typeof(T));
        }
        public static T FromJsonString<T>(this string jsonString, Type type)
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                return (mem.FromJsonStream<T>(type));
        }

        public static T FromJsonStream<T>(this Stream jsonStream, Type type)
        {
            var c = new DataContractJsonSerializer(type);
            var parsedObject = c.ReadObject(jsonStream);
            return ((T)parsedObject);
        }

    }
}
