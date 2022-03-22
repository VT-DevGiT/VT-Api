using System;

namespace VT_Api.Core.Events
{
    public class ServerEvents
    {
        internal ServerEvents() { }

        #region Events
        public event Action SynapsePostLoad;
        #endregion

        #region Invoke
        internal void SynapsePostLoadPostEvent()
        {
            SynapsePostLoad?.Invoke();
        }
        #endregion
    }
}