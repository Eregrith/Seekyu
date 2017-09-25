namespace Seekyu
{
    public interface IQueryDispatcher
    {
        TResponse Handle<TResponse>(IQuery query)
            where TResponse : IResponse;
    }
}
