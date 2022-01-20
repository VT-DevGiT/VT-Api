using HarmonyLib;
using InventorySystem.Items.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Patches.VtEvent.ItemPatches
{
    [HarmonyPatch(typeof(CollisionDetectionPickup), nameof(CollisionDetectionPickup.ProcessCollision))]
    class CollisionPatch
    {
        [HarmonyPrefix]
        private static bool CollisionDetectioPatch(CollisionDetectionPickup __instance, Collision collision)
        {
            try
            {
                var allow = true;
                var item = __instance.GetSynapseItem();

                VtController.Get.Events.Item.InvokeCollisionEvent(item, ref allow);

                if (!allow)
                    return false;

                var sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
                var num = __instance.Info.Weight * sqrMagnitude / 2f;
                if (num > 30f)
                {
                    float damage = num * 0.25f;
                    if (collision.collider.TryGetComponent(out BreakableWindow component))
                    {
                        component.Damage(damage, null, Vector3.zero);
                    }
                }

                __instance.MakeCollisionSound(sqrMagnitude);

                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: GrenadeCollisionEnter failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
