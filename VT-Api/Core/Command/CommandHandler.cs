using Synapse.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using VT_Api.Core.Command.Commands;
using VT_Api.Reflexion;

namespace VT_Api.Core.Command
{
    public class CommandHandler
    {

        internal void Init()
        {
            Synapse.Api.Events.EventHandler.Get.Round.WaitingForPlayersEvent += RegisterSubCommand;

            RegisterVtCommands();
        }

        public static CommandHandler Get { get => VtController.Get.Commands; }
        internal List<GeneratedMainCommand> MainCommands { get; } = new List<GeneratedMainCommand>();
        internal List<GeneratedSubCommand> SubCommands { get; } = new List<GeneratedSubCommand>();

        private bool _firstLoad = true;

        internal void RegisterCommand(ISynapseCommand iSynapseCommand, bool awaitPluginInitialisation)
            => typeof(Synapse.Command.Handlers).CallMethod("RegisterCommand", iSynapseCommand, awaitPluginInitialisation);

        private void RegisterSubCommand()
        {
            if (_firstLoad)
            {
                foreach (var command in SubCommands)
                {
                    var mainCommand = MainCommands.FirstOrDefault(main => main.Name == command.MainCommandName);
                    if (mainCommand == null)
                    {
                        Synapse.Api.Logger.Get.Error($"Vt-Command : MainCommand {mainCommand.Name} not found for the SubCommand {command.MainCommandName}");
                    }
                    else
                    {
                        mainCommand.AddSubCommand(command);
                    }
                }

                MainCommands.Clear();
                SubCommands.Clear();
                _firstLoad = false;
            }

            Synapse.Api.Events.EventHandler.Get.Round.WaitingForPlayersEvent -= RegisterSubCommand;
        }

        private void RegisterVtCommands()
        {
            if (!_firstLoad)
                return;

            RegisterCommand(new CallPower(), false);
            
        }
    }
}
