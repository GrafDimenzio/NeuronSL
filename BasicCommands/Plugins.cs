using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neuron.Core.Plugins;
using Neuron.Core.Meta;
using SLCommandModule;
using Neuron.Modules.Commands;
using Neuron.Modules.Commands.Command;
using Ninject;
using Neuron.Core.Modules;

namespace BasicCommands;

[Automatic]
[NeuronCommand(
        CommandName = "Plugins",
        Description = "Shows all Modules and Plugins",
        Parameters = new string[] { },
        Permission = "neuron.plugins",
        Platforms = new[] { CommandPlatform.RemoteAdminConsole, CommandPlatform.ServerConsole }
        )]
public class Plugins : NeuronCommand
{
    [Inject]
    public PluginManager PluginManager { get; set; }

    [Inject]
    public ModuleManager ModuleManager { get; set; }

    public override void Execute(NeuronCommandContext context, ref CommandResult result)
    {
        Logger.Warn(PluginManager == null);
        Logger.Warn(ModuleManager == null);
        var msg = "All Modules:";
        foreach (var module in ModuleManager.GetAllModules())
            msg += $"\n{module.Attribute.Name}" +
                $"\n    - Description: {SplitDescription(module.Attribute.Description, context.Platform)}" +
                $"\n    - Author: {module.Attribute.Author}" +
                $"\n    - Version: {module.Attribute.Version}" +
                $"\n    - Repository: {module.Attribute.Repository}" +
                $"\n    - Website: {module.Attribute.Website}";

        msg += "\n\nAll Plugins:";
        foreach (var module in PluginManager.Plugins)
            msg += $"\n{module.Attribute.Name}" +
                $"\n    - Description: {SplitDescription(module.Attribute.Description, context.Platform)}" +
                $"\n    - Author: {module.Attribute.Author}" +
                $"\n    - Version: {module.Attribute.Version}" +
                $"\n    - Repository: {module.Attribute.Repository}" +
                $"\n    - Website: {module.Attribute.Website}";

        result.Response = msg;
        result.StatusCode = CommandStatusCode.Ok;
    }

    private string SplitDescription(string message, CommandPlatform platform)
    {
        var count = 0;
        var msg = "";

        foreach (var word in message.Split(' '))
        {
            count += word.Length;

            if (count > _maxLetters[platform])
            {
                msg += "\n                ";
                count = 0;
            }

            if (msg == string.Empty)
            {
                msg += word;
            }
            else
            {
                msg += " " + word;
            }
        }

        return msg;
    }

    private readonly Dictionary<CommandPlatform, int> _maxLetters = new()
    {
        { CommandPlatform.PlayerConsole, 50 },
        { CommandPlatform.RemoteAdminConsole, 50 },
        { CommandPlatform.ServerConsole, 75 }
    };
}
