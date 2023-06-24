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
using RemoteAdmin;
using ICommand = Neuron.Modules.Commands.Command.ICommand;

namespace BasicCommands;

[Automatic]
[NeuronCommand(
    CommandName = "help",
    Aliases = new[] { "h" },
    Parameters = new[] { "(Command)" },
    Description = "Shows all available commands with usage description",
    Permission = "neuron.help",
    Platforms = new[] { CommandPlatform.PlayerConsole, CommandPlatform.RemoteAdminConsole, CommandPlatform.ServerConsole }
    )]
public class HelpCommand : NeuronCommand
{
    private readonly SLCommandService _commandService;

    public HelpCommand( SLCommandService commandService)
    {
        _commandService = commandService;
    }

    public override void Execute(NeuronCommandContext context, ref CommandResult result)
    {
        List<ICommand> commandlist = new List<ICommand>();
        IEnumerable<CommandSystem.ICommand> vanilla = new List<CommandSystem.ICommand>();

        switch (context.Platform)
        {
            case CommandPlatform.PlayerConsole:
                commandlist = _commandService.PlayerConsole.Handler.Commands.ToList();
                vanilla = QueryProcessor.DotCommandHandler.AllCommands;
                break;

            case CommandPlatform.RemoteAdminConsole:
                commandlist = _commandService.RemoteAdmin.Handler.Commands.ToList();
                vanilla = CommandProcessor.RemoteAdminCommandHandler.AllCommands;
                break;

            case CommandPlatform.ServerConsole:
                commandlist = _commandService.ServerConsole.Handler.Commands.ToList();
                vanilla = GameCore.Console.singleton.ConsoleCommandHandler.AllCommands;
                break;

            default:
                result.Response = "Information for this Console is not supported";
                result.StatusCode = CommandStatusCode.Error;
                return;
        }


        if (context.Arguments.Length > 0 && !string.IsNullOrWhiteSpace(context.Arguments.First()))
        {
            foreach (var command in commandlist)
            {
                if (!string.Equals(command.Meta.CommandName, context.Arguments[0],
                        StringComparison.OrdinalIgnoreCase)) continue;

                result.Response = GenerateCustomCommandInfo(command, context.Platform, context.Player);
                return;
            }

            foreach (var command in vanilla)
            {
                if (!string.Equals(command.Command, context.Arguments[0],
                        StringComparison.OrdinalIgnoreCase)) continue;

                result.Response = GenerateVanillaCommandInfo(command, context.Platform);
                return;
            }

            result.StatusCode = CommandStatusCode.NotFound;
            result.Response = "Command was not found";
            return;
        }

        result.Response = GenerateCommandList(commandlist, vanilla, context.Player, context.Platform);
        result.StatusCode = CommandStatusCode.Ok;
    }

    public string GenerateCommandList(List<ICommand> customCommands,
        IEnumerable<CommandSystem.ICommand> vanillaCommands, PermissionModule.PermissionPlayer player, CommandPlatform platform)
    {
        var msg = "All Available Vanilla Commands:";
        foreach (var command in vanillaCommands)
        {
            msg += $"\n{command.Command}";

            if ((command.Aliases?.Length ?? 0) > 0)
            {
                msg += $"\n    Aliases: " + string.Join(", ", command.Aliases);
            }

            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                if (command.Description.Length <= _maxLetters[platform])
                {
                    msg += "\n    Description: " + command.Description;
                }
                else
                {
                    msg += "\n    Description: " + SplitDescription(command.Description, platform);
                }
            }

            msg += "\n";
        }

        msg += "\n" + "All Available Synapse Commands:";

        foreach (var customCommand in customCommands)
        {
            if (customCommand.Meta is NeuronCommandAttribute synapseCommandAttribute)
            {
                if (!string.IsNullOrWhiteSpace(synapseCommandAttribute.Permission) &&
                    !player.HasPermission(synapseCommandAttribute.Permission)) continue;
            }

            msg += $"\n{customCommand.Meta.CommandName}";
            if ((customCommand.Meta.Aliases?.Length ?? 0) > 0)
            {
                msg += $"\n    Aliases: " + string.Join(", ", customCommand.Meta.Aliases);
            }

            if (!string.IsNullOrWhiteSpace(customCommand.Meta.Description))
            {
                if (customCommand.Meta.Description.Length <= _maxLetters[platform])
                {
                    msg += "\n    Description: " + customCommand.Meta.Description;
                }
                else
                {
                    msg += "\n    Description: " + SplitDescription(customCommand.Meta.Description, platform);
                }
            }

            msg += "\n";
        }

        return msg.TrimEnd('\n');
    }

    public string GenerateCustomCommandInfo(ICommand command, CommandPlatform platform, PermissionModule.PermissionPlayer player)
    {
        var msg = "\n" + command.Meta.CommandName;

        if ((command.Meta.Aliases?.Length ?? 0) > 0)
        {
            msg += $"\n    Aliases: " + string.Join(", ", command.Meta.Aliases);
        }

        if (command.Meta is NeuronCommandAttribute synapseCommandAttribute)
        {
            msg += $"\n    Platforms: " + string.Join(", ", synapseCommandAttribute.Platforms);

            if (!string.IsNullOrWhiteSpace(synapseCommandAttribute.Permission))
            {
                msg += "\n    Permission: " + synapseCommandAttribute.Permission;
            }
        }

        if (!string.IsNullOrWhiteSpace(command.Meta.Description))
        {
            if (command.Meta.Description.Length <= _maxLetters[platform])
            {
                msg += "\n    Description: " + command.Meta.Description;
            }
            else
            {
                msg += "\n    Description: " + SplitDescription(command.Meta.Description, platform);
            }
        }

        return msg;
    }

    public string GenerateVanillaCommandInfo(CommandSystem.ICommand command, CommandPlatform platform)
    {
        var msg = "\n" + command.Command;

        if ((command.Aliases?.Length ?? 0) > 0)
        {
            msg += $"\n    Aliases: " + string.Join(", ", command.Aliases);
        }

        if (!string.IsNullOrWhiteSpace(command.Description))
        {
            if (command.Description.Length <= _maxLetters[platform])
            {
                msg += "\n    Description: " + command.Description;
            }
            else
            {
                msg += "\n    Description: " + SplitDescription(command.Description, platform);
            }
        }

        return msg;
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
