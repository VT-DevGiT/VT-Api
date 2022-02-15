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
            if (!(context.Plugin is IVtPlugin vtPlugin) || !vtPlugin.AutoRegister) return;

            foreach (var teamType in context.Classes)
            {
                if (!typeof(Synapse.Api.Teams.ISynapseTeam).IsAssignableFrom(teamType) || teamType.GetCustomAttribute<AutoRegisterManager.Ignore>() != null)
                    continue;

                try
                {
                    ISynapseTeam synapseTeam = Activator.CreateInstance(teamType) as ISynapseTeam;

                    if (synapseTeam.Info == null)
                        synapseTeam.Info = teamType.GetCustomAttribute<SynapseTeamInformation>();

                    if (synapseTeam.Info == null)
                        Logger.Get.Error($"The custom Item {teamType.Name} ave no information !");

                    if (Synapse.Api.Teams.TeamManager.Get.IsIDRegistered(synapseTeam.Info.ID))
                        Logger.Get.Error($"A Plugin tried to register a CustomTeam with an already used Id : {synapseTeam.Info.ID}");


                    Synapse.Api.Teams.TeamManager.Get.GetFieldValueOrPerties<List<ISynapseTeam>>("teams").Add(synapseTeam);
                    synapseTeam.Initialise();
                }
                catch (Exception e)
                {
                    Logger.Get.Error($"Error auto register tem {teamType.Name} from {context.Plugin.Information.Name}\n{e}");
                }

                //VtController.Get.Team.AwaitingFinalization.Add(teamType, info);
            }
        }
    }
}
