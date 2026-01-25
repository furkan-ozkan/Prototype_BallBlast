using System;

namespace HappyEvents
{
    public interface IEvent { }

    public interface ISubscription
    {
        void Invoke(IEvent eventData);
        Delegate Action { get; }
        bool IsAlive { get; }
    }
}