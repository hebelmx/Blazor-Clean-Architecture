namespace BlazorCleanArchitecture.Application.Abstractions.RequestHandler;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();