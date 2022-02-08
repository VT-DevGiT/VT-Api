using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Patches.VtEvent.MapPaches
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdUseElevator))]
    class UseElevatorPatch
    {
        [HarmonyPrefix]
        private static bool UseElevatorIneractPatch(PlayerInteract __instance, GameObject elevator)
        {
            try
            {
                if (!__instance.CanInteract || elevator == null)
                    return false;
                Lift component = elevator.GetComponent<Lift>();
                if (component == null)
                    return false;
                foreach (Lift.Elevator _elevator in component.elevators)
                {
                    if (__instance.ChckDis(_elevator.door.transform.position))
                    {
                        bool flag = true;
                        VtController.Get.Events.Map.InvokeElevatorIneractEvent(__instance.GetPlayer(), component.GetElevator(), ref flag);
                        if (flag)
                        {
                            component.UseLift();
                            __instance.OnInteract();
                        }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: UseElevator failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
