using HarmonyLib;
using MapGeneration.Distributors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Patches.VtEvent.MapPaches
{
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerUpdate))]
    class GeneratorActivated
    {
        [HarmonyPrefix]
        private static bool GeneratorUpdatePatch(Scp079Generator __instance)
        {
            try
            {
                var allow = __instance._currentTime >= __instance._totalActivationTime;
                
                if (!allow)
                {
                    var num = Mathf.FloorToInt(__instance._totalActivationTime - __instance._currentTime);
                    
                    if (num != __instance._syncTime)
                        __instance.Network_syncTime = (short)num;
                }

                if (__instance.ActivationReady)
                {
                    if (allow && !__instance.Engaged)
                    {
                        VtController.Get.Events.Map.InvokeGeneratorActivatedEvent(__instance.GetGenerator());

                        __instance.Engaged = true;
                        __instance.Activating = false;

                        return false;
                    }
                    __instance._currentTime += Time.deltaTime;
                }
                else
                {
                    if (__instance._currentTime == 0.0 | allow)
                        return false;
                    
                    __instance._currentTime -= __instance.DropdownSpeed * Time.deltaTime;
                }
                
                __instance._currentTime = Mathf.Clamp(__instance._currentTime, 0.0f, __instance._totalActivationTime);
                
                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: GeneratorActivatedEvent failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
