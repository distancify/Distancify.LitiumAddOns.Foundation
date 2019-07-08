using System;
using System.IO;
using System.Net;
using System.Text;

namespace Distancify.LitiumAddOns.Foundation.Http
{
    public class TextRequest : HttpRequest<string>
    {
        private readonly string _url;
        private readonly string _queryString;
        private readonly Encoding _encoding;

        protected override Uri Uri { get { return new Uri(String.IsNullOrEmpty(_queryString) ? _url : $"{_url}?{_queryString}"); } }

        public TextRequest(string url) : this(url, null, Encoding.UTF8) { }

        public TextRequest(string url, string queryString, Encoding encoding)
        {
            _url = url;
            _queryString = queryString;
            _encoding = encoding;
        }

        public override string ProcessResponse(HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            using (var streamReader = new StreamReader(stream, _encoding))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}