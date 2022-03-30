using Synapse;
using Synapse.Api;
using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api.Roles;
using System;
using System.Collections.Generic;
using UnityEngine;
using VT_Api.Core.Enum;
using VT_Api.Extension;
using SynRoleManager = Synapse.Api.Roles.RoleManager;

namespace VT_Api.Core.Roles
{
    public class RoleManager
    {

        public Dictionary<Player, int> OldPlayerRole { get; } = new Dictionary<Player, int>();

        public static int[] VanilaScpID { get; } = { (int)RoleType.Scp049,   (int)RoleType.Scp0492, (int)RoleType.Scp079,
                                                     (int)RoleType.Scp096,   (int)RoleType.Scp106,  (int)RoleType.Scp173,
                                                     (int)RoleType.Scp93953, (int)RoleType.Scp93989 };
        
        public static RoleManager Get => VtController.Get.Role;


        internal RoleManager() { }

        internal void Init()
        {
            Synapse.Api.Events.EventHandler.Get.Player.PlayerSetClassEvent += OnSetClass;
            Synapse.Api.Events.EventHandler.Get.Player.PlayerDeathEvent += OnPlayerDeath;
            Synapse.Api.Events.EventHandler.Get.Player.PlayerKeyPressEvent += OnPressKey;
        }

        public bool IsVanilla(int roleID)
            => roleID > (int)RoleID.None && roleID <= SynRoleManager.HighestRole;
        

        public int GetHierachy(int roleID)
        {
            if (IsVanilla(roleID))
                return ((RoleType)roleID).GetHierachy();
            var hierarchy = SynRoleManager.Get.GetCustomRole((int)roleID) as IHierarchy;
            return hierarchy?.PowerHierachy ?? (int)Hierarchy.Default;
        }

        public int GetHierachy(RoleType role) => role switch
        {
            RoleType.NtfCaptain     => (int)Hierarchy.Captain,
            RoleType.NtfSpecialist  => (int)Hierarchy.Specialist,
            RoleType.NtfSergeant    => (int)Hierarchy.Sergeant,
            RoleType.NtfPrivate     => (int)Hierarchy.Private,
            _                       => (int)Hierarchy.Default,
        };

        public int OldRoleID(Player player)
        {
            if (OldPlayerRole.ContainsKey(player))
                return OldPlayerRole[player];
            return (int)TeamID.None;
        }

        public int OldTeam(Player player)
        {
            if (OldPlayerRole.ContainsKey(player))
            {
                int oldRoleID = OldPlayerRole[player];
                return ((RoleID)oldRoleID).GetTeam();
            }
            return (int)TeamID.None;
        }

        private void OnPressKey(PlayerKeyPressEventArgs ev)
        {
            if (ev.Player.CustomRole is IVtRole role)
            {
                var key = ev.KeyCode;
                if (ev.Player.RealTeam == Team.SCP && key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
                {
                    if (key == KeyCode.Alpha0)
                        key = KeyCode.Alpha9 + 1;

                    if (!role.CallPower((byte)(key - KeyCode.Alpha0 + 1), out var message))
                        message = "<color=red>" + message + "</color>";
                    
                    ev.Player.GiveTextHint(message, 3);
                }
                else if (ev.Player.RealTeam != Team.SCP && (key >= KeyCode.Alpha5 && key <= KeyCode.Alpha9 || key == KeyCode.Alpha0))
                {
                    if (key == KeyCode.Alpha0)
                        key = KeyCode.Alpha9 + 1;

                    if (!role.CallPower((byte)(ev.KeyCode - KeyCode.Alpha5 + 1), out var message))
                        message = "<color=red>" + message + "</color>";

                    ev.Player.GiveTextHint(message, 3);
                }
            }
        }

        private void OnSetClass(PlayerSetClassEventArgs ev)
        {
            if (ev.Player.CustomRole is IVtRole role && !role.Spawned)
            {
                role.InitAll(ev);
            }
        }

        private void OnPlayerDeath(PlayerDeathEventArgs ev)
        {
            if (OldPlayerRole.ContainsKey(ev.Victim))
                 OldPlayerRole[ev.Victim] = ev.Victim.RoleID;
            else OldPlayerRole.Add(ev.Victim, ev.Victim.RoleID);
            
            if (ev.Victim.CustomRole is IScpDeathAnnonce scpDeathAnnonce)
            { 
                var scpName = scpDeathAnnonce.ScpName;
                var unityName = ev.Killer?.Team == Team.MTF ? ev.Killer.UnitName : "UNKNOWN";
                Server.Get.Map.AnnounceScpDeath(scpName, ev.DamageType.GetScpRecontainmentType(ev.Killer), unityName);
            }
        }
    }
}
