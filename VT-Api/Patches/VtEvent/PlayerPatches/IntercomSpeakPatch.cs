using HarmonyLib;
using Synapse;
using System;

namespace VT_Api.Patches.VtEvent.PlayerPatches
{
    [HarmonyPatch(typeof(Intercom), nameof(Intercom.CmdSetTransmit))]

    class IntercomSpeakPatch
    {
        [HarmonyPrefix]
        private static bool IntercomStartTransmitPatch(Intercom __instance, bool player)
        {
            try
            {
                if (!__instance._interactRateLimit.CanExecute() || Intercom.AdminSpeaking)
                    return false;

                if (player)
                {
                    if (!__instance.ServerAllowToSpeak())
                        return false;

                    var flag = true;
                    var Player = __instance.GetPlayer();

                    VtController.Get.Events.Player.InvokePlayerSpeakIntercomEvent(Player, ref flag);
                    
                    if (flag) 
                        Intercom.host.RequestTransmission(__instance.gameObject);
                }
                else
                {
                    if (!(Intercom.host.Networkspeaker == __instance.gameObject))
                        return false;

                    Intercom.host.RequestTransmission(null);
                }
                return false;
            }
            catch (Exception e)
            {
                Server.Get.Logger.Error($"Vt-Event: IntercomSpeakEvent failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
