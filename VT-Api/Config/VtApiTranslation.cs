using Synapse.Translation;
using System.ComponentModel;

namespace VT_Api.Config
{
    public class VtApiTranslation : IPluginTranslation
    {
        [Description("Message returned when a player has no power for their role")]
        public string NoPower { get; set; } = "You don't have any power";
        public string NotANumber { get; set; } = "The argument must be a number. exemple : 1";

        [Description("The info about levels of accreditation")]
        public string RankOver { get; set; } = "GIVE ORDERS";
        public string RankSame { get; set; } = "SAME RANK";
        public string RankUnder { get; set; } = "FOLLOW ORDERS";

        [Description("The death message when you are kill by a custom class of the VT-API")]
        public string DeathMessage { get; set; } = "You were killed by\\n%PlayerName%\\nas\\n%RoleName%";
    }
}
