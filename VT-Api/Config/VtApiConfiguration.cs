using Synapse.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Config
{
    public class VtApiConfiguration : AbstractConfigSection
    {
        [Description("If active then the plugins and the api this metron is up to date on its own")]
        public bool AutoUpdate { get; set; } = true;

    }
}
