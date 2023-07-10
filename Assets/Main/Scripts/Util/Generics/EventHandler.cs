using System;
using System.Collections.Generic;

namespace Main.Scripts.Util.Generics
{
    public abstract class EventHandler<T> where T : Enum
    {
        private Dictionary<T, Action<object>> _subscriberMap = new();

        public void Subscribe(T eventType, Action<object> methodInstance)
        {
            if (_subscriberMap.TryGetValue(eventType, out var eventToSubscribe))
            {
                eventToSubscribe += methodInstance;
                _subscriberMap[eventType] = eventToSubscribe;
            }
            else
            {
                eventToSubscribe += methodInstance;
                _subscriberMap.Add(eventType, eventToSubscribe);
            }
        }

        public void Unsubscribe(T eventType, Action<object> methodInstance)
        {
            if (_subscriberMap.TryGetValue(eventType, out var eventToUnsubscribe))
            {
                eventToUnsubscribe -= methodInstance;
                _subscriberMap[eventType] = eventToUnsubscribe;
            }
        }

        public void Notify(T eventType, object eventData = null)
        {
            if (_subscriberMap.TryGetValue(eventType, out var eventToTrigger))
            {
                eventToTrigger?.Invoke(eventData);
            }
        }
        
    }
}