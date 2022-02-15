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
            if (!(context.Plugin is IVtPlugin vtPlugin) || !vtPlugin.AutoRegister) return;

            foreach (var miniGameType in context.Classes)
            {
                if (!typeof(IMiniGame).IsAssignableFrom(miniGameType) || miniGameType.GetCustomAttribute<AutoRegisterManager.Ignore>() != null)
                        continue;

                try
                {
                    var miniGame = (IMiniGame)Activator.CreateInstance(miniGameType);
                    var info = new MiniGameInformation(miniGame);

                    VT_Api.Core.MiniGame.MiniGameManager.Get.RegisterMiniGame(info);
                }
                catch (Exception e)
                {
                    Logger.Get.Error($"Error auto register minigame {miniGameType.Name} from {context.Plugin.Information.Name}\n{e}");
                }

                //VtController.Get.MinGames.AwaitingFinalization.Add(miniGameType);
            }
        }
    }
}
