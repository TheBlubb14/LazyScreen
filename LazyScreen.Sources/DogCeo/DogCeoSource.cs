using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace LazyScreen.Sources.DogCeo
{
    public sealed class DogCeoSource : ISource
    {
        public const string QUERY = "https://dog.ceo/api/breeds/image/random";

        private readonly HttpClient _httpClient;

        public DogCeoSource(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetImage()
        {
            var response = await _httpClient.GetStringAsync(QUERY).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<DogResult>(response).message;
        }
    }
}
