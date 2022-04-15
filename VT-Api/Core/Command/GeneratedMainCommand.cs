using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Synapse.Api;
using Synapse.Command;
using VT_Api.Extension;

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
            if (!context.Arguments.Any() || string.IsNullOrEmpty(context.Arguments.First()) || context.Arguments.First().ToLower() == "help")
            {
                context.Arguments = context.Arguments.Segment(1);
                return ShowHelp(context);
            }

            var result = new CommandResult();

            if (!Commands.TryGetValue(context.Platform, out var commands))
            {
                result.Message = "This platform is not suported";
                result.State = CommandResultState.Error;
                return result;
            }

            var subCommandName = context.Arguments.First();
            context.Arguments = context.Arguments.Segment(1);

            var subCommand = commands.FirstOrDefault(c => c.Name == subCommandName);

            if (subCommand == null)
                subCommand = commands.FirstOrDefault(c => c.Aliases.Contains(subCommandName));
            
            if (subCommand == null)
            {
                result.Message = "Sub-Command not found, please use help to get all possible command.";
                result.State = CommandResultState.Error;
                return result;
            }

            result = OnCommand.Invoke(context);

            if (result.State != CommandResultState.Ok)
                return result;

            var subresult = subCommand.Execute(context);

            if (!string.IsNullOrEmpty(result.Message))
                result.Message += "\n";
            else
                result.Message = "";

            result.Message += subresult.Message;
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
                {
                    string alias = "{ " + string.Join(", ", command.Aliases) + " }";

                    if (command.Arguments.Any())
                    {
                        string arguments = "{ " + string.Join(", ", command.Arguments) + " }";

                        result.Message = $"\n{command.Name}\n    - Description: {command.Description}\n    - Usage: {command.Usage}\n    - Aliases: {alias}\n    - Argument:{arguments}";
                    }
                    else
                    {
                        result.Message = $"\n{command.Name}\n    - Description: {command.Description}\n    - Usage: {command.Usage}\n    - Aliases: {alias}";
                    }
                }
                else
                {
                    string alias = "{ " + string.Join(", ", command.Aliases) + " }";

                    if (command.Arguments.Any())
                    {
                        string arguments = "{ " + string.Join(", ", command.Arguments) + " }";

                        result.Message = $"\n{command.Name}\n    - Description: {command.Description}\n    - Usage: {command.Usage}\n    - Aliases: {alias}\n    - Argument:{arguments}";
                    }
                    else
                    {
                        result.Message = $"\n{command.Name}\n    - Description: {command.Description}\n    - Usage: {command.Usage}\n    - Aliases: {alias}";
                    }
                }

                result.State = CommandResultState.Ok;
                return result;
            }
            else if (commandlist.Any())
            {
                var msg = $"All Commands which you can execute for {context.Platform}:";

                foreach (var command in commandlist)
                {
                    string alias = "{ " + string.Join(", ", command.Aliases) + " }";

                    if (command.Arguments.Any())
                    {
                        string arguments = "{ " + string.Join(", ", command.Arguments) + " }";

                        msg += $"\n{command.Name}\n    - Description: {command.Description}\n    - Usage: {command.Usage}\n    - Aliases: {alias}\n    - Argument:{arguments}";
                    }
                    else
                    {
                        msg += $"\n{command.Name}\n    - Description: {command.Description}\n    - Usage: {command.Usage}\n    - Aliases: {alias}";
                    }
                }

                result.Message = msg;
                result.State = CommandResultState.Ok;
                return result;
            }
            else
            {
                var msg = $"You cannot execute a command for {context.Platform}:";

                result.Message = msg;
                result.State = CommandResultState.Ok;
                return result;
            }
        }

        public static GeneratedMainCommand FromSynapseCommand(IMainCommand command)
        {
            var type = command.GetType();
            var cmdInf = type.GetCustomAttribute<CommandInformation>();
            var result = new GeneratedMainCommand
            {
                OnCommand = command.Execute,
                Name = cmdInf.Name.ToLower(),
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
