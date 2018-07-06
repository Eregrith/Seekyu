using Seekyu.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seekyu.Dispatchers
{
    public class Dispatcher<TDispatchable> : DecoratableDispatcher<TDispatchable>
        where TDispatchable : IDispatchable
    {
        private readonly string HandlerName = typeof(IHandler<,>).Name;
        private readonly Dictionary<Tuple<Type, Type>, IHandler> TypedHandlers;

        public Dispatcher(params IHandler[] handlers)
        {
            TypedHandlers = new Dictionary<Tuple<Type, Type>, IHandler>();
            foreach (IHandler handler in handlers)
            {
                foreach (Type @interface in handler.GetType().GetInterfaces().Where(i => i.Name == HandlerName))
                {
                    var generics = @interface.GetGenericArguments();
                    if (generics.Count() == 2)
                    {
                        if (TypedHandlers.Any(k => k.Key.Item1 == generics[0] && k.Key.Item2 == generics[1]))
                            throw new DuplicatedHandlerException(generics[0], generics[1]);

                        TypedHandlers.Add(new Tuple<Type, Type>(generics[0], generics[1]), handler);
                    }
                }
            }
        }

        public override TResponse Dispatch<TResponse>(TDispatchable dispatchable)
        {
            Type queryType = dispatchable.GetType();
            Type responseType = typeof(TResponse);
            Func<KeyValuePair<Tuple<Type, Type>, IHandler>, bool> matchingHandler = k => k.Key.Item1 == queryType && k.Key.Item2 == responseType;
            if (!TypedHandlers.Any(matchingHandler))
            {
                MissingHandlerException exception = (MissingHandlerException)typeof(MissingHandlerException)
                                                                        .GetMethod("For")
                                                                        .MakeGenericMethod(queryType, responseType)
                                                                        .Invoke(null, null);
                throw exception;
            }
            IHandler candidate = TypedHandlers.First(matchingHandler).Value;

            Type handlerInterfaceSpecificType = typeof(IHandler<,>).MakeGenericType(queryType, responseType);

            var type = candidate.GetType();
            var @interface = type.GetInterfaces().First(i => i.FullName == handlerInterfaceSpecificType.FullName);
            var tryHandle = @interface.GetMethod("TryHandle", new Type[] { queryType });

            return (TResponse)tryHandle.Invoke(candidate, new object[] { dispatchable });
        }
    }
}
