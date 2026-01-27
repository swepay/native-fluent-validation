# Native.FluentValidation Roadmap

## ✅ Concluído

- **Fase 1 — Core Absoluto (MVP)**
  - `ValidationResult` e `ValidationFailure`
  - `Rule`, `RuleBuilder` e regras básicas
  - API fluente AOT-safe com `RuleFor(..., propertyName)`
  - Testes unitários (regras, combinações e pipeline)

- **Fase 2 — API Fluente Avançada**
  - `WithMessage` e `WithErrorCode`
  - `CascadeMode` (Stop / Continue)
  - `When` / `Unless` (predicados explícitos)

- **Benchmarks (Native AOT)**
  - Harness simples com `Stopwatch` para medir throughput

- **Exemplo AWS Lambda**
  - Handler simples com validação explícita e sem reflection

- **Fase 3 — Integrações**
  - NativeMediator (builder + múltiplos validators)
  - NativeLambdaMediator (base para Lambda + DI)
  - Exemplo AWS Lambda completo (deploy tooling)

## 🚧 Próximos passos

- **Fase 4 — Performance & AOT**
  - Publicação Native AOT
  - Zero warnings de trimming
  - Benchmarks comparativos

- **Fase 5 — Source Generator (Opcional)**
  - Gerar `propertyName`
  - Regras estáticas sem boilerplate

- **Fase 6 — Ecossistema**
  - Pacotes de regras extras
  - Validação assíncrona (opt-in)
  - Comparativos com FluentValidation

- **Infraestrutura**
  - Split em repositórios separados para cada pacote
