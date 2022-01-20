using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Items;
using Synapse.Config;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VT_Api.Core.Enum;

namespace VT_Api.Extension
{
    static internal class VtExtensions
    {
        internal static void Debug(this Synapse.Api.Logger logger, object message)
            => logger.Send($"VtApi-Debug: {message}", ConsoleColor.DarkYellow);

        public static bool IsDefined(this SerializedPlayerInventory item)
            => item.Ammo != null || (item.Items != null && item.Items.Any());

        public static bool IsDefined(this SynapseItem item)
            => item != null && item != SynapseItem.None && item.ItemType != ItemType.None;

        public static void ChangeRoomLightColor(this Room room, Color color, bool activeColor = true)
        {
            room.LightController.WarheadLightColor = color;
            room.LightController.WarheadLightOverride = activeColor;
        }

        public static bool Is939(this RoleType roleType)
            => roleType == RoleType.Scp93953 || roleType == RoleType.Scp93989;

        public static bool Is939(this RoleID roleID)
            => roleID == RoleID.Scp93953 || roleID == RoleID.Scp93989;

        public static bool TryAddOrDrop(this PlayerInventory playerInv, SynapseItem item)
        {
            if (playerInv.Items.Count < 8)
            {
                playerInv.AddItem(item);
                return true;
            }
            else
            {
                playerInv.Drop(item);
                return false;
            }
        }

        public static bool TryPickupOrDrop(this SynapseItem item, Player player)
        {
            if (player.Inventory.Items.Count < 8)
            {
                item.PickUp(player);
                return true;
            }
            else
            {
                item.Drop(player.Position);
                return false;
            }
        }

        public static bool TryPickup(this Player.PlayerAmmoBox ammos, SynapseItem item)
        {
            if (item.ItemCategory != ItemCategory.Ammo)
            {
                return false;
            }
            else
            {
                ammos[(AmmoType)item.ItemType] = (ushort)item.Durabillity;
                return true;
            }
        }
    }
}