using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using System.Reflection;
using System.IO;

namespace NeuronSL;

public class NeuronBootstrap
{
    [PluginEntryPoint(
        "NeuronBootstrap",
        "1.0.0",
        "Neuron loader",
        "Dimenzio"
        )]
    public void Start()
    {
        try
        {
            if (StartupArgs.Args.Any(x => x.Equals("--noneuron", StringComparison.OrdinalIgnoreCase))) return;
            Log.Info("Bootstraping Neuron Platform");

            var assemblies = new List<Assembly>();
            var domain = AppDomain.CurrentDomain;
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SCP Secret Laboratory");

            foreach (var file in Directory.GetFiles(Path.Combine(path,"Neuron","Managed"), "*.dll"))
            {
                try
                {
                    var assembly = domain.Load(File.ReadAllBytes(file));
                    assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to load Assembly\n" + file + "\n" + ex);
                }
            }

            domain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                return assemblies.First(x => x.FullName == args.Name);
            };

            var coreAssembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == "SLPlatform");
            var entryPoint = coreAssembly?.GetType("SLPlatform.Platform");
            if (entryPoint == null) throw new Exception("No Valid EntryPoint was found. Please Reinstall the SLPlatform");
            var method = entryPoint.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
            if (method == null) throw new Exception("No Valid Main Method was found. Please Reinstall the SLPlatform");
            method.Invoke(null,Array.Empty<object>());
        }
        catch(Exception ex)
        {
            Log.Error("Loading of Neuron failed:\n" + ex, "Neuron");
        }
    }
}
