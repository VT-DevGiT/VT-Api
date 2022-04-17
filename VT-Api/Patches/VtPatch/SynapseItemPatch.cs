using HarmonyLib;
using MEC;
using Synapse.Api;
using Synapse.Api.Items;
using System;
using System.Collections.Generic;
using VT_Api.Extension;

namespace VT_Api.Patches.VtPatch
{
    [HarmonyPatch(typeof(SynapseItem), MethodType.Constructor, new Type[] { })]
    class SynapseItemPatch
    {
        [HarmonyPostfix]
        private static void AddCustomItemScript(SynapseItem __instance)
        {
            Timing.RunCoroutine(SetItemScript(__instance));
        }

        public static IEnumerator<float> SetItemScript(SynapseItem item)
        {
            yield return 0f;
            try
            {
                var script = VtController.Get.Item.GetNewScript(item.ID);
                if (script != null)
                {
                    script.Item = item;
                    item.ItemData.Add(Core.Items.ItemManager.KeySynapseItemData, script);
                    script.Init();
                }
            }
            catch (Exception e)
            {
                Logger.Get.Error($"Vt-Patch: AddCustomItemScript failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
            }
        }
    }
}
