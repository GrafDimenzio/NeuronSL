using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neuron.Core.Modules;
using Neuron.Core.Meta;
using Ninject;
using Neuron.Core.Plugins;

namespace SLCommandModule;

[Module(
    Name = "SLCommandModule",
    Description = "Module that adds a Command API for SCP:SL Commands",
    Author = "Dimenzio",
    Dependencies = new[]
    {
        typeof(Neuron.Modules.Patcher.PatcherModule),
        typeof(Neuron.Modules.Commands.CommandsModule),
        typeof(PermissionModule.PermissionModule)
    },
    Version = "1.0.0"
    )]
public class CommandModule : Module
{
    [Inject]
    public SLCommandService CommandService { get; set; }

    public override void Enable()
    {
        Logger.Info("SLCommand Module enabled");
    }

    internal readonly Queue<NeuronCommandBinding> ModuleCommandBindingQueue = new();

    public override void Load(IKernel kernel)
    {
        kernel.Get<MetaManager>().MetaGenerateBindings.Subscribe((MetaGenerateBindingsEvent args) =>
        {
            if (!args.MetaType.TryGetAttribute<AutomaticAttribute>(out _)) return;
            if (!args.MetaType.TryGetAttribute<NeuronCommandAttribute>(out var _)) return;
            if (!args.MetaType.Is<NeuronCommand>()) return;

            args.Outputs.Add(new NeuronCommandBinding
            {
                Type = args.MetaType.Type,
            });
        });

        kernel.Get<PluginManager>().PluginLoadLate.Subscribe((PluginLoadEvent args) =>
        {
            args.Context.MetaBindings
            .OfType<NeuronCommandBinding>()
            .ToList().ForEach(x => CommandService.LoadBinding(x));
        });

        kernel.Get<ModuleManager>().ModuleLoadLate.Subscribe((ModuleLoadEvent args) =>
        {
            args.Context.MetaBindings
            .OfType<NeuronCommandBinding>()
            .ToList().ForEach(binding =>
            {
                ModuleCommandBindingQueue.Enqueue(binding);
            });
        });
    }
}

public class NeuronCommandBinding : IMetaBinding
{
    public Type Type { get; set; }

    public IEnumerable<Type> PromisedServices => new Type[] { };
}
