/*
 * MIT License
 *
 * Copyright(c) 2019 kelicto
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/*
             ,---------------------------------------------------,              ,---------,
        ,----------------------------------------------------------,          ,"        ,"|
      ,"                                                         ,"|        ,"        ,"  |
     +----------------------------------------------------------+  |      ,"        ,"    |
     |  .----------------------------------------------------.  |  |     +---------+      |
     |  | C:\>FILE -INFO                                     |  |  |     | -==----'|      |
     |  |                                                    |  |  |     |         |      |
     |  |                                                    |  |  |/----|`---=    |      |
     |  |              Author: kelicto                       |  |  |     |         |      |
     |  |              Email: kelistudy@163.com              |  |  |     |         |      |
     |  |              Creation Time: 10/30/2019 07:08:41 PM |  |  |     |         |      |
     |  | C:\>_                                              |  |  |     | -==----'|      |
     |  |                                                    |  |  |   ,/|==== ooo |      ;
     |  |                                                    |  |  |  // |(((( [66]|    ,"
     |  `----------------------------------------------------'  |," .;'| |((((     |  ,"
     +----------------------------------------------------------+  ;;  | |         |,"
        /_)_________________________________________________(_/  //'   | +---------+
           ___________________________/___  `,
          /  oooooooooooooooo  .o.  oooo /,   \,"-----------
         / ==ooooooooooooooo==.o.  ooo= //   ,`\--{)B     ,"
        /_==__==========__==_ooo__ooo=_/'   /___________,"
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;

namespace KeLi.Common.Tool.Web
{
    /// <summary>
    /// Http Assist.
    /// </summary>
    public static class HttpAssist
    {
        /// <summary>
        /// The cookies.
        /// </summary>
        public static CookieCollection Cookies { get; } = new CookieCollection();

        /// <summary>
        /// Gets all type request string data.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string GetTypeRequest(this ResponseParam param, string url, string filePath = null, string postData = null)
        {
            var response = param.CreateHttpResponse(url, filePath, postData);

            using (var reader = new StreamReader(response.GetResponseStream(), param.EncodeType))
                return reader.ReadToEnd();
        }

        /// <summary>
        /// Gets all type request model data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static T GetTypeRequest<T>(this ResponseParam param, string url, string filePath = null, string postData = null)
        {
            var response = GetTypeRequest(param, url, filePath, postData);

            return JsonConvert.DeserializeObject<T>(response);
        }

        /// <summary>
        /// Gets all type request data.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string GetTypeRequest(this ResponseParam param, string url, string filePath = null, IDictionary<string, string> postData = null)
        {
            var postDatal = CreateParameter(postData);
            var response = param.CreateHttpResponse(url, filePath, postDatal);

            using (var reader = new StreamReader(response.GetResponseStream(), param.EncodeType))
                return reader.ReadToEnd();
        }

        /// <summary>
        /// Gets all type request model data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static T GetTypeRequest<T>(this ResponseParam param, string url, string filePath = null, IDictionary<string, string> postData = null)
        {
            var response = GetTypeRequest(param, url, filePath, postData);

            return JsonConvert.DeserializeObject<T>(response);
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] DownloadFile(this ResponseParam param, string url, string filePath = null)
        {
            var response = param.CreateHttpResponse(url, filePath);
            var st = response.GetResponseStream();
            var results = new byte[response.ContentLength];

            st.Read(results, 0, results.Length);
            st.Close();

            return results;
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string UploadFile(this ResponseParam param, string url, string filePath)
        {
            var response = param.CreateHttpResponse(url, filePath);

            using (var reader = new StreamReader(response.GetResponseStream(), param.EncodeType))
                return reader.ReadToEnd();
        }

        /// <summary>
        /// Sets the request's stream.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="request"></param>
        private static void SetFileStream(string filePath, HttpWebRequest request)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var boundary = $"----------{DateTime.Now.Ticks:x}";
                var boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                var sb = new StringBuilder();

                sb.Append("--");
                sb.Append(boundary);
                sb.AppendLine();
                sb.Append($"Content-Disposition: \"form-data; name=file; filename={fs.Name}\"");
                sb.AppendLine();
                sb.Append("Content-Type: ");
                sb.Append("application/octet-stream");
                sb.AppendLine();
                sb.AppendLine();

                var postHeader = sb.ToString();
                var postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);
                var fileContent = new byte[fs.Length];

                request.ContentType = $"multipart/form-data; boundary={boundary}";
                request.ContentLength = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
                fs.Read(fileContent, 0, fileContent.Length);

                using (var stream = request.GetRequestStream())
                {
                    // Sends the file header.
                    stream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                    // Sends the file content.
                    stream.Write(fileContent, 0, fileContent.Length);

                    // Send the file time tick.
                    stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                }
            }
        }

        /// <summary>
        /// Creates http request data.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private static HttpWebResponse CreateHttpResponse(this ResponseParam param, string url, string filePath = null, string postData = null)
        {
            HttpWebRequest request;

            if (url.StartsWith(@"https:\\", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = CheckResultValidation;
                request = WebRequest.Create(url) as HttpWebRequest;

                if (request != null)
                    request.ProtocolVersion = HttpVersion.Version10;
            }
            else
                request = WebRequest.Create(url) as HttpWebRequest;

            if (param.Proxy != null && request != null)
                request.Proxy = param.Proxy;

            switch (param.Type)
            {
                case RequestType.Post:
                    request.Method = "POST";
                    break;

                case RequestType.Delete:
                    request.Method = "DELETE";
                    break;

                case RequestType.Put:
                    request.Method = "PUT";
                    break;

                case RequestType.Patch:
                    request.Method = "PATCH";
                    break;

                case RequestType.Get:
                    request.Method = "GET";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            request.Accept = "text/html, application/xhtml+xml, application/json, text/javascript, */*; q=0.01";
            request.Headers["Accept-Language"] = "en-US,en;q=0.5";
            request.Headers["Pragma"] = "no-cache";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("x-authentication-token", param.Token);
            request.Headers.Add("X-CORAL-TENANT", param.TenantId);
            request.Referer = param.Referer;
            request.UserAgent = param.UserAgent;
            request.ContentType = param.ContentType;

            if (param.Cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(param.Cookies);
            }
            else
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(Cookies);
            }

            if (param.Headers != null)
                foreach (var header in param.Headers)
                    request.Headers.Add(header.Key, header.Value);

            if (param.Timeout.HasValue)
                request.Timeout = param.Timeout.Value * 1000;

            request.Expect = string.Empty;

            if (!string.IsNullOrEmpty(filePath))
                SetFileStream(filePath, request);

            if (!string.IsNullOrEmpty(postData))
            {
                var data = param.EncodeType.GetBytes(postData);

                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);
            }

            var result = request.GetResponse() as HttpWebResponse;

            Cookies.Add(request.CookieContainer.GetCookies(new Uri(@"http:\\" + new Uri(url).Host)));
            Cookies.Add(request.CookieContainer.GetCookies(new Uri(@"https:\\" + new Uri(url).Host)));
            Cookies.Add(result.Cookies);

            return result;
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string CreateParameter(IDictionary<string, string> parameters)
        {
            var buffer = new StringBuilder();

            foreach (var key in parameters.Keys)
                buffer.AppendFormat("&{0}={1}", key, parameters[key]);

            return buffer.ToString().TrimStart('&');
        }

        /// <summary>
        /// Checks the result vaildation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckResultValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}