using Synapse;
using Synapse.Api.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Utf8Json;
using VT_Api.Exceptions;
using VT_Api.Reflexion;

namespace VT_Api.Core.Plugin.Updater
{

    /// <summary>
    /// for plugin on github
    /// </summary>
    public abstract class AbstractUpdateHandler : IUpdateHandler
    {
        #region Properties & Variable
        public const string Unknow = "Unknown";
        public const string GitHubPage = "https://api.github.com/repositories/{0}/releases?per_page=20&page=1";

        private static bool FirstLoad { get; set; } = true;

        private string _tempDirectory;
        public string TempDirectory
        {
            get
            {
                if (!Directory.Exists(_tempDirectory))
                {
                    Directory.CreateDirectory(_tempDirectory);
                }

                return _tempDirectory;
            }
            private set
            {
                _tempDirectory = value;
            }
        }

        private string _oldDllDirectory;
        public string OldDllDirectory
        {
            get
            {
                if (!Directory.Exists(_oldDllDirectory))
                {
                    if (!Directory.Exists(_tempDirectory))
                        Directory.CreateDirectory(_tempDirectory);
                    
                    Directory.CreateDirectory(_oldDllDirectory);
                }

                return _oldDllDirectory;
            }
            private set
            {
                _oldDllDirectory = value;
            }
        }


        private string _DownloadDirectory;
        public string DownloadDirectory
        {
            get
            {
                if (!Directory.Exists(_DownloadDirectory))
                {
                    if (!Directory.Exists(_tempDirectory))
                        Directory.CreateDirectory(_tempDirectory);

                    Directory.CreateDirectory(_DownloadDirectory);
                }

                return _DownloadDirectory;
            }
            private set
            {
                _DownloadDirectory = value;
            }
        }

        public virtual string RegexExpressionVersion { get; } = PluginVersion.DefaultsRegexVersion;
        #endregion

        #region Constructor & Destructor
        public AbstractUpdateHandler()
        {
            TempDirectory = Path.Combine(Server.Get.Files.SynapseDirectory, "temp");
            DownloadDirectory = Path.Combine(_tempDirectory, "download");
            OldDllDirectory = Path.Combine(_tempDirectory, "old");
            DeletetTempDirectory();
        }
        #endregion

        #region Methods
        public void DeletetTempDirectory()
        {
            if (Directory.Exists(_tempDirectory) && FirstLoad)
            {
                DeleteDirectory(_tempDirectory);
                FirstLoad = false;
            }
        }

        private void DeleteDirectory(string target_dir)
        {
            var files = Directory.GetFiles(target_dir);
            var dirs = Directory.GetDirectories(target_dir);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
                DeleteDirectory(dir);
            
            Directory.Delete(target_dir, false);
        }

        public virtual bool TryDownload(HttpClient client, Release release, string name, out string filePath)
        {
            var asset = release.Assets.FirstOrDefault(r => r.Name.Contains(name) && r.Name.Contains(".dll"));

            if (asset.Size == 0)
            {
                filePath = string.Empty;
                return false;
            }

            using var reponse = client.GetAsync(asset.BrowserDownloadUrl).ConfigureAwait(false).GetAwaiter().GetResult();
            using var stream = reponse.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            filePath = Path.Combine(DownloadDirectory, asset.Name);
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            stream.CopyToAsync(fileStream).ConfigureAwait(false).GetAwaiter().GetResult();

            return true;
        }

        public virtual PluginVersion GetPluginVersion<T>()
        {
            var info = (PluginInformation)Attribute.GetCustomAttribute(typeof(T), typeof(PluginInformation));
            if (info.Version == Unknow)
            {
                if (info.Name == Unknow)
                    throw new VtUnknownVersionException($"Vt-AutoUppdate : The plugin in the assembly {typeof(T).Assembly.GetName().Name} did not set its version", typeof(T).Assembly.FullName);
                else
                    throw new VtUnknownVersionException($"Vt-AutoUppdate : The plugin {info.Name} in the assembly {typeof(T).Assembly.GetName().Name} did not set its version", typeof(T).Assembly.FullName, info.Name);
            }
            return new PluginVersion(info.Version, RegexExpressionVersion);
        }

        public virtual PluginVersion GetGitVersion(HttpClient client, string link, out Release release, bool prerealase = false)
        {
            var realases = GetRealases(client, link);

            PluginVersion highestVersion = new PluginVersion(0,0,0);
            Release highestRelease = null;
            foreach (var realase in realases)
            {
                if (!prerealase && realase.PreRelease)
                    continue;
                if (PluginVersion.TryParse(realase.TagName, out var version) && version > highestVersion)
                {
                    highestVersion = version;
                    highestRelease = realase;
                }
            }
            release = highestRelease;
            return highestVersion;
        } 

        private List<Release> GetRealases(HttpClient client, string link)
        {
            client.Timeout = TimeSpan.FromSeconds(500);
            client.DefaultRequestHeaders.Add("User-Agent", $"VT-API");

            var url = link;
            using var response = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
            using var stream = response.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            
            return JsonSerializer.Deserialize<Release[]>(stream).OrderByDescending(r => r.CreatedAt.Ticks).ToList();
        }

        public virtual bool NeedToUpdate(PluginVersion PluginVersion, PluginVersion GitVersion)
            => PluginVersion < GitVersion;
            
        public virtual void Replace(string newPluginPath, string pluinName, string pluginDirectory)
        {

            var pluginName = pluinName + ".dll";
            var pluginPath = Path.Combine(pluginDirectory, pluginName);
            var pluginNewPath = Path.Combine(OldDllDirectory, pluginName);

            var newPluginName = Path.GetFileName(pluginPath);
            var newPluginNewPath = Path.Combine(pluginDirectory, newPluginName);

            File.Move(pluginPath, pluginNewPath);
            File.Move(newPluginPath, newPluginNewPath);
        }
        #endregion
    }
}
