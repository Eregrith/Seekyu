namespace Seekyu
{
    public class QueryDispatcher : Dispatcher<IQuery>, IQueryDispatcher
    {
        public QueryDispatcher(params IQueryHandler[] handlers) : base(handlers)
        {
        }
    }
}
