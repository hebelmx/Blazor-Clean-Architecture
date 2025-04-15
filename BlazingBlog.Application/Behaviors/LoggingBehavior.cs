
using Microsoft.Extensions.Logging;

namespace BlazorCleanArchitecture.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        // Get the type name, handling generics appropriately
        var responseType = typeof(TResponse);
        var responseName = ResponseName(responseType);


        _logger.LogInformation("LoggingBehavior: {RequestName} of {ResponseName}", requestName, responseName);


        // Call the next handler in the pipeline
        // Proceed with the next handler in the pipeline
        TResponse response = await next();


        _logger.LogInformation("LoggingBehavior: {RequestName} of {ResponseName}", requestName, responseName);

 
        return response;
    }

    private static string ResponseName(Type responseType)
    {
        string responseName;

        if (responseType.IsGenericType)
        {
            // Handle generic types like Result<T>
            var genericTypeDefinition = responseType.GetGenericTypeDefinition();
            var genericArguments = responseType.GetGenericArguments();
            var genericArgumentNames = string.Join(", ", genericArguments.Select(arg => arg.Name));

            // Format as Result<T>
            responseName = $"{genericTypeDefinition.Name.Split('`')[0]}<{genericArgumentNames}>";
        }
        else
        {
            // Non-generic types
            responseName = responseType.Name;
        }

        return responseName;
    }
}