using System;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.ServiceProvider
{
    public static class ServiceProvider
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceProvider] {type.Name} Already registered...");
            }
            _services[type] = service;
        }

        public static T Get<T>()
        {
            var type = typeof(T);
            if (!_services.TryGetValue(type, out var service))
            {
                Debug.LogError($"[ServiceProvider] {type.Name} Couldnt find!");
                return default;
            }
            return (T)service;
        }

        public static void Unregister<T>()
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services.Remove(type);
            }
        }

        public static void ClearAll() => _services.Clear();
    }
}