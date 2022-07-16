using HarmonyLib;
using InventorySystem;
using PlayerStatsSystem;
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
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
    class DeathPatch
    {
        [HarmonyPrefix]
        [HarmonyAfter("synapse.patches")]
        private static bool DeathEventPatch(PlayerStats __instance, DamageHandlerBase handler)
        {
            try
            {
                var victim = __instance.GetPlayer();
                var attacker = handler is AttackerDamageHandler ahandler ? ahandler.Attacker.GetPlayer() : null;
                string reason = null;

                VtController.Get.Events.Player.InvokePlayerDeathReasonEvent(victim, attacker, handler, ref reason);

                Ragdoll.ServerSpawnRagdoll(__instance._hub, handler);

                if (reason != null)
                    handler = new CustomReasonDamageHandler(reason);

                if (handler is AttackerDamageHandler attackerDamageHandler)
                    __instance.TargetReceiveAttackerDeathReason(attackerDamageHandler.Attacker.Nickname, attackerDamageHandler.Attacker.Role);
                else
                    __instance.TargetReceiveSpecificDeathReason(handler);

                __instance._hub.inventory.ServerDropEverything();
                CharacterClassManager characterClassManager = __instance._hub.characterClassManager;
                characterClassManager.SetClassID(RoleType.Spectator, CharacterClassManager.SpawnReason.Died);
                characterClassManager.TargetConsolePrint(characterClassManager.connectionToClient, "You died. Reason: " + handler.ServerLogsText, "yellow");

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
