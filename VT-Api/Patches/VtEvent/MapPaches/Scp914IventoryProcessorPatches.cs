using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Firearms.Attachments;
using Scp914;
using Scp914.Processors;
using Synapse.Api.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VT_Api.Extension;

namespace VT_Api.Patches.VtEvent.MapPaches
{
    class Scp914IventoryProcessorPatch
    {
        // the ammo cannot be upgrade if they are in the inventory, even if the method exists I do not see any interest in the patch
        // call Warkis if you want an event for the ammo or do a pull request

        [HarmonyPatch(typeof(FirearmItemProcessor), nameof(FirearmItemProcessor.OnInventoryItemUpgraded))]
        [HarmonyPrefix]
        private static bool FirearmUpgradePatch(FirearmItemProcessor __instance, Scp914KnobSetting setting, ReferenceHub hub, ushort serial)
        {
            try
            {
                if (hub.inventory.UserInventory.Items.TryGetValue(serial, out ItemBase item))
                {
                    var items = __instance.GetItems(setting, item.ItemTypeId);
                    var player = hub.GetPlayer();
                    var oldItem = item.GetSynapseItem();

                    foreach (ItemType newType in items)
                    {
                        if (item is not Firearm firearm1)
                            throw new InvalidOperationException("FirearmItemProcessor can't be used for non-firearm items, such as " + item.ItemTypeId);

                        var destroyOldItem = newType == oldItem.ItemType;
                        var newItem = destroyOldItem ? new SynapseItem(newType) : SynapseItem.None;


                        if (InventoryItemLoader.AvailableItems.TryGetValue(newType, out ItemBase value) && value is Firearm firearm2)
                        {
                            var change = 0;
                            var exchangedAmmo = 0;

                            if (AmmoItemProcessor.TryGetAmmoItem(firearm1.AmmoType, out AmmoItem ammoItem) &&
                                InventoryItemLoader.AvailableItems.TryGetValue(firearm2.AmmoType, out ItemBase itembase) &&
                                itembase is AmmoItem ammoItem2)
                            {
                                var unitPrice1 = ammoItem.UnitPrice;
                                var unitPrice2 = ammoItem2.UnitPrice;
                                var num1 = 0;
                                var num2 = 0;
                                var num3 = 0;

                                for (var i = 0; i < firearm1.Status.Ammo; i++)
                                {
                                    num3 += unitPrice1;
                                    num1++;
                                    if (num3 % unitPrice2 == 0)
                                    {
                                        num2 += num3 / unitPrice2;
                                        num1 = 0;
                                        num3 = 0;
                                    }
                                }

                                exchangedAmmo = num2;
                                change = num1;
                            }


                            var destroyOldAmmo = change == 0;
                            var oldAmmo = new SynapseItem(firearm1.AmmoType) { Durabillity = change };
                            var newAmmo = exchangedAmmo > 0 ? new SynapseItem(firearm2.AmmoType) { Durabillity = exchangedAmmo } : SynapseItem.None;

                            VtController.Get.Events.Map.InvokeScp914UpgradeItemEvent(setting, oldAmmo, ref newAmmo, ref destroyOldAmmo);

                            if (destroyOldAmmo)
                            {
                                oldAmmo.Destroy();
                            }
                            else
                            {
                                oldAmmo.TryPickupOrDrop(player);
                            }

                            if (newAmmo.IsDefined())
                            {
                                if (player.AmmoBox.TryPickup(newAmmo))
                                    newAmmo.TryPickupOrDrop(player);
                            }

                        }

                        if (!destroyOldItem)
                        {
                            uint randomAttachmentsCode = AttachmentsUtils.GetRandomAttachmentsCode(newType);
                            firearm1.Status = new FirearmStatus(0, FirearmStatusFlags.None, randomAttachmentsCode);
                            firearm1.ApplyAttachmentsCode(randomAttachmentsCode, reValidate: false);
                            return false;
                        }

                        VtController.Get.Events.Map.InvokeScp914UpgradeItemEvent(setting, oldItem, ref newItem, ref destroyOldItem);

                        if (destroyOldItem)
                        {
                            oldItem.Destroy();
                        }

                        if (newItem.IsDefined())
                        {
                            newItem.TryPickupOrDrop(player);
                        }
                    }

                }
                return false;
            }
            catch (InvalidOperationException e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: Scp914IventoryProcessorPatch-Firearm throw a error !\n{e}\nStackTrace:\n{e.StackTrace}");
                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: Scp914IventoryProcessorPatch-Firearm failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }

        [HarmonyPatch(typeof(StandardItemProcessor), nameof(StandardItemProcessor.OnInventoryItemUpgraded))]
        [HarmonyPrefix]
        private static bool ItemUpgradePatch(StandardItemProcessor __instance, Scp914KnobSetting setting, ReferenceHub hub, ushort serial)
        {
            try
            {
                if (!hub.inventory.UserInventory.Items.TryGetValue(serial, out ItemBase value))
                    return false;

                var player = hub.GetPlayer();
                var itemType = __instance.RandomOutput(setting, value.ItemTypeId);
                var oldItem = value.GetSynapseItem();
                var newItem = itemType == ItemType.None || itemType == oldItem.ItemType ? SynapseItem.None : new SynapseItem(itemType);
                var destroyOldItem = newItem.IsDefined();

                if (!destroyOldItem && __instance._fireUpgradeTrigger && value is IUpgradeTrigger upgradeTrigger)
                {
                    upgradeTrigger.ServerOnUpgraded(setting);
                }

                VtController.Get.Events.Map.InvokeScp914UpgradeItemEvent(setting, oldItem, ref newItem, ref destroyOldItem);

                if (destroyOldItem)
                {
                    oldItem.Destroy();
                }

                if (newItem.IsDefined())
                {
                    newItem.TryPickupOrDrop(player);
                }

                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: Scp914IventoryProcessorPatch-StandardItem failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
