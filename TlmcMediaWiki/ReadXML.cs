using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace TlmcMediaWiki
{
    public class ReadXML
    {



        public string ReadFilePath(XDocument xDocument)
        {
            try
            {
                var query = from n in xDocument.Root.Descendants("文件信息")
                            select n;
                foreach (XElement item in query)
                {
                    return (item.Element("Path").Value);
                }

                return null;
            
        }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 读取专辑信息内指定节点的值
        /// </summary>
        /// <param name="xDocument">xml结构</param>
        /// <param name="node">节点名称</param>
        /// <returns></returns>

        public string ReadAlbumSingle(XDocument xDocument, string node)
        {
            try
            {
                var query = from n in xDocument.Root.Descendants("专辑信息")
                            select n;
                foreach (XElement item in query)
                {
                    return (item.Element(node).Value);
                }

                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 获取歌曲信息内指定曲目需要的指定内容的值
        /// </summary>
        /// <param name="xDocument">xml结构</param>
        /// <param name="soungid">歌曲编号</param>
        /// <param name="node">节点名称</param>
        /// <returns></returns>
        public string ReadSongSingleSong(XDocument xDocument, string soungid, string node)
        {
            try
            {
                var query = from n in xDocument.Root.Descendants("歌曲信息")
                            select n;
                foreach (XElement item in query)
                {
                    if (item.Element("歌曲编号").Value == soungid)
                    {
                        return (item.Element(node).Value);
                    }

                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public Tuple<List<string>, List<string>> ReadAlbumAll(XDocument xDocument)
        {
            try
            {
                List<string> nodename = new List<string>();
                List<string> valve = new List<string>();
                var query = from n in xDocument.Root.Descendants("专辑信息")
                            select n;
                foreach (XElement item in query)
                {
                    nodename.Add(item.Name.ToString());
                    valve.Add(item.Value);
                }
                Tuple<List<string>, List<string>> info = Tuple.Create(nodename, valve);
                return info;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public string[] ReadSingleSongALL(XDocument xDocument,string songid)
        {
            try
            {
                List<string> strings = new List<string>();
                var query = from n in xDocument.Root.Descendants("歌曲信息")
                            select n;
                foreach (XElement item in query)
                {

                    strings.Add(item.Value);
                }

                return strings.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
