using System;
using System.Text;

namespace LazyScreen.Sources.Nasa
{

    internal class Collection
    {
        public Link[] links { get; set; }
        public Metadata metadata { get; set; }
        public string version { get; set; }
        public string href { get; set; }
        public Item[] items { get; set; }
    }
}
