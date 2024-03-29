﻿using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api.Items;
using System.Text.RegularExpressions;

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

        public Player Holder => Item.ItemHolder;

        private SynapseItem item;
        public SynapseItem Item { get => item; 
            set
            {
                if (item == default)
                    item = value;
            }
        }

        #endregion

        #region Constructors & Destructor
        public AbstractItem()
        {

        }
        #endregion

        #region Methods
        public virtual bool Drop(ref bool Throw) => true;

        public virtual bool Damage(ref float damage, DamageType damageType) => true;

        /// <summary>
        /// This method display (by default) the message <see cref="MessageChangeTo"/>
        /// </summary>
        /// <param name="isNewItem">true if the new item is this script</param>
        public virtual bool Change(bool isNewItem)
        {
            if (!string.IsNullOrEmpty(MessageChangeTo))
            {
                string message = Regex.Replace(MessageChangeTo, "%Name%", ScreenName, RegexOptions.IgnoreCase);
                Holder.GiveTextHint(message);
            }
            return true;
        }

        /// <summary>
        /// This method display (by default) the message <see cref="MessagePickUp"/>
        /// </summary>
        public virtual bool PickUp(Player player) 
        {
            if (!string.IsNullOrEmpty(MessagePickUp))
            {
                string message = Regex.Replace(MessagePickUp, "%Name%", ScreenName, RegexOptions.IgnoreCase);
                player.GiveTextHint(message);
            }
            return true;
        }

        public virtual bool Use(ItemInteractState state) => true;

        public virtual void Init() { }
        #endregion
    }
}
