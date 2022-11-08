using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.MediaWIKI
{
    internal class SearchListReq
    {
        private string text;
        private string url;
        private string description;
        private double similarity;

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
            }
        }
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public double Similarity
        {
            get { return similarity; }
            set { similarity = value; }
        }
    }
}
