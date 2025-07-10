using System.Reflection;

namespace PluginSystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : Attribute
    {
        public Type[] Dependencies { get; }

        public PluginAttribute(params Type[] dependencies)
        {
            Dependencies = dependencies ?? Array.Empty<Type>();
        }
    }

    public interface IPlugin
    {
        void Execute();
    }

    public class PluginLoader
    {
        private readonly List<IPlugin> _plugins = new();

        public void LoadPlugins(string folderPath)
        {
            var pendingPlugins = new List<(Type Type, PluginAttribute Attr)>();

            foreach (var dll in Directory.GetFiles(folderPath, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(dll);
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<PluginAttribute>();
                    if (attr != null && typeof(IPlugin).IsAssignableFrom(type))
                    {
                        pendingPlugins.Add((type, attr));
                    }
                }
            }

            while (pendingPlugins.Count > 0)
            {
                var loadedCount = 0;

                foreach (var (type, attr) in pendingPlugins.ToList())
                {
                    if (attr.Dependencies.All(dep => _plugins.Any(p => p.GetType() == dep)))
                    {
                        _plugins.Add((IPlugin)Activator.CreateInstance(type)!);
                        pendingPlugins.Remove((type, attr));
                        loadedCount++;
                    }
                }

                if (loadedCount == 0) break;
            }
        }

        public void ExecuteAll()
        {
            foreach (var plugin in _plugins)
            {
                plugin.Execute();
            }
        }
    }
}
