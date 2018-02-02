namespace Seekyu
{
    public interface IQueryDispatcher
    {
        TResponse Dispatch<TResponse>(IQuery query)
            where TResponse : IResponse;
    }
}
