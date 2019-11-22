using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LazyScreen.ImageSources
{
    public sealed class Nasa : IImageSource
    {
        // see https://images.nasa.gov/docs/images.nasa.gov_api_docs.pdf
        const string SEARCH = "orion nebula";

        private ImageSearch imageSearch;
        private string[] cachedImages => imageSearch?.collection?.items?.Where(x => !string.IsNullOrWhiteSpace(x.data[0].ImgUrl)).Select(x => x.data[0].ImgUrl).ToArray();
        private readonly Random rnd = new Random();

        public async Task<string> GetImageUrl(HttpClient httpClient)
        {
            if (imageSearch is null)
            {
                var searchResponse = await httpClient.GetStringAsync($"https://images-api.nasa.gov/search?q={HttpUtility.UrlEncode(SEARCH)}&media_type=image").ConfigureAwait(false);
                imageSearch = JsonConvert.DeserializeObject<ImageSearch>(searchResponse);
            }

            var item = imageSearch.collection.items[rnd.Next(0, imageSearch.collection.items.Length)].data[0];

            var assetsResponse = await httpClient.GetStringAsync($"https://images-api.nasa.gov/asset/{HttpUtility.UrlEncode(item.nasa_id)}").ConfigureAwait(false);
            item.ImgUrl = JsonConvert.DeserializeObject<Assets>(assetsResponse).collection.items[0].href;

            return item.ImgUrl;
        }


        public class Assets
        {
            public AssetsCollection collection { get; set; }
        }

        public class AssetsCollection
        {
            public AssetsItem[] items { get; set; }
            public string version { get; set; }
            public string href { get; set; }
        }

        public class AssetsItem
        {
            public string href { get; set; }
        }



        private class ImageSearch
        {
            public Collection collection { get; set; }
        }

        private class Collection
        {
            public Link[] links { get; set; }
            public Metadata metadata { get; set; }
            public string version { get; set; }
            public string href { get; set; }
            public Item[] items { get; set; }
        }

        private class Metadata
        {
            public int total_hits { get; set; }
        }

        private class Link
        {
            public string prompt { get; set; }
            public string href { get; set; }
            public string rel { get; set; }
        }

        private class Item
        {
            public Link[] links { get; set; }
            public Data[] data { get; set; }
            public string href { get; set; }
        }

        private class Data
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
}
