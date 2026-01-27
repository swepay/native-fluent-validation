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

    public static void Main()
    {
        const int iterations = 1_000_000;
        var validator = new UserValidator();

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

        Warmup(validator, validUser, invalidUser);

        var validElapsed = Measure(validator, validUser, iterations);
        var invalidElapsed = Measure(validator, invalidUser, iterations);

        Console.WriteLine("Native.FluentValidation Benchmark");
        Console.WriteLine($"Iterations: {iterations:N0}");
        Console.WriteLine($"Valid total: {validElapsed.TotalMilliseconds:N2} ms");
        Console.WriteLine($"Invalid total: {invalidElapsed.TotalMilliseconds:N2} ms");
    }

    private static void Warmup(UserValidator validator, User validUser, User invalidUser)
    {
        for (var i = 0; i < 10_000; i++)
        {
            validator.Validate(validUser);
            validator.Validate(invalidUser);
        }
    }

    private static TimeSpan Measure(UserValidator validator, User user, int iterations)
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
