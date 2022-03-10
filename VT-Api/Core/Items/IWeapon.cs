using Synapse.Api;
using Synapse.Api.Enum;
using UnityEngine;

namespace VT_Api.Core.Items
{
    public interface IWeapon : IItem
    {
        bool AllowAttack(Player victim, ref float damage, DamageType type);
        bool AllowRealod();
        bool AllowShoot(Vector3 targetPosition);
        bool AllowShoot(Vector3 targetPosition, Player target);
    }
}
