using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Code.Utilities.Enums;

namespace Code.Events
{
    public class EventManager : IEventManager
    {
        private readonly ConcurrentDictionary<EventTypeEnum, ConcurrentBag<Delegate>> _events = new();

        public void Subscribe(EventTypeEnum eventType, Action listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            GetOrCreateDelegates(eventType).Add(listener);
        }

        public void Subscribe<T>(EventTypeEnum eventType, Action<T> listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            GetOrCreateDelegates(eventType).Add(listener);
        }

        public void Unsubscribe(EventTypeEnum eventType, Action listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            RemoveDelegate(eventType, listener);
        }

        public void Unsubscribe<T>(EventTypeEnum eventType, Action<T> listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            RemoveDelegate(eventType, listener);
        }

        public async Task TriggerEventAsync(EventTypeEnum eventType, params object[] parameters)
        {
            if (!_events.TryGetValue(eventType, out var delegates)) return;

            var delegatesCopy = delegates.ToList();

            foreach (var eventDelegate in delegatesCopy)
            {
                try
                {
                    var result = eventDelegate?.DynamicInvoke(parameters);

                    if (result is Task task)
                    {
                        await task;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error invoking delegate for event {eventType}: {ex.Message}");
                }
            }
        }

        private ConcurrentBag<Delegate> GetOrCreateDelegates(EventTypeEnum eventType)
        {
            return _events.GetOrAdd(eventType, _ => new ConcurrentBag<Delegate>());
        }

        private void RemoveDelegate(EventTypeEnum eventType, Delegate listener)
        {
            if (!_events.TryGetValue(eventType, out var delegates)) return;

            var newDelegates = new ConcurrentBag<Delegate>(delegates.Where(d => d != listener));
            _events[eventType] = newDelegates;

            if (newDelegates.IsEmpty)
            {
                _events.TryRemove(eventType, out _);
            }
        }
    }
}