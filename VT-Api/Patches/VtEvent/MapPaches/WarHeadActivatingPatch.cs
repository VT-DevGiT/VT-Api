using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using Synapse.Config;
using System;
using System.Linq;
using UnityEngine;
using VT_Api.Reflexion;

namespace VT_Api.Patches.VtEvent.MapPaches
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdSwitchAWButton))]
    class ActivatingWarheadPanelPatch
    {
        [HarmonyPrefix]
        private static bool SwitchAWButtonPatch(PlayerInteract __instance)
        {
            try
            {
                var gameObject = GameObject.Find("OutsitePanelScript");

                if (!__instance.CanInteract || !__instance.ChckDis(gameObject.transform.position))
                    return false;

                var player = __instance.GetPlayer();
                var item = player.ItemInHand;
                var allow = player.Bypass;

                if (item.ItemCategory == ItemCategory.Keycard)
                {
                    allow = item.ItemBase is KeycardItem keycard && keycard.Permissions.HasFlag(KeycardPermissions.AlphaWarhead);
                    var par = new object[] { player, item, Synapse.Api.Events.SynapseEventArguments.ItemInteractState.Finalizing, allow };

                    Synapse.Server.Get.Events.Player.CallMethod("InvokePlayerItemUseEvent", par);
                }
                else if (SynapseController.Server.Configs.GetFieldValueOrPerties<SynapseConfiguration>("synapseConfiguration").RemoteKeyCard)
                {
                    foreach (var keyCard in player.Inventory.Items.Where(x => x != item && x.ItemCategory == ItemCategory.Keycard))
                    {
                        var allowcard = keyCard.ItemBase is KeycardItem keycard && keycard.Permissions.HasFlag(KeycardPermissions.AlphaWarhead);
                        var par = new object[] { player, keyCard, Synapse.Api.Events.SynapseEventArguments.ItemInteractState.Finalizing, allow };

                        Synapse.Server.Get.Events.Player.CallMethod("InvokePlayerItemUseEvent", par);

                        if (allowcard)
                        {
                            allow = true;
                            break;
                        }
                    }

                }

                VtController.Get.Events.Map.InvokeUnlockWarheadEvent(player, ref allow);

                if (allow)
                {
                    gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>().NetworkkeycardEntered = true;
                    __instance.OnInteract();
                }
                return false;
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: ActivatingWarheadPanel failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                return true;
            }
        }
    }
}
