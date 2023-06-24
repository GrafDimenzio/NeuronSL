using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neuron.Core.Modules;
using Ninject;

namespace PermissionModule;

[Module(
    Name = "PermissionModule",
    Author = "Dimenzio",
    Description = "SL Module for Permission Management",
    Dependencies = new[]
    {
        typeof(Neuron.Modules.Configs.ConfigsModule),
        typeof(Neuron.Modules.Patcher.PatcherModule),
        typeof(ReloadModule.ReloadModule)
    },
    Version = "1.0.0"
    )]
public class PermissionModule : Module
{
    public override void Load(IKernel kernel)
    {
        PluginAPI.Core.FactoryManager.RegisterPlayerFactory(this, new PermissionPlayerFactory());
    }

    public override void Enable()
    {
        Logger.Info("Permission Module enabled");
    }
}
