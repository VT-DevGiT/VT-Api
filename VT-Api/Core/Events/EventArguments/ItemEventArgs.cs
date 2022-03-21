using System.Collections.Generic;
using System.Collections.ObjectModel;
using InventorySystem.Items.ThrowableProjectiles;
using Synapse.Api;
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

    public class CheckLimitItemEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public Player Player { get; internal set; }
        public List<SynapseItem> RemovItem { get; internal set; }
        public ReadOnlyDictionary<ItemCategory, int> CatergoryMax { get; internal set; }
    }

    public class CheckLimitAmmoEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public Player Player { get; internal set; }
        public Dictionary<AmmoType, ushort> RemovAmmo { get; internal set; }
        public ReadOnlyDictionary<AmmoType, ushort> CatergoryMax { get; internal set; }
    }
}

