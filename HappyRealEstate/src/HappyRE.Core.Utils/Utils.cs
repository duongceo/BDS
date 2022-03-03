using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using log4net;

namespace HappyRE.Core.Utils
{
    public static class StringUtils
    {
        private static readonly ILog _log = LogManager.GetLogger("StringUtils");

        public static string RemoveComma(string text)
        {
            if (text == null) return "";
            text = text.Replace(", ,", ",").Replace(": ,", ": ");
            var c = text.IndexOf(",");
            if (c == 0) text = text.Substring(1, text.Length - 1);
            return text;
        }
        public static string GenerateCode()
        {
            return DateTime.Now.GetHashCode().ToString("x");
        }
        public static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static Stream StringToStream(string src)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(src);
            return new MemoryStream(byteArray);
        }

        public static string ToFriendlyUrl(string value)
        {
            return Regex.Replace(value, @"[^A-Za-z0-9_\.~]+", "-");
        }

        public static string GetRequestIP()
        {
            HttpRequest request = HttpContext.Current.Request;
            string ip = "";
            if (request.Url.AbsoluteUri.Contains("//HappyRE.com") || request.Url.AbsoluteUri.Contains(".HappyRE.com"))
            {
                if (string.IsNullOrWhiteSpace(ip))
                {
                    ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }
                if (string.IsNullOrWhiteSpace(ip))
                {
                    ip = request.ServerVariables["X-Forwarded-For"];
                }
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = request.ServerVariables["REMOTE_ADDR"];
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                return request.UserHostAddress;
            }
            string s = "";
            if (ip.Contains(",") == true)
            {
                //var list_ip = ip.Split(new char[1] { ',' });
                //s = list_ip.LastOrDefault();

                var xx = request.Headers.GetValues("Referer");
                var referer = xx == null ? "" : string.Join(", ", xx);
                var list_ip = ip.Split(new char[1] { ',' });
                s = list_ip.LastOrDefault();
            }
            else
            {
                s = ip;
            }
            return s;
        }

        //public static string GetRequestIP()
        //{
        //    HttpRequest request = HttpContext.Current.Request;

        //    string ip = "";
        //    if (string.IsNullOrWhiteSpace(ip))
        //    {
        //        ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    }
        //    if (string.IsNullOrWhiteSpace(ip))
        //    {
        //        ip = request.ServerVariables["X-Forwarded-For"];
        //    }
        //    if (string.IsNullOrWhiteSpace(ip))
        //    {
        //        ip = request.ServerVariables["REMOTE_ADDR"];
        //    }

        //    if (string.IsNullOrWhiteSpace(ip))
        //    {
        //        return request.UserHostAddress;
        //    }

        //    string s = "";
        //    if (ip.Contains(",") == true)
        //    {
        //        var xx = request.Headers.GetValues("Referer");
        //        var referer = xx == null ? "" : string.Join(", ", xx);
        //        var list_ip = ip.Split(new char[1] { ',' });
        //        _log.Info(list_ip);
        //        //if (referer.Contains("HappyRE.link"))
        //        //{
        //        //    s = list_ip.FirstOrDefault();
        //        //}
        //        //else
        //        s = list_ip.LastOrDefault();
        //    }
        //    else
        //    {
        //        s = ip;
        //    }
        //    return s;
        //}

        public static bool ValidateIpByDomain(string domain, string serverIp)
        {
            try
            {
                var entry = Dns.GetHostEntry(domain);
                var hosts = Dns.GetHostAddresses(domain);

                if (hosts.Length == 0 && entry.AddressList.Length==0) return false;

                var sip = IPAddress.Parse(serverIp);
                if (hosts.Any(x => x.Equals(sip))) return true;
                if (entry.AddressList.Any(x => x.Equals(sip))) return true;
                else
                {
                    _log.ErrorFormat("Lỗi domain {0} Ip = {1}", domain, serverIp);

                    string errs = "";
                    foreach (var item in entry.AddressList)
                    {
                        errs += item.ToString() + ";";
                    }
                    _log.ErrorFormat("entry ips {0}", errs);
                    errs = "";
                    foreach (var item in hosts)
                    {
                        errs += item.ToString() + ";";
                    }
                    _log.ErrorFormat("hosts ips {0}", errs);
                    return false;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    var msg = ex.Message;
                    string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (!string.IsNullOrEmpty(ip))
                    {
                        string[] ipRange = ip.Split(',');
                        //int le = ipRange.Length - 1;
                        return ipRange.Contains(serverIp);
                    }
                    else
                    {
                        var rdr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        return rdr.Contains(serverIp);
                    }
                }catch(Exception exx)
                {
                    _log.Error(exx);
                    return false;
                }
            }
        }

        public static string Encode(string s)
        {
            return HttpUtility.HtmlEncode(s);
        }

        static string GetClientIP()
        {
            string result = string.Empty;
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(ip))
            {
                string[] ipRange = ip.Split(',');
                int le = ipRange.Length - 1;
                result = ipRange[0];
            }
            else
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return result;
        }
    }
}
