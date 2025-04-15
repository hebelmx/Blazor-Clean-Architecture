namespace BlazorCleanArchitecture.Application.Abstractions.RequestHandler;

public interface IRequestBus
{
    Task<Result> Send(ICommand command, CancellationToken cancellationToken = default);
    Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task<Result<TResponse>> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}