using System;
using System.Collections.Generic;
using System.Linq;

namespace Seekyu
{
    public class TooManyQueryHandlersException : Exception
    {
    }

    public class TooManyQueryHandlersException<TQuery, TResponse> : TooManyQueryHandlersException
        where TQuery : IQuery
        where TResponse : IResponse
    {
        public override string Message => $"Too many query handlers implementing IQueryHandler<{typeof(TQuery).Name}, {typeof(TResponse).Name}> are registered :{Environment.NewLine}{HandlersFormatted}";

        public IEnumerable<IQueryHandler<TQuery, TResponse>> Handlers;

        public string HandlersFormatted => String.Join(Environment.NewLine, Handlers.Select(h => $"- {h.GetType().Name}"));

        public TooManyQueryHandlersException(IEnumerable<IQueryHandler<TQuery, TResponse>> handlers)
        {
            Handlers = handlers;
        }
    }
}
