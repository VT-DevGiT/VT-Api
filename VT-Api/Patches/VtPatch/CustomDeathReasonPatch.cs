using HarmonyLib;
using Mirror;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Patches.VtPatch
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.TargetReceiveSpecificDeathReason))]
    class CustomDeathReasonPatch
    {
        public static string CustomReason { get; set; }
        
        [HarmonyPrefix]
        public static void TargetReceiveSpecificDeathReason(PlayerStats __instance, DamageHandlerBase handler)
        {
            if (CustomReason != null)
            {
                handler = new CustomReasonDamageHandler(CustomReason);
                CustomReason = null;
            }
        }
    }
}
