using Synapse.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VT_Api.Core.Roles;

namespace VT_Api.Core.Command.Commands
{
    [CommandInformation(
       Name = "CallPower",
       Aliases = new[] { "power" },
       Description = "Call the power of your role",
       Usage = "no argument if you want to call your main power, if not add the id of the power",
       Permission = "",
       Platforms = new[] { Platform.RemoteAdmin, Platform.ServerConsole },
       Arguments = new[] { "(powerId)" }
       )]
    public class CallPower : ISynapseCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            try
            {
                if (context.Player.CustomRole is IVtRole role)
                {
                    if (!context.Arguments.Any())
                    {
                        result.State = role.CallPower(1, out var message) ? CommandResultState.Ok : CommandResultState.NoPermission;
                        result.Message = message;
                    }
                    else if (byte.TryParse(context.Arguments.First(), out var power))
                    {
                        result.State = role.CallPower(power, out var message) ? CommandResultState.Ok : CommandResultState.NoPermission;
                        result.Message = message;
                    }
                    else
                    {
                        result.Message = VtController.Get.Configs.VtTranslation.ActiveTranslation.NotANumber;
                        result.State = CommandResultState.Error;
                    }
                }
                else
                {
                    result.Message = VtController.Get.Configs.VtTranslation.ActiveTranslation.NoPower;
                    result.State = CommandResultState.NoPermission;
                }
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Command : CallPower failed!!\n{e}\nStackTrace:\n{e.StackTrace}"); 
                result.Message = e.Message;
                result.State = CommandResultState.Error;
            }
            return result;
        }
    }
}
