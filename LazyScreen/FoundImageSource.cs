using LazyScreen.Sources;

namespace LazyScreen
{
    public class FoundImageSource
    {
        public string Name { get; set; }

        public ISource Instance { get; set; }

        public FoundImageSource(string name, ISource instance)
        {
            Name = name;
            Instance = instance;
        }

        public override string ToString() => Name;
    }
}
