using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Pickups;
using Scp914;
using Scp914.Processors;
using Synapse.Api.Items;
using System;
using UnityEngine;
using VT_Api.Extension;

namespace VT_Api.Patches.VtEvent.MapPaches
{
    class Scp914ItemPickupPatch
    {
        [HarmonyPatch(typeof(AmmoItemProcessor), nameof(AmmoItemProcessor.OnPickupUpgraded))]
        [HarmonyPrefix]
        private static bool AmmoUpgradePatch(AmmoItemProcessor __instance, Scp914KnobSetting setting, ItemPickupBase ipb, Vector3 newPos)
        {
            try
            { 
                ItemType itemType;
                switch (setting)
                {
                    case Scp914KnobSetting.Rough:
                    case Scp914KnobSetting.Coarse:
                        itemType = __instance._previousAmmo;
                        break;
                    case Scp914KnobSetting.OneToOne:
                        itemType = __instance._oneToOne;
                        break;
                    case Scp914KnobSetting.Fine:
                    case Scp914KnobSetting.VeryFine:
                        itemType = __instance._nextAmmo;
                        break;
                    default:
                        return false;
                }

                if (ipb is AmmoPickup ammoPickup == false) // change this in C#9 
                    return false;
            
                var change = 0;
                var exchangedAmmo = 0;

                if (AmmoItemProcessor.TryGetAmmoItem(ammoPickup.Info.ItemId, out AmmoItem ammoItem) && 
                    InventoryItemLoader.AvailableItems.TryGetValue(itemType, out ItemBase itembase) && 
                    itembase is AmmoItem ammoItem2)
                { 
                    var unitPrice1 = ammoItem.UnitPrice;
                    var unitPrice2 = ammoItem2.UnitPrice;
                    var num1 = 0;
                    var num2 = 0;
                    var num3 = 0;

                    for (var i = 0; i < ammoPickup.SavedAmmo; i++)
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

                var destroyOldItem = change == 0;
                var oldItem = ammoPickup.GetSynapseItem();
                var newItem = exchangedAmmo > 0 ? new SynapseItem(itemType) { Durabillity = exchangedAmmo } : SynapseItem.None;
            
                oldItem.Durabillity = change;
                oldItem.Position = newPos;

                VtController.Get.Events.Map.InvokeScp914UpgradeItemEvent(setting, oldItem, ref newItem, ref destroyOldItem);

                if (newItem.IsDefined())
                {
                    newItem.Drop(newPos);
                    newItem.Rotation = oldItem.Rotation;
                }

                if (destroyOldItem)
                {
                    oldItem.Destroy();
                }

                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: Scp914ItemPickupPatch-Ammo failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }

        [HarmonyPatch(typeof(FirearmItemProcessor), nameof(FirearmItemProcessor.OnPickupUpgraded))]
        [HarmonyPrefix]
        private static bool FirearmUpgradePatch(FirearmItemProcessor __instance, Scp914KnobSetting setting, ItemPickupBase ipb, Vector3 newPos)
        {
            try
            { 
                var items = __instance.GetItems(setting, ipb.Info.ItemId);

                foreach (ItemType newItemType in items)
                {
                    var oldItem = ipb.GetSynapseItem();
                    var newItem = newItemType == ItemType.None || newItemType == oldItem.ItemType ? SynapseItem.None : new SynapseItem(newItemType);
                    var destroyOldItem = newItem.IsDefined();
                    var attachments = 0u;

                    if (!InventoryItemLoader.AvailableItems.TryGetValue(newItemType, out ItemBase newItemBase))
                    {
                        destroyOldItem = true;
                        newItem = SynapseItem.None;
                    }

                    if (ipb is FirearmPickup oldFirearmPickup == false)
                        throw new InvalidOperationException("FirearmItemProcessor can't be used for non-firearm items, such as " + newItemBase?.ItemTypeId ?? "Unknow");


                    if (newItemBase is Firearm newFirearm && 
                        InventoryItemLoader.AvailableItems.TryGetValue(oldFirearmPickup.Info.ItemId, out ItemBase oldItemBase) &&
                        oldItemBase is Firearm oldFirearm)
                    {
                        var change = 0;
                        var exchangedAmmo = 0;

                        if (AmmoItemProcessor.TryGetAmmoItem(oldFirearm.AmmoType, out AmmoItem ammoItem) &&
                            InventoryItemLoader.AvailableItems.TryGetValue(newFirearm.AmmoType, out ItemBase itembase) &&
                            itembase is AmmoItem ammoItem2)
                        {
                            var unitPrice1 = ammoItem.UnitPrice;
                            var unitPrice2 = ammoItem2.UnitPrice;
                            var num1 = 0;
                            var num2 = 0;
                            var num3 = 0;

                            for (var i = 0; i < oldFirearmPickup.Status.Ammo; i++)
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
                        var oldAmmo = new SynapseItem(oldFirearm.AmmoType) { Durabillity = change, Position = newPos };
                        var newAmmo = exchangedAmmo > 0 ? new SynapseItem(newFirearm.AmmoType) { Durabillity = exchangedAmmo, Position = newPos } : SynapseItem.None;

                        VtController.Get.Events.Map.InvokeScp914UpgradeItemEvent(setting, oldAmmo, ref newAmmo, ref destroyOldAmmo);

                        if (destroyOldAmmo)
                        {
                            oldAmmo.Destroy();
                        }
                        else
                        {
                            oldAmmo.Drop(newPos);
                        }

                        if (newAmmo.IsDefined())
                        {
                            newAmmo.Drop(newPos);
                        }

                        attachments = newFirearm.ValidateAttachmentsCode(0u);
                    }

                    if (!destroyOldItem)
                    {
                        oldFirearmPickup.NetworkStatus = new FirearmStatus(0, FirearmStatusFlags.None, AttachmentsUtils.GetRandomAttachmentsCode(newItemType));
                    }

                    oldItem.Position = newPos;

                    VtController.Get.Events.Map.InvokeScp914UpgradeItemEvent(setting, oldItem, ref newItem, ref destroyOldItem);

                    if (newItem.IsDefined())
                    {
                        newItem.Drop(newPos);
                        newItem.Rotation = oldItem.Rotation;
                    }

                    if (newItem.PickupBase is FirearmPickup firearmPickup)
                    {
                        firearmPickup.NetworkStatus = new FirearmStatus(0, FirearmStatusFlags.None, attachments);
                    }

                    if (destroyOldItem)
                    {
                        oldItem.Destroy();
                    }
                }

                return false;
            }
            catch (InvalidOperationException e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: Scp914ItemPickupPatch-Firearm throw a error !\n{e}\nStackTrace:\n{e.StackTrace}");
                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: Scp914ItemPickupPatch-Firearm failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }

        [HarmonyPatch(typeof(StandardItemProcessor), nameof(StandardItemProcessor.OnPickupUpgraded))]
        [HarmonyPrefix]
        private static bool ItemUpgradePatch(StandardItemProcessor __instance, Scp914KnobSetting setting, ItemPickupBase ipb, Vector3 newPosition)
        {
            try
            {
                var itemType = __instance.RandomOutput(setting, ipb.Info.ItemId);
                var oldItem = ipb.GetSynapseItem();
                var newItem = itemType == ItemType.None || itemType == oldItem.ItemType ? SynapseItem.None : new SynapseItem(itemType);
                var destroyOldItem = newItem.IsDefined();

                if (!destroyOldItem && __instance._fireUpgradeTrigger && ipb is IUpgradeTrigger upgradeTrigger)
                {
                    upgradeTrigger.ServerOnUpgraded(setting);
                }

                oldItem.Position = newPosition;

                VtController.Get.Events.Map.InvokeScp914UpgradeItemEvent(setting, oldItem, ref newItem, ref destroyOldItem);

                if (newItem.IsDefined())
                {
                    newItem.Drop(newPosition);
                    newItem.Rotation = oldItem.Rotation;
                }

                if (destroyOldItem)
                {
                    oldItem.Destroy();
                }

                ipb.DestroySelf();
                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: Scp914ItemPickupPatch-StandardItem failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
