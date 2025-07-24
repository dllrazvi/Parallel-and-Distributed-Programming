﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4.Utils
{
    internal class HttpUtils
    {

        public static readonly int HTTP_PORT = 80;



        public static Tuple<string, string> parseURL(string url)
        {
            var hostInfo = url.Split('/')[0];
            var link = string.Join("/", url.Split('/').Skip(1).ToArray());

            return new Tuple<string, string>(hostInfo, link);
        }

        public static string createGetRequestString(string url)
        {
            Tuple<string, string> splitedUrl = parseURL(url);
            return string.Format("GET /{0} HTTP/1.1\r\nHOST: {1}\r\n\r\n", splitedUrl.Item2, splitedUrl.Item1);

        }

        public static string getResponseBody(string responseContent)
        {
            var responseParts = responseContent.Split(new[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            return responseParts.Length > 1 ? responseParts[1] : "";
        }

        public static bool responseHeaderFullyObtained(string responseContent)
        {
            return responseContent.Contains("\r\n\r\n");
        }

        public static int getContentLength(string responseContent)
        {
            var responseLines = responseContent.Split('\r', '\n');

            foreach (var responseLine in responseLines)
            {
                var headerDetails = responseLine.Split(':');

                if (headerDetails[0].CompareTo("Content-Length") == 0)
                {
                    return int.Parse(headerDetails[1]);
                }
            }

            return 0;
        }
    }

}
