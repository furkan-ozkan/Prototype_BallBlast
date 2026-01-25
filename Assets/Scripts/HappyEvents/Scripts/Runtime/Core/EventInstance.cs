using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HappyEvents
{
    public class EventInstance
    {
        public string Name { get; private set; }
        private readonly Dictionary<Type, List<ISubscription>> _subscribers = new();
        public List<EventLog> History { get; private set; } = new();

        public EventInstance(string name)
        {
            Name = name;
            EventBusRegistry.Register(this);
        }

        public void Subscribe<T>(Action<T> callback) where T : IEvent
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type)) _subscribers[type] = new List<ISubscription>();
            if (_subscribers[type].Any(s => s.Action == (Delegate)callback)) return;
            _subscribers[type].Add(new Subscription<T>(callback));
        }

        public void Unsubscribe<T>(Action<T> callback) where T : IEvent
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var subs))
            {
                var sub = subs.FirstOrDefault(s => s.Action == (Delegate)callback);
                if (sub != null) subs.Remove(sub);
            }
        }

        public void Publish(IEvent eventData)
        {
            var type = eventData.GetType();

            Stopwatch sw = Stopwatch.StartNew();

            if (_subscribers.TryGetValue(type, out var subscriptions))
            {
                for (int i = subscriptions.Count - 1; i >= 0; i--)
                {
                    if (!subscriptions[i].IsAlive)
                    {
                        subscriptions.RemoveAt(i);
                        continue;
                    }
                    subscriptions[i].Invoke(eventData);
                }
            }

            sw.Stop();
            string trace = "";
#if UNITY_EDITOR
            trace = UnityEngine.StackTraceUtility.ExtractStackTrace();
#endif
            
            History.Add(new EventLog(type.Name, eventData, trace, sw.Elapsed.TotalMilliseconds));
        }

        public Dictionary<Type, List<ISubscription>> GetSubscribers() => _subscribers;
        public void ClearHistory() => History.Clear();
    }
}