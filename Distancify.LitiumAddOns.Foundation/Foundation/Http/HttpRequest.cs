using System;
using System.IO;
using System.Net;
using Distancify.SerilogExtensions;

namespace Distancify.LitiumAddOns.Foundation.Foundation.Http
{
    public abstract class HttpRequest<T>
    {
        protected abstract Uri Uri { get; }
        protected virtual string Method => null;
        protected virtual string ContentType => null;
        protected virtual string Body => null;
        protected virtual HttpStatusCode? ExpectedStatusCode => null;
        private const int TwentyMinutes = 1200000;

        public T GetResult(bool ignoreCertificateErrors = false)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(Uri);
                if (!string.IsNullOrEmpty(Method)) request.Method = Method;
                if (!string.IsNullOrEmpty(ContentType)) request.ContentType = ContentType;
                if (!string.IsNullOrEmpty(Body)) SetRequestBody(request, Body);
                request.Timeout = TwentyMinutes;
                if(ignoreCertificateErrors)
                {
                    request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (ExpectedStatusCode != null && ExpectedStatusCode != response.StatusCode) throw new Exception(
                        $"Expected HTTP status code {ExpectedStatusCode} from {Uri}, but was {response.StatusCode}.");

                    return ProcessResponse(response);
                }
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "Failed to process request to {Uri}.", Uri);
                throw;
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