using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LazyScreen.Sources.Nasa
{
    /// <summary>
    /// Nasa Images
    /// <br/>
    /// https://images.nasa.gov/docs/images.nasa.gov_api_docs.pdf
    /// </summary>
    public sealed class NasaSource : ISource
    {
        public const string SEARCH_QUERY = "https://images-api.nasa.gov/search?media_type=image&q=";
        public const string ASSETS_QUERY = "https://images-api.nasa.gov/asset/";

        private NasaResult _result;

        private readonly Random rnd;
        private readonly HttpClient _httpClient;
        private readonly string _search;

        public NasaSource(HttpClient httpClient, string search)
        {
            rnd = new Random();
            _httpClient = httpClient;
            _search = search;
        }

        private async Task QuerySearch()
        {
            var response = await _httpClient.GetStringAsync(SEARCH_QUERY + HttpUtility.UrlEncode(_search)).ConfigureAwait(false);
            _result = JsonConvert.DeserializeObject<NasaResult>(response);
        }

        private async Task<string> GetOriginalAsset(Data item)
        {
            // Is it already cached?
            if (!string.IsNullOrWhiteSpace(item.ImgUrl))
                return item.ImgUrl;

            var response = await _httpClient.GetStringAsync(ASSETS_QUERY + HttpUtility.UrlEncode(item.nasa_id)).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<NasaResult>(response);

            // Index 0 means original image
            var url = result.collection.items[0].href;

            // Cache inside the search result
            item.ImgUrl = url;

            return url;
        }

        public async Task<string> GetImage()
        {
            // Lazy search
            if (_result is null)
                await QuerySearch().ConfigureAwait(false);

            // Select random item in the search query
            // Indey 0 because there is only one item in data
            var item = _result.collection.items[rnd.Next(0, _result.collection.items.Length)].data[0];

            return await GetOriginalAsset(item).ConfigureAwait(false);
        }
    }
}
