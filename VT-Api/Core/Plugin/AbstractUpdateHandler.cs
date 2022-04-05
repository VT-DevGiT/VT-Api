using Synapse.Api.Plugin;
using System;
using VT_Api.Exceptions;

namespace VT_Api.Core.Plugin
{
    public abstract class AbstractUpdateHandler<T> : IUpdateHandler
        where T : IPlugin
    {

        const string Unknow = "Unknown";

        public virtual bool TryDownload()
        {
            return false;
        }

        public virtual bool NeedToUpdate(Version PluginVersion, Version GitVersion)
            => PluginVersion < GitVersion;
        

        public virtual void Update()
        {

        }

        public abstract string GitSource { get; }

        public virtual string RegexExpression { get; } = @"[v,V]?(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)";


        private  Version _pluginVersion;
        private bool _versionGet;
        public virtual Version PluginVersion 
        { 
            get
            {
                if (!_versionGet)
                {
                    var info = (PluginInformation)Attribute.GetCustomAttribute(typeof(T), typeof(PluginInformation));
                    if (info.Version == Unknow)
                    {
                        if (info.Name == Unknow)
                            throw new VtUnknownVersionException($"Vt-AutoUppdate : The plugin in the assembly {typeof(T).Assembly.FullName} did not set its version", typeof(T).Assembly.FullName);
                        else
                            throw new VtUnknownVersionException($"Vt-AutoUppdate : The plugin {info.Name} in the assembly {typeof(T).Assembly.FullName} did not set its version", typeof(T).Assembly.FullName, info.Name);
                    }
                    _pluginVersion = new Version(info.Version);
                    _versionGet = true;
                }

                return _pluginVersion;
            }
        }
    }
}
