using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Distancify.LitiumAddOns.Foundation.Foundation.XML
{
    public abstract class XMLConvertible
    {
        private readonly Encoding _encoding;

        public XMLConvertible()
        {
            _encoding = Encoding.UTF8;
        }

        public XMLConvertible(Encoding encoding)
        {
            _encoding = encoding;
        }

        public string ToXML()
        {
            var serializer = new XmlSerializer(GetType());
            var namespaces = GetNamespaces();
            var settings = GetSettings();

            string xml = null;

            using (var stringWriter = _encoding == null ? new StringWriter() : new StringWriterWithSetEncoding(_encoding))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(stringWriter, this, namespaces);
                    xml = stringWriter.ToString();
                }
            }

            return xml;
        }

        protected virtual XmlSerializerNamespaces GetNamespaces()
        {
            return null;
        }

        protected virtual XmlWriterSettings GetSettings()
        {
            return null;
        }

        public static T XMLToObject<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            T result;

            using (TextReader reader = new StringReader(xml))
            {
                result = (T)serializer.Deserialize(reader);
            }

            return result;
        }

        private sealed class StringWriterWithSetEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithSetEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding => encoding;
        }
    }
}