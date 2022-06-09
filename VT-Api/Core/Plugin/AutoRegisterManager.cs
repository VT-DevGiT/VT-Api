using Synapse.Api;
using Synapse.Api.Plugin;
using System;
using System.Collections.Generic;
using VT_Api.Core.Plugin.AutoRegisterProcess;
using VT_Api.Extension;
using VT_Api.Reflexion;

namespace VT_Api.Core.Plugin
{
    public class AutoRegisterManager
    {
        internal AutoRegisterManager() { }


        readonly IContextProcessor[] AddedRegisterProcesses = { new CommandProcess(), new ItemProcess(), new MiniGameProcess(), new RoleProcess(), new TeamProcess(), new DebugCheckProcess(), new AutoUpdatePorecess() }; 

        internal void Init()
        {
            var processors = SynapseController.PluginLoader.GetFieldValueOrPerties<List<IContextProcessor>>("_processors");
            processors.RemoveAll(p => p is Synapse.Api.Plugin.Processors.CommandProcessor);
            foreach (var addProcesse in AddedRegisterProcesses)
                processors.Add(addProcesse);
            
        }


        /// <summary>
        /// Ignore Only this class for the AutoRegister
        /// </summary>
        public class Ignore : Attribute { }
    }
}
