using Synapse.Api.Roles;

namespace VT_Api.Core.Roles
{
    public interface ICustomPhysicalRole : IRole
    {
        void UpdateBody();
    }
}
