using Synapse.Api.Plugin;
using System;
using System.Reflection;
using VT_Api.Core.Plugin.Updater;

namespace VT_Api.Core.Plugin.AutoRegisterProcess
{
    internal class AutoUpdatePorecess : IContextProcessor
    {
        public void Process(PluginLoadContext context)
        {
            if (!VtController.Get.Configs.VtConfiguration.AutoUpdate)
                return;

            foreach (Type autoUpdate in context.Classes)
            {
                try
                {
                    if (!typeof(IAutoUpdate).IsAssignableFrom(autoUpdate) || autoUpdate.GetCustomAttribute<AutoRegisterManager.Ignore>(false) != null)
                        continue;

                    var autoUpdater = (IAutoUpdate)Activator.CreateInstance(autoUpdate);

                    var isUpdate = autoUpdater.Update();

                    if (isUpdate)
                        Synapse.Api.Logger.Get.Warn($"Plugin {context.Plugin.Information.Name} is now Update ! You need to restart the server for use the new version");
                }
                catch (Exception e)
                {
                    Synapse.Api.Logger.Get.Error($"Error auto updating plugin {autoUpdate.Name} from {context.Information.Name}\n{e}");
                }
            }
        }
    }
}
