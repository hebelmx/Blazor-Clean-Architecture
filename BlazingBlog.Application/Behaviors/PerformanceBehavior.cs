using System.Diagnostics;
using Microsoft.Extensions.Logging;


namespace BlazorCleanArchitecture.Application.Behaviors;

/// <summary>
/// Pipeline Behavior for logging the performance of request handling.
/// </summary>
/// <typeparam name="TRequest">Type of the request being processed.</typeparam>
/// <typeparam name="TResponse">Type of the response produced.</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;
    private readonly Stopwatch _timer;

    /// <summary>
    /// Constructor initializes logger and stopwatch.
    /// </summary>
    public PerformanceBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    /// <summary>
    /// Handle the incoming request, measure and log the execution time.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">Delegate for the next action in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response of the action.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();



        if (_timer.ElapsedMilliseconds <= 500)
        {
            _logger.LogInformation("Performance Behavior: {Name} ({ElapsedMilliseconds} ms) {@Request}",
                typeof(TRequest).Name, _timer.ElapsedMilliseconds, request);
        }
        else
        {
            _logger.LogWarning("Performance Behavior Long Running Request: {Name} ({ElapsedMilliseconds} ms) {@Request}",
                typeof(TRequest).Name, _timer.ElapsedMilliseconds, request);
        }

        return response;
    }
}
