using System;
using System.Text;

namespace LazyScreen.Sources.Nasa
{

    internal class Data
    {
        public string nasa_id { get; set; }
        public string[] keywords { get; set; }
        public string media_type { get; set; }
        public DateTime date_created { get; set; }
        public string center { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public string photographer { get; set; }
        public string[] album { get; set; }

        // Manually set
        public string ImgUrl { get; set; }
    }
}
