using Synapse.Api.Items;
using System.Linq;
using VT_Api.Extension;
using UnityEngine;
using VT_Api.Core.Behaviour;

using SyanpseEventHandler = Synapse.Api.Events.EventHandler;

namespace VT_Api.Core.Events
{
    public class EventHandler
    {

        #region Properties & Variable
        public static EventHandler Get => VtController.Get.Events;

        public ServerEvents Server { get; } = new ServerEvents();

        public PlayerEvents Player { get; } = new PlayerEvents();

        public RoundEvents Round { get; } = new RoundEvents();

        public MapEvents Map { get; } = new MapEvents();

        public ScpEvents Scp { get; } = new ScpEvents();

        public ItemEvents Item { get; } = new ItemEvents();

        #endregion

        #region Constructor & Destructor
        internal EventHandler()
        {
            SyanpseEventHandler.Get.Player.PlayerJoinEvent += PlayerJoin;
            //SyanpseEventHandler.Get.Server.RemoteAdminCommandEvent += OnRaOverwatchFix;
            SyanpseEventHandler.Get.Round.WaitingForPlayersEvent += OnWaiting;
#if DEBUG
            SyanpseEventHandler.Get.Player.PlayerKeyPressEvent += KeyPress;
#endif
        }
        #endregion

        #region Methods
        internal void Init()
        {

        }
        #endregion

        #region Events
        private void OnWaiting()
        {
            SynapseController.Server.Host.gameObject.GetOrAddComponent<ServerStopTrap>();
        }

        private void PlayerJoin(Synapse.Api.Events.SynapseEventArguments.PlayerJoinEventArgs ev)
        {
            //DataBase
        }

        //Add SheildManager

        //Fix vanila Bugs
        private void OnRaOverwatchFix(Synapse.Api.Events.SynapseEventArguments.RemoteAdminCommandEventArgs ev)
        {
            var args = ev.Command.Split(' ');
            if (args.Count() <= 1 || args[0].ToUpper() != "OVERWATCH") return;
            var ids = args[1].Split('.');
            foreach (var id in ids)
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                var player = Synapse.Server.Get.GetPlayer(int.Parse(id));
                if (player != null && player.ItemInHand.IsDefined())
                {
                    player.ItemInHand = SynapseItem.None;
                    MEC.Timing.CallDelayed(0.1f, () =>
                    {
                        if (player != null)
                            player.OverWatch = true;
                    });
                }
            }
        }

#if DEBUG
        private void KeyPress(Synapse.Api.Events.SynapseEventArguments.PlayerKeyPressEventArgs ev)
        {
            switch (ev.KeyCode)
            {
                case KeyCode.Alpha1:

                break;
            }
        }
#endif

        #endregion
    }
}