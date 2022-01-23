using Synapse.Api;
using Synapse.Api.Teams;
using System.Collections.Generic;

namespace VT_Api.Core.Teams
{
    public abstract class AbstractTeam : ISynapseTeam
    {
        public SynapseTeamInformation Info { get; set; }

        public abstract List<RespawnRoleInfo> Roles { get; set; }

        public abstract string GetSpawnAnnonce();

        public abstract void Initialise();

        public virtual void Spawn(List<Player> players) => TeamManager.Get.CustomSpawn(Roles, players, null, GetSpawnAnnonce());
        
    }
}
