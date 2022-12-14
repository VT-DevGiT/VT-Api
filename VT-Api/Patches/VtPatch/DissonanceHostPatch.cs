using Dissonance;
using Dissonance.Networking;
using Dissonance.Integrations.MirrorIgnorance;
using HarmonyLib;
using VT_Api.Reflexion;

namespace VT_Api.Patches.VtPatch
{
    [HarmonyPatch(typeof(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit>), "RunAsDedicatedServer")]
    class DissonanceHostPatch
    {
        public static bool RunAsDedicatedServerPatch(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit> __instance)
        {
            __instance.CallMethod("RunAsHost", new object[] { Unit.None, Unit.None });
            return false;
        }
    }
}
