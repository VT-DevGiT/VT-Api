using Synapse.Translation;
using System;

namespace VT_Api.Config
{
    public class Config
    {
        [Synapse.Api.Plugin.Config(section = "VT-API Configuration")]
        public VtApiConfiguration VtConfiguration { get; }
        internal SynapseTranslation<VtApiTranslation> synapseTranslation;



        internal void Init()
        {
            throw new NotImplementedException();
        }
    }
}
