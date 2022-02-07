using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Items
{
    public class ItemManager
    {
        internal ItemManager() { }

        internal void Init()
        {
            Synapse.Api.Events.EventHandler.Get.Round.RoundStartEvent += OnStart;
        }

        internal List<Type> ItemsToRegister = new List<Type>();

        private static bool FistStart = true;
        private void OnStart()
        {            
            if (FistStart)
            {
                foreach (var item in ItemsToRegister)
                {
                    var customItem = Activator.CreateInstance(item) as IItem;
                    var info = customItem.Info;

                    if (info == null)
                        customItem.Info = item.GetCustomAttribute<VtItemInformation>();

                    Synapse.Api.Items.ItemManager.Get.RegisterCustomItem(info);
                }
                FistStart = false;
            }
        }
    }
}
