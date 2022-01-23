using Synapse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Teams
{
    public class RespawnTeamInfo
    {
        public RespawnTeamInfo(int teamId = -1) => TeamID = teamId;
        
        public Action<List<Player>> Action { get; set; }

        public string Cassie { get; set; }

        public int TeamID { get; set; }

        public List<RespawnRoleInfo> Roles { get; } = new List<RespawnRoleInfo>();

        public int AmountOfPlayers { get; set; }
    }

    public struct RespawnRoleInfo
    {
        public int Priority;

        public int RoleID;

        public int Min;

        public int Max;

        public Player[] PriorityPlayer;
    }
}
