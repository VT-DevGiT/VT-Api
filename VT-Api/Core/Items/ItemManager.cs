using Synapse.Api.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using VT_Api.Reflexion;

using synItemManager = Synapse.Api.Items.ItemManager;
using synEvents = Synapse.Api.Events.EventHandler;
using Synapse.Api.Events.SynapseEventArguments;

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

        /// <returns><see langword="null"/> if the item ave no script</returns>
        public IItem GetScript(SynapseItem item) 
            => item.ItemData[KeySynapseItemData] as IItem;
        public bool TryGetScript(SynapseItem item, out IItem script) 
            => (script = GetScript(item)) != null;

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
            if (TryGetScript(ev.Item, out var script))
                ev.Allow = script.AllowDrop(ev.Throw);


        }

        private void OnDamage(PlayerDamageEventArgs ev)
        {
            
            if (ev.Killer?.ItemInHand != null && TryGetScript(ev.Killer.ItemInHand, out var script) && script is IWeapon weapon)
                ev.Allow = weapon.AllowAttack(ev.Victim, ev.Damage, ev.DamageType);
            //TODO
            
            /*if (ev.Victim.ItemInHand != null && TryGetScript(ev.Victim.ItemInHand, out var item))
                ev.Allow = item.*/
            
        }

        private void OnReload(PlayerReloadEventArgs ev)
        {
            throw new NotImplementedException();
        }

        private void OnShoot(PlayerShootEventArgs ev)
        {
            throw new NotImplementedException();
        }

        private void OnChangeItem(PlayerChangeItemEventArgs ev)
        {
            throw new NotImplementedException();
        }

        private void OnPickUp(PlayerPickUpItemEventArgs ev)
        {
            throw new NotImplementedException();
        }

        private void OnUse(PlayerItemInteractEventArgs ev)
        {
            throw new NotImplementedException();
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
