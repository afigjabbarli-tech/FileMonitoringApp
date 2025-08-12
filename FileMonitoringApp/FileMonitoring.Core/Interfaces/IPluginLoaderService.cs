namespace FileMonitoring.Core.Interfaces
{
    public interface IPluginLoaderService
    {
        List<ILoader> InstallPluginsFromCurrentDirectory(string pluginFolder);
    }
}
