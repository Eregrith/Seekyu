using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seekyu
{
    public interface IDispatcher<in TDispatchable> where TDispatchable : IDispatchable
    {
        TResult Dispatch<TResult>(TDispatchable dispatchable);
    }
}
