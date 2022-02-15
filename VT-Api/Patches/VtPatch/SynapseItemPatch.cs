using HarmonyLib;
using Synapse.Api.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Patches.VtPatch
{
    [HarmonyPatch(typeof(SynapseItem), MethodType.Constructor, new Type[] { })]
    class SynapseItemPatch
    {
        [HarmonyPostfix]
        private static void AddCustomItemScript(SynapseItem __instance)
        {
            var script = VtController.Get.Item.GetNewScript(__instance.ID);
            __instance.ItemData.Add(Core.Items.ItemManager.KeySynapseItemData, script);
        }

    }
}
