using HarmonyLib;
using System;

namespace VT_Api.Patches.VtEvent.PlayerPatches
{
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    class DestroyHubPatch
    {
        [HarmonyPrefix]
        private static void HubDestroyPatch(ReferenceHub __instance)
        {
            try
            {
                var player = __instance.GetPlayer();

                if (player == null)
                    return;

                VtController.Get.Events.Player.InvokePlayerDestroyEvent(player);
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: DestroyHub failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
            }
        }
    }
}
