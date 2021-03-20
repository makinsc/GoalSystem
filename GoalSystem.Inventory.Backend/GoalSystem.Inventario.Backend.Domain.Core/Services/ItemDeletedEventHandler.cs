using GoalSystem.Inventario.Backend.Transversal.Models;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace GoalSystem.Inventario.Backend.Domain.Core.Services
{
    public class ItemDeletedEventHandler : IItemDeletedElementEventHandler, IDisposable
    {
        private readonly Subject<DeletedMessageReceived> _subject;
        private readonly Dictionary<string, IDisposable> _subscribers;
        public ItemDeletedEventHandler()
        {
            _subject = new Subject<DeletedMessageReceived>();
            _subscribers = new Dictionary<string, IDisposable>();
        }
        public void Publish(DeletedMessageReceived eventMessage)
        {
            _subject.OnNext(eventMessage);
        }

        public void Subscribe(string subscriberName, Action<DeletedMessageReceived> action)
        {
            if (!_subscribers.ContainsKey(subscriberName))
            {
                _subscribers.Add(subscriberName, _subject.Subscribe(action));
            }
        }

        public void Subscribe(string subscriberName, Func<DeletedMessageReceived, bool> predicate, Action<DeletedMessageReceived> action)
        {
            if (!_subscribers.ContainsKey(subscriberName))
            {
                _subscribers.Add(subscriberName, _subject.Where(predicate).Subscribe(action));
            }
        }

        public void Dispose()
        {
            if (_subject != null)
            {
                _subject.Dispose();
            }

            foreach (var subscriber in _subscribers)
            {
                subscriber.Value.Dispose();
            }
        }
    }
}
