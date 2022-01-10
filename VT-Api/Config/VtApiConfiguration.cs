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
        [Description("Juste a stupide Test ¯\\_(ツ)_/¯ ")]
        public bool Train { get; set; } = true;

    }
}
