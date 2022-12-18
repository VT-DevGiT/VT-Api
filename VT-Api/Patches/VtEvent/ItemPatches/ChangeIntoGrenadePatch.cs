using Footprinting;
using HarmonyLib;
using InventorySystem.Items.MicroHID;
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
                if (!(Vector3.Distance(__instance.transform.position, source) / range > TimedGrenadePickup.ActivationRange) && !Physics.Linecast(__instance.gameObject.transform.position, source, MicroHIDItem.WallMask))
                {
                    var allow = true;
                    var item = __instance.GetSynapseItem();
                    var type = (GrenadeType)__instance.Info.ItemId;

                    VtController.Get.Events.Item.InvokeChangeIntoFragEvent(item, FragExplosionGrenadePatch.grenade, type, ref allow);

                    if (!allow)
                        return false;

                    __instance._replaceNextFrame = true;
                    __instance._attacker = (Footprint)attacker;
                }
                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: ChangeIntoGrenade failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
