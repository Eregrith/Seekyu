using System;
using System.Collections.Generic;
using System.Linq;

namespace Seekyu
{
    public class TooManyHandlersException : Exception
    {
    }

    public class TooManyHandlersException<TDispatchable, TResponse> : TooManyHandlersException
        where TDispatchable : IDispatchable
        where TResponse : IResponse
    {
        public override string Message => $"Too many handlers implementing IHandler<{typeof(TDispatchable).Name}, {typeof(TResponse).Name}> are registered :{Environment.NewLine}{HandlersFormatted}";

        public IEnumerable<IHandler<TDispatchable, TResponse>> Handlers;

        public string HandlersFormatted => String.Join(Environment.NewLine, Handlers.Select(h => $"- {h.GetType().Name}"));

        public TooManyHandlersException(IEnumerable<IHandler<TDispatchable, TResponse>> handlers)
        {
            Handlers = handlers;
        }
    }
}
