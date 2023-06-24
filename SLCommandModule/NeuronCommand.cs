using Neuron.Modules.Commands;
using Neuron.Modules.Commands.Command;
using Neuron.Core;
using System.Linq;

namespace SLCommandModule;

public abstract class NeuronCommand : Command<NeuronCommandContext>
{
    protected NeuronCommand()
    {
        Globals.Kernel.Bind(GetType()).ToConstant(this).InSingletonScope();
        Globals.Kernel.Inject(this);
    }

    public override CommandResult PreExecute(NeuronCommandContext context)
    {
        if (context.IsAdmin) return null;

        if (Meta is not NeuronCommandAttribute meta)
            return new CommandResult
            {
                Response = "Invalid Command, can't check for Permission",
                StatusCode = CommandStatusCode.Error
            };

        if(!string.IsNullOrWhiteSpace(meta.Permission))
        {
            if (!context.Player.HasPermission(meta.Permission))
                return new CommandResult
                {
                    Response = $"You don't have access to this Command ({meta.Permission})",
                    StatusCode = CommandStatusCode.Forbidden
                };
        }

        return null;
    }
}
