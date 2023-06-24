using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neuron.Core.Plugins;
using Neuron.Core.Meta;
using SLCommandModule;
using Neuron.Modules.Commands;

namespace TestCommand
{
    [Plugin(
        Name = "TestCommand",
        Description = "Small Plugin to test the Command Module",
        Author = "Dimenzio",
        Version = "1.0.0"
        )]
    public class TestCommandPlugin : Plugin {  }

    [Automatic]
    [NeuronCommand(
        CommandName = "Test",
        Description = "Small Test command",
        Parameters = new[] { "1", "2", "3" },
        Permission = "example.test",
        Platforms = new[] {CommandPlatform.RemoteAdminConsole,CommandPlatform.ServerConsole},
        Aliases = new [] { "example" }
        )]
    public class TestCommand : NeuronCommand
    {
        public override void Execute(NeuronCommandContext context, ref CommandResult result)
        {
            result.Response = "Success " + context.Player.Nickname + " " + Meta.Aliases[0];
        }
    }
}
