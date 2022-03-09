using Synapse.Api;
using Synapse.Api.Enum;

namespace VT_Api.Core.Items
{
    internal interface IWeapon : IItem
    {
        bool AllowAttack(Player victim, float damage, DamageType type);


    }
}
