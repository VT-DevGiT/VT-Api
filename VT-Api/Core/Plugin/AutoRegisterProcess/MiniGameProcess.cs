using Synapse.Api;
using Synapse.Api.Plugin;
using System;
using System.Reflection;
using VT_Api.Core.MiniGame;

namespace VT_Api.Core.Plugin.AutoRegisterProcess
{
    internal class MiniGameProcess : IContextProcessor
    {
        public void Process(PluginLoadContext context)
        {
            if (!(context.PluginType is IVtPlugin vtPlugin) || !vtPlugin.AutoRegister) return;

            foreach (var miniGameType in context.Classes)
            {
                try
                {
                    if (!typeof(IMiniGame).IsAssignableFrom(miniGameType) ||
                        miniGameType.GetCustomAttribute<AutoRegisterManager.Ignore>() != null)
                        continue;

                    var classObject = (IMiniGame)Activator.CreateInstance(miniGameType);

                    var info = new MiniGameInformation(classObject.GetMiniGameName(), classObject.GetMiniGameID(), miniGameType);

                    VtController.Get.MinGames.RegisterMiniGame(info);

                }
                catch (Exception e)
                {
                    Logger.Get.Error($"Error auto register minigame {miniGameType.Name} from {context.Information.Name}\n{e}");
                }
            }
        }
    }
}
