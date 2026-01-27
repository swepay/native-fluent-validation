using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Native.FluentValidation.Abstractions;
using Native.FluentValidation.Pipelines;
using NativeMediator;

namespace Native.FluentValidation.Extensions;

public sealed class NativeFluentValidationBuilder
{
    private readonly IServiceCollection _services;

    internal NativeFluentValidationBuilder(IServiceCollection services)
    {
        _services = services;
        _services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    }

    public NativeFluentValidationBuilder AddValidator<TRequest, TValidator>()
        where TValidator : class, INativeValidator<TRequest>, new()
    {
        _services.AddTransient<INativeValidator<TRequest>>(_ => new TValidator());
        return this;
    }

    public NativeFluentValidationBuilder AddValidator<TRequest>(
        Func<IServiceProvider, INativeValidator<TRequest>> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _services.AddTransient(factory);
        return this;
    }
}
