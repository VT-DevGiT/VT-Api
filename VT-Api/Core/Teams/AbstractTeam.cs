using Synapse.Api;
using Synapse.Api.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Teams
{
    public abstract class AbstractTeam : Synapse.Api.Teams.ISynapseTeam
    {
        public SynapseTeamInformation Info { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Initialise()
        {
            throw new NotImplementedException();
        }

        public void Spawn(List<Player> players)
        {
            throw new NotImplementedException();
        }
    }
}
