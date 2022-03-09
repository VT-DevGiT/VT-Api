using Synapse.Api.Items;

namespace VT_Api.Core.Items
{
    public interface IItem
    {
        VtItemInformation Info { get; }
        SynapseItem Item { get; set; }
        bool AllowDrop(bool Throw);
            
    }
}
