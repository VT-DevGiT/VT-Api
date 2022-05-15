using GameCore;
using HarmonyLib;
using InventorySystem.Configs;
using InventorySystem.Items.Armor;
using UnityEngine;
using VT_Api.Core.Items;
using VT_Api.Extension;

namespace VT_Api.Patches.VtPatch
{
    [HarmonyPatch(typeof(InventoryLimits), nameof(InventoryLimits.GetCategoryLimit), new[] { typeof(BodyArmor), typeof(ItemCategory) })]
    class GetCategoryLimitPatchs
    {
        [HarmonyPrefix]
        private static bool GetCategoryLimit(BodyArmor armor, ItemCategory category, ref sbyte __result)
        {
            int limit = ItemManager.Get.ItemCategoryLimit[category];
            if (limit < 0)
            {
                __result = 8;
                return false;
            }

            if (armor != null)
            {
                BodyArmor.ArmorCategoryLimitModifier[] categoryLimits = armor.CategoryLimits;
                for (int j = 0; j < categoryLimits.Length; j++)
                {
                    BodyArmor.ArmorCategoryLimitModifier armorCategoryLimitModifier = categoryLimits[j];
                    if (armorCategoryLimitModifier.Category == category)
                    {
                        limit += armorCategoryLimitModifier.Limit;
                        break;
                    }
                }
            }

            __result = (sbyte)Mathf.Clamp(limit, -8, 8);
            return false;
        }
    }

    [HarmonyPatch(typeof(ServerConfigSynchronizer), nameof(ServerConfigSynchronizer.RefreshCategoryLimits))]
    class RefreshCategoryLimitsPatch 
    {
        [HarmonyPrefix]
        private static bool RefreshCategoryLimits()
        {
            int count = InventoryLimits.StandardCategoryLimits.Count;
            InventoryLimits.Config.CategoryLimits.Clear();
            for (int i = 0; i < count; i++)
                InventoryLimits.Config.CategoryLimits.Add(9);

            ItemManager.Get.ItemCategoryLimit.Clear();

            ItemManager.Get.ItemCategoryLimit.Add(ItemCategory.None, -1);
            
            for (int i = 1; System.Enum.IsDefined(typeof(ItemCategory), (ItemCategory)i); i++)
            {
                var key = (ItemCategory)i;
                if (InventoryLimits.StandardCategoryLimits.TryGetValue(key, out sbyte value))
                {
                    var limit = ConfigFile.ServerConfig.GetSByte("limit_category_" + key.ToString().ToLowerInvariant(), value);
                    ItemManager.Get.ItemCategoryLimit.Add(key, limit);
                }
            }
            return false;
        }
    }
}
