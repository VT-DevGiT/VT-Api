﻿using Synapse.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple_Plugin
{
    public class PluginTranslation : IPluginTranslation
    {
        public string ClassName { get; set; } = "SPY";

        public string SpawnMessage { get; set; } = "You Spawn as a %RoleName%\nKill the others NTF.";

    }
}
