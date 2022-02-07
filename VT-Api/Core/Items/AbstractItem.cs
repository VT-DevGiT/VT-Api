using Synapse;
using Synapse.Api.Events.SynapseEventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VT_Api.Core.Items
{
    public abstract class AbstractItem : IItem
    {
        #region Attributes & Properties
        public int      ID          => Info.ID;
        public ItemType ItemType    => Info.BasedItemType;
        public string   Name        => Info.Name;

        public virtual string ScrenName { get; set; } = null;
        public virtual string MessagePickUp { get; set; } = null;
        public virtual string MessageChangeTo { get; set; } = null;
        public VtItemInformation Info { get; set; }

        

        #endregion

        #region Constructors & Destructor
        public AbstractItem() => Event();
        #endregion

        #region Methods

        protected virtual void Event()
        {
            Server.Get.Events.Player.PlayerDropItemEvent += OnDrop;
            Server.Get.Events.Player.PlayerItemUseEvent += OnUse;
            Server.Get.Events.Player.PlayerPickUpItemEvent += OnPickUp;
            Server.Get.Events.Player.PlayerChangeItemEvent += OnChangeItem;
        }

        private void OnChangeItem(PlayerChangeItemEventArgs ev)
        {
            if (ev.NewItem?.ID == ID)
                this.ChangeToItem(ev);
            else if (ev.OldItem?.ID == ID)
                this.ChangedFromItem(ev);
        }

        protected virtual void ChangeToItem(PlayerChangeItemEventArgs ev)
        {
            if (!string.IsNullOrEmpty(MessageChangeTo))
            {
                string message = Regex.Replace(MessageChangeTo, "%Name%", ScrenName, RegexOptions.IgnoreCase);
                ev.Player.GiveTextHint(message);
            }
        }
        protected virtual void ChangedFromItem(PlayerChangeItemEventArgs ev) { }
        protected virtual void Use(PlayerItemInteractEventArgs ev) { }
        protected virtual void Drop(PlayerDropItemEventArgs ev) { }
        protected virtual void PickUp(PlayerPickUpItemEventArgs ev)
        {
            if (!string.IsNullOrEmpty(MessagePickUp))
            {
                string message = Regex.Replace(MessagePickUp, "%Name%", ScrenName, RegexOptions.IgnoreCase);
                ev.Player.GiveTextHint(message);
            }
        }
        #endregion

        #region Events
        private void OnUse(PlayerItemInteractEventArgs ev)
        {
            if (ev.CurrentItem.ID == ID)
                this.Use(ev);
        }
        private void OnPickUp(PlayerPickUpItemEventArgs ev)
        {
            if (ev.Item.ID == ID)
                this.PickUp(ev);
        }
        private void OnDrop(PlayerDropItemEventArgs ev)
        {
            if (ev.Item.ID == ID)
                this.Drop(ev);
        }

        #endregion
    }
}
