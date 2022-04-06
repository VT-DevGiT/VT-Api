using Synapse.Api.Plugin;
using System.Net.Http;

namespace VT_Api.Core.Plugin.Updater
{
    public interface IUpdateHandler<T>
         where T : IPlugin
    {
        string RegexExpressionVersion { get; }


        void DeletetTempDirectory();
        Version GetPluginVersion();
        Version GetGithubVersion(HttpClient client, out Release release, bool ignorePrerealase = true);
        bool NeedToUpdate(Version PluginVersion, Version GitVersion);
        bool TryDownload(HttpClient client, Release release, string name, out string filePath);
        void Replace(string newPluginPath);
    }
}
