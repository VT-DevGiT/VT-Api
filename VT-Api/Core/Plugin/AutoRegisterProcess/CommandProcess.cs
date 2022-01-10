using Synapse.Api.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Plugin.AutoRegisterProcess
{
    internal class CommandProcess : IContextProcessor
    {
        public void Process(PluginLoadContext context)
        {


            foreach (var commandType in context.Classes)
            {


            }
        }
    }
}
