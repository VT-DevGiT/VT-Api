using Mirror;
using Synapse;
using Synapse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Behaviour
{
    public abstract class RoundBehaviour : NetworkBehaviour
    {
        public RoundBehaviour(bool killAtRoundStart = true)
        {
            if (killAtRoundStart) Server.Get.Events.Round.RoundRestartEvent += Kill;
        }

        public virtual void Kill()
        {
            try
            {
                Destroy(this);
            }
            catch (Exception e)
            {
                Logger.Get.Error($"Vt-Event: RoundBehaviour kill faild!!\n{e}\nStackTrace:\n{e.StackTrace}");
            }
        }
    }
}
