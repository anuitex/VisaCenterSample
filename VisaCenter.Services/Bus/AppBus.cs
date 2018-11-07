using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisaCenter.Interfaces.Handlers;

namespace VisaCenter.Bus
{
    public sealed class AppBus : IBus
    {
        private readonly IEnumerable<IEventHandler> _handlers;

        public AppBus(IEnumerable<IEventHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task Raise(IDomainEvent ev)
        {
            var handlers = _handlers.Where(x => x.GetType().GetInterfaces().Any(z => z.GetGenericArguments().Any(a => a.IsAssignableFrom(ev.GetType())))).ToList();
            foreach (var handler in handlers)
            {
                try
                {
                    var method = handler.GetType().GetMethod("HandleAsync", new Type[] { ev.GetType(), typeof(IBus) });
                    var task = (Task)method.Invoke(handler, new object[] { ev, this });
                    if (task != null)
                        await task;
                }
                catch (Exception ex)
                {
                    //Log.Failure(handler.GetType().Name + " threw an exception." + ExceptionHelper.ToString(ex));
                    throw;
                }
            }
        }

        public async Task<Z> Raise<T, Z>(T ev) where T : class, IDomainEvent
        {
            var handlers = _handlers.Where(x => x.GetType().GetInterfaces().Any(z => z.GetGenericArguments().Any(a => a.IsAssignableFrom(typeof(T))))).Select(x => x as IEventHandler<T, Z>).ToList();
            foreach (var handler in handlers)
            {
                try
                {
                    var t = await handler.HandleAsync(ev, this);
                    return t;
                }
                catch (Exception ex)
                {
                    //Log.Failure(handler.GetType().Name + " threw an exception." + ExceptionHelper.ToString(ex));
                    throw;
                }
            }
            return default(Z);
        }
    }
}
