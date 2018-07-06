using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seekyu
{
    public interface IHandler { }
    public interface IHandler<in TDispatchable, out TResponse> : IHandler
        where TDispatchable : IDispatchable
    {
        TResponse TryHandle(TDispatchable dispatchable);
    }
}
