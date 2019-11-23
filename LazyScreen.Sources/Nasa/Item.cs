using System;
using System.Text;

namespace LazyScreen.Sources.Nasa
{

    internal class Item
    {
        public Link[] links { get; set; }
        public Data[] data { get; set; }
        public string href { get; set; }
    }
}
