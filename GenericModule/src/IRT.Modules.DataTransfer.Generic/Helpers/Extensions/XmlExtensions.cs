using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using IRT.Modules.DataTransfer.Generic.Helpers.Infrastucture;

namespace IRT.Modules.DataTransfer.Generic.Helpers.Extensions
{
    public static class XmlExtensions
    {
        public static string FormatToXml(this string xml)
        {
            try
            {
                // The XML declaration must be the first node in the document,
                // and no whitespace characters are allowed to appear before it.
                xml = xml?.Trim();

                var doc = XDocument.Parse(xml);
                var builder = new StringBuilder();

                using (TextWriter writer = new EncodingStringWriter(builder, Encoding.UTF8))
                {
                    doc.Save(writer);
                }

                return builder.ToString();
            }
            catch (Exception)
            {
                return xml;
            }
        }

        public static bool CanFormatToXml(this string xml)
        {
            try
            {
                // The XML declaration must be the first node in the document,
                // and no whitespace characters are allowed to appear before it.
                var newxml = xml?.Trim();

                XDocument.Parse(newxml);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static T Deserialize<T>(this string input) where T : class
        {
            var ser = new XmlSerializer(typeof(T));

            using (var sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static string Serialize<T>(this T objectToSerialize)
        {
            var xmlSerializer = new XmlSerializer(objectToSerialize.GetType());

            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, objectToSerialize);
                return textWriter.ToString();
            }
        }
    }
}
