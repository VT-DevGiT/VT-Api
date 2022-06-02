using Synapse.Config;
using Synapse.Translation;
using VT_Api.Reflexion;

namespace VT_Api.Config
{
    public class Config
    {
        public static Config Get { get => VtController.Get.Configs; }

        public VtApiConfiguration VtConfiguration;


        public SynapseTranslation<VtApiTranslation> VtTranslation;
        public SynapseConfiguration SynapseConfiguration => Synapse.Server.Get.Configs.GetFieldValueOrPerties<SynapseConfiguration>("synapseConfiguration");



        internal void Init()
        {
            VtTranslation = new SynapseTranslation<VtApiTranslation>(SynapseController.Server.Files.GetTranslationPath("Vt-Api"));
            VtTranslation.AddTranslation(new VtApiTranslation(), "ENGLISH");
            VtTranslation.AddTranslation(new VtApiTranslation
            {
                NoPower = "Du hast Keine Macht",
                NotANumber = "Das Argument muss eine Zahl sein. Beispiel : 1",
                RankOver = "GIB ANWEISUNGEN",
                RankSame = "GLEICHER RANG",
                RankUnder = "FOLGE ANWEISUNGEN",
                DeathMessage = "<size=20%>Du wurdest gertöet von</size>\\n%PlayerName%\\n<size=20%>als</size%>\\n%RoleName%"
            }, "GERMAN");
            VtTranslation.AddTranslation(new VtApiTranslation
            {
                NoPower = "Tu n'as aucun pouvoir",
                NotANumber = "L'argument doit être un nombre. Exemple : 1",
                RankOver = "VOUS POUVEZ LUI DONNER DES ORDRES",
                RankSame = "MÊME NIVEAU D'ACCRÉDITATION",
                RankUnder = "SUIVEZ SES ORDRES",
                DeathMessage = "<size=20%>Vous avez été tué par:</size>\\n%PlayerName%\\n<size=20%>en tent que:</size%>\\n%RoleName%"

            }, "FRENCH");

            VtConfiguration = new VtApiConfiguration();
            VtConfiguration = Synapse.Server.Get.Configs.GetOrSetDefault("VT-API", new VtApiConfiguration());
        }
    }
}
