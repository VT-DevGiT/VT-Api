using HarmonyLib;
using Hints;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using Synapse.Api.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VT_Api.Patches.VtEvent.ItemPatches
{
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.ValidateAny))]
    class ItemSearchValidatePatch
    {
        static internal List<ItemSearchCompletor> AlredyAllowed = new List<ItemSearchCompletor>();

        [HarmonyPrefix]
        private static bool ValidateAny(ItemSearchCompletor __instance, ref bool __result)
        {
            try
            {
                if (!SubValidateAny())
                {
                    ItemSearchCompletorItemSave.Items.Remove(__instance);
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

                    ItemSearchCompletorItemSave.Items.Remove(__instance);
                    __result = false;
                    return false;
                }

                if (__instance._category != 0 && !AlredyAllowed.Contains(__instance))
                {
                    int categoryLimit = Mathf.Abs(InventoryLimits.GetCategoryLimit(__instance._category, player.Hub));
                    bool allow = __instance.CategoryCount < categoryLimit;

                    VtController.Get.Events.Item.InvokeCheckLimitItemEvent(player, ItemSearchCompletorItemSave.Items[__instance], categoryLimit, ref allow);

                    if (!allow)
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
                    else 
                        AlredyAllowed.Add(__instance);
                }

                __result = true;
                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: ValidateAny faild !!\n{e}\nStackTrace:\n{e.StackTrace}");
                ItemSearchCompletorItemSave.Items.Remove(__instance); 
                return true;
            }

            bool SubValidateAny()
            {
                if (__instance.Hub.characterClassManager.IsHuman() && !__instance.Hub.interCoordinator.Handcuffed)
                    return !__instance.TargetPickup.Info.Locked;
                

                return false;
            }
        }

        
    }
    
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.CheckCategoryLimitHint))]
    class ItemSearchLimitHintPatch
    {
        [HarmonyPrefix]
        private static bool CheckCategoryLimitHint(ItemSearchCompletor __instance)
        {
            try
            {
                var categoryLimit = InventoryLimits.GetCategoryLimit(__instance._category, __instance.Hub);

                var player = __instance.Hub.GetPlayer();
                var allow = __instance._category == 0 || categoryLimit < 0 || __instance.CategoryCount < categoryLimit;

                VtController.Get.Events.Item.InvokeCheckLimitItemEvent(player, ItemSearchCompletorItemSave.Items[__instance], categoryLimit, ref allow);

                if (!allow)
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
                if (ItemSearchValidatePatch.AlredyAllowed.Contains(__instance))
                    ItemSearchValidatePatch.AlredyAllowed.Remove(__instance); 
                return false;
            }
            catch (Exception e)
            {
                if (ItemSearchValidatePatch.AlredyAllowed.Contains(__instance))
                    ItemSearchValidatePatch.AlredyAllowed.Remove(__instance);
                Synapse.Api.Logger.Get.Error($"Vt-Event: ItemSearchLimitHint faild !!\n{e}\nStackTrace:\n{e.StackTrace}");
                ItemSearchCompletorItemSave.Items.Remove(__instance); 
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(ItemSearchCompletor), MethodType.Constructor, new Type[] { typeof(ReferenceHub), typeof(ItemPickupBase), typeof(ItemBase), typeof(double) })]
    class ItemSearchCompletorItemSave
    {
        public static Dictionary<ItemSearchCompletor, SynapseItem> Items { get; } = new Dictionary<ItemSearchCompletor, SynapseItem>();


        [HarmonyPostfix]//todo fix null item
        private static void AddItem(ItemSearchCompletor __instance, ReferenceHub hub, ItemPickupBase targetPickup, ItemBase targetItem, double maxDistanceSquared)
        {
            try
            {
                Items.Add(__instance, targetPickup.GetSynapseItem());
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: ItemSearchCompletorItemSave faild !!\n{e}\nStackTrace:\n{e.StackTrace}");
            }
        }

    }
}
