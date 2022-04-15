﻿using Synapse.Api;
using Synapse.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using VT_Api.Core.Command.Commands;
using VT_Api.Extension;
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

        private readonly List<IMainCommand> AwaitingFinalization = new List<IMainCommand>();


        private bool _firstLoad = true;

        internal void RegisterSynapseCommand(ISynapseCommand iSynapseCommand, bool awaitPluginInitialisation)
            => typeof(Synapse.Command.Handlers).CallMethod("RegisterCommand", iSynapseCommand, awaitPluginInitialisation);

        internal void RegisterMainCommand(IMainCommand iSynapseCommand, bool awaitPluginInitialisation)
        {
            if (awaitPluginInitialisation)
            {
                AwaitingFinalization.Add(iSynapseCommand);
            }
            else
            {
                RegisterGeneratedCommand(GeneratedMainCommand.FromSynapseCommand(iSynapseCommand));
            }
        }

        internal void RegisterGeneratedCommand(GeneratedMainCommand command)
        {
            Platform[] platforms = command.Platforms;
            for (int i = 0; i < platforms.Length; i++)
            {
                switch (platforms[i])
                {
                    case Platform.ClientConsole:
                        SynapseController.CommandHandlers.ClientCommandHandler.RegisterCommand(command);
                        break;
                    case Platform.RemoteAdmin:
                        SynapseController.CommandHandlers.RemoteAdminHandler.RegisterCommand(command);
                        break;
                    case Platform.ServerConsole:
                        SynapseController.CommandHandlers.ServerConsoleHandler.RegisterCommand(command);
                        break;
                }
            }
            MainCommands.Add(command);
        }

        internal void FinalizePluginsCommands()
        {
            foreach (var item in AwaitingFinalization)
                RegisterGeneratedCommand(GeneratedMainCommand.FromSynapseCommand(item));

            AwaitingFinalization.Clear();
        }

        private void RegisterSubCommand()
        {
            if (_firstLoad)
            {
                FinalizePluginsCommands();

                foreach (var command in SubCommands)
                {
                    var mainCommand = MainCommands.FirstOrDefault(main => main.Name == command.MainCommandName);
                    if (mainCommand == null)
                    {
                        Logger.Get.Error($"Vt-Command : MainCommand {mainCommand.Name} not found for the SubCommand {command.MainCommandName}");
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

            RegisterSynapseCommand(new CallPower(), false);
        }
    }
}
