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
        public RespawnTeamInfo(int teamId = -1, int amountPlayers = -1)
        {
            TeamID = teamId;
            AmountOfPlayers = amountPlayers;
        }
        
        public Action<List<Player>> Action { get; set; }

        public string Cassie { get; set; }

        public int TeamID { get; set; }

        public List<RespawnRoleInfo> Roles { get; } = new List<RespawnRoleInfo>();

        public int AmountOfPlayers { get; set; }
    }

    public struct RespawnRoleInfo
    {
        public RespawnRoleInfo(int roleID)
        {
            RoleID = roleID;
            Priority = 0;
            Max = -1;
            Min = -1;
            PriorityPlayer = new Player[0];
        }

        public RespawnRoleInfo(int roleID, int priority, int max)
        {
            RoleID = roleID;
            Priority = priority;
            Max = max;
            Min = -1;
            PriorityPlayer = new Player[0];
        }

        public RespawnRoleInfo(int roleID, int priority, int max, int min)
        {
            RoleID = roleID;
            Priority = priority;
            Max = max;
            Min = min;
            PriorityPlayer = new Player[0];
        }

        public int Priority;

        public int RoleID;

        public int Min;

        public int Max;

        public Player[] PriorityPlayer;
    }
}
