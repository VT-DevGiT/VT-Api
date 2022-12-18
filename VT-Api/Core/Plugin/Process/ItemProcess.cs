using Synapse.Api;
using Synapse.Api.Plugin;
using System;
using System.Reflection;
using VT_Api.Core.Items;

namespace VT_Api.Core.Plugin.AutoRegisterProcess
{
    internal class ItemProcess : IContextProcessor
    {
        public void Process(PluginLoadContext context)
        {
            if (context.Plugin is not IVtPlugin vtPlugin || !vtPlugin.AutoRegister) return;
            Logger.Get.Warn($"Try auto register Item for {context.Plugin.Information.Name}");

            foreach (var itemType in context.Classes)
            {
                if (!typeof(IItem).IsAssignableFrom(itemType) || itemType.GetCustomAttribute<AutoRegisterManager.Ignore>(false) != null)
                    continue;
                
                try
                {
                    var info = itemType.GetCustomAttribute<VtItemInformation>();
                    if (info == null)
                    {
                        var customItem = Activator.CreateInstance(itemType) as IItem;
                        if (customItem.Info == null)
                        {
                            Logger.Get.Error($"The custom Item {itemType.Name} ave no information !");
                            continue;
                        }
                        Logger.Get.Warn($"The custom Item {info.Name} ({info.ID}) is auto regitred !"); 
                        VtController.Get.Item.RegisterCustomItem(customItem);
                    }
                    else
                    {
                        VtController.Get.Item.RegisterCustomItem(itemType, info.ID, info.Name, info.BasedItemType);
                        Logger.Get.Error($"The custom Item {info.Name} ({info.ID}) is auto regitred ");
                    } 
                }
                catch (Exception e)
                {
                    Logger.Get.Error($"Error auto register item {itemType.Name} from {context.Plugin.Information.Name}\n{e}");
                }

                //VtController.Get.Item.AwaitingFinalization.Add(itemType, info);
            }
        }
    }
}
