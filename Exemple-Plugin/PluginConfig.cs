using Synapse.Config;
using System.Collections.Generic;
using System.ComponentModel;
using VT_Api.Config;

namespace Exemple_Plugin
{
    public class PluginConfig : IConfigSection
    {
        [Description("The config of the Spy")]
        public SerializedPlayerRole SpyRoleConfig { get; set; } = new SerializedPlayerRole()
        {
            Health = 75,
            Inventory = new SerializedPlayerInventory()
            {
                Ammo = new SerializedAmmo()
                {
                    Ammo12 = 75,
                    Ammo44 = 75,
                    Ammo5 = 75,
                    Ammo7 = 75,
                    Ammo9 = 75
                },
                Items = new List<SerializedPlayerItem>()
                {
                    new SerializedPlayerItem()
                    {
                        Chance = 100,
                        ID = 10,
                    },
                    new SerializedPlayerItem()
                    {
                        Chance = 100,
                        ID = 5,
                        Durabillity = 10,
                    },
                    new SerializedPlayerItem()
                    {
                        Chance = 100,
                        ID = 21,
                        Durabillity = 10,
                    },
                    new SerializedPlayerItem()
                    {
                        Chance = 100,
                        ID = 36,
                        Durabillity = 10,
                    },
                }
            }
            


        };

        [Description("Spawn chance per respawn")]
        public short ChanceSpawn = 10;
    }
}
