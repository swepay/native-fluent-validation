# Native.FluentValidation Benchmarks

Harness simples para medir throughput da validação sem reflection e com AOT-friendly code paths.
Inclui comparação com FluentValidation para referência.

## Como executar

- `dotnet run -c Release`

## Native AOT (opcional)

- `dotnet publish -c Release -r win-x64 /p:PublishAot=true`
