using Synapse.Config;
using Synapse.Translation;
using VT_Api.Reflexion;

namespace VT_Api.Config
{
    public class Config
    {
        public static Config Get { get => VtController.Get.Configs; }

        //[Synapse.Api.Plugin.Config(section = "VT-API Configuration")]
        //public VtApiConfiguration VtConfiguration { get; }

        public SynapseConfiguration SynapseConfiguration => Synapse.Server.Get.Configs.GetFieldValueOrPerties<SynapseConfiguration>("synapseConfiguration");

        public SynapseTranslation<VtApiTranslation> VtTranslation;



        internal void Init()
        {
            VtTranslation = new SynapseTranslation<VtApiTranslation>(SynapseController.Server.Files.GetTranslationPath("Vt-Api"));
            VtTranslation.AddTranslation(new VtApiTranslation(), "ENGLISH");
            VtTranslation.AddTranslation(new VtApiTranslation
            {
                NoPower = "Du hast Keine Macht",
                NotANumber = "Das Argument muss eine Zahl sein. Beispiel : 1"
            }, "GERMAN");
            VtTranslation.AddTranslation(new VtApiTranslation
            {
                NoPower = "Tu n'as aucun pouvoir",
                NotANumber = "L'argument doit être un nombre. Exemple 1"
            }, "FRENCH");
        }
    }
}
