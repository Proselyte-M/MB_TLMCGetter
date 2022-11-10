using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;

namespace TlmcMediaWiki
{
    internal class StringFormat
    {
        /// <summary>
        /// 判断节点是否为空，为空返回false;
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public static bool CheakNodeIsNotEmpty(XmlNode xmlNode)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlNode.OuterXml);
                string root = xml.SelectSingleNode("/").InnerXml;

                if (root != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool CheakNodeIsNotEmpty(XmlNodeList xmlNode)
        {
            try
            {
                if (xmlNode.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }



        /// <summary>
        /// 截取两个字符中间的字符串
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="startstr"></param>
        /// <param name="endstr"></param>
        /// <returns></returns>
        public static string MidStrEx(string sourse, string startstr, string endstr)
        {
            Regex rg = new Regex("(?<=(" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(sourse).Value;
        }

      /// <summary>
        /// 请求url内容并解析为xml
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="refer">refer</param>
        /// <returns></returns>
        static public XmlDocument requestJObject(string url, string refer)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {

                if (!String.IsNullOrEmpty(url))
                {
                    Console.WriteLine("request: " + url);
                    //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    //WebClient wc = new WebClient();
                    //wc.Headers.Add("user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.35");
                    //wc.Encoding = System.Text.Encoding.UTF8;
                    //wc.UseDefaultCredentials = true;
                    //var SearchString = wc.DownloadString(url);
                    var request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));

                    //request.CookieContainer = CookieHelper.get().GetCookieCollection();
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36 Edg/89.0.774.54";
                    if (!String.IsNullOrEmpty(refer))
                        request.Referer = refer;
                    request.Timeout = 20000;
                    request.Accept = "*/*";
                    request.KeepAlive = true;
                    request.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                    request.ReadWriteTimeout = 10000;

                    var response = (HttpWebResponse)request.GetResponse();
                    var SearchString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    CookieHelper.get().addCookie(response.Cookies);

                    if (!String.IsNullOrEmpty(SearchString))
                    {
                        Console.WriteLine("request result str: " + SearchString.Substring(0, Math.Min(100, SearchString.Length - 1)));
                        //return JObject.Parse(SearchString);
                        //XmlReaderSettings settings = new XmlReaderSettings();
                        //settings.IgnoreComments = true;
                        //XmlReader reader = XmlReader.Create(SearchString, settings);
                        SearchString = HttpUtility.HtmlDecode(SearchString);
                        xmlDocument.LoadXml(SearchString);
                        //reader.Close();
                        return xmlDocument;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("readXML:" + url);
                Console.WriteLine(e);
            }
            xmlDocument.LoadXml("<null></null>");
            return xmlDocument;

        }

    }
}