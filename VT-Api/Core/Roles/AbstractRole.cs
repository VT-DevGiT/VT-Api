using Synapse;
using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api.Items;
using Synapse.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using VT_Api.Config;
using VT_Api.Core.Behaviour;
using VT_Api.Extension;

namespace VT_Api.Core.Roles
{
    public abstract class AbstractRole : Synapse.Api.Roles.Role, IVtRole
    {
        #region Attributes & Properties
        protected abstract string SpawnMessage { get; }
        protected virtual bool SetDisplayInfo => true;
        protected abstract List<int> EnemysList { get; }
        protected abstract List<int> FriendsList { get; }
        protected virtual List<int> FfFriendsList { get; } = new List<int>();
        protected abstract RoleType RoleType { get; }
        protected abstract int RoleTeam { get; }
        protected abstract int RoleId { get; }
        protected abstract string RoleName { get; }
        protected abstract SerializedPlayerRole Config { get; }



        public SerializedPlayerRole GetConfig() => Config;
        public sealed override List<int> GetEnemiesID() => EnemysList.ToList();
        public sealed override List<int> GetFriendsID() => Server.Get.FF ? FfFriendsList : FriendsList;
        public sealed override int GetRoleID() => RoleId;
        public sealed override string GetRoleName() => RoleName;
        public sealed override int GetTeamID() => RoleTeam;
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
        protected virtual void AditionalInit(PlayerSetClassEventArgs ev) { }


        public sealed override void Spawn()
        {
            Player.RoleType = RoleType;

            if (!string.IsNullOrEmpty(SpawnMessage))
                Player.OpenReportWindow(SpawnMessage.Replace("%RoleName%", RoleName));

            if (SetDisplayInfo)
                Player.SetDisplayInfoRole(RoleName);
        }

        public void InitAll(PlayerSetClassEventArgs ev)
        {
            Spawned = true;

            if ()
                InitEvent();

            PlayerInit(ev);

            AditionalInit(ev);
        }

        /**
         * <summary> 
         * This method is call only one time when the first player spawn in this role. 
         * <para>Use <see langword="static"/> method for the <see langword="event"/> method ! <example>For exemple :
         * <code>
         * <see langword="protected"/> <see langword="override"/> <see langword="void"/> DeSpawn()
         * {
         *   Server.Get.Events.Player.PlayerDamageEvent += OnDamage;
         * }
         * </code><code>
         * <see langword="static"/> <see langword="void"/> OnDamage(PlayerDamageEventArgs ev)
         * {
         *   if (ev.Victim.roleID == (int)RoleID.TestClass)
         *       ev.allow = false;
         * }
         * </code> </example> </para> </summary> 
         */
        [API]
        protected virtual void InitEvent() { }

        private void PlayerInit(PlayerSetClassEventArgs ev)
        {
            if (Config == null) return;

            checkItems(Config.Inventory);

            try
            {
                Config.Extract(Player, out var postion, out var rotation, out var items, out var ammos);
                
                ev.Items = items ?? new List<SynapseItem>();
                ev.Ammo  = ammos ?? new Dictionary<AmmoType, ushort>();
                ev.Rotation = rotation?.x ?? 0;
                
                if (postion != null)
                    ev.Position = postion.Position;

                if (Config.Health != null)
                    ev.Player.Health = (float)Config.Health;
                ev.Player.MaxHealth = Config.MaxHealth ?? ev.Player.Health;

                if (Config.ArtificialHealth != null)
                    ev.Player.ArtificialHealth = (float)Config.ArtificialHealth;
                if (Config.MaxArtificialHealth != null)
                    ev.Player.MaxArtificialHealth = (int)Config.MaxArtificialHealth;

            }
            catch (Exception e)
            {
                Server.Get.Logger.Error($"Vt-Role : Error for spawn the role {this}\n{e}\nStackTrace:\n{e.StackTrace}");
            }
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
