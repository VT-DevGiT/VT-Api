using HarmonyLib;
using Synapse.Api.Plugin;
using VT_Api.Core.Events;

namespace VT_Api.Patches.VtPatch
{
    //[HarmonyPatch(typeof(PluginLoader), "LoadPlugins")]
    class SynapseLoadPatch
    {
        [HarmonyPostfix]
        void PluginLoader()
        {
            VtController.Get.Item.Init();

            //UnPatch
            var instance = new Harmony("Vt_Api.patches");
            var original = typeof(PluginLoader).GetMethod("LoadPlugins");
            var patch    = typeof(SynapseLoadPatch).GetMethod(nameof(PluginLoader));
            instance.Unpatch(original, patch);
        }
    }
}
