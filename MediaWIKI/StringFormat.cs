using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace MusicBeePlugin.MediaWIKI
{
    internal class StringFormat
    {



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
        /// 获取的专辑信息整理
        /// </summary>
        /// <param name="albumInfos"></param>
        /// <returns></returns>
        public static AlbumInfo[] FormatAlbumInfo(AlbumInfo[] albumInfos)
        {
            for (int i = 0; i < albumInfos.Count(); i++)
            {
                if (string.IsNullOrEmpty(albumInfos[i].Cover))
                {
                    albumInfos[i].Cover = albumInfos[0].Cover;
                }
                if (string.IsNullOrEmpty(albumInfos[i].CoverCor))
                {
                    albumInfos[i].CoverCor = albumInfos[0].CoverCor;
                }
                if (string.IsNullOrEmpty(albumInfos[i].AlbumName))
                {
                    albumInfos[i].AlbumName = albumInfos[0].AlbumName;
                }
                if (string.IsNullOrEmpty(albumInfos[i].Pronunciation))
                {
                    albumInfos[i].Pronunciation = albumInfos[0].Pronunciation;
                }
                if (string.IsNullOrEmpty(albumInfos[i].AlbumArtis))
                {
                    albumInfos[i].AlbumArtis = albumInfos[0].AlbumArtis;
                }
                if (string.IsNullOrEmpty(albumInfos[i].Date))
                {
                    albumInfos[i].Date = albumInfos[0].Date;
                }
                if (string.IsNullOrEmpty(albumInfos[i].Number))
                {
                    albumInfos[i].Number = albumInfos[0].Number;
                }
                if (string.IsNullOrEmpty(albumInfos[i].Type))
                {
                    albumInfos[i].Type = albumInfos[0].Type;
                }
                if (string.IsNullOrEmpty(albumInfos[i].Style))
                {
                    albumInfos[i].Style = albumInfos[0].Style;
                }
                if (string.IsNullOrEmpty(albumInfos[i].Website))
                {
                    albumInfos[i].Website = albumInfos[0].Website;
                }
                if (string.IsNullOrEmpty(albumInfos[i].DiscCount))
                {
                    albumInfos[i].DiscCount = albumInfos[0].DiscCount;
                }
                if (string.IsNullOrEmpty(albumInfos[i].TotalSong))
                {
                    albumInfos[i].TotalSong = albumInfos[0].TotalSong;
                }
            }

            return albumInfos;
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