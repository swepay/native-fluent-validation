using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Native.FluentValidation.Builders;
using Native.FluentValidation.Core;
using Native.FluentValidation.Extensions;
using Native.FluentValidation.NativeLambdaMediator;
using Native.FluentValidation.Results;
using NativeMediator;
using Xunit;

namespace Native.FluentValidation.NativeLambdaMediator.Tests;

public sealed class LambdaMediatorFunctionTests
{
    private sealed class CreateUserRequest : IRequest<CreateUserResponse>
    {
        public string? Email { get; set; }
    }

    private sealed class CreateUserResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    private sealed class CreateUserValidator : NativeValidator<CreateUserRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email, nameof(CreateUserRequest.Email))
                .NotEmpty()
                .Email();
        }
    }

    private sealed class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserResponse>
    {
        public ValueTask<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult(new CreateUserResponse { Message = "ok" });
        }
    }

    private sealed class TestFunction : LambdaMediatorFunction<CreateUserRequest, CreateUserResponse>
    {
        public TestFunction(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
    }

    private sealed class StubLambdaContext : ILambdaContext
    {
        public string AwsRequestId => "req";
        public IClientContext ClientContext => null!;
        public string FunctionName => "test";
        public string FunctionVersion => "1";
        public ICognitoIdentity Identity => null!;
        public string InvokedFunctionArn => "arn";
        public ILambdaLogger Logger => new StubLambdaLogger();
        public string LogGroupName => "log-group";
        public string LogStreamName => "log-stream";
        public int MemoryLimitInMB => 256;
        public TimeSpan RemainingTime => TimeSpan.FromMinutes(1);
    }

    private sealed class StubLambdaLogger : ILambdaLogger
    {
        public void Log(string message) { }
        public void LogLine(string message) { }
    }

    [Fact]
    public async Task Handler_ValidRequest_ReturnsResponse()
    {
        var services = new ServiceCollection();
        services.AddNativeFluentValidation<CreateUserRequest, CreateUserValidator>();
        services.AddTransient<IRequestHandler<CreateUserRequest, CreateUserResponse>, CreateUserHandler>();

        var function = new TestFunction(services.BuildServiceProvider());
        var response = await function.Handler(new CreateUserRequest { Email = "user@example.com" }, new StubLambdaContext());

        Assert.Equal("ok", response.Message);
    }

    [Fact]
    public async Task Handler_InvalidRequest_ThrowsValidationException()
    {
        var services = new ServiceCollection();
        services.AddNativeFluentValidation<CreateUserRequest, CreateUserValidator>();
        services.AddTransient<IRequestHandler<CreateUserRequest, CreateUserResponse>, CreateUserHandler>();

        var function = new TestFunction(services.BuildServiceProvider());

        await Assert.ThrowsAsync<ValidationException>(
            () => function.Handler(new CreateUserRequest { Email = "invalid" }, new StubLambdaContext()));
    }
}
