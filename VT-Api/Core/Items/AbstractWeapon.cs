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

        #region Constructors & Destructor
        public AbstractWeapon() : base()
        {

        }
        #endregion

        #region Methods

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
    }
}
