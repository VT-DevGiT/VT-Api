using Synapse.Api;
using Synapse.Api.Plugin;
using Synapse.Config;
using Synapse.Translation;
using System;
using System.IO;
using VT_Api.Extension;

namespace VT_Api.Core.Plugin
{

    /// <typeparam name="TPlugin">The inheritance class</typeparam>
    public abstract class VtAbstractPlugin<TPlugin, TEventHandler, TConfig, TTranslation> : IPlugin, IVtPlugin
        where TEventHandler : new()
        where TConfig : IConfigSection
        where TTranslation : IPluginTranslation, new()
        where TPlugin : VtAbstractPlugin<TPlugin, TEventHandler, TConfig, TTranslation>
    {
        #region Properties & Variable
        public abstract bool AutoRegister { get; }
        public static TPlugin Instance { get; protected set; }


        [Synapse.Api.Plugin.SynapseTranslation]
        public SynapseTranslation<TTranslation> Translation { get; set; }


        [Synapse.Api.Plugin.Config]
        public virtual TConfig Config { get; protected set; }
        public TEventHandler EventHandler { get; protected set; }
        public PluginInformation Information { get; set; }

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


        [Obsolete("This is the old Translation system, use Translation", true)]
        Translation IPlugin.Translation { get; set; }
        #endregion

        #region Constructor & Destructor
        public VtAbstractPlugin()
        {
            VtController.InitApi();
            VtController.Get.Events.Server.ServerStopEvent += Unload;

            Instance = (TPlugin)this;
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

        public virtual void Unload()
        {
            Logger.Get.Info(Information.Name + " by " + Information.Author + " has unloaded!");
        }
        #endregion
    }

    public abstract class VtAbstractPlugin<TPlugin, TEventHandler, TConfig> : IPlugin, IVtPlugin
    where TEventHandler : new()
    where TConfig : IConfigSection
    where TPlugin : VtAbstractPlugin<TPlugin, TEventHandler, TConfig>
    {
        #region Properties & Variable
        public abstract bool AutoRegister { get; }
        public static TPlugin Instance { get; protected set; }


        [Synapse.Api.Plugin.Config]
        public virtual TConfig Config { get; protected set; }
        public TEventHandler EventHandler { get; protected set; }
        public PluginInformation Information { get; set; }

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


        [Obsolete("This is the old Translation Systemn, use Translation", true)]
        Translation IPlugin.Translation { get; set; }
        #endregion

        #region Constructor & Destructor
        public VtAbstractPlugin()
        {
            VtController.InitApi();
            VtController.Get.Events.Server.ServerStopEvent += Unload;

            if (this is not TPlugin)
                throw new Exception($"{this.GetType().Name} is not the TPlugin ! Plis fix this. Check the doc.");
            Instance = (TPlugin)this;
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

        public virtual void Unload()
        {
            Logger.Get.Info(Information.Name + " by " + Information.Author + " has unloaded!");
        }
        #endregion
    }

    public abstract class VtAbstractPlugin<TPlugin, TEventHandler> : IPlugin, IVtPlugin
    where TEventHandler : new()
    where TPlugin : VtAbstractPlugin<TPlugin, TEventHandler>
    {
        #region Properties & Variable
        public abstract bool AutoRegister { get; }
        public static TPlugin Instance { get; protected set; }


        public TEventHandler EventHandler { get; protected set; }
        public PluginInformation Information { get; set; }


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


        [Obsolete("This is the old Translation Systemn, use Translation", true)]
        Translation IPlugin.Translation { get; set; }
        #endregion

        #region Constructor & Destructor
        public VtAbstractPlugin()
        {
            VtController.InitApi();
            VtController.Get.Events.Server.ServerStopEvent += Unload;

            if (this is not TPlugin)
                throw new Exception($"{this.GetType().Name} is not the TPlugin ! Plis fix this. Check the doc.");
            Instance = (TPlugin)this;
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

        public virtual void Unload()
        {
            Logger.Get.Info(Information.Name + " by " + Information.Author + " has unloaded!");
        }
        #endregion
    }
}
