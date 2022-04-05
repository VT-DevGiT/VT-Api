using Synapse.Api.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Plugin.Updater
{
    public interface IUpdateHandler<T>
         where T : IPlugin
    {
        string RegexExpressionVersion { get; }

        Version GetPluginVersion();
        Version GetGithubVersion(out Release release, bool ignorePrerealase = true);
        bool NeedToUpdate(Version PluginVersion, Version GitVersion);
        bool TryDownload(Release release, string name);
        void Replace();
    }
}
