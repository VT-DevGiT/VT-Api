﻿using Synapse.Api;
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
            if (!(context.PluginType is IVtPlugin vtPlugin) || !vtPlugin.AutoRegister) return;

            foreach (var itemType in context.Classes)
            {
                try
                {
                    if (!typeof(IItem).IsAssignableFrom(itemType) ||
                        itemType.GetCustomAttribute<AutoRegisterManager.Ignore>() != null)
                        continue;
                }
                catch (Exception e)
                {
                    Logger.Get.Error($"Error auto register item {itemType.Name} from {context.Information.Name}\n{e}");
                }
            }
        }
    }
}