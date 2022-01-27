using Synapse.Api.Items;
using System;

namespace VT_Api.Core.Items
{
    public class VtItemInformation : Attribute
    {
        public VtItemInformation()
        {

        }

        public VtItemInformation(int id, ItemType basedItemType, string name)
        {
            ID = id;
            BasedItemType = basedItemType;
            Name = name;
        }

        public int ID;

        public ItemType BasedItemType;

        public string Name;

        public static explicit operator VtItemInformation(CustomItemInformation info) => new VtItemInformation(info.ID, info.BasedItemType, info.Name);
        public static implicit operator CustomItemInformation(VtItemInformation info) 
            => new CustomItemInformation() 
            {
                ID = info.ID,
                BasedItemType = info.BasedItemType,
                Name = info.Name,
            };
    }
}
