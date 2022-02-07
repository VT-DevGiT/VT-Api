using Synapse;
using Synapse.Api.Enum;
using Synapse.Api.Events.SynapseEventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Items
{
    public abstract class AbstractWeapon : AbstractItem
    {
        #region Attributes & Properties
        public abstract ushort Ammos { get; }
        public abstract AmmoType AmmoType { get; }
        public abstract int DamageAmmont { get; }

        #endregion

        #region Methods

        protected override void Event()
        {
            base.Event();
            Server.Get.Events.Player.PlayerShootEvent += OnShoot;
            Server.Get.Events.Player.PlayerReloadEvent += OnReload;
            Server.Get.Events.Player.PlayerDamageEvent += OnDamage;
        }

        protected virtual void Reload(PlayerReloadEventArgs ev)
        {
            if (ev.Item.Durabillity < Ammos && ev.Allow)
            {
                ushort ammo = Math.Min(ev.Player.AmmoBox[AmmoType], Ammos);
                ev.Player.AmmoBox[AmmoType] -= ammo;
                ev.Item.Durabillity += ammo;
            }
        }
        protected virtual void Shoot(PlayerShootEventArgs ev) { }

        protected virtual void Damage(PlayerDamageEventArgs ev)
        {
            ev.Damage = DamageAmmont;
        }
        #endregion

        #region Event

        private void OnReload(PlayerReloadEventArgs ev)
        {
            if (ev.Item.ID == ID)
                this.Reload(ev);
        }

        private void OnShoot(PlayerShootEventArgs ev)
        {
            if (ev.Weapon?.ID == ID)
                this.Shoot(ev);
        }

        private void OnDamage(PlayerDamageEventArgs ev)
        {
            if (ev.Killer?.ItemInHand?.ID == ID)
                this.Damage(ev);
        }
        #endregion
    }

}
