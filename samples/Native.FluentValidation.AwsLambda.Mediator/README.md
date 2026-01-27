# Native.FluentValidation AWS Lambda (Mediator) Sample

Exemplo completo usando **NativeMediator** + **Native.FluentValidation** em AWS Lambda, com template de deploy (AWS SAM).

## Handler

`Native.FluentValidation.AwsLambda.Mediator::Native.FluentValidation.AwsLambda.Mediator.Function::Handler`

## Fluxo

1. O handler recebe `CreateUserRequest`.
2. O `LambdaMediatorFunction` resolve o `IRequestHandler` via DI.
3. Se houver validator registrado, a validação ocorre antes do handler.

## Deploy (AWS SAM)

O template está em `template.yaml`.

```bash
sam build
sam deploy --guided
```

## Estrutura

- `Function.cs` - Entry point da Lambda
- `Models.cs` - Request/Response
- `Validators.cs` - Regras de validação
- `Handlers.cs` - Handler NativeMediator
- `template.yaml` - Infra SAM
- `deploy.ps1` - Script opcional para deploy
