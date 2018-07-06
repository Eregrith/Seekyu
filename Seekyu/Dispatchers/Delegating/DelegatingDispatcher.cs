using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Seekyu.Tests")]
namespace Seekyu.Dispatchers.Delegating
{
    public abstract class DelegatingDispatcher<TDispatchable> : DecoratableDispatcher<TDispatchable>
        where TDispatchable : IDispatchable
    {
        internal IDispatcher<TDispatchable> Next;
    }
}
