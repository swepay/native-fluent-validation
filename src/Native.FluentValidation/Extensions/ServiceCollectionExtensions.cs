using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Native.FluentValidation.Abstractions;
using Native.FluentValidation.Pipelines;
using NativeMediator;

namespace Native.FluentValidation.Extensions;

public static class ServiceCollectionExtensions
{
    public static NativeFluentValidationBuilder AddNativeFluentValidation(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return new NativeFluentValidationBuilder(services);
    }

    public static IServiceCollection AddNativeFluentValidation(
        this IServiceCollection services,
        Action<NativeFluentValidationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new NativeFluentValidationBuilder(services);
        configure(builder);
        return services;
    }

    public static IServiceCollection AddNativeFluentValidation<TRequest, TValidator>(
        this IServiceCollection services)
        where TValidator : class, INativeValidator<TRequest>, new()
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = new NativeFluentValidationBuilder(services);
        builder.AddValidator<TRequest, TValidator>();

        return services;
    }
}
