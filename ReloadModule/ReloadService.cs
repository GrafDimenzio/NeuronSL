using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Neuron.Core.Events;
using Neuron.Core.Meta;

namespace ReloadModule;

public class ReloadService : Service
{
    private EventManager _eventManager;
    public ReloadService(EventManager eventManager) => _eventManager = eventManager;

    public EventReactor<ReloadEvent> Reload = new();

    public override void Enable()
    {
        _eventManager.RegisterEvent(Reload);
    }

    public override void Disable()
    {
        _eventManager.UnregisterEvent(Reload);
    }
}

public class ReloadEvent : IEvent { }
