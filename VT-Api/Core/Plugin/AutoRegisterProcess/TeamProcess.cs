using Synapse.Api;
using Synapse.Api.Plugin;
using Synapse.Api.Teams;
using System;
using System.Collections.Generic;
using System.Reflection;
using VT_Api.Reflexion;

namespace VT_Api.Core.Plugin.AutoRegisterProcess
{
    internal class TeamProcess : IContextProcessor
    {
        public void Process(PluginLoadContext context)
        {
            if (!(context.PluginType is IVtPlugin vtPlugin) || !vtPlugin.AutoRegister) return;

            foreach (var teamType in context.Classes)
            {
                try
                {
                    if (!typeof(Synapse.Api.Teams.ISynapseTeam).IsAssignableFrom(teamType) ||
                        teamType.GetCustomAttribute<AutoRegisterManager.Ignore>() != null)
                        continue;

                    ISynapseTeam synapseTeam = Activator.CreateInstance(teamType) as ISynapseTeam;
                    if (synapseTeam.Info == null)
                        synapseTeam.Info = teamType.GetCustomAttribute<SynapseTeamInformation>();
                    

                    if (TeamManager.Get.IsIDRegistered(synapseTeam.Info.ID))
                        throw new Exception("A Plugin tried to register a CustomTeam with an already used Id");
                    

                    TeamManager.Get.GetFieldValueOrPerties<List<ISynapseTeam>>("teams").Add(synapseTeam);
                    synapseTeam.Initialise();
                }
                catch (Exception e)
                {
                    Logger.Get.Error($"Error auto register tem {teamType.Name} from {context.Information.Name}\n{e}");
                }
            }
        }
    }
}
