using Mirror;
using Synapse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Behaviour
{
    public abstract class RoundBehaviour : NetworkBehaviour
    {
        public RoundBehaviour(bool killAtRoundStart)
        {
            if (killAtRoundStart) Server.Get.Events.Round.RoundRestartEvent += Kill;
        }

        public virtual void Kill() => Destroy(this);
        
    }
}
