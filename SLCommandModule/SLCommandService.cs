using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Neuron.Core.Meta;
using Neuron.Modules.Commands;
using Neuron.Modules.Commands.Event;
using RemoteAdmin;

namespace SLCommandModule;

public class SLCommandService : Service
{
    private CommandModule _commandModule;

    public CommandReactor ServerConsole { get; private set; }
    public CommandReactor RemoteAdmin { get; private set; }
    public CommandReactor PlayerConsole { get; private set; }

    public SLCommandService(CommandService command, CommandModule commandModule)
    {
        _commandModule = commandModule;

        ServerConsole = command.CreateCommandReactor();
        ServerConsole.NotFoundFallbackHandler = NotFound;

        RemoteAdmin = command.CreateCommandReactor();
        RemoteAdmin.NotFoundFallbackHandler = NotFound;

        PlayerConsole = command.CreateCommandReactor();
        PlayerConsole.NotFoundFallbackHandler = NotFound;
    }

    private static CommandResult NotFound(CommandEvent args)
    {
        return new CommandResult
        {
            StatusCode = 0,
            Response = "You shouldn't be able to see this since the default game response should come"
        };
    }

    public override void Enable()
    {
        PluginAPI.Events.EventManager.RegisterEvents(this);

        while (_commandModule.ModuleCommandBindingQueue.Count != 0)
        {
            var binding = _commandModule.ModuleCommandBindingQueue.Dequeue();
            LoadBinding(binding);
        }
    }

    internal void LoadBinding(NeuronCommandBinding binding) => RegisterCommand(binding.Type);

    public void RegisterCommand(Type command)
    {
        var rawMeta = command.GetCustomAttribute(typeof(NeuronCommandAttribute));
        if (rawMeta == null) return;
        var meta = (NeuronCommandAttribute)rawMeta;

        foreach (var platform in meta.Platforms)
        {
            switch (platform)
            {
                case CommandPlatform.PlayerConsole:
                    PlayerConsole.RegisterCommand(command);
                    break;

                case CommandPlatform.RemoteAdminConsole:
                    RemoteAdmin.RegisterCommand(command);
                    break;

                case CommandPlatform.ServerConsole:
                    ServerConsole.RegisterCommand(command);
                    break;
            }
        }
    }

    [PluginAPI.Core.Attributes.PluginEvent(PluginAPI.Enums.ServerEventType.WaitingForPlayers)]
    private void GenerateCommandCompletion()
    {
        var list = QueryProcessor.ParseCommandsToStruct(CommandProcessor.GetAllCommands()).ToList();
        list.Remove(list.FirstOrDefault(x => x.Command == "give"));

        foreach (var command in RemoteAdmin.Handler.Commands)
        {
            if (command.Meta is not NeuronCommandAttribute meta) continue;

            list.Add(new QueryProcessor.CommandData
            {
                Command = meta.CommandName,
                AliasOf = null,
                Description = meta.Description,
                Hidden = false,
                Usage = meta.Parameters
            });

            if (meta.Aliases == null) continue;

            foreach (var alias in meta.Aliases)
            {
                list.Add(new QueryProcessor.CommandData
                {
                    Command = alias,
                    AliasOf = meta.CommandName,
                    Description = meta.Description,
                    Hidden = false,
                    Usage = meta.Parameters
                });
            }
        }

        QueryProcessor._commands = list.ToArray();
    }
}
