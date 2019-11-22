using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace LazyScreen.ImageSources
{
    public sealed class DogCeo : IImageSource
    {
        public async Task<string> GetImageUrl(HttpClient httpClient)
        {
            var response = await httpClient.GetStringAsync("https://dog.ceo/api/breeds/image/random").ConfigureAwait(false);
            return JsonConvert.DeserializeObject<Result>(response).message;
        }

        private class Result
        {
            public string status { get; set; }
            public string message { get; set; }
        }
    }
}
