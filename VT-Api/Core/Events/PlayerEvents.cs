using PlayerStatsSystem;
using Synapse.Api;
using Synapse.Api.Enum;
using System;
using VT_Api.Core.Events.EventArguments;

namespace VT_Api.Core.Events
{
    public class PlayerEvents
    {
        internal PlayerEvents() { }

        #region Events
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<PlayerDamagePostEventArgs> PlayerDamagePostEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<PlayerKillEventArgs> PlayerDeathReasonEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<PlayerDestroyEventArgs> PlayerUnloadEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<PlayerSpeakIntercomEventEventArgs> PlayerSpeakIntercomEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<PlayerSetClassAdvEventArgs> PlayerSetClassAdvEvent;
        #endregion

        #region Invoke

        internal void InvokePlayerSetClassAdvEvent(Player player, RoleType role)
        {
            var ev = new PlayerSetClassAdvEventArgs
            {
                Role = role,
                Player = player
            };

            PlayerSetClassAdvEvent?.Invoke(ev);
        }

        internal void InvokePlayerDeathReasonEvent(Player victim, Player killer, DamageHandlerBase handler, ref string victimeDeathReason)
        {
            var ev = new PlayerKillEventArgs
            {
                DamageHandler = handler,
                DeathReason = victimeDeathReason,
                Killer = killer,
                Victim = victim
            };

            PlayerDeathReasonEvent?.Invoke(ev);

            victimeDeathReason = ev.DeathReason;
        }

        internal void InvokePlayerSpeakIntercomEvent(Player player, ref bool allow)
        {
            var ev = new PlayerSpeakIntercomEventEventArgs
            {
                Player = player,
                Allow = allow
            };

            PlayerSpeakIntercomEvent?.Invoke(ev);

            allow = ev.Allow;
        }

        internal void InvokePlayerDamagePostEvent(Player victim, Player killer, ref float damage, DamageType type, ref bool allow)
        {
            var ev = new PlayerDamagePostEventArgs
            {
                Killer = killer,
                Victim = victim,
                Damage = damage,
                DamageType = type,
                Allow = allow
            };
            PlayerDamagePostEvent?.Invoke(ev);

            damage = ev.Damage;
            allow = ev.Allow;
        }

        internal void InvokePlayerDestroyEvent(Player player)
        {
            var ev = new PlayerDestroyEventArgs
            {
                Player = player,
            };

            PlayerUnloadEvent?.Invoke(ev);
        }
        #endregion
    }
}