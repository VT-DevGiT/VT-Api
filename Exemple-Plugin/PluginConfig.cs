using Synapse.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VT_Api.Core.Roles;

namespace Exemple_Plugin
{
    public class PluginConfig : IConfigSection
    {
        public string SpawnMessage { get; set; } = "You Spawn as a %RoleName%\\nKill the others NTF";
        
        public SerializedPlayerRole SpyRoleConfig { get; set; } = new SerializedPlayerRole();
    }
}
