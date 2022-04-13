using HarmonyLib;
using Hints;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using Synapse.Api.Items;
using System.Collections.Generic;
using UnityEngine;

namespace VT_Api.Patches.VtEvent.ItemPatches
{

    class ItemSearchCompletorLimitsPatch
    {
        [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.ValidateAny))]
        [HarmonyPrefix]
        private bool ValidateAny(ItemSearchCompletor __instance, ref bool __result)
        {
            if (!((SearchCompletor)__instance).ValidateAny())
            {
                __result = false;
                return false;
            }

            var player = __instance.Hub.GetPlayer();

            if (player.Inventory.Items.Count >= 8)
            {
                player.HintDisplay.Show(new TranslationHint(HintTranslations.MaxItemsAlreadyReached, new HintParameter[1]
                {
                    new ByteHintParameter(8)
                }, new HintEffect[1]
                {
                    HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 3)
                }, 2f));
                __result = false;
                return false;
            }

            if (__instance._category != 0)
            {
                int categoryLimit = Mathf.Abs(InventoryLimits.GetCategoryLimit(__instance._category, player.Hub));
                bool allow = __instance.CategoryCount < categoryLimit;

                VtController.Get.Events.Item.InvokeCheckLimitItemEvent(player, ItemSearchCompletorItemSave.Items[__instance], categoryLimit, ref allow);
                if (allow)
                {
                    player.HintDisplay.Show(new TranslationHint(HintTranslations.MaxItemCategoryAlreadyReached, new HintParameter[2]
                    {
                        new ItemCategoryHintParameter(__instance._category),
                        new ByteHintParameter((byte)categoryLimit)
                    }, new HintEffect[1]
                    {
                        HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 2)
                    }, 2f));
                    __result = false;
                    return false;
                }
            }

            __result = true;
            return false;
        }

        [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.CheckCategoryLimitHint))]
        [HarmonyPrefix]
        private void CheckCategoryLimitHint(ItemSearchCompletor __instance, ref bool __result)
        {
            var categoryLimit = InventoryLimits.GetCategoryLimit(__instance._category, __instance.Hub);

            var player = __instance.Hub.GetPlayer();
            var allow = __instance._category == 0 || categoryLimit < 0 || __instance.CategoryCount < categoryLimit;

            VtController.Get.Events.Item.InvokeCheckLimitItemEvent(player, ItemSearchCompletorItemSave.Items[__instance], categoryLimit, ref allow);

            if (__instance._category != 0 && categoryLimit >= 0 && __instance.CategoryCount >= categoryLimit)
            {
                HintEffect[] effects = HintEffectPresets.FadeInAndOut(0.25f);
                HintParameter[] parameters = new HintParameter[2]
                {
                    new ItemCategoryHintParameter(__instance._category),
                    new ByteHintParameter((byte)categoryLimit)
                };
                __instance.Hub.hints.Show(new TranslationHint(HintTranslations.MaxItemCategoryReached, parameters, effects, 1.5f));
            }
            ItemSearchCompletorItemSave.Items.Remove(__instance);
        }
    }

    class ItemSearchCompletorItemSave
    {
        public static Dictionary<ItemSearchCompletor, SynapseItem> Items = new Dictionary<ItemSearchCompletor, SynapseItem>();

        [HarmonyPatch(typeof(ItemSearchCompletor), ".ctor")]
        [HarmonyPostfix]
        private void AddItem(ItemSearchCompletor __instance, ReferenceHub hub, ItemPickupBase targetPickup, ItemBase targetItem, double maxDistanceSquared)
        {
            Items.Add(__instance, targetItem.GetSynapseItem());
        }

    }
}
