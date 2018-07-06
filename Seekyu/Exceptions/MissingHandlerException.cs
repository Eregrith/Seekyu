using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seekyu.Exceptions
{
    public class MissingHandlerException : Exception
    {
        public static MissingHandlerException For<TDispatchable, TResult>()
        {
            return new MissingHandlerException($"No handler implementing IHandler<{typeof(TDispatchable).Name}, {typeof(TResult).Name}> has been found");
        }


        public MissingHandlerException(string message) : base(message)
        {
        }
    }
}
