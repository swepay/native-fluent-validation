using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Native.FluentValidation.Extensions;
using Native.FluentValidation.NativeLambdaMediator;
using NativeMediator;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace Native.FluentValidation.AwsLambda.Mediator;

public sealed class Function : LambdaMediatorFunction<CreateUserRequest, CreateUserResponse>
{
    public Function()
        : base(BuildServiceProvider())
    {
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddNativeFluentValidation<CreateUserRequest, CreateUserValidator>();
        services.AddTransient<IRequestHandler<CreateUserRequest, CreateUserResponse>, CreateUserHandler>();

        return services.BuildServiceProvider();
    }
}
