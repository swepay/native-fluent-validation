# Native.FluentValidation

> Validação fluente AOT-first em .NET 10 — sem reflexão, sem expression trees, sem scanning em runtime. Construída para AWS Lambda, Azure Functions e qualquer app que publique com `PublishAot=true`.

[![NuGet — Native.FluentValidation](https://img.shields.io/nuget/v/Native.FluentValidation.svg?label=Native.FluentValidation)](https://www.nuget.org/packages/Native.FluentValidation/)
[![NuGet — ExtraRules](https://img.shields.io/nuget/v/Native.FluentValidation.ExtraRules.svg?label=ExtraRules)](https://www.nuget.org/packages/Native.FluentValidation.ExtraRules/)
[![NuGet — NativeLambdaMediator](https://img.shields.io/nuget/v/Native.FluentValidation.NativeLambdaMediator.svg?label=NativeLambdaMediator)](https://www.nuget.org/packages/Native.FluentValidation.NativeLambdaMediator/)
[![Build](https://github.com/swepay/native-fluent-validation/actions/workflows/dotnet.yml/badge.svg)](https://github.com/swepay/native-fluent-validation/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/swepay/native-fluent-validation/branch/main/graph/badge.svg)](https://codecov.io/gh/swepay/native-fluent-validation)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Monorepo

Este repositório contém três pacotes NuGet e um source generator interno:

| Pacote | Propósito | README detalhado |
| ------ | --------- | ---------------- |
| [Native.FluentValidation](src/Native.FluentValidation/README.md) | Biblioteca principal — `INativeValidator<T>`, `RuleFor(...)`, regras embutidas | [↗](src/Native.FluentValidation/README.md) |
| [Native.FluentValidation.ExtraRules](src/Native.FluentValidation.ExtraRules/README.md) | Regras extras específicas do domínio Swepay (CPF, CNPJ, e outras) | [↗](src/Native.FluentValidation.ExtraRules/README.md) |
| [Native.FluentValidation.NativeLambdaMediator](src/Native.FluentValidation.NativeLambdaMediator/README.md) | Integração com [NativeMediator](https://github.com/swepay/native-mediator) + AWS Lambda | [↗](src/Native.FluentValidation.NativeLambdaMediator/README.md) |
| `Native.FluentValidation.SourceGenerator` | Gerador Roslyn interno que emite o boilerplate de construção de validators — distribuído dentro do pacote principal | — |

## Quickstart

### 1. Install

```bash
dotnet add package Native.FluentValidation
```

### 2. Declare o validator

```csharp
using Native.FluentValidation;

public sealed class CreateUserValidator : NativeValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(nameof(CreateUserCommand.Email))
            .NotEmpty()
            .EmailAddress();

        RuleFor(nameof(CreateUserCommand.Age))
            .GreaterThanOrEqualTo(18);
    }
}

public sealed record CreateUserCommand(string Email, int Age);
```

### 3. Valide

```csharp
var validator = new CreateUserValidator();
var result = validator.Validate(new CreateUserCommand("alex@swepay.com.br", 30));

if (!result.IsValid)
    throw new ValidationException(result.Errors);
```

## Quando usar (e quando não)

- **Use** em handlers AOT-first (`NativeMediator`, AWS Lambda com `PublishAot=true`, Azure Functions isolated).
- **Use** quando você quer zero alocações de reflection no caminho quente.
- **Prefira `FluentValidation` oficial** se AOT não é requisito e você precisa de validators discovered por assembly scanning.

## Samples executáveis

- [`samples/Native.FluentValidation.AwsLambda`](samples/Native.FluentValidation.AwsLambda/) — AWS Lambda puro
- [`samples/Native.FluentValidation.AwsLambda.Mediator`](samples/Native.FluentValidation.AwsLambda.Mediator/) — integração com NativeMediator

## Governança do ecossistema

- [Charter de papeis — Arquiteto, Security, UX Writer](https://github.com/swepay/.github/blob/main/CHARTER.md)
- [GAPS_ROADMAP do ecossistema](https://github.com/swepay/.github/blob/main/GAPS_ROADMAP.md)

## Contribuindo, Segurança, Licença

- [Contribuindo](CONTRIBUTING.md)
- [Política de segurança](SECURITY.md) — disclosure coordenado via `security@swepay.com.br`
- MIT — veja [LICENSE](LICENSE)

## Roadmap

Ver [ROADMAP.md](ROADMAP.md) para fases concluídas e próximas.
