

namespace BlazorCleanArchitecture.Application.Abstractions.RequestHandler
{
    // CQRS Pattern
    // Command Handler for commands that do not return any specific result other than success or failure
    public interface ICommandHandler<TCommand> 
        where TCommand : ICommand
    {
        Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
    }
    // CQRS Pattern
    // Command Handler for commands that do return a specific response type
    public interface ICommandHandler<TCommand, TResponse> 
        where TCommand : ICommand<TResponse>
    {
        Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
    }
}
