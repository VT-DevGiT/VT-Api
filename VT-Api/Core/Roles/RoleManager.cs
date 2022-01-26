using Synapse;
using Synapse.Api;
using Synapse.Api.Events.SynapseEventArguments;
using System.Collections.Generic;
using UnityEngine;
using VT_Api.Extension;

namespace VT_Api.Core.Roles
{
    public class RoleManager
    {
        public Dictionary<Player, int> OldPlayerRole = new Dictionary<Player, int>();

        internal RoleManager() { }

        internal void Init()
        {
            Synapse.Api.Events.EventHandler.Get.Player.PlayerSetClassEvent += OnSetClass;
            Synapse.Api.Events.EventHandler.Get.Player.PlayerDeathEvent += OnPlayerDeath;
            Synapse.Api.Events.EventHandler.Get.Player.PlayerKeyPressEvent += OnPressKey;
        }

        private void OnPressKey(PlayerKeyPressEventArgs ev)
        {
            if (ev.Player.CustomRole is IVtRole role)
            {
                if (ev.Player.RealTeam == Team.SCP && ev.KeyCode >= KeyCode.Alpha0 && ev.KeyCode <= KeyCode.Alpha9)
                {
                    if (!role.CallPower((byte)(ev.KeyCode - KeyCode.Slash), out var message))
                        message = "<color=red>" + message + "</color>";
                    
                    ev.Player.GiveTextHint(message, 3);
                }
                else if (ev.Player.RealTeam != Team.SCP && ev.KeyCode >= KeyCode.Alpha5 && ev.KeyCode <= KeyCode.Alpha9)
                {
                    if (!role.CallPower((byte)(ev.KeyCode - KeyCode.Alpha4), out var message))
                        message = "<color=red>" + message + "</color>";

                    ev.Player.GiveTextHint(message, 3);
                }
            }
        }

        private void OnSetClass(PlayerSetClassEventArgs ev)
        {
            if(ev.Player.CustomRole is IVtRole role && !role.Spawned)
                role.InitAll(ev);
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
