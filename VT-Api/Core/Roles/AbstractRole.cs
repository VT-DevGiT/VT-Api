using Synapse;
using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api.Items;
using Synapse.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VT_Api.Config;
using VT_Api.Core.Behaviour;
using VT_Api.Extension;

namespace VT_Api.Core.Roles
{
    public abstract class AbstractRole : Synapse.Api.Roles.Role, IVtRole
    {
        #region Attributes & Properties
        protected abstract string SpawnMessage { get; }
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
        [Obsolete] 
        public sealed override List<Team> GetFriends() => new List<Team>();
        [Obsolete]
        public sealed override List<Team> GetEnemys() => new List<Team>();
        [Obsolete] 
        public sealed override int GetEscapeRole() => 0;
        [Obsolete] 
        public sealed override Team GetTeam() => Team.RIP;

        public bool Spawned { get; set; } = false;

        private static List<Type> _firstSpawnClass = new List<Type>();

        private bool _fristSpawn
        {
            get 
            {
                return !_firstSpawnClass.Any(p => p == this.GetType());
            }
            set
            {
                if (!value && !_firstSpawnClass.Any(p => p == this.GetType()))
                {
                    _firstSpawnClass.Add(this.GetType());
                }
                else if (value && _firstSpawnClass.Any(p => p == this.GetType()))
                {
                    _firstSpawnClass.Remove(this.GetType());
                }
            }
        }
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

        [API]
        public virtual bool CallPower(byte power, out string message)
        {
            message = VtController.Get.Configs.VtTranslation.ActiveTranslation.NoPower;
            return false;
        }

        /// <summary>
        /// Is called when the player ave the corect roleType.
        /// Call the base to apply the config.
        /// </summary>
        [API]
        public virtual void Spawning()
        {
            if (Config != null)
            {
                if (Config.Health != null)
                    Player.Health = (float)Config.Health;
                Player.MaxHealth = Config.MaxHealth ?? Player.Health;

                if (Config.ArtificialHealth != null)
                    Player.ArtificialHealth = (float)Config.ArtificialHealth;
                if (Config.MaxArtificialHealth != null)
                    Player.MaxArtificialHealth = (int)Config.MaxArtificialHealth;
            }
        }

        /**
        * <summary> 
        * Dont call this method ! 
        * This method is call only one time when the first player spawn in this role. 
        * <para>Use <see langword="static"/> method for the <see langword="event"/> method ! <example>For exemple :
        * <code>
        * <see langword="protected"/> <see langword="override"/> <see langword="void"/> DeSpawn()
        * {
        *   Server.Get.Events.Player.PlayerDamageEvent += OnDamage;
        * }
        * </code><code>
        * <see langword="static"/> <see langword="void"/> OnDamage(<see cref="PlayerDamageEventArgs"/> ev)
        * {
        *   if (ev.Victim.roleID == (int)RoleID.TestClass)
        *       ev.allow = false;
        * }
        * </code> </example> </para> </summary> 
        */
        [API]
        protected virtual void InitEvent() 
        {
        
        }

        public virtual void SetDisplayInfo()
        {
            Player.SetDisplayInfoRole(RoleName);
        }

        public sealed override void Spawn()
        {
            if (_fristSpawn)
            {
                InitEvent();
                _fristSpawn = false;
            }

            Player.RoleType = RoleType;

            if (!string.IsNullOrEmpty(SpawnMessage))
            { 
                string message = Regex.Replace(SpawnMessage, "%RoleName%", RoleName, RegexOptions.IgnoreCase);
                Player.OpenReportWindow(message.Replace("\\n", "\n"));
            }

            SetDisplayInfo();
            Spawning();
        }

        public void InitAll(PlayerSetClassEventArgs ev)
        {
            if (Spawned) return;
            Spawned = true;

            PlayerInit(ev);

            AditionalInit(ev);
        }

        private void PlayerInit(PlayerSetClassEventArgs ev)
        {
            if (Config == null) return;
            CheckItems(Config.Inventory);
            try
            {
                Config.Extract(Player, out var postion, out var rotation, out var items, out var ammos);
                
                ev.Items = items ?? new List<SynapseItem>();
                ev.Ammo  = ammos ?? new Dictionary<AmmoType, ushort>();
                ev.Rotation = rotation.x;
                
                if (postion != null)
                    ev.Position = postion.Position;
            }
            catch (Exception e)
            {
                Server.Get.Logger.Error($"Vt-Role : Error for spawn the role {this}\n{e}\nStackTrace:\n{e.StackTrace}");
            }
        }

        private void CheckItems(SerializedPlayerInventory inventory)
        {
            if (inventory != null && inventory.Items.Any())
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
            if (Player == null)
                return;
            Player.DisplayInfo = null;
            Player.AddDisplayInfo(PlayerInfoArea.Role);
            Player.AddDisplayInfo(PlayerInfoArea.UnitName);
        }

        public override string ToString() => $"{this.RoleName}({this.RoleId})";
        #endregion
    }
}
