using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neuron.Core.Plugins;
using Neuron.Core.Meta;
using SLCommandModule;
using Neuron.Modules.Commands;
using Neuron.Core;
using Neuron.Core.Logging;

namespace BasicCommands;

[Automatic]
[NeuronCommand(
        CommandName = "Reload",
        Description = "Command to reload all configs translations and more from neuron",
        Parameters = new string[] { },
        Permission = "neuron.reload",
        Platforms = new[] { CommandPlatform.RemoteAdminConsole, CommandPlatform.ServerConsole }
        )]
public class ReloadCommand : NeuronCommand
{
    public override void Execute(NeuronCommandContext context, ref CommandResult result)
    {
        try
        {
            Globals.Get<ReloadModule.ReloadModule>().Reload();
            result.StatusCode = Neuron.Modules.Commands.Command.CommandStatusCode.Ok;
            result.Response = "Reloaded";
        }
        catch(Exception ex)
        {
            result.StatusCode = Neuron.Modules.Commands.Command.CommandStatusCode.Error;
            result.Response = "Error while reloading. Check console for further information";
            NeuronLogger<BasicCommandPlugin>.Error("Error while reloading\n" + ex);
        }
    }
}
