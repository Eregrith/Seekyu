using System;

namespace Seekyu
{
    public class MissingQueryHandlerException : Exception
    {
    }

    public class MissingQueryHandlerException<TQuery, TResponse> : MissingQueryHandlerException
    {
        public override string Message => $"Missing query handler IQueryHandler<{typeof(TQuery).Name}, {typeof(TResponse).Name}>";
    }
}
