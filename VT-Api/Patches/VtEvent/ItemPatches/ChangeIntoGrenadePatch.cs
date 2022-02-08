using Footprinting;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Synapse.Api.Enum;
using System;
using UnityEngine;

namespace VT_Api.Patches.VtEvent.ItemPatches
{
    [HarmonyPatch(typeof(TimedGrenadePickup), nameof(TimedGrenadePickup.OnExplosionDetected))]
    class ChangeIntoFragPatch
    {
        [HarmonyPrefix]
        private static bool ExplosionDetectedPatch(TimedGrenadePickup __instance, Footprint attacker, Vector3 source, float range)
        {
            try
            {
                if (Vector3.Distance(__instance.transform.position, source) / range > TimedGrenadePickup.ActivationRange)
                    return false;

                var allow = true;
                var item = __instance.GetSynapseItem();
                var type = (GrenadeType)__instance.Info.ItemId;

                VtController.Get.Events.Item.InvokeChangeIntoFragEvent(item, FragExplosionGrenadePatch.grenade, type, ref allow);

                return allow;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: ChangeIntoGrenade failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
