using Synapse.Api;
using Synapse.Api.Plugin;
using Synapse.Api.Roles;
using System;
using System.Reflection;

namespace VT_Api.Core.Plugin.AutoRegisterProcess
{
    internal class RoleProcess : IContextProcessor
    {
        public void Process(PluginLoadContext context)
        {
            if (!(context.PluginType is IVtPlugin vtPlugin) || !vtPlugin.AutoRegister) return;

            foreach (var roleType in context.Classes)
            {
                try
                {
                    if (!(typeof(IRole).IsAssignableFrom(roleType) || 
                        roleType.GetCustomAttribute<AutoRegisterManager.Ignore>() != null))
                        continue;

                    var classObject = (IRole)Activator.CreateInstance(roleType);

                    var info = new RoleInformation(classObject.GetRoleName(), classObject.GetRoleID(), roleType);

                    RoleManager.Get.RegisterCustomRole(info);

                }
                catch (Exception e)
                {
                    Logger.Get.Error($"Error auto register role {roleType.Name} from {context.Information.Name}\n{e}");
                }
            }
        }
    }
}
