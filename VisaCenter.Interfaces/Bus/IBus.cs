using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VisaCenter.Interfaces.Handlers
{
    public interface IMessage
    {
        string DataAsJson();
        int? ReferenceId { get; set; }
        Guid? TransactionId { get; set; }
        Guid? WorkId { get; set; }
    }

    public interface ICommand : IMessage
    {
        T GetResult<T>();
        object Result { get; set; }
    }

    public interface IDomainEvent {

    }

    public interface IEventHandler { }

    public interface IEventHandler<in T, Z> : IEventHandler where T : IDomainEvent
    {
        Task<Z> HandleAsync(T ev, IBus bus);
    }

    public interface IBus
    {
        Task Raise(IDomainEvent ev);
        Task<Z> Raise<T,Z>(T ev) where T : class, IDomainEvent;
    }
}
