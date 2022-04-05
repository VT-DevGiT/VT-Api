using Synapse.Api.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Utf8Json;
using VT_Api.Exceptions;

namespace VT_Api.Core.Plugin.Updater
{
    public abstract class AbstractUpdateHandler<T> : IUpdateHandler<T>
        where T : IPlugin
    {
        public const string Unknow = "Unknown";
        public const string GitHubPage = "https://api.github.com/repositories/{0}/releases/?per_page=20&page=1";
        public const string DefaultsRegexVersion = @"[v,V]?(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)";


        public abstract long GithubID { get; }
        public virtual string RegexExpressionVersion { get; } = DefaultsRegexVersion;


        public virtual bool TryDownload(Release release, string name)
        {
            var asset = release.Assets.FirstOrDefault(r => r.Name.Contains(name) && r.Name.Contains(".dll"));

            if (asset.Size == 0)
                return false;


            return false;
        }

        public virtual Version GetPluginVersion()
        {
            var info = (PluginInformation)Attribute.GetCustomAttribute(typeof(T), typeof(PluginInformation));
            if (info.Version == Unknow)
            {
                if (info.Name == Unknow)
                    throw new VtUnknownVersionException($"Vt-AutoUppdate : The plugin in the assembly {typeof(T).Assembly.GetName()} did not set its version", typeof(T).Assembly.FullName);
                else
                    throw new VtUnknownVersionException($"Vt-AutoUppdate : The plugin {info.Name} in the assembly {typeof(T).Assembly.GetName()} did not set its version", typeof(T).Assembly.FullName, info.Name);
            }
            return new Version(info.Version, RegexExpressionVersion);
        }

        public virtual Version GetGithubVersion(out Release release, bool ignorePrerealase = true)
        {
            var realases = GetRealases().GetAwaiter().GetResult();

            Version highestVersion = new Version(0,0,0);
            Release highestRelease = null;
            foreach (var realase in realases)
            {
                if (ignorePrerealase && realase.PreRelease)
                    continue;
                if (Version.TryParse(realase.TagName, out var version) && version > highestVersion)
                {
                    highestVersion = version;
                    highestRelease = realase;
                }
            }
            release = highestRelease;
            return highestVersion;
        } 

        private async Task<List<Release>> GetRealases()
        {
            var client = new HttpClient();

            client.Timeout = TimeSpan.FromSeconds(500);
            client.DefaultRequestHeaders.Add("User-Agent", $"VT-API");

            var url = string.Format(GitHubPage, GithubID);
            var response = await client.GetAsync(url).ConfigureAwait(false);
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            
            return JsonSerializer.Deserialize<Release[]>(stream).OrderByDescending(r => r.CreatedAt.Ticks).ToList();
        }

        public virtual bool NeedToUpdate(Version PluginVersion, Version GitVersion)
            => PluginVersion < GitVersion;
        

        public virtual void Replace()
        {

        }
    }
}
