using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Native.FluentValidation.Abstractions;
using Native.FluentValidation.Results;
using NativeMediator;

namespace Native.FluentValidation.NativeLambdaMediator;

public abstract class LambdaMediatorFunction<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;

    protected LambdaMediatorFunction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handler(TRequest request, ILambdaContext context)
    {
        using var scope = _serviceProvider.CreateScope();

        var validator = scope.ServiceProvider.GetService<INativeValidator<TRequest>>();
        if (validator is not null)
        {
            var result = validator.Validate(request);
            if (!result.IsValid)
            {
                throw new ValidationException(result);
            }
        }

        var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        return await handler.Handle(request, CancellationToken.None);
    }
}
