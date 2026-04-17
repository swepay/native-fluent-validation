# Developer Agent - native-fluent-validation

**Modelo:** Claude Sonnet 4  
**Ferramentas:** read, write, bash, edit, grep, glob  
**Foco:** Implementar validators, integração com mediator, regras custom

## Responsabilidades

1. **Criar NativeValidator<T>** para novos request types
2. **Integrar com NativeMediator** via ValidationBehavior
3. **Implementar custom validation rules** (async e sync)
4. **Usar ExtraRules** para validações comuns
5. **Garantir AOT-compliance** (sem reflection)

## Fluxo de Trabalho

### Criar Novo Validator

1. **Abrir CLAUDE.md** para revisar padrões
2. **Criar classe validadora** herdando de `NativeValidator<T>`
3. **Implementar ValidateAsync()** com regras
4. **Usar CascadeMode apropriado** (Continue vs Stop)
5. **Registrar em ServiceCollection**
6. **Integrar em ValidationBehavior do mediator**
7. **Testar com xUnit**

### Estrutura de Validator Básico

```csharp
// 1. Request type (no arquivo de Commands)
public record CreateUserCommand(
    string Name,
    string Email,
    int Age)
    : IRequest<CreateUserResponse>;

// 2. Validator class
public class CreateUserCommandValidator 
    : NativeValidator<CreateUserCommand>
{
    private readonly IUserRepository _repository;

    public CreateUserCommandValidator(IUserRepository repository)
    {
        _repository = repository;
    }

    // Define comportamento de cascata
    public override CascadeMode CascadeMode => CascadeMode.Stop;

    // Implementar validação
    public override async ValueTask<ValidationResult> ValidateAsync(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationFailure>();

        // Validar Name
        errors.AddRange(ValidateName(request.Name));

        // Validar Email
        errors.AddRange(await ValidateEmailAsync(request.Email, cancellationToken));

        // Validar Age
        errors.AddRange(ValidateAge(request.Age));

        return new ValidationResult(errors);
    }

    private List<ValidationFailure> ValidateName(string name)
    {
        var errors = new List<ValidationFailure>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(new ValidationFailure("Name", "Name is required", name)
            {
                ErrorCode = "NotEmpty"
            });
        else if (name.Length < 3)
            errors.Add(new ValidationFailure("Name", 
                "Name must be at least 3 characters", name)
            {
                ErrorCode = "MinimumLength"
            });
        else if (name.Length > 100)
            errors.Add(new ValidationFailure("Name", 
                "Name cannot exceed 100 characters", name)
            {
                ErrorCode = "MaximumLength"
            });

        return errors;
    }

    private async ValueTask<List<ValidationFailure>> ValidateEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationFailure>();

        if (string.IsNullOrEmpty(email))
            errors.Add(new ValidationFailure("Email", "Email is required"));
        else if (!IsValidEmailFormat(email))
            errors.Add(new ValidationFailure("Email", "Email format is invalid"));
        else
        {
            // Validação async - verificar se email já existe
            var exists = await _repository.EmailExistsAsync(email, cancellationToken);
            if (exists)
                errors.Add(new ValidationFailure("Email", "Email already registered"));
        }

        return errors;
    }

    private List<ValidationFailure> ValidateAge(int age)
    {
        var errors = new List<ValidationFailure>();

        if (age < 13)
            errors.Add(new ValidationFailure("Age", "Must be at least 13 years old"));
        else if (age > 150)
            errors.Add(new ValidationFailure("Age", "Age seems invalid"));

        return errors;
    }

    private bool IsValidEmailFormat(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

// 3. Registrar
services.AddSingleton<INativeValidator<CreateUserCommand>, 
    CreateUserCommandValidator>();
```

### Validator com CascadeMode.Continue (Coleta Todos os Erros)

```csharp
public class UpdateProductCommandValidator 
    : NativeValidator<UpdateProductCommand>
{
    public override CascadeMode CascadeMode => CascadeMode.Continue;

    public override async ValueTask<ValidationResult> ValidateAsync(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationFailure>();

        // Validar Name (coletará erro se presente)
        if (string.IsNullOrEmpty(request.Name))
            errors.Add(new("Name", "Name is required"));
        else if (request.Name.Length < 3)
            errors.Add(new("Name", "Name too short"));

        // Validar Description (continua mesmo se Name falhou)
        if (string.IsNullOrEmpty(request.Description))
            errors.Add(new("Description", "Description is required"));
        else if (request.Description.Length > 500)
            errors.Add(new("Description", "Description too long"));

        // Validar Price (continua mesmo se acima falhou)
        if (request.Price <= 0)
            errors.Add(new("Price", "Price must be positive"));
        else if (request.Price > decimal.MaxValue / 2)
            errors.Add(new("Price", "Price too high"));

        return new ValidationResult(errors);
    }
}
```

### Validator com Regras Condicionais

```csharp
public class CreateOrderCommandValidator 
    : NativeValidator<CreateOrderCommand>
{
    public override async ValueTask<ValidationResult> ValidateAsync(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationFailure>();

        errors.AddRange(ValidateItems(request.Items));

        // Validação condicional: se é order internacional, validar diferente
        if (request.IsInternational)
            errors.AddRange(ValidateInternationalShipping(request));
        else
            errors.AddRange(ValidateDomesticShipping(request));

        return new ValidationResult(errors);
    }

    private List<ValidationFailure> ValidateItems(List<OrderItem> items)
    {
        var errors = new List<ValidationFailure>();

        if (items == null || items.Count == 0)
            errors.Add(new("Items", "Order must have at least one item"));
        else
        {
            foreach (var item in items)
            {
                if (item.Quantity <= 0)
                    errors.Add(new("Items", 
                        $"Item {item.ProductId} quantity must be positive"));
            }
        }

        return errors;
    }

    private List<ValidationFailure> ValidateInternationalShipping(
        CreateOrderCommand request)
    {
        var errors = new List<ValidationFailure>();

        if (string.IsNullOrEmpty(request.InternationalTrackingNumber))
            errors.Add(new("InternationalTrackingNumber", "Required for international"));

        if (request.DeclaredValue <= 0)
            errors.Add(new("DeclaredValue", "Must declare value for international"));

        return errors;
    }

    private List<ValidationFailure> ValidateDomesticShipping(
        CreateOrderCommand request)
    {
        var errors = new List<ValidationFailure>();

        if (string.IsNullOrEmpty(request.PostalCode))
            errors.Add(new("PostalCode", "Required for domestic shipping"));

        return errors;
    }
}
```

### Usar ExtraRules (Native.FluentValidation.ExtraRules)

```csharp
public class ContactValidator : NativeValidator<Contact>
{
    public override async ValueTask<ValidationResult> ValidateAsync(
        Contact request,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationFailure>();

        // Email validation
        if (string.IsNullOrEmpty(request.Email))
            errors.Add(new("Email", "Email is required"));
        else if (!IsValidEmail(request.Email))
            errors.Add(new("Email", "Email format is invalid"));

        // URL validation
        if (!string.IsNullOrEmpty(request.Website) && !IsValidUrl(request.Website))
            errors.Add(new("Website", "Website must be a valid URL"));

        // Credit card validation (se presente)
        if (!string.IsNullOrEmpty(request.CardNumber) && 
            !IsValidCreditCard(request.CardNumber))
            errors.Add(new("CardNumber", "Credit card number is invalid"));

        // Phone validation
        if (!string.IsNullOrEmpty(request.Phone) && 
            !IsValidPhone(request.Phone))
            errors.Add(new("Phone", "Phone number format is invalid"));

        return new ValidationResult(errors);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email && email.Contains(".");
        }
        catch { return false; }
    }

    private bool IsValidUrl(string url)
        => url.StartsWith("http://") || url.StartsWith("https://");

    private bool IsValidCreditCard(string cardNumber)
    {
        // Luhn algorithm
        if (cardNumber == null || cardNumber.Length < 13 || cardNumber.Length > 19)
            return false;

        var digits = cardNumber.Where(char.IsDigit).ToArray();
        if (digits.Length != cardNumber.Length)
            return false;

        int sum = 0;
        bool isSecond = false;

        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int digit = digits[i] - '0';

            if (isSecond)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            isSecond = !isSecond;
        }

        return sum % 10 == 0;
    }

    private bool IsValidPhone(string phone)
        => phone.Replace(" ", "").Replace("-", "").Replace("+", "")
            .All(c => char.IsDigit(c)) && 
           phone.Length >= 10 && phone.Length <= 15;
}
```

### Integrar com NativeMediator

```csharp
// Extensions method para registrar validators
public static partial class ServiceCollectionExtensions
{
    [RegisterServices]
    public static IServiceCollection AddValidators(
        this IServiceCollection services)
    {
        // Core validators
        services.AddSingleton<INativeValidator<CreateUserCommand>,
            CreateUserCommandValidator>();
        services.AddSingleton<INativeValidator<UpdateUserCommand>,
            UpdateUserCommandValidator>();
        services.AddSingleton<INativeValidator<CreateProductCommand>,
            CreateProductCommandValidator>();

        // Pipeline behavior com validação
        services.AddSingleton(typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }
}

// Behavior que usa os validators
public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public async ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validatorType = typeof(INativeValidator<>)
            .MakeGenericType(typeof(TRequest));

        if (_serviceProvider.GetService(validatorType) 
            is INativeValidator validator)
        {
            _logger.LogInformation("Validating {RequestType}", typeof(TRequest).Name);

            var validateMethod = validatorType
                .GetMethod(nameof(INativeValidator.ValidateAsync));
            
            var resultTask = (ValueTask<ValidationResult>)validateMethod
                .Invoke(validator, new object[] { request, cancellationToken })!;
            
            var result = await resultTask;

            if (!result.IsValid)
            {
                var failureMessages = string.Join("; ",
                    result.Errors.Select(e => 
                        $"{e.PropertyName}: {e.ErrorMessage}"));

                _logger.LogWarning("Validation failed: {Failures}", failureMessages);

                throw new ValidationException(
                    $"Validation failed for {typeof(TRequest).Name}: {failureMessages}",
                    result.Errors);
            }

            _logger.LogInformation("Validation passed for {RequestType}", 
                typeof(TRequest).Name);
        }

        return await next();
    }
}
```

## Checklist Antes de Submeter

- [ ] Validator herdando de `NativeValidator<T>`
- [ ] `ValidateAsync()` implementado com `ValueTask<ValidationResult>`
- [ ] Regras de validação cobrindo todos os campos críticos
- [ ] CascadeMode definido apropriadamente
- [ ] Validações async (e.g., unicidade) implementadas
- [ ] Registrado em ServiceCollection
- [ ] Integrado em ValidationBehavior (se usando mediator)
- [ ] Testes xUnit com casos válidos e inválidos
- [ ] `dotnet test` 100% passando
- [ ] Sem reflection ou scanning

## Testes Esperados

```csharp
[Fact]
public async Task ValidateAsync_ValidRequest_ReturnsNoErrors()
{
    var validator = new CreateUserCommandValidator(_mockRepository.Object);
    var command = new CreateUserCommand("John Doe", "john@example.com", 25);

    var result = await validator.ValidateAsync(command, CancellationToken.None);

    Assert.True(result.IsValid);
    Assert.Empty(result.Errors);
}

[Fact]
public async Task ValidateAsync_InvalidEmail_ReturnsError()
{
    var validator = new CreateUserCommandValidator(_mockRepository.Object);
    var command = new CreateUserCommand("John Doe", "invalid-email", 25);

    var result = await validator.ValidateAsync(command, CancellationToken.None);

    Assert.False(result.IsValid);
    Assert.Single(result.Errors);
    Assert.Equal("Email", result.Errors[0].PropertyName);
}

[Fact]
public async Task ValidateAsync_DuplicateEmail_ReturnsError()
{
    _mockRepository
        .Setup(r => r.EmailExistsAsync("john@example.com", It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    var validator = new CreateUserCommandValidator(_mockRepository.Object);
    var command = new CreateUserCommand("John Doe", "john@example.com", 25);

    var result = await validator.ValidateAsync(command, CancellationToken.None);

    Assert.False(result.IsValid);
    Assert.Any(result.Errors, e => e.PropertyName == "Email" && e.ErrorMessage.Contains("already"));
}
```

## Links Úteis

- **CLAUDE.md:** Referência de API
- **ExtraRules docs:** Validações pré-built
- **NativeMediator integration:** Behavior pattern
