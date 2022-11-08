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

namespace MusicBeePlugin.MediaWIKI
{
    internal class mediawiki : StringFormat
    {




        //private string cmd;
        //https://thwiki.cc/api.php?action=opensearch&format=json&search=%E7%81%B5%E6%A2%A6
        static List<AlbumInfo> albumInfoList = new List<AlbumInfo>();


        public static AlbumInfo[] GetAlbumInfo(string artis,string album)
        {
            SearchListReq slr = search(artis, album);

            if (string.IsNullOrEmpty(slr.Url))
            {
                Console.WriteLine("GetAlbumInfo字段为空。");
                return null;
            }
            string url = "https://thwiki.cc/" + slr.Url;
            url = "https://thwiki.cc/api.php?action=parse&format=xml&page=" + slr.Url + "&prop=text";
            Console.WriteLine("开始获取专辑信息" + url);
            XmlDocument xmlDocument = requestJObject(url, null);
            //XmlElement root = null;
            //root = xmlDocument.DocumentElement;
            //XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            //nsmgr.AddNamespace("tlmc", "http://opensearch.org/searchsuggest2");
            //XmlNodeList listNodes = null;


            var listAlbumInfo = xmlDocument.SelectNodes("/api/parse/text/div/table/tbody");
            int circle = 0;
            AlbumInfo albumInfo = new AlbumInfo();
            foreach (XmlNode item in listAlbumInfo)
            {

                var node = item.SelectNodes("tr");
                string str = null;
                //foreach (XmlNode item2 in node)
                int i = 0;
                IEnumerator ienum = node.GetEnumerator();
                bool flag = false; //一首歌扫描完的标志

                while (ienum.MoveNext())
                {

                    new AlbumInfo();


                    str = node.Item(i).InnerText;




                    if (circle == 0)  //第一圈获取专辑的信息
                    {
                        flag = GetAlbumInfo(albumInfo, str, i);
                    }
                    if (circle == 2) //第三圈是歌曲信息
                    {
                        flag = GetSongInfo(albumInfo, str, flag);

                    }

                    i++;
                    if (flag && !string.IsNullOrEmpty(albumInfo.Title))
                    {
                        albumInfoList.Add(albumInfo);
                        flag = false;
                        albumInfo = new AlbumInfo();
                    }
                }
                circle++;
                Console.WriteLine();
            }
            return albumInfoList.ToArray();
        }

        private static bool GetAlbumInfo(AlbumInfo albumInfo, string str, int i)
        {
            bool flag = false;
            Console.WriteLine(i + "-：专辑信息：" + str);
            string substr = str.Substring(0, 2);
            //Console.WriteLine(substr);
            str = str.Replace(substr, "").Replace("：", "").Replace("\r", "").Replace("\n", "");
            switch (substr)
            {

                case "角色": { albumInfo.CoverCor = str.Trim(); break; }
                case "名称": { albumInfo.AlbumName = str.Trim(); break; }
                case "读音": { albumInfo.Pronunciation = str.Trim(); break; }
                case "制作": { albumInfo.AlbumArtis = str.Substring(1, str.Length - 1).Trim(); break; }
                case "首发": { albumInfo.Date = str.Substring(2, str.Length - 2).Trim(); break; }
                case "类型": { albumInfo.Type = str.Trim(); break; }
                case "编号": { albumInfo.Number = str.Trim(); break; }
                case "碟数": { albumInfo.DiscCount = str.Trim(); break; }
                case "音轨": { albumInfo.TotalSong = str.Substring(1, str.Length - 1).Trim(); break; }
                case "官网": { albumInfo.Website = str.Substring(2, str.Length - 2).Trim(); break; flag = true; }
                default:
                    break;
            }

            return flag;
        }

        private static bool GetSongInfo(AlbumInfo albumInfo, string str, bool flag)
        {
            //Console.WriteLine(i + "-：歌曲信息信息：" + str);
            string substr = str.Substring(0, 2);
            Console.WriteLine(substr);
            //str = str.Replace(substr, "").Replace("：", "").Replace("\r", "").Replace("\n", "");
            switch (substr)
            {
                case "编曲":
                    {
                        albumInfo.Arrangement = str.Substring(2, str.Length - 2).Trim();
                        break;
                    }
                case "演唱":
                    {
                        albumInfo.Singing = str.Substring(2, str.Length - 2).Trim();
                        break;
                    }
                case "作词":
                    {
                        albumInfo.Lyricist = str.Substring(2, str.Length - 2).Trim();
                        break;
                    }
                default:
                    {
                        if (substr.All(char.IsDigit)) //如果是数字则是曲目号
                        {
                            albumInfo.SongNo = substr.Trim();
                            string pattern = @"\d\d:\d\d";
                            MatchCollection mc = Regex.Matches(str, pattern);
                            foreach (Match m in mc)
                            {
                                albumInfo.Time = m.Value.Trim();
                            }
                            string tstr = str.Substring(2, str.Length - 2);
                            tstr = tstr.Substring(0, tstr.Length - 5);
                            albumInfo.Title = tstr.Trim();
                            Console.WriteLine(albumInfo.SongNo + albumInfo.Title + albumInfo.Time);
                        }
                        if (substr == "原曲")
                        {
                            if (string.IsNullOrEmpty(albumInfo.OriginalMusicName))
                            {
                                flag = true;
                            }
                            albumInfo.OriginalMusicName += str.Substring(2, str.Length - 2).Trim();

                        }
                        if (substr == "初发")
                        {
                            if (string.IsNullOrEmpty(albumInfo.OriginalMusicName))
                            {
                                flag = true;
                            }
                            albumInfo.OriginalMusicName += "原创曲：" + str.Substring(3, str.Length - 3).Trim();

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
        public static SearchListReq search(string artist, string album)
        {
            Console.WriteLine("开始搜索专辑是否存在" + artist + album);
            if (string.IsNullOrEmpty(album))
            {
                return null;
            }

            string[] searchUrls;

            string baseUrl = "https://thwiki.cc/api.php?action=opensearch&format=xml&search=" + album;

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
                xmlDocument = requestJObject(SearchUrl, null); //搜索结果曲目列表
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
                    var Similarity = (double)MathS.GetSimilarityWith(artist, MidStrEx(description, "由 ", " 于"));
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
