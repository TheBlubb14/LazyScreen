using System.Net.Http;
using System.Threading.Tasks;

namespace LazyScreen.ImageSources
{
    public interface IImageSource
    {
        Task<string> GetImageUrl(HttpClient httpClient);
    }
}
