using InventorySystem.Items.ThrowableProjectiles;
using Synapse.Api.Enum;
using Synapse.Api.Items;

namespace VT_Api.Core.Events.EventArguments
{
    public class ChangeIntoFragEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public TimeGrenade Grenade { get; internal set; }
        public SynapseItem Item { get; internal set; }
        public GrenadeType Type { get; internal set; }
        public bool Allow { get; set; }
    }

    public class ExplosionGrenadeEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public TimeGrenade Grenade { get; internal set; }
        public GrenadeType Type { get; internal set; }
        public bool Allow { get; set; }
    }

    public class CollisionEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public SynapseItem Item { get; internal set; }
        public bool Allow { get; set; }
    }
    
}
