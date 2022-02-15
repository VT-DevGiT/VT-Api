using Synapse.Api;
using Synapse.Api.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using VT_Api.Reflexion;
using VT_Api.Extension;
using synItemManager = Synapse.Api.Items.ItemManager;

namespace VT_Api.Core.Items
{
    public class ItemManager
    {
        public const string KeySynapseItemData = "VtScript";

        private readonly List<VtCustomItemInfo> customItems = new List<VtCustomItemInfo>();

        internal ItemManager() { }


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

        internal void Init()    
        {
            // TODO EVENT
/*          Server.Get.Events.Player.PlayerDropItemEvent += OnDrop;
            Server.Get.Events.Player.PlayerItemUseEvent += OnUse;
            Server.Get.Events.Player.PlayerPickUpItemEvent += OnPickUp;
            Server.Get.Events.Player.PlayerChangeItemEvent += OnChangeItem;
            Server.Get.Events.Player.PlayerShootEvent += OnShoot;
            Server.Get.Events.Player.PlayerReloadEvent += OnReload;
            Server.Get.Events.Player.PlayerDamageEvent += OnDamage;*/
        }

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

    }
}
