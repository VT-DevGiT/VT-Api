using Synapse.Api;
using Synapse.Api.Teams;
using System.Collections.Generic;

namespace VT_Api.Core.Teams
{
    public abstract class AbstractTeam : ISynapseTeam
    {
        public SynapseTeamInformation Info { get; set; }

        public abstract List<RespawnRoleInfo> Roles { get; set; }

        public virtual string CurrentUniteName { get; protected set; }

        public virtual string GetSpawnAnnonce(string uniteName) => "";

        public virtual string GetNewUniteName() => "";

        public virtual void Initialise() { }

        public virtual void Spawn(List<Player> players)
        {
            CurrentUniteName = GetNewUniteName();
            TeamManager.Get.CustomSpawn(Roles, players, null, CurrentUniteName, GetSpawnAnnonce(CurrentUniteName));
        }
    }
}
