using Synapse.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Command.Commands
{
    [CommandInformation(
       Name = "Language",
       Aliases = new[] { "Lang" },
       Description = "For change the language of the text of the serveur",
       Usage = "no arguement send you the list of possible language, else add the desired language",
       Permission = "",
       Platforms = new[] { Platform.ClientConsole },
       Arguments = new[] { "(new language)" }
       )]
    public class ChangeLanguage : ISynapseCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            try
            {
                result.State = CommandResultState.Ok;
                if (context.Arguments.Any())
                {
                    var language = context.Arguments.First().ToUpper();
                    if (language == "HELP")
                    {
                        result.Message = Help();
                    }
                    else if (Translation.TranslationManager.Get.AvailableLanguage.Contains(language))
                    {
                        context.Player.SetData("Language", language);
                        Translation.TranslationManager.Get.PlayersLanguage[context.Player.NickName] = language;
                        result.Message = "New language set successfully";
                    }
                    else
                    {
                        result.Message = "Invalide Language";
                    }
                }
                else
                {
                    result.Message = Help();
                }
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Command : Language failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                result.Message = e.Message;
                result.State = CommandResultState.Error;
            }
            return result;
        }

        public string Help()
        {
            var message = "All possible languages:\n";
            message += String.Join("\n", Translation.TranslationManager.Get.AvailableLanguage);
            return message;
        }
    }
}
