using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Items
{
    public interface IItem
    {
        VtItemInformation Info { get; set; }
        int ID { get; }
        ItemType ItemType { get; }
        string Name { get; }
    }
}
