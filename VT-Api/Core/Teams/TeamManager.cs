using Synapse.Api;
using Synapse.Api.Events.SynapseEventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using VT_Api.Core.Enum;

namespace VT_Api.Core.Teams
{
    public class TeamManager
    {

        #region Properties & Variable
        public static TeamManager Get => VtController.Get.Team;

        public RespawnTeamInfo NextRespawnInfo { get; set; } = new RespawnTeamInfo();
        #endregion

        #region Constructor & Destructor
        internal TeamManager() { }
        #endregion

        #region Methods
        internal void Init()
        {
            Synapse.Api.Events.EventHandler.Get.Round.TeamRespawnEvent += OnRespawn;
        }

        #endregion

        #region Events
        private void OnRespawn(TeamRespawnEventArgs ev)
        {
            if (NextRespawnInfo.TeamID == (int)TeamID.None)
                return;

            ev.Allow = false;

            if (!NextRespawnInfo.Custom)
            {
                Synapse.Api.Teams.TeamManager.Get.SpawnTeam(NextRespawnInfo.TeamID, ev.Players);
                return;
            }

            if (NextRespawnInfo.Roles.Any())
            {
                List<Player> players = ev.Players;
                List<RespawnRoleInfo> rolesinfos = NextRespawnInfo.Roles.ToList();

                if (players.Count > NextRespawnInfo.AmountOfPlayer)
                {
                    var newplayers = RoleType.Spectator.GetPlayers().Where(p => !players.Contains(p)).ToList();
                    newplayers.ShuffleList();
                    newplayers.RemoveRange(0, players.Count - NextRespawnInfo.AmountOfPlayer);
                }
                else if (players.Count < NextRespawnInfo.AmountOfPlayer)
                {

                }

                foreach (var roleinfo in rolesinfos)
                {
                    
                }
                throw new NotImplementedException();
            }
        }
        #endregion

        public static class Groupe
        {
            #region Ally
            public static int[] SCPally = { (int)TeamID.SCP };
            
            public static int[] NetralSCPally = { (int)TeamID.NetralSCP, (int)TeamID.SHA };

            public static int[] BerserkSCPally = { };

            public static int[] MTFally = { (int)TeamID.NTF, (int)TeamID.RSC, (int)TeamID.VIP, (int)TeamID.CDM, (int)TeamID.U2I, (int)TeamID.ASI, (int)TeamID.AL1 };

            public static int[] RSCally = { (int)TeamID.NTF, (int)TeamID.RSC, (int)TeamID.VIP, (int)TeamID.CDM, (int)TeamID.U2I, (int)TeamID.ASI, (int)TeamID.AL1 };

            public static int[] VIPally = { (int)TeamID.NTF, (int)TeamID.RSC, (int)TeamID.VIP, (int)TeamID.CDM, (int)TeamID.U2I, (int)TeamID.ASI, (int)TeamID.AL1 };

            public static int[] CHIally = { (int)TeamID.CHI, (int)TeamID.CDP };

            public static int[] CDPally = { (int)TeamID.CHI, (int)TeamID.CDP };

            public static int[] SHAally = { (int)TeamID.SCP, (int)TeamID.SHA, (int)TeamID.NetralSCP };

            public static int[] ANDally = { (int)TeamID.AND };
            #endregion

            #region Enemy
            public static int[] SCPenemy = { (int)TeamID.NTF, (int)TeamID.RSC, (int)TeamID.VIP, (int)TeamID.CDM, (int)TeamID.U2I, (int)TeamID.ASI, (int)TeamID.AL1, (int)TeamID.CDP };

            public static int[] NetralSCPennemy = { (int)TeamID.CHI };

            public static int[] BerserkSCPennemy = { (int)TeamID.NTF, (int)TeamID.RSC, (int)TeamID.VIP, (int)TeamID.CDM, (int)TeamID.U2I, (int)TeamID.ASI, (int)TeamID.AL1, (int)TeamID.NetralSCP, (int)TeamID.BerserkSCP, (int)TeamID.SCP, (int)TeamID.CHI, (int)TeamID.CDP, (int)TeamID.SHA, (int)TeamID.BerserkSCP, (int)TeamID.AND };

            public static int[] MTFenemy = { (int)TeamID.SCP, (int)TeamID.CHI, (int)TeamID.CDP, (int)TeamID.SHA, (int)TeamID.BerserkSCP, (int)TeamID.AND };

            public static int[] RSCennemy = { (int)TeamID.SCP, (int)TeamID.CHI, (int)TeamID.CDP, (int)TeamID.SHA, (int)TeamID.BerserkSCP, (int)TeamID.AND };

            public static int[] VIPennemy = { (int)TeamID.SCP, (int)TeamID.CHI, (int)TeamID.CDP, (int)TeamID.SHA, (int)TeamID.BerserkSCP, (int)TeamID.AND };

            public static int[] CHIenemy = { (int)TeamID.NTF, (int)TeamID.RSC, (int)TeamID.VIP, (int)TeamID.CDM, (int)TeamID.U2I, (int)TeamID.ASI, (int)TeamID.AL1, (int)TeamID.NetralSCP, (int)TeamID.BerserkSCP };

            public static int[] CDPennemy = { (int)TeamID.NTF, (int)TeamID.RSC, (int)TeamID.VIP, (int)TeamID.CDM, (int)TeamID.U2I, (int)TeamID.ASI, (int)TeamID.AL1, (int)TeamID.SCP, (int)TeamID.SHA, (int)TeamID.BerserkSCP };

            public static int[] SHAennemy = { (int)TeamID.NTF, (int)TeamID.RSC, (int)TeamID.VIP, (int)TeamID.CDM, (int)TeamID.U2I, (int)TeamID.ASI, (int)TeamID.AL1, (int)TeamID.CDP, (int)TeamID.BerserkSCP };

            public static int[] ANDennemy = { (int)TeamID.NTF, (int)TeamID.RSC, (int)TeamID.VIP, (int)TeamID.CDM, (int)TeamID.U2I, (int)TeamID.ASI, (int)TeamID.AL1 };
            #endregion
        }
    }
}
