using Synapse.Api;
using Synapse.Api.Plugin;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace VT_Api.Core.Plugin.AutoRegisterProcess
{
    internal class DebugCheckProcess : IContextProcessor
    {
        public void Process(PluginLoadContext context)
        {
            if (!VtVersion.Debug && IsAssemblyDebugBuild(context.PluginType.Assembly))
                Logger.Get.Send($"The plugin {context.Plugin.Information.Name} is in DEBUG ! The plugin is for test and not game", System.ConsoleColor.DarkYellow);
        }

        public bool IsAssemblyDebugBuild(Assembly assembly)
        {
            return assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled);
        }
    }
}
