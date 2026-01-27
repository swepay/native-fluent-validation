# Native.FluentValidation AWS Lambda Sample

Exemplo simples de uso do Native.FluentValidation em um handler de AWS Lambda, sem reflection e com validação explícita.

## Handler

`Native.FluentValidation.AwsLambda::Native.FluentValidation.AwsLambda.Function::Handler`

## Fluxo

1. O handler recebe `CreateUserRequest`.
2. O validator roda de forma síncrona e AOT-safe.
3. Retorna `CreateUserResponse` com erros caso inválido.
