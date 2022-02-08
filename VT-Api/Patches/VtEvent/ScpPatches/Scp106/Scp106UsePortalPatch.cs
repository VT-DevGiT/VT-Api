using HarmonyLib;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Patches.VtEvent.ScpPatches.Scp106
{
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.UserCode_CmdUsePortal))]
    class Scp106UsePortalPatch
    {
        [HarmonyPrefix]
        private static bool Scp106HabilityPatch(Scp106PlayerScript __instance)
        {
            try
            {
                if (!__instance._interactRateLimit.CanExecute() || !__instance._hub.playerMovementSync.Grounded || 
                    !__instance.iAm106 || __instance.portalPosition == Vector3.zero || __instance.goingViaThePortal)
                    return false;

                var allow = true;

                VtController.Get.Events.Scp.Scp106.InvokePortalUseEvent(__instance.GetPlayer(), ref allow);

                if (allow)
                    Timing.RunCoroutine(__instance._DoTeleportAnimation(), Segment.Update);   

                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: UsePortal failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }

    }
}
