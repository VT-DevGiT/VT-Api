using Synapse.Api.Roles;

namespace VT_Api.Core.Roles
{
    public interface IScpDeathAnnonce : IRole
    {
        string ScpName { get; }
    }
}