using Synapse.Api.Plugin;
using System.Net.Http;

namespace VT_Api.Core.Plugin.Updater
{
    public interface IUpdateHandler
    {
        string RegexExpressionVersion { get; }

        void DeletetTempDirectory();
        PluginVersion GetPluginVersion<T>();
        PluginVersion GetGitVersion(HttpClient client, string link, out Release release, bool ignorePrerealase = true);
        bool NeedToUpdate(PluginVersion PluginVersion, PluginVersion GitVersion);
        bool TryDownload(HttpClient client, Release release, string name, out string filePath);
        void Replace(string newPluginPath, string pluinName, string pluginDirectory);
    }
}
