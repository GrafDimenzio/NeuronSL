using System.Collections.Generic;
using System.IO;
using MEC;
using Neuron.Core;
using Neuron.Core.Platform;
using Neuron.Core.Scheduling;
using System;
using Neuron.Core.Logging;
using Neuron.Core.Logging.Processing;
using System.Text;

namespace SLPlatform;

public class Platform : IPlatform
{
    public static void Main()
    {
        var entrypoint = new Platform();
        entrypoint.Boostrap();
    }

    public PlatformConfiguration Configuration { get; set; } = new();
    public NeuronBase NeuronBase { get; set; }
    public ActionCoroutineReactor CoroutineReactor { get; set; } = new();

    private CoroutineHandle _mainCoroutineHandle;

    public void Load()
    {
        Configuration.ConsoleWidth = 85;
        Configuration.BaseDirectory = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SCP Secret Laboratory", "Neuron"));
        Configuration.FileIo = true;
        Configuration.CoroutineReactor = CoroutineReactor;
        Configuration.OverrideConsoleEncoding = false;
        Configuration.EnableConsoleLogging = false;
        Configuration.LogEventSink = new LogRenderer();
    }

    public void Continue() { }

    public void Disable() { }

    public void Enable() => _mainCoroutineHandle = Timing.RunCoroutine(TickCoroutines());

    public class LogRenderer : ILogRender
    {
        public void Render(LogOutput output)
        {
            var buffer = new StringBuilder();
            var color = output.Level switch
            {
                LogLevel.Verbose => ConsoleColor.DarkGray,
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Information => ConsoleColor.Cyan,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Fatal => ConsoleColor.DarkRed,
                _ => throw new ArgumentOutOfRangeException()
            };

            foreach (var token in output.Tokens)
            {
                buffer.Append(token.Message);
            }

            try
            {
                ServerConsole.AddLog($"[{output.Level}] [{output.Caller}] " + buffer, color);
            }
            catch
            {
                PluginAPI.Core.Log.Raw($"[{output.Level}] [{output.Caller}] " + buffer, "NEURON");
            }
        }
    }

    private IEnumerator<float> TickCoroutines()
    {
        var ticker = CoroutineReactor.GetTickAction();
        while (true)
        {
            ticker.Invoke();
            yield return 0;
        }
    }
}
