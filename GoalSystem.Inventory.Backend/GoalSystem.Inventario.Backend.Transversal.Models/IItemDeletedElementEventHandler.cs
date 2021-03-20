using System;

namespace GoalSystem.Inventario.Backend.Transversal.Models
{
    public interface IItemDeletedElementEventHandler
    {
        void Publish(DeletedMessageReceived eventMessage);
        void Subscribe(string subscriberName, Action<DeletedMessageReceived> action);
        void Subscribe(string subscriberName, Func<DeletedMessageReceived, bool> predicate, Action<DeletedMessageReceived> action);
    }
}
