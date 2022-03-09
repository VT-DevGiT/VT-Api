using Mirror;
using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Items;
using Synapse.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using VT_Api.Config;
using VT_Api.Core;
using VT_Api.Core.Enum;
using VT_Api.Core.Items;
using VT_Api.Core.Roles;

using Server = Synapse.Server;
using SynItemManager = Synapse.Api.Items.ItemManager;
using SynLogger = Synapse.Api.Logger;
using SynRoleManager = Synapse.Api.Roles.RoleManager;
using UERandom = UnityEngine.Random;
using VtItemManager = VT_Api.Core.Items.ItemManager;

namespace VT_Api.Extension
{
    public static class VtExtensions
    {
        internal static void Debug(this SynLogger logger, object message)
        {
            if (VtVersion.Debug)
                logger.Send($"VtApi-Debug: {message}", ConsoleColor.DarkYellow);
        }

        public static void PlayAmbientSound(this Map _, int id)
            => VtController.Get.MapAction.PlayAmbientSound(id);

        public static void StartAirBombardement()
            => VtController.Get.MapAction.StartAirBombardement();

        public static void StopAirBombardement()
            => MapActionManager.Get.isAirBombCurrently = false;

        public static void ResetRoomsLightColor(this Map _)
            => VtController.Get.MapAction.ResetRoomsLightColor();

        public static void ChangeRoomsLightColor(this Map _, Color color)
            => VtController.Get.MapAction.ChangeRoomsLightColor(color);

        public static int GetVoltage(this Map map)
            => VtController.Get.MapAction.GetVoltage();

        public static List<Player> GetDeadPlayersInRangeOfPlayer(this Player player, float range)
        {
            var players = MapActionManager.Get.GetRagdollOwners(player, range);

            players.RemoveAll(p => p.Team == Team.RIP && !p.OverWatch);
            
            if (players.Any())
                return players;
            return new List<Player>();
        }

        public static Player GetDeadPlayerInRangeOfPlayer(this Player player, float range)
        {
            var players = MapActionManager.Get.GetRagdollOwners(player, range);

            players.RemoveAll(p => p.Team == Team.RIP && !p.OverWatch);

            return players.FirstOrDefault();
        }

        public static List<Player> GetPlayer(this RoleID[] roleID)
            => SynapseController.Server.Players.Where(x => roleID.Any(r => x.RoleID == (int)r)).ToList();

        public static List<Player> GetPlayer(this RoleID roleID)
            => SynapseController.Server.Players.Where(x => x.RoleID == (int)roleID).ToList();

        public static bool Is939(this Player player)
            => player.RoleID == (int)RoleID.Scp93953 || player.RoleID == (int)RoleID.Scp93989;

        public static bool Is939(this RoleID roleID)
            => roleID == RoleID.Scp93953 || roleID == RoleID.Scp93989;

        public static bool IsUTR(this Player player) 
            => player.CustomRole is IUtrRole;

        public static T GetOrAddComponent<T>(this Component component) where T : Component
            => component.gameObject.GetOrAddComponent<T>();

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T Component;
            if (!gameObject.TryGetComponent(out Component))
                Component = gameObject.AddComponent<T>();
            return Component;
        }


        public static void ChangeRoomLightColor(this Room room, Color color, bool activeColor = true)
        {
            room.LightController.WarheadLightColor = color;
            room.LightController.WarheadLightOverride = activeColor;
        }

        public static void ResetRoomLightColor(this Room room)
        {
            room.LightController.WarheadLightColor = new Color(1, 0, 0);
            room.LightController.WarheadLightOverride = false;
        }

        public static void FakeRole(this Player player, RoleType role)
            => FakeRole(player, role, Server.Get.Players);

        public static void FakeRole(this Player player, RoleType role, List<Player> players)
            => NetworkLiar.Get.SendRole(player, role, players);

        public static bool IsTargetVisible(this Player player, GameObject obj)
            => IsTargetVisible(player.gameObject.GetComponent<UnityEngine.Camera>(), obj);

        public static bool IsTargetVisible(this UnityEngine.Camera camera, GameObject obj)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            var point = obj.transform.position;
            foreach (var plan in planes)
            {
                if (plan.GetDistanceToPoint(point) < 0)
                    return false;
            }
            return true;
        }

        public static bool IsDefined(this SerializedPlayerInventory item)
           => item.Ammo != null || (item.Items != null && item.Items.Any());

        public static bool IsDefined(this SynapseItem item)
            => item != null && item != SynapseItem.None && item.ItemType != ItemType.None;

        public static void Extract(this SerializedPlayerRole playerRole, Player player, out MapPoint postion, out Vector2 rotation, out List<SynapseItem> items, out Dictionary<AmmoType, ushort> ammos)
        {
            if (playerRole.SpawnPoints != null && playerRole.SpawnPoints.Any())
                postion = playerRole.SpawnPoints[UnityEngine.Random.Range(0, playerRole.SpawnPoints.Count - 1)]?.Parse();
            else
                postion = null;

            rotation = playerRole.Rotation ?? new Vector2(0, 0);

            if (playerRole.Inventory != null && playerRole.Inventory.IsDefined()) 
                playerRole.Inventory.Extract(player, out items, out ammos);
            else
            {
                items = null;
                ammos = null;
            }

        }

        public static void Extract(this SerializedPlayerInventory playerInventory, Player player, out List<SynapseItem> items, out Dictionary<AmmoType, ushort> ammos)
        {
            if (playerInventory.Items != null && playerInventory.Items.Any())
            {
                items = new List<SynapseItem>();

                foreach (var item in playerInventory.Items)
                {
                    if (item != null && item.Extract(player, out var synapseItem))
                        items.Add(synapseItem);
                }
            }
            else
            {
                items = null;
            }

            if (playerInventory.Ammo != null)
                playerInventory.Ammo.Extract(out ammos);
            else ammos = null;
        }

        public static void Extract(this SerializedAmmo serializedAmmo, out Dictionary<AmmoType, ushort> ammos)
        {
            ammos = new Dictionary<AmmoType, ushort>();
            ammos.Add(AmmoType.Ammo556x45, serializedAmmo.Ammo5);
            ammos.Add(AmmoType.Ammo762x39, serializedAmmo.Ammo7);
            ammos.Add(AmmoType.Ammo9x19, serializedAmmo.Ammo9);
            ammos.Add(AmmoType.Ammo12gauge, serializedAmmo.Ammo12);
            ammos.Add(AmmoType.Ammo44cal, serializedAmmo.Ammo44);
        }

        public static bool Extract(this SerializedPlayerItem serializedItem, Player player, out SynapseItem item)
        {
            item = serializedItem.Parse();

            if (serializedItem.UsePreferences && item.ItemCategory == ItemCategory.Firearm)
            {
                item.WeaponAttachments = player.GetPreference(SynItemManager.Get.GetBaseType(serializedItem.ID));
            }
            return UERandom.Range(1f, 100f) <= serializedItem.Chance;
        }


        public static ScpRecontainmentType GetScpRecontainmentType(this DamageType damage, Player player = null)
        {
            switch (damage)
            {
                case DamageType.Warhead:
                    return ScpRecontainmentType.Nuke;
                case DamageType.Decontamination:
                    return ScpRecontainmentType.Decontamination;
                case DamageType.Tesla:
                    return ScpRecontainmentType.Tesla;
                default:
                    if (player == null)
                        return ScpRecontainmentType.Unknown;
                    if (player.TeamID <= (int)TeamID.CDP && player.TeamID >= (int)TeamID.NTF)
                        return (ScpRecontainmentType)player.TeamID;
                    return ScpRecontainmentType.Unspecified;
            }
        }


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


        public static void SetDisplayInfoRole(this Player player, string roleName)
        {
            /*
             * TODO Rework This :
             * 
             * Badge "pas touche"
             * 
             * Nickname
             * Role (Unit)
             * CustomInfo
             * 
             * PowerStatus
             */

            player.RemoveDisplayInfo(PlayerInfoArea.Role);

            if (player.Team == Team.MTF)
            {
                player.RemoveDisplayInfo(PlayerInfoArea.UnitName);
                player.DisplayInfo = $"{roleName} ({player.UnitName})";
            }
            else
            {
                player.DisplayInfo = roleName;
            }
        }


        public static int GetTeam(this RoleID roleID)
        {
            if (roleID > RoleID.None && roleID <= (RoleID)SynRoleManager.HighestRole)
                 return (int)((RoleType)roleID).GetTeam();
            else return SynRoleManager.Get.GetCustomRole((int)roleID).GetTeamID();
        }

        public static bool TryGetScript(this SynapseItem item, out IItem script)
        {
            item.ItemData.TryGetValue(VtItemManager.KeySynapseItemData, out var value);
            script = value as IItem;
            return script != null;
        }

        public static IItem GetScript(this SynapseItem item)
            => (IItem)item.ItemData[VtItemManager.KeySynapseItemData];
    }
}