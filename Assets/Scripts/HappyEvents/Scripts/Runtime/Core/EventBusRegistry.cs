using System.Collections.Generic;

namespace HappyEvents
{
    public static class EventBusRegistry
    {
        private static readonly List<EventInstance> _activeBuses = new();
        public static void Register(EventInstance bus) { if (!_activeBuses.Contains(bus)) _activeBuses.Add(bus); }
        public static List<EventInstance> GetAllBuses() => _activeBuses;
    }
}