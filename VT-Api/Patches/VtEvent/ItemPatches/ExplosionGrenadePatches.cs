using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Synapse.Api.Enum;
using System;

namespace VT_Api.Patches.VtEvent.ItemPatches 
{
    [HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.PlayExplosionEffects))]
    class FragExplosionGrenadePatch
    {
        internal static TimeGrenade grenade;

        [HarmonyPrefix]
        private static bool GrenadeExplosionPatch(ExplosionGrenade __instance)
        {
            try
            {
                var allow = true;
                var type = (GrenadeType)__instance.Info.ItemId;

                VtController.Get.Events.Item.InvokeExplosionGrenadeEvent(__instance, type, ref allow);

                if (allow) 
                    grenade = __instance;
                return allow;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: GrenadeFragExplosion failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PlayExplosionEffects))]
    class FlashExplosionGrenadePatch
    {
        [HarmonyPrefix]
        private static bool FlashExplosionPatch(FlashbangGrenade __instance)
        {
            try
            {
                var allow = true;
                var type = (GrenadeType)__instance.Info.ItemId;

                VtController.Get.Events.Item.InvokeExplosionGrenadeEvent(__instance, type, ref allow);
                return allow;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: GrenadeFlashExplosion failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }

}
