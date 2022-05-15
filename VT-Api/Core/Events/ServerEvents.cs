using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Events
{
    public class ServerEvents
    {
        internal ServerEvents() { }

        #region Events
        public event Action ServerStopEvent;
        #endregion

        #region Invoke
        internal void InvokeServerStopEvent()
        {
            ServerStopEvent?.Invoke();
        }
        #endregion
    }
}
