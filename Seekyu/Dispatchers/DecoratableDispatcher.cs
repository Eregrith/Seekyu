using Seekyu.Dispatchers.Delegating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seekyu.Dispatchers
{
    public abstract class DecoratableDispatcher<TDispatchable> : IDispatcher<TDispatchable>
        where TDispatchable : IDispatchable
    {
        public DelegatingDispatcher<TDispatchable> DecorateWith(DelegatingDispatcher<TDispatchable> decorator)
        {
            decorator.Next = this;
            return decorator;
        }

        public abstract TResult Dispatch<TResult>(TDispatchable dispatchable);
    }
}
