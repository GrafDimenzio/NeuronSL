using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Neuron.Core.Meta;

namespace PermissionModule;

[Automatic]
[HarmonyPatch]
public static class Patches
{
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.RefreshPermissions)), HarmonyPrefix]
    public static bool RefreshPermission(ServerRoles __instance, bool disp = false)
    {
        try
        {
            var player = PluginAPI.Core.Player.Get<PermissionPlayer>(__instance.gameObject);
            player?.RefreshPermission(disp);
        }
        catch (Exception e)
        {
            Neuron.Core.Logging.NeuronLogger<PermissionModule>.Error($"Sy3 Permission: RefreshPermissionPatch failed!!\n{e}");
        }

        return false;
    }

    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetGroup)), HarmonyPrefix]
    public static bool SetGroup() => false;
}
