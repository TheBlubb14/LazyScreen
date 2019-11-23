using LazyScreen.Sources.Nasa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace LazyScreen.Sources
{
    public sealed class Source
    {
        public IEnumerable<(string Name, ISource Instance)> Sources { get; }

        private readonly Random _rnd;
        private readonly HttpClient _httpClient;

        public Source(HttpClient httpClient, string nasaQuery = null)
        {
            _rnd = new Random();
            _httpClient = httpClient;

            var defaultParameters = new object[] { httpClient };
            var nasaParameters = new object[] { httpClient, nasaQuery };

            Sources =
                Assembly
                .GetAssembly(typeof(Source))
                .GetTypes()
                .Where(x => !x.IsInterface && x.GetInterface(nameof(ISource)) != null)
                .Select(x => (x.Name, (ISource)Activator.CreateInstance(x, x == typeof(NasaSource) ? nasaParameters : defaultParameters)));
        }

        public (string SourceName, Task<string> Source) GetRandomSource()
        {
            var (Name, Instance) = Sources.ElementAt(_rnd.Next(0, Sources.Count()));
            return (Name, Instance.GetImage());
        }
    }
}
