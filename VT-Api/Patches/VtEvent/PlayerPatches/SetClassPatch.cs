using HarmonyLib;
using Synapse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CharacterClassManager;

namespace VT_Api.Patches.VtEvent.PlayerPatches
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetClassIDAdv))]
    class SetClassPatch
    {

        [HarmonyPostfix]
        private static void SetClassIDAdvPatch(CharacterClassManager __instance, RoleType id, bool lite, SpawnReason spawnReason, bool isHook = false)
        {
            try
            {
                VtController.Get.Events.Player.InvokePlayerSetClassAdvEvent(__instance.GetPlayer(), id);
            }
            catch (Exception e)
            {
                Server.Get.Logger.Error($"Vt-Event: SetClassAdvEvent failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
            }
        }
        
    }
}
