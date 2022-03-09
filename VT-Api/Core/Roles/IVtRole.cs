using Synapse.Api.Events.SynapseEventArguments;

namespace VT_Api.Core.Roles
{
    public interface IVtRole
    {
        void InitAll(PlayerSetClassEventArgs ev);

        bool CallPower(byte power, out string message);

        bool Spawned { get; set; }

    }
}
