using System;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Xml.Linq;

namespace TlmcMediaWiki
{
    public class Mediawiki
    {
        static XDocument xml = new XDocument();
        public Mediawiki()
        {

                xml.Add(new XElement("root"));

        }

        ~Mediawiki()
        {
            GC.SuppressFinalize(true);
        }



        static int songNum = 1;
        public XDocument GetAlbumInfo(string Path, string artis, string album)
        {

            XElement pathNode = new XElement("文件信息");
            pathNode.SetElementValue("Path", Path);
            xml.Root.Add(pathNode);
            SearchListReq slr = search(artis, album);

            if (string.IsNullOrEmpty(slr.Url))
            {
                Console.WriteLine("GetAlbumInfo字段为空。");
            }
            string url = "https://thwiki.cc/" + slr.Url;
            url = "https://thwiki.cc/api.php?action=parse&format=xml&page=" + slr.Url + "&prop=text";
            Console.WriteLine("开始获取专辑信息" + url);
            XmlDocument xmlDocument = StringFormat.requestJObject(url, null);

            var table = xmlDocument.SelectNodes("/api/parse/text/div/table/tbody");
            foreach (XmlNode nodes in table)
            {
                XmlNodeList tr = nodes.SelectNodes("tr");
                if (StringFormat.CheakNodeIsNotEmpty(tr))
                {
                    if (tr[0].FirstChild.InnerText == "基本信息")
                    {

                        GetALbumInfoNext(tr);
                    }
                }
                if (StringFormat.CheakNodeIsNotEmpty(tr))
                {

                    if (tr[0].FirstChild.InnerText == "01")
                    {
                        Console.WriteLine("歌曲信息:\r\n");
                        XElement xElement = new XElement("歌曲信息");
                        foreach (XmlNode node in tr)
                        {
                            if (node.FirstChild.InnerText.All(char.IsDigit) && !string.IsNullOrEmpty(node.FirstChild.InnerText))
                            {
                                if (int.Parse(node.FirstChild.InnerText) > songNum)
                                {
                                    xml.Root.Add(xElement);
                                    xElement = new XElement("歌曲信息");
                                    Console.WriteLine("第" + node.FirstChild.InnerText + "首");
                                }
                            }
                            int i = 0;

                            foreach (object nodess in node.ChildNodes)
                            {
                                if (nodess == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (i > 1)
                                    {

                                        GetSongInfo(node.ChildNodes[i - 2].InnerText, node.ChildNodes[i - 1].InnerText, node.ChildNodes[i].InnerText, xElement);
                                    }
                                }
                                i++;
                            }

                        }
                    }

                }

            }
            return xml;
        }

        private static void GetALbumInfoNext(XmlNodeList tr)
        {
            XElement xElement = new XElement("专辑信息");
            Console.WriteLine("基本信息:\r\n");
            foreach (XmlNode node in tr)
            {
                int i = 0;
                foreach (object nodess in node.ChildNodes)
                {

                    if (nodess == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (i > 0)
                        {
                            string str = node.ChildNodes[i].InnerText;
                            string substr = node.ChildNodes[i - 1].InnerText;
                            try
                            {

                                if (substr.Contains("："))
                                {
                                    str = substr;
                                    substr = substr.Substring(0, substr.IndexOf("："));
                                    str = str.Replace(substr, "").Replace("：", "");
                                }
                                if (substr.Contains(":"))
                                {
                                    str = substr;
                                    substr = substr.Substring(0, substr.IndexOf(":"));
                                    str = str.Replace(substr, "").Replace(":", "");
                                }
                                xElement.SetElementValue(substr, str);

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }
                        }
                        XmlNode xmlNode = (XmlNode)nodess;
                        Console.WriteLine(xmlNode.InnerText);

                    }
                    i++;
                }
            }
            xml.Root.Add(xElement);
        }

        private static bool GetSongInfo(string substr, string str, string nextstr, XElement xElement)
        {

            if (string.IsNullOrEmpty(substr))
            {
                substr = str;
            }
            bool flag = false;
            switch (substr)
            {
                case "编曲":
                    {
                        xElement.SetElementValue(substr, nextstr);
                        break;
                    }
                case "演唱":
                    {
                        xElement.SetElementValue(substr, nextstr);
                        break;
                    }
                case "作词":
                    {
                        xElement.SetElementValue(substr, nextstr);
                        break;
                    }
                default:
                    {
                        if (substr.All(char.IsDigit)) //如果是数字则是曲目号
                        {
                            xElement.SetElementValue("歌曲编号", substr);
                            xElement.SetElementValue("曲名", str);
                            xElement.SetElementValue("时长", nextstr);
                            songNum = int.Parse(substr.Trim());
                            Console.WriteLine(substr + str + nextstr);
                        }
                        if (substr == "原曲")
                        {
                            xElement.SetElementValue(substr, nextstr);

                            //albumInfo.OriginalMusicName += nextstr.Trim();

                        }
                        if (substr == "初发")
                        {
                            xElement.SetElementValue(substr, nextstr);
                            break;
                        }


                        break;
                    }
            }

            return flag;
        }



        /// <summary>
        /// 搜索专辑名称并与乐队比对。
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="album"></param>
        /// <returns></returns>
        private static SearchListReq search(string artist, string album)
        {
            Console.WriteLine("开始搜索专辑是否存在" + artist + album);
            if (string.IsNullOrEmpty(album))
            {
                return null;
            }

            string[] searchUrls;

            string baseUrl = "https://thwiki.cc/api.php?action=opensearch&format=xml&search=" + HttpUtility.UrlEncode(album);

            if (String.IsNullOrEmpty(artist))
            {
                searchUrls = new string[] { baseUrl };
            }
            else
            {
                //searchUrls = new string[] { baseUrl + HttpUtility.UrlEncode(" " + artist), baseUrl };
                searchUrls = new string[] { baseUrl };
            }
            string R_album = prepareString(album, false);



            XmlDocument xmlDocument = new XmlDocument();
            SearchListReq searchList = new SearchListReq();
            searchList.Similarity = 0.0;
            foreach (string SearchUrl in searchUrls)
            {
                xmlDocument = StringFormat.requestJObject(SearchUrl, null); //搜索结果曲目列表
                XmlElement root = null;
                root = xmlDocument.DocumentElement;
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
                nsmgr.AddNamespace("tlmc", "http://opensearch.org/searchsuggest2");
                XmlNodeList listNodes = null;
                listNodes = xmlDocument.SelectNodes("/tlmc:SearchSuggestion/tlmc:Section/tlmc:Item", nsmgr);

                foreach (XmlNode item in listNodes)
                {
                    var text = item.SelectSingleNode("tlmc:Text", nsmgr).InnerText;
                    var url = item.SelectSingleNode("tlmc:Url", nsmgr).InnerText;
                    var description = item.SelectSingleNode("tlmc:Description", nsmgr).InnerText;
                    if (description.IndexOf("专辑") < 0)
                    {

                        Console.WriteLine(text + "不是专辑，跳过");
                        break;
                    }
                    Console.WriteLine(text);

                    url = url.Replace("https://thwiki.cc/", "");
                    Console.WriteLine(url);
                    Console.WriteLine(description);
                    var Similarity = (double)MathS.GetSimilarityWith(artist, StringFormat.MidStrEx(description, "由 ", " 于"));
                    Console.WriteLine("搜索结果与本地音乐家相似程度" + Similarity * 100 + "%");
                    //if (Similarity<0.5)
                    //{
                    //    Console.WriteLine("相似度低于50%，放弃该结果");
                    //    break;
                    //}

                    Console.WriteLine();
                    if (searchList.Similarity > Similarity)
                    {
                        Console.WriteLine("已缓存的结果较优，放弃该条");
                        break;
                    }
                    else
                    {
                        searchList.Text = text;
                        searchList.Url = url;
                        searchList.Description = description;
                        searchList.Similarity = Similarity;
                    }
                }
            }
            return searchList;
        }





        static public string prepareString(string s, bool replace_to_space)
        {
            System.Text.RegularExpressions.Regex rgx = new Regex("[\\s\\]\\[\\(\\)`~!@#$%^&\\*()+=|{}':;',\\.<>/\\?~～〜（）「」［］！@#￥%……&*——+|{}【】‘；：”“’。，、？]+");
            if (replace_to_space)
                return rgx.Replace(s.ToLower(), " ");
            return rgx.Replace(s.ToLower(), "");
        }
    }



}

