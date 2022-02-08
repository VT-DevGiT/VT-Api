using Synapse;
using Synapse.Api;
using Synapse.Api.Events.SynapseEventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VT_Api.Core.Enum;
using VT_Api.Core.Events.EventArguments;

namespace Exemple_Plugin
{
    public class EventHandlers
    {
        public EventHandlers()
        {
            //Vt Event
            VtController.Get.Events.Map.GeneratorActivatedEvent += OnGeneratorActivated;
            //Synapse Event
            Server.Get.Events.Player.PlayerSetClassEvent += OnSetClass;
            Server.Get.Events.Round.TeamRespawnEvent += OnRespawn;
        }

        private List<Player> spawnPlayers = new List<Player>();

        private void OnRespawn(TeamRespawnEventArgs ev) => spawnPlayers.AddRange(ev.Players);
        

        private void OnSetClass(Synapse.Api.Events.SynapseEventArguments.PlayerSetClassEventArgs ev)
        {
            if (ev.Role == RoleType.NtfPrivate && spawnPlayers.Contains(ev.Player) && ev.Player.CustomRole == null)
            {
                if (UnityEngine.Random.Range(1f, 100f) <= PluginClass.Instance.Config.ChanceSpawn)
                {
                    ev.Player.RoleID = (int)RoleID.ChaosSpy;
                    spawnPlayers.Clear();
                }
            }
        }

        private void OnGeneratorActivated(GeneratorActivatedEventArgs ev)
            => Server.Get.Map.HeavyController.LightsOut(50);
        
    }
}
