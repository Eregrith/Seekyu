using System;
using System.Collections.Generic;
using System.Linq;

namespace Seekyu
{
    public class Dispatcher<TDispatchable>
        where TDispatchable : IDispatchable
    {
        private List<IHandler> Handlers;
        private Dictionary<Tuple<Type, Type>, IHandler> TypedHandlers;

        public Dispatcher(params IHandler[] handlers)
        {
            Handlers = new List<IHandler>();
            Handlers.AddRange(handlers);

            TypedHandlers = new Dictionary<Tuple<Type, Type>, IHandler>();
            foreach (IHandler handler in handlers)
            {
                var t = handler.GetType().GetInterfaces();
                var generics = t.First(i => i.Name == "IHandler`2").GetGenericArguments();
                if (generics.Count() == 2)
                {
                    if (TypedHandlers.Any(k => k.Key.Item1 == generics[0] && k.Key.Item2 == generics[1]))
                        throw new TooManyHandlersException();

                    TypedHandlers.Add(new Tuple<Type, Type>(generics[0], generics[1]), handler);
                }
            }
        }

        public TResponse Handle<TResponse>(TDispatchable dispatchable)
            where TResponse : IResponse
        {
            Type queryType = dispatchable.GetType();
            Type responseType = typeof(TResponse);
            Type handlerType = typeof(IHandler<,>).MakeGenericType(queryType, responseType);
            Func<KeyValuePair<Tuple<Type, Type>, IHandler>, bool> matchingHandler = k => k.Key.Item1 == queryType && k.Key.Item2 == responseType;
            if (!TypedHandlers.Any(matchingHandler))
            {
                throw (MissingHandlerException)Activator.CreateInstance(typeof(MissingHandlerException<,>).MakeGenericType(queryType, responseType));
            }
            IHandler candidate = TypedHandlers.First(matchingHandler).Value;
            var tryHandle = candidate.GetType().GetInterface("IHandler`2").GetMethod("TryHandle");
            return (TResponse)tryHandle.Invoke(candidate, new object[] { dispatchable });
        }
    }

    public interface IHandler { }
    public interface IHandler<in TDispatchable, TResponse> : IHandler, IConvertible
        where TDispatchable : IDispatchable
    {
        TResponse TryHandle(TDispatchable dispatchable);
    }

    public interface IDispatchable
    {
    }
}
