namespace Seekyu
{
    public interface IQueryHandler : IHandler
    {
    }

    public interface IQueryHandler<TQuery, TResponse> : IQueryHandler, IHandler<TQuery, TResponse>
        where TQuery : IQuery
        where TResponse : IResponse
    {
    }
}
