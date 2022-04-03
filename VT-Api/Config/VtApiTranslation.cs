using Synapse.Translation;
using System.ComponentModel;

namespace VT_Api.Config
{
    public class VtApiTranslation : IPluginTranslation
    {
        [Description("Message returned when a player has no power for their role")]
        public string NoPower = "You don't have any power";
        public string NotANumber = "The argument must be a number. exemple : 1";

        [Description("The info about levels of accreditation")]
        public string RankOver = "GIVE ORDERS";
        public string RankSame = "SAME RANK";
        public string RankUnder = "FOLLOW ORDERS";
    }
}
