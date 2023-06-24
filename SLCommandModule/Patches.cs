using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Neuron.Core.Meta;
using Neuron.Core;
using RemoteAdmin;
using Neuron.Core.Logging;
using Console = GameCore.Console;
using Neuron.Modules.Commands.Command;

namespace SLCommandModule;

[Automatic]
[HarmonyPatch]
public static class Patches
{
    private static readonly SLCommandService _commandService;
    static Patches() => _commandService = Globals.Get<SLCommandService>();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.ProcessGameConsoleQuery))]
    public static bool OnPlayerConsoleCommand(QueryProcessor __instance, string query)
    {
        try
        {
            var player = PluginAPI.Core.Player.Get<PermissionModule.PermissionPlayer>(__instance.netIdentity);
            if (player == null) return true;
            if (player.ReferenceHub.Mode != ClientInstanceMode.ReadyClient && !player.IsServer)
                return true;

            var result = _commandService.PlayerConsole
                .Invoke(NeuronCommandContext.Of(query, player, CommandPlatform.PlayerConsole));
            if (result.StatusCodeInt == 0) return true;

            var color = "white";
            switch (result.StatusCode)
            {
                case CommandStatusCode.Ok:
                    color = "gray";
                    break;

                case CommandStatusCode.Error:
                    color = "red";
                    break;

                case CommandStatusCode.Forbidden:
                    color = "darkred";
                    break;

                case CommandStatusCode.BadSyntax:
                    color = "yellow";
                    break;

                case CommandStatusCode.NotFound:
                    color = "green";
                    break;
            }

            player.SendConsoleMessage(result.Response, color);
            return false;
        }
        catch (Exception ex)
        {
            NeuronLogger<CommandModule>.Error($"PlayerConsole command failed:\n{ex}");
            return true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    public static bool OnRemoteAdminCommand(string q, CommandSender sender)
    {
        try
        {
            var player = PluginAPI.Core.Player.Get<PermissionModule.PermissionPlayer>(sender);
            if (player == null) return true;

            //@ is used for AdminChat and $ for Communication like getting the playerList
            if (q.StartsWith("@") || q.StartsWith("$"))
                return true;


            var result = _commandService.RemoteAdmin
                .Invoke(NeuronCommandContext.Of(q, player, CommandPlatform.RemoteAdminConsole));
            if (result.StatusCodeInt == 0) return true;

            sender.RaReply(result.Response, result.StatusCodeInt == (int)CommandStatusCode.Ok, true, "");
            return false;
        }
        catch (Exception ex)
        {
            NeuronLogger<CommandModule>.Error($"RemoteAdmin command failed:\n{ex}");
            return true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Console), nameof(Console.TypeCommand))]
    public static bool OnServerConsoleCommand(string cmd)
    {
        try
        {
            if (cmd.StartsWith(".") || cmd.StartsWith("/") || cmd.StartsWith("@") || cmd.StartsWith("!"))
                return true;
            var context = NeuronCommandContext.Of(cmd, PluginAPI.Core.Player.Get<PermissionModule.PermissionPlayer>(ReferenceHub.HostHub),
                    CommandPlatform.ServerConsole);
            context.IsAdmin = true;
            var result =
                _commandService.ServerConsole.Invoke(context);

            if (result.StatusCodeInt == 0) return true;

            var color = ConsoleColor.White;
            switch (result.StatusCode)
            {
                case CommandStatusCode.Ok:
                    color = ConsoleColor.Cyan;
                    break;

                case CommandStatusCode.Error:
                    color = ConsoleColor.Red;
                    break;

                case CommandStatusCode.Forbidden:
                    color = ConsoleColor.DarkRed;
                    break;

                case CommandStatusCode.BadSyntax:
                    color = ConsoleColor.Yellow;
                    break;

                case CommandStatusCode.NotFound:
                    color = ConsoleColor.DarkGreen;
                    break;
            }
            ServerConsole.AddLog(result.Response, color);
            return false;
        }
        catch (Exception ex)
        {
            NeuronLogger<CommandModule>.Error($"ServerConsole command failed:\n{ex}");
            return true;
        }
    }
}
