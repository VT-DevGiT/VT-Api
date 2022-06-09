using Synapse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using VT_Api.Core.Plugin.Updater;

namespace VT_Api.Core
{
    internal class Updater : AbstractUpdateHandler, IAutoUpdate
    {
        public const long GithubID = 442298975;

        public bool Update()
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(500);
            client.DefaultRequestHeaders.Add("User-Agent", $"VT-API");

            var githubVerison = GetGitVersion(client, string.Format(GitHubPage, GithubID), out var release);
            var curentVersion = VtVersion.GetVersion();
            var assmeblyName = this.GetType().Assembly.GetName().Name;

            if (!NeedToUpdate(curentVersion, githubVerison))
                return false;

            if (!TryDownload(client, release, assmeblyName, out var filePath))
                return false;

            var depedencyPath = Path.Combine(Server.Get.Files.SynapseDirectory, "dependencies");

            base.Replace(filePath, assmeblyName, depedencyPath);

            return true;
        }
    }
}
