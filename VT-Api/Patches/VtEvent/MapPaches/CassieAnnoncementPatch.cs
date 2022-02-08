using HarmonyLib;
using Respawning;
using System;

namespace VT_Api.Patches.VtEvent.MapPaches
{
    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.RpcCassieAnnouncement))]
    class CassiePatch
    {
        [HarmonyPrefix]
        private static bool CassieAnnoncementPatch(RespawnEffectsController __instance, string words, bool makeHold, bool makeNoise)
        {
            try
            {
                var allow = true;

                VtController.Get.Events.Map.InvokeCassieAnnouncementEvent(ref words, ref makeHold, ref makeNoise, ref allow);

                return allow;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: CassieAnnouncement failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
