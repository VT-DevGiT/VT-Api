using Synapse.Api;
using VT_Api.Core.Events.EventArguments;

namespace VT_Api.Core.Events
{
    public class ScpEvents
    {
        internal ScpEvents() { }


        #region Properties & Variable
        public Scp106Events Scp106 { get; } = new Scp106Events();
        #endregion

        #region Events
        public class Scp106Events
        {
            internal Scp106Events() { }

            #region Events
            public event Synapse.Api.Events.EventHandler.OnSynapseEvent<PortalUseEventArgs> PortalUseEvent;
            #endregion

            #region Invoke106Events
            internal void InvokePortalUseEvent(Player player, ref bool allow)
            {
                var ev = new PortalUseEventArgs
                {
                    Scp106 = player,
                    Allow = allow
                };

                PortalUseEvent?.Invoke(ev);

                allow = ev.Allow;
            }
            #endregion
        }
        #endregion
    }
}