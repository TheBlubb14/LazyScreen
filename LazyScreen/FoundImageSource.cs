using System;

namespace LazyScreen
{
    public class FoundImageSource
    {
        public string Name { get; set; }

        public object Instance { get; set; }

        public FoundImageSource(Type type)
        {
            Name = type.Name;
            Instance = Activator.CreateInstance(type);
        }

        public override string ToString() => Name;
    }
}
