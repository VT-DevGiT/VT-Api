using Synapse.Api.Events.SynapseEventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Core.Roles
{
    public interface IVtRole
    {
        void InitAll(PlayerSetClassEventArgs ev);

        bool CallPower(byte power, out string message);

        bool Spawned { get; set; }

    }
}
