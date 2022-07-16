using HarmonyLib;
using Scp914;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Patches.VtEvent.MapPaches
{
    [HarmonyPatch(typeof(Scp914Controller), nameof(Scp914Controller.ServerInteract))]
    class Scp914InteractPatch
    {
        [HarmonyPrefix]
        private static bool Scp914UsePatch(Scp914Controller __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                if (__instance._remainingCooldown > 0.0)
                    return false;
                bool flag = true;
                switch ((Scp914InteractCode)colliderId)
                {
                    case Scp914InteractCode.ChangeMode:
                        VtController.Get.Events.Map.InvokeChange914KnobSettingEvent(ply.GetPlayer(), ref flag);
                        return flag;
                    case Scp914InteractCode.Activate:
                        VtController.Get.Events.Map.InvokeScp914ActivateEvent(ply.GetPlayer(), ref flag);
                        return flag;
                }
                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: Activate914Patch failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
