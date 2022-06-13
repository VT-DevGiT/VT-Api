using Achievements;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using Subtitles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils.Networking;

namespace VT_Api.Patches.VtEvent.MapPaches
{
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CancelDetonation), new Type[] { typeof(GameObject) })]
    class StoppingWarHeadPatch
    {
        [HarmonyPrefix]
        private static bool NukeCancelButon(AlphaWarheadController __instance, GameObject disabler)
        {
            try
            {
                ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Detonation cancelled.", ServerLogs.ServerLogType.GameEvent);

                var flag = __instance.inProgress && __instance.timeToDetonation > 10f && !__instance._isLocked;

                VtController.Get.Events.Map.InvokeWarheadStopEvent(disabler?.GetPlayer(), ref flag);
                
                if (!flag)
                    return false;

                if (__instance.timeToDetonation <= 15f && disabler != null)
                    AchievementHandlerBase.ServerAchieve(disabler.GetComponent<NetworkIdentity>().connectionToClient, AchievementName.ThatWasClose);

                for (sbyte b = 0; b < __instance.scenarios_resume.Length; b = (sbyte)(b + 1))
                    if (__instance.scenarios_resume[b].SumTime() > __instance.timeToDetonation && __instance.scenarios_resume[b].SumTime() < __instance.scenarios_start[AlphaWarheadController._startScenario].SumTime())
                        __instance.NetworksyncResumeScenario = b;

                __instance.NetworktimeToDetonation = ((AlphaWarheadController._resumeScenario < 0) ? __instance.scenarios_start[AlphaWarheadController._startScenario].SumTime() : __instance.scenarios_resume[AlphaWarheadController._resumeScenario].SumTime()) + (float)__instance.cooldown;
                __instance.NetworkinProgress = false;
                DoorEventOpenerExtension.TriggerAction(DoorEventOpenerExtension.OpenerEventType.WarheadCancel);
                if (NetworkServer.active)
                {
                    __instance._autoDetonate = false;
                    NetworkUtils.SendToAuthenticated(new SubtitleMessage(new SubtitlePart[1]
                    {
                        new SubtitlePart(SubtitleType.AlphaWarheadCancelled, null)
                    }));
                }
                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: CancelDetonation failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
