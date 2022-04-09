using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using VT_Api.Extension;
using VT_Api.Reflexion;

using synEvents = Synapse.Api.Events.EventHandler;
using synItemManager = Synapse.Api.Items.ItemManager;

namespace VT_Api.Core.Items
{
    public class ItemManager
    {

        #region Properties & Variable
        public const string KeySynapseItemData = "VtScript";

        private readonly List<VtCustomItemInfo> customItems = new List<VtCustomItemInfo>();
        #endregion

        #region Constructor & Destructor
        internal ItemManager() { }

        internal void Init()
        {
            synEvents.Get.Player.PlayerDropItemEvent += OnDrop;
            synEvents.Get.Player.PlayerItemUseEvent += OnUse;
            synEvents.Get.Player.PlayerPickUpItemEvent += OnPickUp;
            synEvents.Get.Player.PlayerChangeItemEvent += OnChangeItem;
            synEvents.Get.Player.PlayerShootEvent += OnShoot;
            synEvents.Get.Player.PlayerReloadEvent += OnReload;
            synEvents.Get.Player.PlayerDamageEvent += OnDamage;
        }
        #endregion

        #region Methods

        /// <returns><see langword="null"/> if id is not register in the API</returns>
        public IItem GetNewScript(int ID)
        {
            var customItem = customItems.Find(i => i.Info.ID == ID);

            if (customItem.Script == null)
                return null;

            if (customItem.Script.GetConstructors().Any(x => x.GetParameters().Count() == 1 && x.GetParameters().First().ParameterType == typeof(VtItemInformation)))
                return (IItem)Activator.CreateInstance(customItem.Script, new object[] { customItem.Info });

            var script = (IItem)Activator.CreateInstance(customItem.Script);

            if (script.Info == null)
                script.SetField(nameof(script.Info), customItem.Info);
            return script;
        }

        /// <returns><see langword="null"/> if the item ave no script or if <paramref name="item"/> is <see langword="null"/> else return the <see cref="IItem"/></returns>
        public IItem GetScript(SynapseItem item) 
            => item?.ItemData[KeySynapseItemData] as IItem;

        /// <returns><see langword="null"/> if the item ave no script as a <see cref="IWeapon"> or if <paramref name="item"/> is <see langword="null"/> else return the <see cref="IWeapon"/></returns>
        public IWeapon GetWeaponScript(SynapseItem item)
            => item?.ItemData[KeySynapseItemData] as IWeapon;

        /// <returns><see langword="false"/> if item ave no script or if <paramref name="item"/> is <see langword="null"/> else return <see langword="true"/></returns>
        public bool TryGetScript(SynapseItem item, out IItem script) 
            => (script = GetScript(item)) != null;

        /// <returns><see langword="false"/> if item ave no script as a <see cref="IWeapon"> or if <paramref name="item"/> is <see langword="null"/> else return <see langword="true"/></returns>
        public bool TryGetWeaponScript(SynapseItem item, out IWeapon script)
            => (script = GetWeaponScript(item)) != null;

        public void RegisterCustomItem(IItem item)
        {
            if (item.Info == null || item.Info == default)
                throw new NullReferenceException($"try to register : {item.GetType().Name}\n\tProperty \"info\" not set !");
            customItems.Add(new VtCustomItemInfo(item));
            synItemManager.Get.RegisterCustomItem(item.Info);

        }

        public void RegisterCustomItem(Type item, int id, string name, ItemType baseItem)
        {
            customItems.Add(new VtCustomItemInfo(item, id, baseItem, name));
            synItemManager.Get.RegisterCustomItem(new CustomItemInformation() { BasedItemType = baseItem, ID = id, Name = name });
        }
        #endregion

        #region Events
        private void OnDrop(PlayerDropItemEventArgs ev)
        {
            if (ev.Allow && TryGetScript(ev.Item, out var script))
            {
                var @throw = ev.Throw; 
                ev.Allow = script.Drop(ref @throw);
                ev.Throw = @throw;
            }
        }

        private void OnDamage(PlayerDamageEventArgs ev)
        {
            if (!ev.Allow)
                return;

            var damage = ev.Damage;
            
            if (TryGetScript(ev.Killer?.ItemInHand, out var script) && script is IWeapon weapon)
                ev.Allow = weapon.Attack(ev.Victim, ref damage, ev.DamageType);

            if (TryGetScript(ev.Victim?.ItemInHand, out var item))
                ev.Allow &= item.Damage(ref damage, ev.DamageType);

            ev.Damage = damage;
        }

        private void OnReload(PlayerReloadEventArgs ev)
        {
            if (ev.Allow && TryGetScript(ev.Item, out var script) && script is IWeapon weapon)
                ev.Allow = weapon.Realod();

        }

        private void OnShoot(PlayerShootEventArgs ev)
        {
            if (ev.Allow && TryGetScript(ev.Weapon, out var script) && script is IWeapon weapon)
            {
                if (ev.Target != null)
                    ev.Allow = weapon.Shoot(ev.TargetPosition, ev.Target);
                else
                    ev.Allow = weapon.Shoot(ev.TargetPosition);
            }
        }

        private void OnChangeItem(PlayerChangeItemEventArgs ev)
        {
            if (!ev.Allow)
                return;
            if (ev.NewItem.IsDefined() && TryGetScript(ev.NewItem, out var newItem))
                ev.Allow = newItem.Change(true);
            if (ev.OldItem.IsDefined() && TryGetScript(ev.OldItem, out var oldItem))
                ev.Allow &= oldItem.Change(false);

        }

        private void OnPickUp(PlayerPickUpItemEventArgs ev)
        {
            if (ev.Allow && TryGetScript(ev.Item, out var item))
                ev.Allow = item.PickUp(ev.Player);
        }

        private void OnUse(PlayerItemInteractEventArgs ev)
        {
            if (ev.Allow && TryGetScript(ev.CurrentItem, out var item))
                ev.Allow = item.Use(ev.State);
        }
        #endregion

        #region Structur
        private struct VtCustomItemInfo
        {
            public VtCustomItemInfo(Type script, int id, ItemType baseItem, string name)
            {
                Script = script;
                Info = new VtItemInformation(id,baseItem, name);
            }

            public VtCustomItemInfo(IItem script)
            {
                Script = script.GetType();
                Info = script.Info;
            }

            public Type Script;

            public VtItemInformation Info;
        }
        #endregion
    }
}
