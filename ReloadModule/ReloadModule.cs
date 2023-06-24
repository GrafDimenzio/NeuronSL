using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neuron.Core.Modules;
using Neuron.Core.Events;
using Ninject;

namespace ReloadModule;

[Module(
    Name = "Reload",
    Description = "Simple universal Neuron Module that adds a reload Event as well as a small API for easily reloadable Plugins and Modules",
    Author = "Dimenzio",
    Version = "1.0.0",
    Dependencies = new[] 
    { 
        typeof(Neuron.Modules.Configs.ConfigsModule)
    }   
    )]
public class ReloadModule : Module
{
    [Inject]
    public ReloadService ReloadService { get; set; }

    public override void Enable()
    {
        Logger.Info("Reload Module enabled");
    }

    public void Reload() => ReloadService.Reload.Raise(new());
}
