using HarmonyLib;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using NorthwoodLib.Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VT_Api.Extension;

namespace VT_Api.Patches.VtEvent.ItemPatches
{
    [HarmonyPatch(typeof(BodyArmorUtils), nameof(BodyArmorUtils.RemoveEverythingExceedingLimits))]

    class RemoveExceedingLimitsPatch
    {
        [HarmonyPrefix]// TODO
        private static bool ItemLimitPatch(Inventory inv, BodyArmor armor, bool removeItems = true, bool removeAmmo = true)
        {
            try
            {
                //SynapseController.Server.Logger.Debug("ItemLimitPatch");

                HashSet<ushort> itemDrop = HashSetPool<ushort>.Shared.Rent();
                Dictionary<ItemCategory, int> catergoryCount = new Dictionary<ItemCategory, int>();
                Dictionary<ItemType, ushort> ammosDrop = new Dictionary<ItemType, ushort>();

                //Item
                foreach (KeyValuePair<ushort, ItemBase> item in inv.UserInventory.Items)
                {
                    if (item.Value.Category != ItemCategory.Armor)
                    {
                        int num = Mathf.Abs(InventoryLimits.GetCategoryLimit(armor, item.Value.Category));
                        int value = (!catergoryCount.TryGetValue(item.Value.Category, out value)) ? 1 : (value + 1);
                        if (value > num)
                        {
                            itemDrop.Add(item.Key);
                        }

                        catergoryCount[item.Value.Category] = value;
                    }
                }

                //Ammo
                foreach (KeyValuePair<ItemType, ushort> ammo in inv.UserInventory.ReserveAmmo)
                {
                    ushort ammoLimit = InventoryLimits.GetAmmoLimit(armor, ammo.Key);
                    if (ammo.Value > ammoLimit)
                    {
                        ammosDrop.Add(ammo.Key, (ushort)(ammo.Value - ammoLimit));
                    }
                }

                if (removeItems)
                {
                    foreach (ushort item in itemDrop)
                    {
                        // Event Here [Player, SynapseItem, LimitOfThisCatergory]
                        inv.ServerDropItem(item);
                    }
                }

                if (!removeAmmo)
                    return false;
                

                foreach (KeyValuePair<ItemType, ushort> ammo in ammosDrop)
                {
                    // Event Here [Player, Ammo, LimitOfThisType]
                    inv.ServerDropAmmo(ammo.Key, ammo.Value);
                }
                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: RemoveExceedingItem failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
