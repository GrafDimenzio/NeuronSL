using System;
using System.Linq;
using Neuron.Modules.Commands;

namespace SLCommandModule;

public class NeuronCommandContext : ICommandContext
{
    public string Command { get; set; }
    public string[] Arguments { get; set; }
    public string FullCommand { get; set; }
    public bool IsAdmin { get; set; } = false;
    public Type ContextType => typeof(NeuronCommandContext);
    public PermissionModule.PermissionPlayer Player { get; private set; }
    public CommandPlatform Platform { get; private set; }

    public static NeuronCommandContext Of(string message, PermissionModule.PermissionPlayer player, CommandPlatform platform)
    {
        var context = new NeuronCommandContext
        {
            FullCommand = message
        };
        var args = message.Split(' ').ToList();
        context.Command = args[0];
        args.RemoveAt(0);
        context.Arguments = args.ToArray();

        context.Player = player;
        context.Platform = platform;

        return context;
    }
}
