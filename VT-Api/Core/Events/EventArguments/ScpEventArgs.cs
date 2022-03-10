using Synapse.Api;

namespace VT_Api.Core.Events.EventArguments
{

    public class PortalUseEventArgs : Synapse.Api.Events.EventHandler.ISynapseEventArgs
    {
        public Player Scp106 { get; internal set; }
        public bool Allow { get; set; }
    }
    
}
