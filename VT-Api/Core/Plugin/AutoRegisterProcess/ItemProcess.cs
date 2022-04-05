using Synapse.Api;
using Synapse.Api.Plugin;
using System;
using System.Reflection;
using VT_Api.Core.Items;
using VT_Api.Extension;

namespace VT_Api.Core.Plugin.AutoRegisterProcess
{
    internal class ItemProcess : IContextProcessor
    {
        public void Process(PluginLoadContext context)
        {
            if (context.Plugin is not IVtPlugin vtPlugin || !vtPlugin.AutoRegister) return;

            foreach (var itemType in context.Classes)
            {
                if (!typeof(IItem).IsAssignableFrom(itemType) || itemType.GetCustomAttribute<AutoRegisterManager.Ignore>() != null)
                    continue;
                
                //var info = itemType.GetCustomAttribute<VtItemInformation>();

                try
                {
                    var info = itemType.GetCustomAttribute<VtItemInformation>();
                    if (info == null)
                    {
                        var customItem = Activator.CreateInstance(itemType) as IItem;
                        if (customItem.Info == null)
                            Logger.Get.Error($"The custom Item {itemType.Name} ave no information !");
                        VtController.Get.Item.RegisterCustomItem(customItem);
                    }
                    else
                    {
                        VtController.Get.Item.RegisterCustomItem(itemType, info.ID, info.Name, info.BasedItemType);
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
