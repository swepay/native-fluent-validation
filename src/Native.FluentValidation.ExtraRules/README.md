# Native.FluentValidation.ExtraRules

Extra validation rules for **Native.FluentValidation**, including regex and comparison helpers.

[![NuGet](https://img.shields.io/nuget/v/Native.FluentValidation.ExtraRules.svg)](https://www.nuget.org/packages/Native.FluentValidation.ExtraRules/)

## Installation

```bash
dotnet add package Native.FluentValidation.ExtraRules
```

## Rules

- `Matches(pattern)` / `Matches(regex)`
- `InclusiveBetween(min, max)`
- `LessThanOrEqual(value)`
- `GreaterThanOrEqual(value)`
- `NotEqual(value)`

## Example

```csharp
RuleFor(x => x.Email, nameof(User.Email))
    .Matches("@example.com$");

RuleFor(x => x.Age, nameof(User.Age))
    .InclusiveBetween(18, 120);
```

## License

MIT License - see [LICENSE](../../LICENSE) for details.
