using Synapse.Translation;
using System.ComponentModel;

namespace VT_Api.Config
{
    public class VtApiTranslation : IPluginTranslation
    {
        [Description("Message returned when a player has no power for their role")]
        public string NoPower = "You don't have any power";
        public string NotANumber = "The argument must be a number. exemple : 1";
    }
}
