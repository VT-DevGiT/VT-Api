using Synapse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Roles
{
    public abstract class AbstractRole : Synapse.Api.Roles.IRole
    {
        public Player Player { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void DeSpawn()
        {
            throw new NotImplementedException();
        }

        public void Escape()
        {
            throw new NotImplementedException();
        }

        public List<int> GetEnemiesID()
        {
            throw new NotImplementedException();
        }

        public List<int> GetFriendsID()
        {
            throw new NotImplementedException();
        }

        public int GetRoleID()
        {
            throw new NotImplementedException();
        }

        public string GetRoleName()
        {
            throw new NotImplementedException();
        }

        public int GetTeamID()
        {
            throw new NotImplementedException();
        }

        public void Spawn()
        {
            throw new NotImplementedException();
        }
    }
}
