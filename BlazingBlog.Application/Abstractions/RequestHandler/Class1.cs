using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Application.Abstractions.RequestHandler;
public interface IRequestBus
{
    Task<Result> Send(ICommand command, CancellationToken cancellationToken = default);
    Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task<Result<TResponse>> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}

public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();


public class RequestBus : IRequestBus
{
    private readonly IServiceProvider _provider;

    public RequestBus(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task<Result> Send(ICommand command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

        return InvokePipeline<Result>(commandType, command, handlerType, cancellationToken);
    }

    public Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));

        return InvokePipeline<Result<TResponse>>(commandType, command, handlerType, cancellationToken);
    }

    public Task<Result<TResponse>> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));

        return InvokePipeline<Result<TResponse>>(queryType, query, handlerType, cancellationToken);
    }

    private Task<TResponse> InvokePipeline<TResponse>(
        Type requestType,
        object request,
        Type handlerInterfaceType,
        CancellationToken cancellationToken)
    {
        var handler = _provider.GetRequiredService(handlerInterfaceType);

        RequestHandlerDelegate<TResponse> finalHandler = () =>
        {
            var handleMethod = handlerInterfaceType.GetMethod("Handle");
            return (Task<TResponse>)handleMethod!.Invoke(handler, [request, cancellationToken])!;
        };

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviors = _provider.GetServices(behaviorType).Reverse().ToList();

        foreach (var behavior in behaviors)
        {
            var next = finalHandler;
            finalHandler = () =>
            {
                var method = behaviorType.GetMethod("Handle");
                return (Task<TResponse>)method!.Invoke(behavior, [request, next, cancellationToken])!;
            };
        }

        return finalHandler();
    }
}
