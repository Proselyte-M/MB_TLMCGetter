using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.MediaWIKI
{
    internal class AlbumInfo
    {
        private string cover; //封面
        private string coverCor; //封面角色
        private string albumName; //专辑名
        private string pronunciation; //发音
        private string albumArtis; //专辑音乐家
        private string date; //发布日期
        private string number; //专辑编号
        private string type; //专辑类型
        private string style; //专辑风格
        private string website; //官网
        private string discCount; //碟数
        private string totalSong; //音轨数



        private string songNo;
        private string title;
        private string time;
        private string arrangement;
        private string remix;
        private string singing;
        private string lyricist;
        private string played;
        private string originalMusicName;

        public string Cover { get { return cover; } set { cover = value; } }
        public string CoverCor { get { return coverCor; } set { coverCor = value; } }
        public string AlbumName { get { return albumName; } set { albumName = value; } }
        public string Pronunciation { get { return pronunciation; } set { pronunciation = value; } }
        public string AlbumArtis { get { return albumArtis; } set { albumArtis = value; } }
        public string Date { get { return date; } set { date = value; } }
        public string Number { get { return number; } set { number = value; } }
        public string Type { get { return type; } set { type = value; } }
        public string Style { get { return style; } set { style = value; } }
        public string Website { get { return website; } set { website = value; } }
        public string DiscCount { get { return discCount; } set { discCount = value; } }
        public string TotalSong { get { return totalSong; } set { totalSong = value; } }




        public string SongNo { get { return songNo; } set { songNo = value; } }
        public string Title { get { return title; } set { title = value; } }
        public string Time { get { return time; } set { time = value; } }
        public string Arrangement { get { return arrangement; } set { arrangement = value; } }
        public string Remix { get { return remix; } set { remix = value; } }
        public string Singing { get { return singing; } set { singing = value; } }
        public string Lyricist { get { return lyricist; } set { lyricist = value; } }
        public string Played { get { return played; } set { played = value; } }
        public string OriginalMusicName { get { return originalMusicName; } set { originalMusicName = value; } }

        public void Clean()
        {
            cover = null;
            coverCor = null;
            albumName = null;
            pronunciation = null;
            albumArtis = null;
            date = null;
            number = null;
            type = null;
            style = null;
            website = null;
            discCount = null;
            totalSong = null;
            songNo = null;
            title = null;
            time = null;
            arrangement = null;
            remix = null;
            singing = null;
            lyricist = null;
            played = null;
            originalMusicName = null;
        }

    }
}
