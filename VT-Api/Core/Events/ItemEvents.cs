using InventorySystem.Items.ThrowableProjectiles;
using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<RemoveLimitItemEventArgs> RemoveLimitItemEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<RemoveAmmoEventArgs> RemoveLimitAmmoEvent;
        public event Synapse.Api.Events.EventHandler.OnSynapseEvent<CheckLimitItemEventArgs> CheckLimitItemEvent;
        #endregion

        #region Invoke
        internal void InvokeRemoveLimitItemEvent(Player player, Dictionary<ItemCategory, int> catergoryMax, ref List<SynapseItem> removItems)
        {
            var ev = new RemoveLimitItemEventArgs
            {
                Player = player,
                CatergoryMax = new ReadOnlyDictionary<ItemCategory, int>(catergoryMax),
                RemovItem = removItems
            };

            RemoveLimitItemEvent.Invoke(ev);

            removItems = ev.RemovItem;
        }

        internal void InvokeCheckLimitItemEvent(Player player, SynapseItem item, int cantgoryMax, ref bool allow)
        {
            var ev = new CheckLimitItemEventArgs
            {
                Player = player,
                CategoryMax = cantgoryMax,
                ExedentingItem = item,
                Allow = allow
            };

            CheckLimitItemEvent.Invoke(ev);

            allow = ev.Allow;
        }

        internal void InvokeRemoveLimitAmmoEvent(Player player, Dictionary<AmmoType, ushort> catergoryMax, ref Dictionary<AmmoType, ushort> removAmmo)
        {
            var ev = new RemoveAmmoEventArgs
            {
                Player = player,
                CatergoryMax = new ReadOnlyDictionary<AmmoType, ushort>(catergoryMax),
                RemovAmmo = removAmmo
            };

            RemoveLimitAmmoEvent.Invoke(ev);

            removAmmo = ev.RemovAmmo;
        }

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