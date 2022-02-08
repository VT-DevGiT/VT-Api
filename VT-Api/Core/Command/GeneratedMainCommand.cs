using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Synapse.Command;

namespace VT_Api.Core.Command
{
    public class GeneratedMainCommand : ICommand
    {

        #region Properties & Variable
        public string Name { get; set; }
        public string[] Aliases { get; set; }
        public string Permission { get; set; }
        public string Usage { get; set; }
        public string[] Arguments { get; set; }
        public string Description { get; set; }
        public Platform[] Platforms { get; set; }
        public Dictionary<Platform, List<GeneratedSubCommand>> Commands { get; private set; }

        public Func<CommandContext, CommandResult> OnCommand { get; set; }
        #endregion

        #region Methods
        public void AddSubCommand(GeneratedSubCommand subCommand)
        {
            foreach (var platform in subCommand.Platforms)
            {
                if (!Commands.TryGetValue(platform, out var commands))
                {
                    Synapse.Api.Logger.Get.Error($"Vt-Command : subcommand {subCommand.Name}, platform {platform} not suported for this command {Name}");
                }
                else
                {
                    commands.Add(subCommand);
                }
            }
        }
        
        public CommandResult Execute(CommandContext context)
        {
            if (context.Arguments.Any() || string.IsNullOrEmpty(context.Arguments.Array[0]) || context.Arguments.Array[0] == "help")
                return ShowHelp(context);

            var result = new CommandResult();
            var subCommandName = context.Arguments.Array[0];
            context.Arguments = context.Arguments.Segment(1);

            if (!Commands.TryGetValue(context.Platform, out var commands))
            {
                result.Message = "This platform is not suported";
                result.State = CommandResultState.Error;
                return result;
            }

            var subCommand = commands.FirstOrDefault(c => c.Name == subCommandName);
            if (subCommand == null)
                subCommand = commands.FirstOrDefault(c => c.Aliases.Contains(subCommandName));
            
            if (subCommand == null)
            {
                result.Message = "Sub command not found, please use help to get all possible command.";
                result.State = CommandResultState.Error;
                return result;
            }

            result = OnCommand.Invoke(context);

            if (result.State != CommandResultState.Ok)
                return result;

            var subresult = subCommand.Execute(context);
            
            result.Message += "\n" + subresult.Message;
            result.State = subresult.State;
            
            return result;
        }

        public CommandResult ShowHelp(CommandContext context)
        {
            var result = new CommandResult();

            if (!Commands.TryGetValue(context.Platform, out var commandlist))
            {
                result.Message = "Information for this Console is not supported";
                result.State = CommandResultState.Error;
                return result;
            }

            commandlist = commandlist.Where(x => context.Player.HasPermission(x.Permission) || string.IsNullOrWhiteSpace(x.Permission) || x.Permission.ToUpper() == "NONE").ToList();

            if (context.Arguments.Count > 0 && !string.IsNullOrWhiteSpace(context.Arguments.First()))
            {
                var command = commandlist.FirstOrDefault(x => x.Name.ToLower() == context.Arguments.First());

                if (command == null)
                {
                    command = commandlist.FirstOrDefault(c => c.Aliases.Contains(context.Arguments.First()));

                    if (command == null)
                    {
                        result.State = CommandResultState.Error;
                        result.Message = "No Command with this Name found";
                        return result;
                    }
                }

                string platforms = "{ " + string.Join(", ", command.Platforms) + " }";
                string aliases = "{ " + string.Join(", ", command.Aliases) + " }";

                if (string.IsNullOrWhiteSpace(command.Permission))
                    result.Message = $"\n{command.Name}\n    - Description: {command.Description}\n    - Usage: {command.Usage}\n    - Platforms: {platforms}\n    - Aliases: {aliases}";
                else
                    result.Message = $"\n{command.Name}\n    - Permission: {command.Permission}\n    - Description: {command.Description}\n    - Usage: {command.Usage}\n    - Platforms: {platforms}\n    - Aliases: {aliases}";

                result.State = CommandResultState.Ok;
                return result;
            }

            var msg = $"All Commands which you can execute for {context.Platform}:";

            foreach (var command in commandlist)
            {
                string alias = "{ " + string.Join(", ", command.Aliases) + " }";

                msg += $"\n{command.Name}:\n    -Usage: {command.Usage}\n    -Description: {command.Description}\n    -Aliases: {alias}";
            }

            result.Message = msg;
            result.State = CommandResultState.Ok;
            return result;
        }

        public static GeneratedMainCommand FromSynapseCommand(IMainCommand command)
        {
            var type = command.GetType();
            var cmdInf = type.GetCustomAttribute<CommandInformation>();
            var result = new GeneratedMainCommand
            {
                OnCommand = command.Execute,
                Name = cmdInf.Name,
                Aliases = cmdInf.Aliases ?? new string[] { },
                Permission = cmdInf.Permission ?? "",
                Usage = cmdInf.Usage,
                Arguments = cmdInf.Arguments,
                Description = cmdInf.Description ?? "",
                Platforms = cmdInf.Platforms ?? new[] { Platform.RemoteAdmin, Platform.ServerConsole },
                Commands = new Dictionary<Platform, List<GeneratedSubCommand>>()
            };
            result.InitDictionaryCmd();
            return result;
        }

        private void InitDictionaryCmd()
        {
            foreach (var platform in Platforms)
                Commands.Add(platform, new List<GeneratedSubCommand>());
        }
        #endregion
    }
}
