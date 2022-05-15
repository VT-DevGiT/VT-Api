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
        public abstract ushort MaxAmmos { get; }
        public abstract AmmoType AmmoType { get; }
        public abstract int DamageAmmont { get; }

        public float Ammo { get => Item.Durabillity; set => Item.Durabillity = value; }
        #endregion

        #region Constructors & Destructor
        public AbstractWeapon() : base()
        {

        }
        #endregion

        #region Methods
        public virtual bool Realod()
        {
            if (Ammo < MaxAmmos)
            {
                ushort ammo = Math.Min(Holder.AmmoBox[AmmoType], MaxAmmos);
                Holder.AmmoBox[AmmoType] -= ammo;
                Ammo += ammo;
            }
            return false;
        }

        public virtual bool Shoot(Vector3 targetPosition) => true;

        public virtual bool Shoot(Vector3 targetPosition, Player target) => true;

        public virtual bool Attack(Player victim, ref float damage, DamageType type)
        {
            damage = DamageAmmont;
            return true;
        }
        #endregion
    }
}
