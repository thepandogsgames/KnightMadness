using System;
using System.Threading.Tasks;
using Code.Utilities.Enums;

namespace Code.Events
{
    public interface IEventManager
    {
        void Subscribe(EventTypeEnum eventType, Action listener);
        void Subscribe<T>(EventTypeEnum eventType, Action<T> listener);
        void Unsubscribe(EventTypeEnum eventType, Action listener);
        void Unsubscribe<T>(EventTypeEnum eventType, Action<T> listener);
        Task TriggerEventAsync(EventTypeEnum eventType, params object[] parameters);
    }
}