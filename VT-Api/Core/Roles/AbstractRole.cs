using HarmonyLib;
using MEC;
using Synapse;
using Synapse.Api;
using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VT_Api.Core.Behaviour;
using VT_Api.Extension;

namespace VT_Api.Core.Roles
{
    public abstract class AbstractRole : Synapse.Api.Roles.Role, IVtRole
    {
        #region Attributes & Properties
        protected abstract string SpawnMessage { get; }
        protected virtual bool SetDisplayInfo => true;
        protected abstract int[] EnemysList { get; }
        protected abstract int[] FriendsList { get; }
        protected abstract RoleType RoleType { get; }
        protected abstract int RoleTeam { get; }
        protected abstract int RoleId { get; }
        protected abstract string RoleName { get; }
        protected abstract SerializedPlayerRole Config { get; }


        public SerializedPlayerRole GetConfig() => Config;
        public override List<int> GetEnemiesID() => EnemysList.ToList();
        public override List<int> GetFriendsID() => FriendsList.ToList();
        public override int GetRoleID() => RoleId;
        public override string GetRoleName() => RoleName;
        public override int GetTeamID() => RoleTeam;
        public virtual bool CallPower(byte power, out string message)
        {
            message = VtController.Get.Configs.VtTranslation.ActiveTranslation.NoPower;
            return false;
        }

        public bool Spawned { get; set; } = false;
        #endregion

        #region Constructors & Destructor
        public AbstractRole() { }
        #endregion

        #region Methods
        protected void InactiveComponent<T>()
            where T : UnityEngine.Behaviour
        {
            T composant;
            if (Player.TryGetComponent<T>(out composant))
                composant.enabled = false;
        }

        protected void KillComponent<T>()
            where T : UnityEngine.Behaviour
        {
            T composant;
            if (Player.TryGetComponent<T>(out composant))
            {
                if (composant is RoundBehaviour rb)
                    rb.Kill();
                else Player.Destroy(composant);
            }
        }

        protected T ActiveComponent<T>()
            where T : UnityEngine.Behaviour
        {
            T composant;
            composant = Player.GetOrAddComponent<T>();
            composant.enabled = true;
            return composant;
        }

        [API]
        protected virtual void AditionalInit(PlayerSetClassEventArgs ev)
        { }

        /// <summary>
        /// call when the class Spawn for add Event on the class
        /// Warning ! don't forget to untie them when the Despawn class
        /// </summary>
        [API]
        protected virtual void Event()
        { }

        public sealed override void Spawn()
        {
            Event();

            
            Player.RoleType = RoleType;

            if (!string.IsNullOrEmpty(SpawnMessage))
                Player.OpenReportWindow(SpawnMessage.Replace("%RoleName%", RoleName).Replace("\\n", "\n"));

        }

        public void InitAll(PlayerSetClassEventArgs ev)
        {
            Spawned = true;

            InitPlayer(ev);

            AditionalInit(ev);
        }

        private void InitPlayer(PlayerSetClassEventArgs ev)
        {
            Player.Inventory.Clear();
            checkItems(Config.Inventory);

            try
            {
                Config.Extract(Player, out var postion, out var rotation, out var items, out var ammos);
                ev.Items = items;
                ev.Ammo = ammos;
                ev.Position = postion;
                ev.Rotation = rotation.x;
            }
            catch (Exception e)
            {
                Server.Get.Logger.Error($"Error for spawn the role {this} : {e}");
            }

            if (SetDisplayInfo)
                Player.SetDisplayInfoRole(RoleName);
        }

        private void checkItems(SerializedPlayerInventory inventory)
        {
            if (inventory.Items.Any())
            {
                var listToRemove = new List<SerializedPlayerItem>();
                foreach (SerializedPlayerItem item in inventory.Items)
                {
                    if (!Server.Get.ItemManager.IsIDRegistered(item.ID))
                    {
                        Server.Get.Logger.Error($"Vt-AbstractRole :\n\tConfig error in {nameof(Config)} of the role {this}\n\tUnknown Item ID : {item.ID} !\n\tYou need to change the configuration!");
                        listToRemove.Add(item);
                    }
                }
                if (listToRemove.Any()) inventory.Items.RemoveAll(p => listToRemove.Contains(p));
            }
        }

        [API]
        public override void DeSpawn()
        {
            Player.DisplayInfo = null;
            Player.AddDisplayInfo(PlayerInfoArea.Role);
            Player.AddDisplayInfo(PlayerInfoArea.UnitName);

        }

        public override string ToString() => $"{this.RoleName}({this.RoleId})";
        #endregion
    }
}
