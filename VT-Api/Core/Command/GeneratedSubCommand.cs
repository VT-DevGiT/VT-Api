using Synapse.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Command
{
    public class GeneratedSubCommand : ICommand
    {

        #region Properties & Variable
        public Func<CommandContext, CommandResult> OnCommand { get; set; }
        public string Name { get; set; }
        public string MainCommandName { get; set; }
        public string[] Aliases { get; set; }
        public string Permission { get; set; }
        public string Usage { get; set; }
        public string[] Arguments { get; set; }
        public string Description { get; set; }

        public Platform[] Platforms { get; set; }
        #endregion

        #region Methods
        public CommandResult Execute(CommandContext command)
        {
            return OnCommand.Invoke(command);
        }

        public static GeneratedSubCommand FromSynapseCommand(ISubCommand command)
        {
            var type = command.GetType();
            var cmdInf = type.GetCustomAttribute<SubCommandInformation>();
            return new GeneratedSubCommand
            {
                OnCommand = command.Execute,
                Name = cmdInf.Name.ToLower(),
                MainCommandName = cmdInf.MainCommandName.ToLower(),
                Aliases = cmdInf.Aliases ?? new string[] { },
                Permission = cmdInf.Permission ?? "",
                Usage = cmdInf.Usage,
                Arguments = cmdInf.Arguments ?? new string[] { },
                Description = cmdInf.Description ?? "",
                Platforms = cmdInf.Platforms ?? new[] { Platform.RemoteAdmin, Platform.ServerConsole }
            };
        }
        #endregion
    }

}
