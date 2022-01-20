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

        public int TeamID { get; set; }

        public bool Custom { get; set; }

        public List<RespawnRoleInfo> Roles { get; set; } = new List<RespawnRoleInfo>();

        public int AmountOfPlayer { get; set; }
    }

    public struct RespawnRoleInfo
    {
        public bool Optional;

        public int RoleID;

        public int Min;

        public int Max;

        public Player[] PriorityPlayer;
    }
}
