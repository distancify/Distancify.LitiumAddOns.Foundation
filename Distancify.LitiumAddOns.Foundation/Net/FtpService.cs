using Distancify.SerilogExtensions;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;

namespace Distancify.LitiumAddOns.Foundation.Net
{
    public class FtpService : IFtpService
    {
        const int BufferSize = 16384;
        
        public bool Put(string host, int port, string user, string password, string fileNameAndPath, Stream content, FtpConnectionType connectionType, bool ignoreSslErrors)
        {
            switch(connectionType)
            {
                case FtpConnectionType.FTP:
                case FtpConnectionType.FTPS:
                    var secureConnection = connectionType == FtpConnectionType.FTPS;
                    return Put(host, port, user, password, fileNameAndPath, content, secureConnection, ignoreSslErrors);

                case FtpConnectionType.SFTP:
                    return PutSFTP(host, port, user, password, fileNameAndPath, content, ignoreSslErrors);
            }

            throw new NotImplementedException("FtpConnectionType not supported");
        }

        private bool Put(string host, int port, string user, string password, string fileNameAndPath, Stream content, bool secureConnection, bool ignoreSslErrors)
        {
            var ftpUrl = new Uri(new Uri($"ftp://{user}:{password}@{host}:{port}"), fileNameAndPath);

            var request = (FtpWebRequest)WebRequest.Create(ftpUrl);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.EnableSsl = secureConnection;

            var originalServerCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback;
            try
            {
                if (ignoreSslErrors)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, cert, chain, sslPolicyErrors) => true);
                }

                using (var uploadStream = request.GetRequestStream())
                {
                    this.Log().Debug("Uploading file {FileName} to FTP {FtpHost} started.",
                        fileNameAndPath, host);

                    var buffer = new byte[BufferSize];
                    int bytesRead;

                    while ((bytesRead = content.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        uploadStream.Write(buffer, 0, bytesRead);
                    }
                }

                var response = (FtpWebResponse)request.GetResponse();

                this.Log().Debug("Uploading file {FileName} to FTP {FtpHost} completed with status code {Status}.",
                    fileNameAndPath, host, response.StatusCode);

                return response.StatusCode == FtpStatusCode.ClosingData;
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "Error uploading file {FilePath} to FTP", fileNameAndPath);
            }
            finally
            {
                if (ignoreSslErrors)
                {
                    ServicePointManager.ServerCertificateValidationCallback = originalServerCertificateValidationCallback;
                }
            }

            return false;
        }

        private bool PutSFTP(string host, int port, string user, string password, string fileNameAndPath, Stream content, bool ignoreSslErrors)
        {
            try
            {
                using (var client = new SftpClient(host, port, user, password))
                {
                    this.Log().Debug("Uploading file {FileName} to FTP {FtpHost} started.", fileNameAndPath, host);

                    client.Connect();

                    client.BufferSize = 4 * 1024; // Levi 14.05.2018 - Certain servers do not support a large buffer
                    client.UploadFile(content, fileNameAndPath, true);

                    this.Log().Debug("Uploading file {FileName} to FTP {FtpHost} completed.", fileNameAndPath, host);

                    client.Disconnect();
                }

                return true;
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "Error uploading file {FilePath} to FTP", fileNameAndPath);
            }

            return false;
        }
    }

}
