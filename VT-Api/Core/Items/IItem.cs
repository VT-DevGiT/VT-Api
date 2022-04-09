using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api.Items;

namespace VT_Api.Core.Items
{
    public interface IItem
    {
        VtItemInformation Info { get; }
        SynapseItem Item { get; set; }
        bool Drop(ref bool Throw);
        bool Damage(ref float damage, DamageType damageType);
        bool Change(bool newItem);
        bool PickUp(Player player);
        bool Use(ItemInteractState state);
    }
}
