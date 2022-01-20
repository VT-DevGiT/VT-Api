using Scp914;
using Synapse.Api;
using Synapse.Api.Items;
using System;
using VT_Api.Core.Events.EventArguments;

namespace VT_Api.Core.Events
{
    public class MapEvents
    {
        internal MapEvents() { }

        #region Events
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<WarHeadInteracteEventArgs> WarHeadStartEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<WarHeadInteracteEventArgs> WarheadUnlockEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<WarHeadInteracteEventArgs> WarheadStopEventEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<CassieAnnouncementEventArgs> CassieAnnouncementEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<GeneratorActivatedEventArgs> GeneratorActivatedEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<ElevatorIneractEventArgs> ElevatorIneractEvent;
        [Obsolete("Use synapse Locker Event (synapse 8.3)")]
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<LockerInteractEventArgs> LockerInteractEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<Scp914ActivateEventArgs> Scp914ActivateEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<Scp914UpgradeItemEventArgs> Scp914UpgradeItemEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<Change914KnobSettingEventArgs> Scp914changeSettingEvent;
        #endregion

        #region Invoke
        internal void InvokeWarheadStopEvent(Player player, ref bool allow)
        {
            var ev = new WarHeadInteracteEventArgs()
            {
                Player = player,
                Allow = allow
            };

            WarheadStopEventEvent?.Invoke(ev);

            allow = ev.Allow;
        }

        internal void InvokeWarHeadStartEvent(Player player, ref bool allow)
        {
            var ev = new WarHeadInteracteEventArgs()
            {
                Player = player,
                Allow = allow
            };

            WarHeadStartEvent?.Invoke(ev);

            allow = ev.Allow;
        }

        internal void InvokeUnlockWarheadEvent(Player player, ref bool allow)
        {
            var ev = new WarHeadInteracteEventArgs()
            {
                Player = player,
                Allow = allow
            };

            WarheadUnlockEvent?.Invoke(ev);

            allow = ev.Allow;
        }

        internal void InvokeChange914KnobSettingEvent(Player player, ref bool allow)
        {
            var ev = new Change914KnobSettingEventArgs()
            {
                Player = player,
                Allow = allow
            };

            Scp914changeSettingEvent?.Invoke(ev);

            allow = ev.Allow;
        }

        internal void InvokeElevatorIneractEvent(Player player, Elevator elevator, ref bool allow)
        {
            var ev = new ElevatorIneractEventArgs()
            {
                Player = player,
                Elevator = elevator,
                Allow = allow
            };

            ElevatorIneractEvent?.Invoke(ev);

            allow = ev.Allow;
        }

        internal void InvokeScp914ActivateEvent(Player player, ref bool allow)
        {
            var ev = new Scp914ActivateEventArgs()
            {
                Player = player,
                Allow = allow
            };

            Scp914ActivateEvent?.Invoke(ev);

            allow = ev.Allow;
        }

        internal void InvokeCassieAnnouncementEvent(ref string words, ref bool makeHold, ref bool makeNoise, ref bool allow)
        {
            var ev = new CassieAnnouncementEventArgs()
            {
                Allow = allow,
                MakeHold = makeHold,
                MakeNoise = makeNoise,
                Words = words
            };

            CassieAnnouncementEvent?.Invoke(ev);

            allow = ev.Allow;
            makeNoise = ev.MakeNoise;
            makeHold = ev.MakeHold;
            words = ev.Words;
        }

        internal void InvokeScp914UpgradeItemEvent(Scp914KnobSetting setting, SynapseItem olditem, ref SynapseItem newItem, ref bool keepOldItem)
        {
            var ev = new Scp914UpgradeItemEventArgs
            {
                Item = olditem,
                NewItem = newItem,
                Setting = setting,
                KeepOldItem = keepOldItem
            };

            Scp914UpgradeItemEvent?.Invoke(ev);

            newItem = ev.NewItem;
            keepOldItem = ev.KeepOldItem;
        }

        internal void InvokeGeneratorActivatedEvent(Generator generator)
        {
            var ev = new GeneratorActivatedEventArgs
            {
                Generator = generator
            };

            GeneratorActivatedEvent?.Invoke(ev);
        }
        #endregion
    }
}