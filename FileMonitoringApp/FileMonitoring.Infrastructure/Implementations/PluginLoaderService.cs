using FileMonitoring.Core.Interfaces;
using System.Reflection;

namespace FileMonitoring.Infrastructure.Implementations
{
    public class PluginLoaderService : IPluginLoaderService
    {
        public List<ILoader> InstallPluginsFromCurrentDirectory(string pluginFolder)
        {
            var loaders = new List<ILoader>();

            if (!Directory.Exists(pluginFolder))
                return loaders;

            var dllFiles = Directory
                .GetFiles(pluginFolder, "*.dll", SearchOption.TopDirectoryOnly);

            foreach (var dllFile in dllFiles)
            {
                var assembly = Assembly.LoadFrom(dllFile);

                var types = assembly.GetTypes()
                .Where(t => typeof(ILoader).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                foreach (var type in types)
                {
                    if (Activator.CreateInstance(type) is ILoader loader)
                    {
                        loaders.Add(loader);
                    }
                }
            }

            return loaders;
        }
    }
}
