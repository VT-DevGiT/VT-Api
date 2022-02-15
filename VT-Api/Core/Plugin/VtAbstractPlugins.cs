using Synapse.Api;
using Synapse.Api.Plugin;
using Synapse.Config;
using Synapse.Translation;
using System;
using System.IO;
using VT_Api.Extension;

namespace VT_Api.Core.Plugin
{
    public abstract class VtAbstractPlugin<TEventHandler, TConfig, TTranslation> : IPlugin, IVtPlugin
        where TEventHandler : new()
        where TConfig : IConfigSection
        where TTranslation : IPluginTranslation, new()
    {
        #region Properties & Variable

        public abstract bool AutoRegister { get; }

        public static VtAbstractPlugin<TEventHandler, TConfig, TTranslation> Instance { get; protected set; }

        [Synapse.Api.Plugin.SynapseTranslation]
        public SynapseTranslation<TTranslation> Translation { get; set; }

        [Synapse.Api.Plugin.Config]
        public virtual TConfig Config { get; protected set; }

        public TEventHandler EventHandler { get; protected set; }

        public PluginInformation Information { get; set; }

        [Obsolete("This is the old Translation Systemn, use Translation", true)]
        Translation IPlugin.Translation { get; set; }

        private string _pluginDirectory;
        public string PluginDirectory
        {
            get
            {
                if (_pluginDirectory == null)
                    return null;

                if (!Directory.Exists(_pluginDirectory))
                    Directory.CreateDirectory(_pluginDirectory);

                return _pluginDirectory;
            }
            set => _pluginDirectory = value;
        }
        #endregion

        #region Constructor & Destructor
        public VtAbstractPlugin()
        {
            AppDomain.CurrentDomain.ProcessExit += Unload;
            VtController.InitApi();
            Instance = this;
        }
        #endregion

        #region Methods
        public virtual void Load()
        {
            EventHandler = new TEventHandler();
            Translation.AddTranslation(new TTranslation());

            Logger.Get.Info(Information.Name + " by " + Information.Author + " has loaded!");
        }

        public virtual void ReloadConfigs()
        {
            if (Config is AbstractConfigSection SynapseConfig)
                SynapseConfig.Update();
        }

        public virtual void Unload(object sender, EventArgs e)
        {
            Logger.Get.Info(Information.Name + " by " + Information.Author + " has unloaded!");
        }
        #endregion
    }

    public abstract class VtAbstractPlugin<TEventHandler, TConfig> : IPlugin, IVtPlugin
    where TEventHandler : new()
    where TConfig : IConfigSection
    {
        #region Properties & Variable

        public abstract bool AutoRegister { get; }

        public static VtAbstractPlugin<TEventHandler, TConfig> Instance { get; protected set; }

        [Synapse.Api.Plugin.Config]
        public virtual TConfig Config { get; protected set; }

        public TEventHandler EventHandler { get; protected set; }

        public PluginInformation Information { get; set; }

        [Obsolete("This is the old Translation Systemn, use Translation", true)]
        Translation IPlugin.Translation { get; set; }

        private string _pluginDirectory;
        public string PluginDirectory
        {
            get
            {
                if (_pluginDirectory == null)
                    return null;

                if (!Directory.Exists(_pluginDirectory))
                    Directory.CreateDirectory(_pluginDirectory);

                return _pluginDirectory;
            }
            set => _pluginDirectory = value;
        }
        #endregion

        #region Constructor & Destructor
        public VtAbstractPlugin()
        {
            AppDomain.CurrentDomain.ProcessExit += Unload;
            VtController.InitApi();
            Instance = this;
        }
        #endregion

        #region Methods
        public virtual void Load()
        {
            EventHandler = new TEventHandler();

            Logger.Get.Info(Information.Name + " by " + Information.Author + " has loaded!");
        }

        public virtual void ReloadConfigs()
        {
            if (Config is AbstractConfigSection SynapseConfig)
                SynapseConfig.Update();
        }

        public virtual void Unload(object sender, EventArgs e)
        {
            Logger.Get.Info(Information.Name + " by " + Information.Author + " has unloaded!");
        }
        #endregion
    }

    public abstract class VtAbstractPlugin<TEventHandler> : IPlugin, IVtPlugin
    where TEventHandler : new()
    {
        #region Properties & Variable

        public abstract bool AutoRegister { get; }

        public static VtAbstractPlugin<TEventHandler> Instance { get; protected set; }

        public TEventHandler EventHandler { get; protected set; }

        public PluginInformation Information { get; set; }

        [Obsolete("This is the old Translation Systemn, use Translation", true)]
        Translation IPlugin.Translation { get; set; }

        private string _pluginDirectory;
        public string PluginDirectory
        {
            get
            {
                if (_pluginDirectory == null)
                    return null;

                if (!Directory.Exists(_pluginDirectory))
                    Directory.CreateDirectory(_pluginDirectory);

                return _pluginDirectory;
            }
            set => _pluginDirectory = value;
        }
        #endregion

        #region Constructor & Destructor
        public VtAbstractPlugin()
        {
            AppDomain.CurrentDomain.ProcessExit += Unload;
            VtController.InitApi();
            Instance = this;
        }
        #endregion

        #region Methods
        public virtual void Load()
        {
            EventHandler = new TEventHandler();

            Logger.Get.Info(Information.Name + " by " + Information.Author + " has loaded!");
        }

        public virtual void ReloadConfigs()
        {

        }

        public virtual void Unload(object sender, EventArgs e)
        {
            Logger.Get.Info(Information.Name + " by " + Information.Author + " has unloaded!");
        }
        #endregion
    }

}
