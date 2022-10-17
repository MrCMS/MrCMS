﻿using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MrCMS.Helpers
{
    public static class HttpRequestExtensions
    {
        public static bool IsLocal(this HttpRequest req)
        {
            if (req == null)
                return false;

            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                return connection.LocalIpAddress != null
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            // for in memory TestServer or when dealing with default connection info
            return connection.RemoteIpAddress == null && connection.LocalIpAddress == null;
        }

        public static string Referer(this HttpRequest req)
        {
            return req?.GetTypedHeaders()?.Referer?.ToString();
        }

        public static string RefererLocal(this HttpRequest req)
        {
            return req?.GetTypedHeaders()?.Referer?.LocalPath;
        }

        //public static string GetCurrentIP(this HttpContext contextBase)
        //{
        //    return contextBase.Request.GetCurrentIP();
        //}

        public static string UserAgent(this HttpRequest request)
        {
            return request.Headers["User-Agent"].ToString();
        }

        public static string GetCurrentIP(this HttpRequest request)
        {
            if (request == null)
                return string.Empty;

            var connectingIp = request.Headers["CF-CONNECTING-IP"];
            if (!string.IsNullOrWhiteSpace(connectingIp))
                return connectingIp;

            string ipAddress = request.Headers["X-Forwarded-For"];

            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    if (addresses[0].Contains(":"))
                        return addresses[0].Split(':')[0];
                    return addresses[0];
                }
            }

            return request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public static string GetRawHttpData(this HttpRequest request)
        {
            try
            {
                var serverVariables = request.Headers;
                var dictionary = serverVariables.Keys.ToDictionary(s => s, s => serverVariables[s].ToString());
                return JsonConvert.SerializeObject(dictionary);
            }
            catch
            {
                return "Could not get data";
            }
        }

        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// 
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }
    }
}