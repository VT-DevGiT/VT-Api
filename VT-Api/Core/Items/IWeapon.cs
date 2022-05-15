using Synapse.Api;
using Synapse.Api.Enum;
using UnityEngine;

namespace VT_Api.Core.Items
{
    public interface IWeapon : IItem
    {
        bool Attack(Player victim, ref float damage, DamageType type);
        bool Realod();
        bool Shoot(Vector3 targetPosition);
        bool Shoot(Vector3 targetPosition, Player target);
    }
}
