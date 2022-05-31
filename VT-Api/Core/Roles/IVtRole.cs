using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api.Roles;

namespace VT_Api.Core.Roles
{
    public interface IVtRole : IRole
    {
        void InitAll(PlayerSetClassEventArgs ev);

        bool CallPower(byte power, out string message);

        bool Spawned { get; set; }

    }
}
