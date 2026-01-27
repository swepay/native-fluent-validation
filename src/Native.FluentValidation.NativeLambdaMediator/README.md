# Native.FluentValidation.NativeLambdaMediator

Native AOT-friendly validation integration for AWS Lambda using **NativeMediator**. This package provides a base `LambdaMediatorFunction<TRequest, TResponse>` that validates requests and executes a mediator handler using DI.

[![NuGet](https://img.shields.io/nuget/v/Native.FluentValidation.NativeLambdaMediator.svg)](https://www.nuget.org/packages/Native.FluentValidation.NativeLambdaMediator/)

## Installation

```bash
dotnet add package Native.FluentValidation.NativeLambdaMediator
```

## Quick Start

```csharp
using Microsoft.Extensions.DependencyInjection;
using Native.FluentValidation.Extensions;
using Native.FluentValidation.NativeLambdaMediator;
using NativeMediator;

public sealed class Function : LambdaMediatorFunction<CreateUserRequest, CreateUserResponse>
{
    public Function() : base(BuildServiceProvider()) { }

    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddNativeFluentValidation<CreateUserRequest, CreateUserValidator>();
        services.AddTransient<IRequestHandler<CreateUserRequest, CreateUserResponse>, CreateUserHandler>();
        return services.BuildServiceProvider();
    }
}
```

## How it works

1. Creates a scoped service provider per invocation
2. Runs `INativeValidator<TRequest>` if registered
3. Resolves `IRequestHandler<TRequest, TResponse>` and executes it
4. Throws `ValidationException` on invalid requests

## Sample

A complete AWS Lambda sample with SAM deploy tooling is available in:

`samples/Native.FluentValidation.AwsLambda.Mediator`

## License

MIT License - see [LICENSE](../../LICENSE) for details.
