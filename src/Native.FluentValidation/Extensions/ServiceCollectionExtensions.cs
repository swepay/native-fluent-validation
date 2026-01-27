using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Native.FluentValidation.Abstractions;
using Native.FluentValidation.Pipelines;
using NativeMediator;

namespace Native.FluentValidation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNativeFluentValidation<TRequest, TValidator>(
        this IServiceCollection services)
        where TValidator : class, INativeValidator<TRequest>, new()
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAdd(ServiceDescriptor.Transient<INativeValidator<TRequest>>(_ => new TValidator()));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }
}
