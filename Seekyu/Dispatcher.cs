using System.Collections.Generic;
using System.Linq;

namespace Seekyu
{
    public class Dispatcher<TDispatchable>
        where TDispatchable : IDispatchable
    {
        private List<IHandler> Handlers;

        public Dispatcher(params IHandler[] handlers)
        {
            Handlers = new List<IHandler>();
            Handlers.AddRange(handlers);
        }

        public TResponse Handle<TResponse>(TDispatchable dispatchable)
            where TResponse : IResponse
        {
            IEnumerable<IHandler<TDispatchable, TResponse>> candidates = Handlers.OfType<IHandler<TDispatchable, TResponse>>();
            if (candidates.Any())
            {
                if (candidates.Count() > 1)
                    throw new TooManyHandlersException<TDispatchable, TResponse>(candidates);
                return candidates.First().TryHandle(dispatchable);
            }
            throw new MissingHandlerException<TDispatchable, TResponse>();
        }
    }

    public interface IHandler { }
    public interface IHandler<TDispatchable, TResponse> : IHandler
    {
        TResponse TryHandle(TDispatchable dispatchable);
    }

    public interface IDispatchable
    {
    }
}
