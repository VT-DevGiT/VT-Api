using Synapse;
using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api.Items;
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

        public virtual string ScreenName { get; set; } = null;
        public virtual string MessagePickUp { get; set; } = null;
        public virtual string MessageChangeTo { get; set; } = null;
        public VtItemInformation Info { get; set; }

        #endregion

        #region Constructors & Destructor
        public AbstractItem()
        {

        }
        #endregion

        #region Methods
        protected virtual void ChangeToItem(PlayerChangeItemEventArgs ev)
        {
            if (!string.IsNullOrEmpty(MessageChangeTo))
            {
                string message = Regex.Replace(MessageChangeTo, "%Name%", ScreenName, RegexOptions.IgnoreCase);
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
                string message = Regex.Replace(MessagePickUp, "%Name%", ScreenName, RegexOptions.IgnoreCase);
                ev.Player.GiveTextHint(message);
            }
        }
        #endregion
    }
}
