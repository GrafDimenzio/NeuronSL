using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neuron.Core.Plugins;
using Neuron.Core.Meta;
using SLCommandModule;
using Neuron.Modules.Commands;


namespace BasicCommands;

[Plugin(
    Name = "BasicCommand",
    Author = "Dimenzio",
    Description = "Adds a Reload,Plugin and Help Command for Neuron",
    Version = "1.0.0"
    )]
public class BasicCommandPlugin : Plugin
{
    public override void Enable()
    {
        Logger.Info("Basic Command Plugin enabled");
    }
}
