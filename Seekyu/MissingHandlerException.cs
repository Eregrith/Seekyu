using System;

namespace Seekyu
{
    public class MissingHandlerException : Exception
    {
    }

    public class MissingHandlerException<TDispatchable, TResponse> : MissingHandlerException
    {
        public override string Message => $"Missing handler implementing IHandler<{typeof(TDispatchable).Name}, {typeof(TResponse).Name}>";
    }
}
