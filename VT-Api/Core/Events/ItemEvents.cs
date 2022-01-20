using InventorySystem.Items.ThrowableProjectiles;
using Synapse.Api.Enum;
using Synapse.Api.Items;
using System;
using VT_Api.Core.Events.EventArguments;

namespace VT_Api.Core.Events
{
    public class ItemEvents
    {
        internal ItemEvents() { }

        #region Events
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<ChangeIntoFragEventArgs> ChangeIntoFragEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<ExplosionGrenadeEventArgs> ExplosionGrenadeEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<CollisionEventArgs> CollisionEvent;
        #endregion

        #region Invoke
        internal void InvokeChangeIntoFragEvent(SynapseItem item, TimeGrenade grenade, GrenadeType type, ref bool allow)
        {
            var ev = new ChangeIntoFragEventArgs
            {
                Item = item,
                Grenade = grenade,
                Type = type,
                Allow = allow
            };

            ChangeIntoFragEvent?.Invoke(ev);

            allow = ev.Allow;
        }

        internal void InvokeCollisionEvent(SynapseItem item, ref bool allow)
        {
            var ev = new CollisionEventArgs
            {
                Item = item,
                Allow = allow
            };

            CollisionEvent?.Invoke(ev);

            allow = ev.Allow;
        }

        internal void InvokeExplosionGrenadeEvent(TimeGrenade grenade, GrenadeType type, ref bool allow)
        {
            var ev = new ExplosionGrenadeEventArgs
            {
                Allow = allow,
                Type = type,
                Grenade = grenade
            };

            ExplosionGrenadeEvent?.Invoke(ev);

            allow = ev.Allow;
        }
        #endregion
    }
}