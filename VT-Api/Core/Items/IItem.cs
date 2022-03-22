using Synapse.Api.Enum;
using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api.Items;

namespace VT_Api.Core.Items
{
    public interface IItem
    {
        VtItemInformation Info { get; }
        SynapseItem Item { get; set; }
        bool AllowDrop(ref bool Throw);
        bool AllowDamage(ref float damage, DamageType damageType);
        bool AllowChange(bool newItem);
        bool AllowPickUp();
        bool AllowUse(ItemInteractState state);
    }
}
