using Synapse.Api;
using Synapse.Api.Plugin;
using Synapse.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VT_Api.Core.Command;

using CommandCtrl = VT_Api.Core.Command.CommandHandler;

namespace VT_Api.Core.Plugin.AutoRegisterProcess
{
    internal class CommandProcess : IContextProcessor
    {
        public void Process(PluginLoadContext context)
        {
            List<Type> listMainCommandType = new List<Type>();
            List<Type> listSubCommandType = new List<Type>();
            List<Type> listSynapseCommandType = new List<Type>();
            foreach (Type commandType in context.Classes)
            {
                if (typeof(IMainCommand).IsAssignableFrom(commandType))
                {
                    listMainCommandType.Add(commandType);
                }
                else if (typeof(ISubCommand).IsAssignableFrom(commandType))
                {
                    listSubCommandType.Add(commandType);
                }
                else if (typeof(ISynapseCommand).IsAssignableFrom(commandType))
                {
                    listSynapseCommandType.Add(commandType);
                }
            }
            // main command processing
            ProcessMainCommand(context, listMainCommandType);
            
            // add sub commands
            ProcessSubCommand(context, listSubCommandType);

            // added synapse command which remains
            ProcessSynapseCommand(context,listSynapseCommandType);
        }

        private void ProcessSubCommand(PluginLoadContext context,List<Type> listSubCommandType)
        {
            foreach (var commandType in listSubCommandType)
            {
                try
                {
                    var cmdInfoAttribute = commandType.GetCustomAttribute<SubCommandInformation>();
                    if (cmdInfoAttribute == null)
                        continue;

                    ISubCommand classObject;
                    ConstructorInfo[] allCtors = commandType.GetConstructors();
                    ConstructorInfo diCtor = allCtors.FirstOrDefault(ctorInfo => ctorInfo.GetParameters()
                        .Any(paramInfo => paramInfo.ParameterType == context.PluginType));

                    if (diCtor != null) //If DI-Ctor is found
                        classObject = (ISubCommand)Activator.CreateInstance(commandType, args: new object[] { context.Plugin });
                    else                //There is no DI-Ctor
                        classObject = (ISubCommand)Activator.CreateInstance(commandType);

                    GeneratedSubCommand command = GeneratedSubCommand.FromSynapseCommand(classObject);

                    if (string.IsNullOrEmpty(command.MainCommandName))
                    {
                        Logger.Get.Error($"Vt-Command : the SubCommand {command.Name} of the plugin {context.Plugin.Information.Name} dont defined the MainCommand !");
                    }
                    else
                    { 
                        VtController.Get.Commands.SubCommands.Add(command);
                    }
                }
                catch (Exception e)
                {
                    Logger.Get.Error($"Error loading command {commandType.Name} from {context.Information.Name}\n{e}");
                }
            }
        }

        private void ProcessMainCommand(PluginLoadContext context, List<Type> listMainCommandType)
        {
            var result = new List<GeneratedMainCommand>();
            foreach (var commandType in listMainCommandType)
            {
                try
                {
                    var cmdInfoAttribute = commandType.GetCustomAttribute<CommandInformation>();
                    if (cmdInfoAttribute == null)
                        continue;

                    IMainCommand classObject;
                    ConstructorInfo[] allCtors = commandType.GetConstructors();
                    ConstructorInfo diCtor = allCtors.FirstOrDefault(ctorInfo => ctorInfo.GetParameters()
                        .Any(paramInfo => paramInfo.ParameterType == context.PluginType));

                    if (diCtor != null) //If DI-Ctor is found
                        classObject = (IMainCommand)Activator.CreateInstance(commandType, args: new object[] { context.Plugin });
                    else                //There is no DI-Ctor
                        classObject = (IMainCommand)Activator.CreateInstance(commandType);
                    CommandCtrl.Get.RegisterMainCommand(classObject, true);
                }
                catch (Exception e)
                {
                    Logger.Get.Error($"Error loading command {commandType.Name} from {context.Information.Name}\n{e}");
                }
            }
        }

        private void ProcessSynapseCommand(PluginLoadContext context, List<Type> listSynapseCommandType) // processor of synapse
        {
            foreach (var commandType in listSynapseCommandType)
            {
                try
                {
                    var cmdInfoAttribute = commandType.GetCustomAttribute<CommandInformation>();
                    if (cmdInfoAttribute == null)
                        continue;

                    object classObject;
                    ConstructorInfo[] allCtors = commandType.GetConstructors();
                    ConstructorInfo diCtor = allCtors.FirstOrDefault(ctorInfo => ctorInfo.GetParameters()
                        .Any(paramInfo => paramInfo.ParameterType == context.PluginType));

                    if (diCtor != null) //If DI-Ctor is found
                        classObject = Activator.CreateInstance(commandType, context.Plugin);
                    else                //There is no DI-Ctor
                        classObject = Activator.CreateInstance(commandType);

                    CommandCtrl.Get.RegisterSynapseCommand(classObject as ISynapseCommand, true);
                }
                catch (Exception e)
                {
                    Logger.Get.Error($"Error loading command {commandType.Name} from {context.Information.Name}\n{e}");
                }
            }
        }
    }
}
