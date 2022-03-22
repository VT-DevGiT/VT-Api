using Synapse;
using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Events.SynapseEventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Core.Items
{
    public abstract class AbstractWeapon : AbstractItem, IWeapon
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
        public virtual bool AllowRealod()
        {
            if (Item.Durabillity < Ammos)
            {
                ushort ammo = Math.Min(Holder.AmmoBox[AmmoType], Ammos);
                Holder.AmmoBox[AmmoType] -= ammo;
                Item.Durabillity += ammo;
            }
            return false;
        }

        public virtual bool AllowShoot(Vector3 targetPosition) => true;

        public virtual bool AllowShoot(Vector3 targetPosition, Player target) => true;

        public virtual bool AllowAttack(Player victim, ref float damage, DamageType type)
        {
            damage = DamageAmmont;
            return true;
        }
        #endregion
    }
}
