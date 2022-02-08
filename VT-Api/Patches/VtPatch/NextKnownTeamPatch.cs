using HarmonyLib;
using Respawning;
using Synapse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Respawning.RespawnManager;

namespace VT_Api.Patches.VtPatch
{
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Update))]
    class NextKnownTeamPatch
    {
        [HarmonyPrefix]
        private static bool SelectNextKnownTeam(RespawnManager __instance) //todo
        {
            if (true) return true;

            try
            {
                if (__instance._stopwatch.Elapsed.TotalSeconds > __instance._timeForNextSequence)
                    ++__instance._curSequence;

                if (__instance.NextKnownTeam == SpawnableTeamType.None)
                    __instance.NextKnownTeam = RespawnTickets.Singleton.DrawRandomTeam();

                if (__instance._curSequence == RespawnSequencePhase.SelectingTeam)
                {
                    if (!Server.Get.Players.Where(p => p.RoleType == RoleType.Spectator && !p.OverWatch).Any())
                    {
                        __instance.RestartSequence();
                        return false;
                    }

                    if (!RespawnWaveGenerator.SpawnableTeams.TryGetValue(__instance.NextKnownTeam, out SpawnableTeamHandlerBase value))
                    {
                        ServerConsole.AddLog(string.Concat("Fatal error, unable to find the '", __instance.NextKnownTeam, "' team"), ConsoleColor.Red);
                        __instance.RestartSequence();
                        return false;
                    }

                    __instance._curSequence = RespawnSequencePhase.PlayingEntryAnimations;
                    __instance._stopwatch.Restart();
                    __instance._timeForNextSequence = value.EffectTime;
                    RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, __instance.NextKnownTeam);
                }

                if (__instance._curSequence == RespawnSequencePhase.SpawningSelectedTeam)
                {
                    __instance.Spawn();
                    __instance.RestartSequence();
                }

                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Patch: NextKnownTeamPatch failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }

    }
}
