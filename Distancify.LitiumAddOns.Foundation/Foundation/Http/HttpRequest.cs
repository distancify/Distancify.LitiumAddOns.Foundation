using System;
using System.IO;
using System.Net;
using Distancify.SerilogExtensions;

namespace Distancify.LitiumAddOns.Foundation.Http
{
    public abstract class HttpRequest<T>
    {
        protected abstract Uri Uri { get; }
        protected virtual string Method { get { return null; }}
        protected virtual string ContentType { get { return null; }}
        protected virtual string Body { get { return null; } }
        protected virtual HttpStatusCode? ExpectedStatusCode { get { return null; } }
        private const int TwentyMinutes = 1200000;

        public T GetResult(bool ignoreCertificateErrors = false)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(Uri);
                if (!String.IsNullOrEmpty(Method)) request.Method = Method;
                if (!String.IsNullOrEmpty(ContentType)) request.ContentType = ContentType;
                if (!String.IsNullOrEmpty(Body)) SetRequestBody(request, Body);
                request.Timeout = TwentyMinutes;
                if(ignoreCertificateErrors)
                {
                    request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (ExpectedStatusCode != null && ExpectedStatusCode != response.StatusCode) throw new Exception(String.Format("Expected HTTP status code {0} from {1}, but was {2}.", ExpectedStatusCode, Uri, response.StatusCode));

                    return ProcessResponse(response);
                }
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "Failed to process request to {Uri}.", Uri);
                throw ex;
            }
        }

        public abstract T ProcessResponse(HttpWebResponse response);

        private static void SetRequestBody(HttpWebRequest request, string body)
        {
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(body);
            }
        }
    }
}