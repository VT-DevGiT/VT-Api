using Synapse.Api;
using Synapse.Api.Plugin;
using Synapse.Api.Roles;
using System;
using System.Reflection;
using VT_Api.Extension;

namespace VT_Api.Core.Plugin.AutoRegisterProcess
{
    internal class RoleProcess : IContextProcessor
    {
        public void Process(PluginLoadContext context)
        {
            if (context.Plugin is not IVtPlugin vtPlugin || !vtPlugin.AutoRegister) return;

            foreach (var roleType in context.Classes)
            {

                if (!typeof(IRole).IsAssignableFrom(roleType) || roleType.GetCustomAttribute<AutoRegisterManager.Ignore>(false) != null)
                        continue;

                try
                {
                    var customRole = Activator.CreateInstance(roleType) as IRole;
                    var info = new RoleInformation(customRole.GetRoleName(), customRole.GetRoleID(), roleType);

                    Synapse.Api.Roles.RoleManager.Get.RegisterCustomRole(info);
                }
                catch (Exception e)
                {
                    Synapse.Api.Logger.Get.Error($"Error auto register role {roleType.Name} from {context.Plugin.Information.Name}\n{e}");
                }

                //VtController.Get.Role.AwaitingFinalization.Add(roleType);
            }
        }
    }
}
