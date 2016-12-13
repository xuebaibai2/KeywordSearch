using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace KeywordSearch.Utility
{
    public class Serializer
    {
        public string GetSerializedXml<T>(T model)
        {
            var obj = (T)model;
            var xml = "";
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (var sw = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sw))
                {
                    xs.Serialize(writer, obj);
                    xml = sw.ToString();
                }
            }
            return xml;
        }

        public T GetDeserializedObj<T>(string xml)
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (var sr = new StringReader(xml))
            {
                using (XmlReader reader = XmlReader.Create(sr))
                {
                    obj = (T)Convert.ChangeType(xs.Deserialize(reader), typeof(T));
                }
            }
            return obj;
        }
    }
}