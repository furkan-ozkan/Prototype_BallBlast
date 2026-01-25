using UnityEngine;

namespace HappyEvents
{
    [CreateAssetMenu(fileName = "New EventBus", menuName = "Happy Event/EventBus Asset")]
    public class EventBusAsset : ScriptableObject
    {
        public string BusName = "GlobalBus";
        private EventInstance _instance;
        public EventInstance Bus => _instance ??= new EventInstance(BusName);
        private void OnEnable() => _instance = new EventInstance(BusName);
    }
}