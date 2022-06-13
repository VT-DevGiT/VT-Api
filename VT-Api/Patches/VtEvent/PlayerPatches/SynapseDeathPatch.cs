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
    [HarmonyPatch(typeof(PlayerEvents), "InvokePlayerDeathEvent")]
    class SynapseDeathPatch
    {
        [HarmonyPrefix]
        private static bool DeathEventPatch(PlayerEvents __instance, Player victim, Player killer, DamageType type, out bool allow)
        {
            var ev = new PlayerDeathEventArgs();
            try
            {

                ev.Allow = true;
                ev.SetProperty<Player>("Killer", killer);
                ev.SetProperty<Player>("Victim", victim);
                ev.SetProperty<DamageType>("DamageType", type);

                __instance.CallEvent("PlayerDeathEvent", ev);

                allow = ev.Allow;
            }
            catch (Exception e)
            {
                allow = ev.Allow;
                Logger.Get.Error($"Synapse-Event: PlayerDeath event failed!!\n{e}");
                return false;
            }
            try
            {
                VtController.Get.Events.Player.InvokePlayerDeathPostEvent(victim, killer, type, ref allow);

                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: PlayerDamagePost failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }

    }
}
