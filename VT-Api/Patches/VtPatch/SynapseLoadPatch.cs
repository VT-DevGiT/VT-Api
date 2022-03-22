using HarmonyLib;
using Synapse.Api.Plugin;
using VT_Api.Core.Events;

namespace VT_Api.Patches.VtPatch
{
    [HarmonyPatch(typeof(PluginLoader), "PluginLoader")]
    class SynapseLoadPatch
    {
        [HarmonyPostfix]
        void PluginLoader()
        {
            VtController.Get.Item.Init();
            EventHandler.Get.Server.SynapsePostLoadPostEvent();

            //UnPatch
            var instance = new Harmony("Vt_Api.patches");
            var original = typeof(PluginLoader).GetMethod("PluginLoader");
            var patch    = typeof(SynapseLoadPatch).GetMethod(nameof(PluginLoader));
            instance.Unpatch(original, patch);
        }
    }
}
