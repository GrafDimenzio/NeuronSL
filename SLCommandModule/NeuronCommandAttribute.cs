using System;
using Neuron.Modules.Commands.Command;

namespace SLCommandModule;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class NeuronCommandAttribute : CommandAttribute
{
    public NeuronCommandAttribute()
    {
        Aliases = Array.Empty<string>();
    }

    public string Permission { get; set; } = "";
    public CommandPlatform[] Platforms { get; set; } = { CommandPlatform.ServerConsole };
    public string[] Parameters { get; set; } = { };
}
