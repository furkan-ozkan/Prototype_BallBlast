using System;
using System.Reflection;

namespace HappyEvents
{
    internal class Subscription<T> : ISubscription where T : IEvent
    {
        private readonly WeakReference _targetReference;
        private readonly MethodInfo _methodInfo;

        public Delegate Action { get; }

        public Subscription(Action<T> callback)
        {
            Action = callback;
            _targetReference = new WeakReference(callback.Target);
            _methodInfo = callback.Method;
        }

        public void Invoke(IEvent eventData)
        {
            var target = _targetReference.Target;
            if (target != null)
            {
                _methodInfo.Invoke(target, new object[] { eventData });
            }
        }

        public bool IsAlive => _targetReference.Target != null;
    }
}