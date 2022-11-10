using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TlmcMediaWiki
{
    internal static class MathS
    {
        /// <summary>
        /// 获取两个字符串的相似度
        /// </summary>
        /// <param name=”sourceString”>第一个字符串</param>
        /// <param name=”str”>第二个字符串</param>
        /// <returns></returns>
        public static decimal GetSimilarityWith(this string sourceString, string str)
        {

            decimal Kq = 2;
            decimal Kr = 1;
            decimal Ks = 1;

            char[] ss = sourceString.ToCharArray();
            char[] st = str.ToCharArray();

            //获取交集数量
            int q = ss.Intersect(st).Count();
            int s = ss.Length - q;
            int r = st.Length - q;

            return Kq * q / (Kq * q + Kr * r + Ks * s);
        }
    }
}
