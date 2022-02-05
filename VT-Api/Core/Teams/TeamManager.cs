using Synapse.Api;
using Synapse.Api.Events.SynapseEventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using VT_Api.Core.Enum;
using rnd = UnityEngine.Random;
using Manager = Synapse.Api.Teams.TeamManager;
using Respawning.NamingRules;
using VT_Api.Reflexion;

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

        public string GenerateNtfUnitName(byte maxNubmer = 20)
        {
            var combi = UnitNamingRule.UsedCombinations;
            string regular;
            do
            {
                var arrayOfValues = NineTailedFoxNamingRule.PossibleCodes;
                regular = arrayOfValues[UnityEngine.Random.Range(0, arrayOfValues.Length)] + "-" + UnityEngine.Random.Range(1, maxNubmer).ToString("00");
            }
            while (combi.Contains(regular));
            combi.Add(regular);
            return regular;
        }

        #endregion

        #region Events
        private void OnRespawn(TeamRespawnEventArgs ev)
        {
            if (NextRespawnInfo.TeamID == (int)TeamID.None)
                return;

            if (NextRespawnInfo.AmountOfPlayers != -1)
                RemoveOrFillWithSpectator(ev.Players, NextRespawnInfo.AmountOfPlayers);

            if (!NextRespawnInfo.Roles.Any())
            {
                if (!Manager.Get.IsDefaultSpawnableID(NextRespawnInfo.TeamID))
                { 
                    ev.Team = Respawning.SpawnableTeamType.None;
                    ev.TeamID = NextRespawnInfo.TeamID;
                }
                else
                {
                    ev.Team = NextRespawnInfo.TeamID == (int)TeamID.NTF ? Respawning.SpawnableTeamType.NineTailedFox :
                                                                          Respawning.SpawnableTeamType.ChaosInsurgency;
                }

                NextRespawnInfo = new RespawnTeamInfo();

                if (NextRespawnInfo.Action != null)
                    NextRespawnInfo.Action.Invoke(ev.Players);
                
                if (string.IsNullOrEmpty(NextRespawnInfo.Cassie))
                    Map.Get.Cassie(NextRespawnInfo.Cassie);

                return;
            }

            ev.Allow = false;

            if (NextRespawnInfo.Roles.Any())
            {
                var rolesinfos = NextRespawnInfo.Roles.ToList();

                CustomSpawn(rolesinfos, ev.Players, NextRespawnInfo.Action, NextRespawnInfo.Cassie);
                NextRespawnInfo = new RespawnTeamInfo();
            }
        }

        public void RemoveOrFillWithSpectator(List<Player> players, int amount)
        {
            if (players.Count > amount)
            {
                var newplayers = RoleType.Spectator.GetPlayers().Where(p => !players.Contains(p)).ToList();
                newplayers.ShuffleList();
                newplayers.RemoveRange(0, players.Count - amount);
                players.AddRange(newplayers);
            }
            else if (players.Count < amount)
            {
                players.ShuffleList();
                players.RemoveRange(0, amount - players.Count);
            }
        }

        public void CustomSpawn(List<RespawnRoleInfo> rolesinfos, List<Player> players, Action<List<Player>> action = null, string uniteName = "", string cassie = "")
        {
            var playersSpwned = new List<Player>();

            foreach (var info in rolesinfos.OrderBy(p => p.Priority))
            {

                var amout = info.Min != -1 ? rnd.Range(info.Min, info.Max) : info.Max;
                var PriorityPlys = info.PriorityPlayer.Where(p => players.Contains(p)).ToList();

                for (int i = 0; (i < amout || info.Max == -1) && players.Any(); i++)
                {
                    if (PriorityPlys.Any())
                    {
                        var ply = PriorityPlys[rnd.Range(0, PriorityPlys.Count() - 1)];
                        ply.RoleID = info.RoleID;
                        ply.UnitName = uniteName;

                        players.Remove(ply);
                        PriorityPlys.Remove(ply);
                        playersSpwned.Add(ply);
                    }
                    else
                    {
                        var ply = players[rnd.Range(0, players.Count() - 1)];
                        ply.RoleID = info.RoleID;
                        ply.UnitName = uniteName;

                        players.Remove(ply);
                        playersSpwned.Add(ply);
                    }
                }
            }

            if (action != null)
                action.Invoke(playersSpwned);
            if (string.IsNullOrEmpty(cassie))
                Map.Get.Cassie(cassie);
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
