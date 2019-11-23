using System.Threading.Tasks;

namespace LazyScreen.Sources
{
    public interface ISource
    {
        Task<string> GetImage();
    }
}
