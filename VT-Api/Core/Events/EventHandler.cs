﻿using System.Linq;
using UnityEngine;

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

        public GrenadEvents Grenade { get; } = new GrenadEvents();

        private VT_Api.Config.VtApiConfiguration Conf => VtController.Get.Configs.VtConfiguration;

        private bool firstLoaded = false;
        #endregion

        #region Constructor & Destructor
        internal EventHandler()
        {
            SyanpseEventHandler.Get.Player.PlayerJoinEvent += PlayerJoin;
            SyanpseEventHandler.Get.Round.WaitingForPlayersEvent += Waiting;
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
        // Vt Api Init
        private void Waiting()
        {
            if (!firstLoaded)
            {
                firstLoaded = true;
                VtController.Get.Commands.GenerateCommandCompletion();
            }
        }
        private void PlayerJoin(Synapse.Api.Events.SynapseEventArguments.PlayerJoinEventArgs ev)
        {
            //DataBase
        }

        //Add SheildManager

        //Fix Som vanila Bugs
        private void OnRaOverwatchFix(Synapse.Api.Events.SynapseEventArguments.RemoteAdminCommandEventArgs ev)
        {
            var args = ev.Command.Split(' ');
            if (args[0].ToUpper() != "OVERWATCH" || args.Count() <= 1) return;
            var ids = args[1].Split('.');
            foreach (var id in ids)
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                var player = Synapse.Server.Get.GetPlayer(int.Parse(id));
                if (player != null)
                    player.ItemInHand = null;
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