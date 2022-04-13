using HarmonyLib;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items.Armor;
using NorthwoodLib.Pools;
using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VT_Api.Patches.VtEvent.ItemPatches
{
    [HarmonyPatch(typeof(BodyArmorUtils), nameof(BodyArmorUtils.RemoveEverythingExceedingLimits))]

    class RemoveExceedingLimitsPatch
    {
        [HarmonyPrefix]// TODO And When Player try to pickup Item
        private static bool ItemLimitPatch(Inventory inv, BodyArmor armor, bool removeItems = true, bool removeAmmo = true)
        {
            try
            {
                //SynapseController.Server.Logger.Debug("ItemLimitPatch");

                var player = inv.GetPlayer();

                //Item
                if (removeItems)
                    RemovItems(inv, armor, player);

                if (removeAmmo)
                    RemovAmmos(inv, armor, player);

                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: RemoveExceedingItem failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }

        private static void RemovItems(Inventory inv, BodyArmor armor, Player player)
        {
             var catergoryMax = new Dictionary<ItemCategory, int>();
            var catergoryCount = new Dictionary<ItemCategory, int>();
            var itemsDrop = HashSetPool<ushort>.Shared.Rent();

            foreach (var item in inv.UserInventory.Items)
            {
                var catergory = item.Value.Category;
                if (catergory == ItemCategory.Armor) continue;

                int num;
                if (!catergoryMax.ContainsKey(catergory))
                    num = catergoryMax[catergory] = Mathf.Abs(InventoryLimits.GetCategoryLimit(armor, catergory));
                else
                    num = catergoryMax[catergory];
                int value = (!catergoryCount.TryGetValue(catergory, out value)) ? 1 : (value + 1);
                if (value > num)
                {
                    itemsDrop.Add(item.Key);
                }

                catergoryCount[item.Value.Category] = value;
            }

            var items = itemsDrop.Select((i) => SynapseItem.AllItems[i]).ToList();
            VtController.Get.Events.Item.InvokeRemoveLimitItemEvent(player, catergoryMax, ref items);
            itemsDrop = items.Select((i) => i.Serial).ToHashSet();

            foreach (ushort item in itemsDrop)
                inv.ServerDropItem(item);
        }

        private static void RemovAmmos(Inventory inv, BodyArmor armor, Player player)
        {
            var ammosDrop = new Dictionary<AmmoType, ushort>();
            var ammosLimit = new Dictionary<AmmoType, ushort>();

            foreach (var ammo in inv.UserInventory.ReserveAmmo)
            {
                ushort ammoLimit = InventoryLimits.GetAmmoLimit(armor, ammo.Key);
                ammosLimit.Add((AmmoType)ammo.Key, ammoLimit);
                if (ammo.Value > ammoLimit)
                {
                    ammosDrop.Add((AmmoType)ammo.Key, (ushort)(ammo.Value - ammoLimit));
                }
            }

            VtController.Get.Events.Item.InvokeRemoveLimitAmmoEvent(player, ammosLimit, ref ammosDrop);

            foreach (var ammo in ammosDrop)
                inv.ServerDropAmmo((ItemType)ammo.Key, ammo.Value);
        }
    }
}
