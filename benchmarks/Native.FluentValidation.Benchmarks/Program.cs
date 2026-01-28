using FluentValidation;
using Native.FluentValidation.Builders;
using Native.FluentValidation.Core;
using System.Diagnostics;

namespace Native.FluentValidation.Benchmarks;

public static class Program
{
    private sealed class User
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    private sealed class UserValidator : NativeValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email, nameof(User.Email))
                .NotEmpty()
                .Email();

            RuleFor(x => x.Name, nameof(User.Name))
                .NotNull()
                .Length(2, 40);

            RuleFor(x => x.Age, nameof(User.Age))
                .GreaterThan(17)
                .Must(age => age < 120);
        }
    }

    private sealed class FluentUserValidator : AbstractValidator<User>
    {
        public FluentUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Name)
                .NotNull()
                .Length(2, 40);

            RuleFor(x => x.Age)
                .GreaterThan(17)
                .Must(age => age < 120);
        }
    }

    public static void Main()
    {
        const int iterations = 1_000_000;
        var nativeValidator = new UserValidator();
        var fluentValidator = new FluentUserValidator();

        var validUser = new User
        {
            Email = "user@example.com",
            Name = "Benchmark",
            Age = 30
        };

        var invalidUser = new User
        {
            Email = string.Empty,
            Name = "",
            Age = 10
        };

        Warmup(nativeValidator, fluentValidator, validUser, invalidUser);

        var nativeValidElapsed = MeasureNative(nativeValidator, validUser, iterations);
        var nativeInvalidElapsed = MeasureNative(nativeValidator, invalidUser, iterations);
        var fluentValidElapsed = MeasureFluent(fluentValidator, validUser, iterations);
        var fluentInvalidElapsed = MeasureFluent(fluentValidator, invalidUser, iterations);

        Console.WriteLine("Native.FluentValidation Benchmark");
        Console.WriteLine($"Iterations: {iterations:N0}");
        Console.WriteLine($"Native Valid total: {nativeValidElapsed.TotalMilliseconds:N2} ms");
        Console.WriteLine($"Native Invalid total: {nativeInvalidElapsed.TotalMilliseconds:N2} ms");
        Console.WriteLine($"Fluent Valid total: {fluentValidElapsed.TotalMilliseconds:N2} ms");
        Console.WriteLine($"Fluent Invalid total: {fluentInvalidElapsed.TotalMilliseconds:N2} ms");
    }

    private static void Warmup(
        UserValidator nativeValidator,
        FluentUserValidator fluentValidator,
        User validUser,
        User invalidUser)
    {
        for (var i = 0; i < 10_000; i++)
        {
            nativeValidator.Validate(validUser);
            nativeValidator.Validate(invalidUser);
            fluentValidator.Validate(validUser);
            fluentValidator.Validate(invalidUser);
        }
    }

    private static TimeSpan MeasureNative(UserValidator validator, User user, int iterations)
    {
        var stopwatch = Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++)
        {
            validator.Validate(user);
        }
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    private static TimeSpan MeasureFluent(FluentUserValidator validator, User user, int iterations)
    {
        var stopwatch = Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++)
        {
            validator.Validate(user);
        }
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }
}
