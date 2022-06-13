using HarmonyLib;
using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Events;
using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VT_Api.Reflexion;

namespace VT_Api.Patches.VtEvent.PlayerPatches
{
    [HarmonyPatch(typeof(PlayerEvents), "InvokePlayerDamageEvent")]
    class SynapseDamagePatch
    {
        [HarmonyPrefix]
        private static bool DamageEventPatch(PlayerEvents __instance, Player victim, Player killer, ref float damage, DamageType type, out bool allow)
        {
            var ev = new PlayerDamageEventArgs
            {
                Damage = damage,
            };

            try
            {
                ev.SetProperty<Player>("Killer", killer);
                ev.SetProperty<Player>("Victim", victim);
                ev.SetProperty<float>("Damage", damage);
                ev.SetProperty<DamageType>("DamageType", type);

                __instance.CallEvent("PlayerDamageEvent", ev);
                
                allow = ev.Allow;
                damage = ev.Damage;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Synapse-Event: PlayerDamage event failed!!\n{e}");
                allow = ev.Allow;
                return false;
            }
            try
            {
                VtController.Get.Events.Player.InvokePlayerDamagePostEvent(victim, killer, ref damage, type, ref allow);

                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: PlayerDamagePost failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return false;
            }
        }

    }
}
