using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
#if SILVERLIGHT
using System.Windows.Browser;
#elif WPF
using System.Web;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    internal class WebRequest : DependencyObject
    {
        private WebClient _client;
        private HttpWebRequest _webRequest;

        internal WebRequest() { }

        #region Events

        public event EventHandler<RequestEventArgs> Completed;

        public event EventHandler<RequestEventArgs> Failed;

        #endregion Events

        #region Public Properties

        public bool ForcePost { get; set; }

        public bool IsBusy
        {
            get
            {
                if (this._client == null)
                {
                    return (this._webRequest != null);
                }
                return true;
            }
        }

        public Dictionary<string, string> Parameters { get; set; }

        public string ProxyUrl { get; set; }

        public string Url { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void BeginDownload(object state, ICredentials credentials = null)
        {
            if (this.IsBusy)
            {
                throw new NotSupportedException("Request does not support concurrent I/O operations");
            }
            RequestInfo info = this.BuildClient(credentials);
            if (info.Post)
            {
                this.UploadStringAsync(this.ProxyEncodeURL(this.Url), info.QueryString, state, false, credentials);
            }
            else
            {
                this._client = info.Client;
                this._client.OpenReadAsync(new Uri(info.FullUri), state);
            }
        }

        public void BeginRequest(ICredentials credentials = null)
        {
            this.BeginRequest(null, credentials);
        }

        public void BeginRequest(object state, ICredentials credentials = null)
        {
            if (this.IsBusy)
            {
                throw new NotSupportedException("Request does not support concurrent I/O operations");
            }
            RequestInfo info = this.BuildClient(credentials);
            this._client = info.Client;
            if (info.Post)
            {
                this._client.UploadStringAsync(this.ProxyEncodeURL(this.Url), "POST", info.QueryString, state);
            }
            else
            {
                this._client.DownloadStringAsync(new Uri(info.FullUri), state);
            }
        }

        public void CancelAsync()
        {
            if (this._client != null)
            {
                this._client.CancelAsync();
                this._client = null;
            }
        }

        #endregion Public Methods

        #region Internal Methods

        internal static string CreateUrl(string url, string querystring, string proxyUrl)
        {
            bool isProxyUrl = IsProxyUrl(url);

            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (!url.Contains<char>('?') || (isProxyUrl == true))
            {
                builder.Append('?');
            }
            else if (!url.EndsWith("&"))
            {
                builder.Append('&');
            }
            builder.Append(querystring);
            return ProxyEncodeUrlAsString(builder.ToString(), proxyUrl);
        }

        internal static string GetUrlEncodedQueryString(Dictionary<string, string> parameters)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in parameters.Keys)
            {
                builder.AppendFormat("{0}={1}&", new object[] { str, HttpUtility.UrlEncode(parameters[str]) });
            }
            // Remove the last "&"
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        internal static string GetContentType(string filename)
        {
            string str = null;
            if (!string.IsNullOrEmpty(filename))
            {
                int num = filename.LastIndexOf(".");
                if (num != -1)
                {
                    str = filename.Substring(num + 1);
                }
            }
            return MIME_MAP(str ?? "");
        }

        internal static string ProxyEncodeUrlAsString(string url, string proxyUrl)
        {
            if (string.IsNullOrEmpty(proxyUrl))
            {
                return url;
            }

#if SILVERLIGHT
            proxyUrl = PrefixProxyUrl(proxyUrl);
#endif
            if (proxyUrl.Contains("?"))
            {
                return string.Format("{0}{1}", proxyUrl, HttpUtility.UrlEncode(url));
            }

            return string.Format("{0}?{1}", proxyUrl, HttpUtility.UrlEncode(url));
        }

        internal static Uri ProxyEncodeUrlAsUri(string url, string proxyUrl)
        {
            return new Uri(ProxyEncodeUrlAsString(url, proxyUrl), UriKind.Absolute);
        }

        #endregion Internal Methods

        #region Event Handlers

        private void Client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (sender == this._client)
            {
                this._client = null;
                if (e.Error != null)
                {
                    this.OnFailed(new RequestEventArgs(e.UserState, e.Error));
                }
                else
                {
                    this.OnComplete(new RequestEventArgs(e.UserState, e.Result));
                }
            }
        }

        private void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (sender == this._client)
            {
                this._client = null;
                if (e.Error != null)
                {
                    this.OnFailed(new RequestEventArgs(e.UserState, e.Error));
                }
                else
                {
                    this.OnComplete(new RequestEventArgs(e.UserState, e.Result));
                }
            }
        }

        private void UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (sender == this._client)
            {
                this._client = null;
                if (e.Error != null)
                {
                    this.OnFailed(new RequestEventArgs(e.UserState, e.Error));
                }
                else
                {
                    this.OnComplete(new RequestEventArgs(e.UserState, e.Result));
                }
            }
        }

        #endregion Event Handlers

        #region Private Methods

        private static bool IsProxyUrl(string url)
        {
            if (string.IsNullOrEmpty(url) == true) return false;

            string[] urlParts = url.Split(@"?".ToCharArray());
            if (urlParts.Length != 2) return false;
            string service = urlParts[1];
            if (string.IsNullOrEmpty(service) == true) return false;

            return service.StartsWith("http");
        }

        private RequestInfo BuildClient(ICredentials credentials = null)
        {
            RequestInfo info = new RequestInfo();
            WebClient client = info.Client = new WebClient();
            if (credentials != null)
            {
                client.Credentials = credentials;
            }
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(this.UploadStringCompleted);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.DownloadStringCompleted);
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(this.Client_OpenReadCompleted);
            client.Encoding = Encoding.UTF8;
            Uri uri = new Uri(string.IsNullOrEmpty(this.ProxyUrl) ? this.Url : this.ProxyUrl, UriKind.RelativeOrAbsolute);
            info.QueryString = GetUrlEncodedQueryString(this.Parameters);
            info.FullUri = CreateUrl(this.Url, info.QueryString, this.ProxyUrl);
            if (this.ForcePost || (info.FullUri.Length > 0x800))
            {
                client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                info.Post = true;
                return info;
            }
#if SILVERLIGHT
            if (!uri.IsAbsoluteUri && string.IsNullOrEmpty(this.ProxyUrl))
            {
                info.FullUri = Application.Current.Host.Source.AbsoluteUri.Substring(0, Application.Current.Host.Source.AbsoluteUri.LastIndexOf('/')) + '/' + info.FullUri;
            }
#endif

            return info;
        }

        private static string MIME_MAP(string fileExt)
        {
            switch (fileExt.ToLower())
            {
                case "avi":
                    return "video/x-msvideo";

                case "csv":
                    return "text/csv";

                case "doc":
                    return "application/msword";

                case "docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                case "f4v":
                    return "video/mp4";

                case "flv":
                    return "video/x-flv";

                case "gif":
                    return "image/gif";

                case "htm":
                case "html":
                    return "text/html";

                case "jpeg":
                case "jpg":
                    return "image/jpeg";

                case "mov":
                    return "video/quicktime";

                case "mpeg":
                case "mpg":
                    return "video/mpeg";

                case "pdf":
                    return "application/pdf";

                case "png":
                    return "image/png";

                case "ppt":
                    return "application/powerpoint";

                case "pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";

                case "swf":
                    return "application/x-shockwave-flash";

                case "tif":
                case "tiff":
                    return "image/tiff";

                case "txt":
                    return "text/plain";

                case "xls":
                    return "application/vnd.ms-excel";

                case "xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                case "xml":
                    return "text/xml";

                case "z":
                    return "application/x-compress";

                case "zip":
                    return "application/zip";
            }
            return "application/octet-stream";
        }

        private void OnComplete(RequestEventArgs args)
        {
            if (this.Completed != null)
            {
                this.Completed(this, args);
            }
        }

        private void OnFailed(RequestEventArgs args)
        {
            if (this.Failed != null)
            {
                this.Failed(this, args);
            }
        }

#if SILVERLIGHT
        private static string PrefixProxyUrl(string ProxyUrl)
        {
            if (string.IsNullOrEmpty(ProxyUrl))
            {
                return null;
            }
            string str = ProxyUrl;
            if (ProxyUrl.StartsWith("~") || ProxyUrl.StartsWith("../"))
            {
                string absoluteUri = Application.Current.Host.Source.AbsoluteUri;
                int length = str.Split(new string[] { "../" }, StringSplitOptions.None).Length;
                for (int i = 0; i < length; i++)
                {
                    absoluteUri = absoluteUri.Substring(0, absoluteUri.LastIndexOf("/"));
                }
                if (!absoluteUri.EndsWith("/"))
                {
                    absoluteUri = absoluteUri + "/";
                }
                return (absoluteUri + str.Replace("~", "").Replace("../", ""));
            }
            if (ProxyUrl.StartsWith("/"))
            {
                str = ProxyUrl.Replace("/", string.Format("{0}://{1}:{2}", Application.Current.Host.Source.Scheme, Application.Current.Host.Source.Host, Application.Current.Host.Source.Port));
            }
            return str;
        }
#endif

        private void ProcessResult(object state)
        {
            object[] objArray = (object[])state;
            HttpWebResponse response = (HttpWebResponse)objArray[0];
            state = objArray[1];
            bool flag = (bool)objArray[2];
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                if (flag)
                {
                    string result = new StreamReader(responseStream).ReadToEnd();
                    this.OnComplete(new RequestEventArgs(state, result));
                }
                else
                {
                    this.OnComplete(new RequestEventArgs(state, responseStream));
                }
            }
            else
            {
                this.OnFailed(new RequestEventArgs(state, new Exception(response.StatusDescription)));
            }
        }

        private Uri ProxyEncodeURL(string url)
        {
            return ProxyEncodeUrlAsUri(url, this.ProxyUrl);
        }

        private void UploadStringAsync(Uri url, string data, object state, bool getResultAtString, ICredentials credentials)
        {
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
            if (credentials != null)
            {
                request.Credentials = credentials;
            }
            request.Method = "POST";
            this._client = null;
            this._webRequest = request;
            request.ContentType = "application/x-www-form-urlencoded";
            SynchronizationContext sc = SynchronizationContext.Current;
            request.BeginGetRequestStream(delegate(IAsyncResult asynchronousResult)
            {
                HttpWebRequest request1 = (HttpWebRequest)asynchronousResult.AsyncState;
                if ((this._webRequest != null) && (this._webRequest == request1))
                {
                    Stream stream = null;
                    try
                    {
                        stream = request1.EndGetRequestStream(asynchronousResult);
                    }
                    catch (Exception exception)
                    {
                        this._webRequest = null;
                        this.OnFailed(new RequestEventArgs(state, exception));
                        return;
                    }
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(data);
                    writer.WriteLine();
                    writer.Flush();
                    stream.Close();
                    request1.BeginGetResponse(delegate(IAsyncResult asynchronousResult2)
                    {
                        HttpWebRequest asyncState = (HttpWebRequest)asynchronousResult2.AsyncState;
                        if ((this._webRequest != null) && (this._webRequest == asyncState))
                        {
                            this._webRequest = null;
                            HttpWebResponse response = (HttpWebResponse)asyncState.EndGetResponse(asynchronousResult2);
                            sc.Post(new SendOrPostCallback(this.ProcessResult), new object[] { response, state, getResultAtString });
                        }
                    }, request1);
                }
            }, request);
        }

        #endregion Private Methods

        internal sealed class RequestEventArgs : AsyncCompletedEventArgs
        {

            // Methods
            internal RequestEventArgs(object state, Exception exception)
                : base(exception, false, state)
            {
            }

            internal RequestEventArgs(object state, Stream result)
                : base(null, false, state)
            {
                this.ResultStream = result;
            }

            internal RequestEventArgs(object state, string result)
                : base(null, false, state)
            {
                this.Result = result;
            }

            // Properties
            public string Result { get; private set; }

            public Stream ResultStream { get; private set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RequestInfo
        {
            public bool Post;
            public string FullUri;
            public string QueryString;
            public WebClient Client;
        }

        private class ServiceException : Exception
        {
            // Methods
            internal ServiceException(int code, string description)
                : base(description)
            {
                this.Code = code;
            }

            // Properties
            public int Code { get; private set; }
        }
    }
}