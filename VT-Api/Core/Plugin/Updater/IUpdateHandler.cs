using Synapse.Api.Plugin;
using System.Net.Http;

namespace VT_Api.Core.Plugin.Updater
{
    public interface IUpdateHandler
    {
        string RegexExpressionVersion { get; }

        void DeletetTempDirectory();
        Version GetPluginVersion<T>();
        Version GetGitVersion(HttpClient client, string link, out Release release, bool ignorePrerealase = true);
        bool NeedToUpdate(Version PluginVersion, Version GitVersion);
        bool TryDownload(HttpClient client, Release release, string name, out string filePath);
        void Replace(string newPluginPath, string pluinName, string pluginDirectory);
    }
}
