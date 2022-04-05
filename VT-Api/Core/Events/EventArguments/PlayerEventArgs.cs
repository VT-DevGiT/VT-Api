using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Items;

namespace VT_Api.Core.Events.EventArguments
{

    public class PlayerDamagePostEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public Player Killer { get; internal set; }
        public Player Victim { get; internal set; }
        public float Damage { get; set; }
        public DamageType DamageType { get; internal set; }
        public bool Allow { get; set; }
    }

    public class PlayerDestroyEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public Player Player { get; internal set; }
    }

    public class PlayerVerifEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public Player Player { get; internal set; }
    }

    public class PlayerSpeakIntercomEventEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public Player Player { get; internal set; }
        public bool Allow { get; set; }
    }

    public class PlayerSetClassAdvEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public Player Player { get; internal set; }

        public RoleType Role { get; internal set; }
    }

    public class PlayerSetClassEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public Player Player { get; internal set; }
        public int OldID { get; internal set; }
        public int NewID { get; internal set; }
    }
}
