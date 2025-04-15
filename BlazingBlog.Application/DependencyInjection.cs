using System.Reflection.Metadata.Ecma335;
using BlazorCleanArchitecture.Application.Articles;
using BlazorCleanArchitecture.Application.Behaviors;
using BlazorCleanArchitecture.Application.Users.LoginUser;

using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace BlazorCleanArchitecture.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IRequestBus, RequestBus>();

            // Automatically register handlers
            services.Scan(scan => scan
                .FromAssemblyOf<LoginUserCommandHandler>()
                .AddClasses(classes => classes.AssignableToAny(
                    typeof(ICommandHandler<>),
                    typeof(ICommandHandler<,>),
                    typeof(IQueryHandler<,>)
                ))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // Register pipeline behaviors
            services.Scan(scan => scan
                .FromAssemblyOf<LoggingBehavior<LoginUserCommand, Result>>()
                .AddClasses(classes => classes.AssignableTo(typeof(Abstractions.RequestHandler.IPipelineBehavior<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        
    }
}
