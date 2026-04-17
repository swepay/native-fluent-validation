# native-fluent-validation

**Pacotes:** 
- Native.FluentValidation (core) v1.0+
- Native.FluentValidation.ExtraRules v1.0.1
- Native.FluentValidation.NativeLambdaMediator v1.0.1

**Tipo:** NuGet Library - Validação AOT-Safe  
**AOT-Safe:** Sim  
**Linguagem:** C# 12+

## O que é

`native-fluent-validation` é uma implementação AOT-safe de validação fluente, compatível com NativeMediator. Diferente do FluentValidation padrão, é zero-reflection e pode ser compilado com PublishAot=true.

## Diferença de FluentValidation Padrão

| Aspecto | native-fluent-validation | FluentValidation |
|---------|---------------------------|------------------|
| Reflection | ❌ Zero | ✅ Sim (scanning) |
| AOT-Safe | ✅ Sim | ❌ Não (requer trimming) |
| Source generators | ✅ Sim | ❌ Não |
| Performance | ✅ Rápido (Lambda) | ❌ Lento (startup) |
| Integração mediator | ✅ Built-in | ❌ Via package adicional |
| CascadeMode | ✅ Suportado | ✅ Suportado |
| Custom rules | ✅ Type-safe | ✅ Type-safe |

## API Pública Principal

### INativeValidator<T>
Interface base para validadores.

```csharp
public interface INativeValidator<T>
{
    ValueTask<ValidationResult> ValidateAsync(
        T instance, 
        CancellationToken cancellationToken = default);
    
    ValidationResult Validate(T instance);
}
```

### NativeValidator<T>
Classe abstrata para criar validadores type-safe.

```csharp
public abstract class NativeValidator<T> : INativeValidator<T>
{
    protected IRuleBuilder<T, TProperty> RuleFor<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression);
    
    public virtual CascadeMode CascadeMode { get; } = CascadeMode.Continue;
    
    public abstract ValueTask<ValidationResult> ValidateAsync(
        T instance, 
        CancellationToken cancellationToken);
    
    public ValidationResult Validate(T instance);
}
```

### RuleBuilder Fluente
```csharp
public interface IRuleBuilder<T, TProperty>
{
    IRuleBuilder<T, TProperty> NotNull();
    IRuleBuilder<T, TProperty> NotEmpty();
    IRuleBuilder<T, TProperty> Length(int min, int max);
    IRuleBuilder<T, TProperty> MinimumLength(int length);
    IRuleBuilder<T, TProperty> MaximumLength(int length);
    IRuleBuilder<T, TProperty> Must(Func<TProperty, bool> predicate);
    IRuleBuilder<T, TProperty> When(Func<T, bool> condition);
    IRuleBuilder<T, TProperty> WithMessage(string message);
    IRuleBuilder<T, TProperty> WithErrorCode(string code);
}
```

### CascadeMode
Define como erros são acumulados.

```csharp
public enum CascadeMode
{
    Continue = 0,    // Continua validando mesmo com erros
    Stop = 1         // Para no primeiro erro de cada propriedade
}
```

## Como Criar um Validator

### Exemplo Básico

```csharp
public record CreateUserCommand(
    string Name,
    string Email,
    int Age)
    : IRequest<CreateUserResponse>;

public class CreateUserCommandValidator 
    : NativeValidator<CreateUserCommand>
{
    public override CascadeMode CascadeMode => CascadeMode.Stop;

    public override async ValueTask<ValidationResult> ValidateAsync(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationFailure>();

        // Name: não vazio, 3-50 caracteres
        var nameBuilder = RuleFor(x => x.Name);
        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(new("Name", "Name is required"));
        else if (request.Name.Length < 3)
            errors.Add(new("Name", "Name must be at least 3 characters"));
        else if (request.Name.Length > 50)
            errors.Add(new("Name", "Name cannot exceed 50 characters"));

        // Email: validar formato
        if (string.IsNullOrEmpty(request.Email))
            errors.Add(new("Email", "Email is required"));
        else if (!request.Email.Contains("@"))
            errors.Add(new("Email", "Email must be valid"));

        // Age: entre 18 e 120
        if (request.Age < 18)
            errors.Add(new("Age", "Must be at least 18 years old"));
        else if (request.Age > 120)
            errors.Add(new("Age", "Age seems invalid"));

        return new ValidationResult(errors);
    }
}
```

### Exemplo com Fluente API (Recomendado)

```csharp
public class CreateProductCommandValidator 
    : NativeValidator<CreateProductCommand>
{
    private readonly IProductRepository _repository;

    public CreateProductCommandValidator(IProductRepository repository)
    {
        _repository = repository;
    }

    public override CascadeMode CascadeMode => CascadeMode.Continue;

    public override async ValueTask<ValidationResult> ValidateAsync(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationFailure>();

        // Validar Name
        var nameErrors = ValidateName(request.Name);
        errors.AddRange(nameErrors);

        // Validar SKU (único no banco)
        var skuExists = await _repository.SkuExistsAsync(request.Sku, cancellationToken);
        if (skuExists)
            errors.Add(new ValidationFailure("Sku", "SKU already exists"));

        // Validar Price
        if (request.Price <= 0)
            errors.Add(new ValidationFailure("Price", "Price must be positive"));

        return new ValidationResult(errors);
    }

    private List<ValidationFailure> ValidateName(string name)
    {
        var errors = new List<ValidationFailure>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(new ValidationFailure("Name", "Product name is required"));
        else if (name.Length < 3)
            errors.Add(new ValidationFailure("Name", "Name must be at least 3 characters"));
        else if (name.Length > 100)
            errors.Add(new ValidationFailure("Name", "Name cannot exceed 100 characters"));

        return errors;
    }
}
```

## Integração com NativeMediator

### Pipeline Behavior de Validação

```csharp
public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;

    public async ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Procurar validator para este request type
        var validatorType = typeof(INativeValidator<>).MakeGenericType(typeof(TRequest));
        
        if (_serviceProvider.GetService(validatorType) is INativeValidator validator)
        {
            var resultType = typeof(IValidationResult);
            var validateMethod = validatorType.GetMethod(nameof(INativeValidator.ValidateAsync));
            
            // Validar
            var result = await validator.ValidateAsync(request, cancellationToken);
            
            if (!result.IsValid)
            {
                var errors = string.Join(", ", 
                    result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                throw new ValidationException($"Validation failed: {errors}");
            }
        }

        return await next();
    }
}
```

### Registrar Validators

```csharp
services.AddSingleton<INativeValidator<CreateProductCommand>, 
    CreateProductCommandValidator>();
services.AddSingleton<INativeValidator<UpdateProductCommand>, 
    UpdateProductCommandValidator>();
services.AddSingleton<INativeValidator<DeleteProductCommand>, 
    DeleteProductCommandValidator>();

// Pipeline behavior
services.AddSingleton(typeof(IPipelineBehavior<,>), 
    typeof(ValidationBehavior<,>));
```

## ExtraRules (Native.FluentValidation.ExtraRules)

Regras adicionais comuns:

### Regras de String
- `.Email()` - Validar formato de email
- `.Url()` - Validar URL válida
- `.CreditCard()` - Validar número de cartão
- `.Regex(pattern)` - Match com regex
- `.StartsWith(value)` - Começa com
- `.EndsWith(value)` - Termina com

### Regras de Número
- `.GreaterThan(value)` - Maior que
- `.LessThan(value)` - Menor que
- `.InclusiveBetween(min, max)` - Entre (inclusive)
- `.ExclusiveBetween(min, max)` - Entre (exclusivo)

### Regras de Coleção
- `.NotEmpty()` - Lista não vazia
- `.MinimumCollectionLength(count)` - Mínimo de itens
- `.MaximumCollectionLength(count)` - Máximo de itens
- `.Must(collection => ...)` - Predicado customizado

### Uso
```csharp
public class ContactValidator : NativeValidator<Contact>
{
    public override async ValueTask<ValidationResult> ValidateAsync(
        Contact request,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationFailure>();

        if (!IsValidEmail(request.Email))
            errors.Add(new("Email", "Email is invalid"));

        if (!IsValidUrl(request.Website))
            errors.Add(new("Website", "Website URL is invalid"));

        if (request.PhoneNumbers?.Count == 0)
            errors.Add(new("PhoneNumbers", "At least one phone number required"));

        return new ValidationResult(errors);
    }

    private bool IsValidEmail(string email)
        => email?.Contains("@") == true && email.Contains(".");

    private bool IsValidUrl(string url)
        => url?.StartsWith("http") == true;
}
```

## Premissas

- **Zero reflection:** Sem scanning automático, registro explícito
- **AOT-Safe:** PublishAot = true, sem runtime type discovery
- **ValueTask:** Validators retornam `ValueTask` ou `ValueTask<ValidationResult>`
- **Namespace:** `Native.FluentValidation`
- **Target:** `net8.0`
- **Sem dependencies externas:** Apenas `System.*` packages
- **CascadeMode é propriedade:** Pode ser personalizado por validator

## Terminologia

- **ValidationFailure:** Erro individual (PropertyName, ErrorMessage, ErrorCode)
- **ValidationResult:** Coleção de erros com flag IsValid
- **CascadeMode.Continue:** Coleta todos os erros de todas as regras
- **CascadeMode.Stop:** Para no primeiro erro por propriedade
- **Async validation:** Validações que precisam hit database (SKU único, etc)

## Limitações

- Um validator por tipo (não há prioridades)
- Sem validação condicional complexa (use `.When()`)
- Sem herança de validators (implemente em classe separada)
- CustomRules requerem método Helper (sem extensão fluente)
