using Synapse;
using Synapse.Api.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VT_Api.Reflexion;

namespace VT_Api.Core.Plugin.Updater
{
    public abstract class AbstractAutoUpdater<T> : AbstractUpdateHandler, IAutoUpdate
        where T : IPlugin
    {
        
        public abstract long GithubID { get; }
        public abstract bool Prerealase { get; }

        public virtual bool Update()
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(500);
            client.DefaultRequestHeaders.Add("User-Agent", $"VT-API");


            var githubVerison = GetGitVersion(client, string.Format(GitHubPage, GithubID), out var release, Prerealase);
            var pluginVersion = GetPluginVersion<T>();
            var pluginName = typeof(T).Assembly.GetName().Name;

            if (!NeedToUpdate(pluginVersion, githubVerison))
                return false;

            if (!TryDownload(client, release, pluginName, out var filePath))
                return false;

            var plugin = SynapseController.PluginLoader.GetFieldValueOrPerties<List<IPlugin>>("_plugins").FirstOrDefault(p => p.GetType() == typeof(T));

            if (plugin == null)
                throw new Exception("AutoUpdater : Plugin note found !");

            var shared = plugin.Information.GetFieldValueOrPerties<bool>("shared");
            var pluginPath = shared ? Server.Get.Files.SharedPluginDirectory : Server.Get.Files.PluginDirectory;

            base.Replace(filePath, pluginName, pluginPath);

            return true;
        }
    }
}
