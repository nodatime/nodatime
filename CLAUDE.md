# Noda Time

Noda Time is an alternative date and time API for .NET. It helps you think
about your data more clearly, and express operations on that data more
precisely.

## Rules

- DO NOT REPORT SOMETHING IS FIXED IF YOU HAVEN'T COMPILED THE APP
- DO NOT SEARCH node_modules for answers. GO ONLINE.
- Use emoji for markdown documents for readability.
- Get to the point, be terse, do not over explain. Tokens are water, we're in the desert. Use emoji instead of prose if you can.
- Never install a package by editing the manifest, always use a package install tool, such as `dotnet add package`.

## 🔒 Hard constraints (any change must hold these)

- Public API back-compat — no breaking changes without major version bump
- Multi-target all TFMs already in the `.csproj` files; don't drop one
- AOT/trimming clean — `NodaTime.AotCompatibilityTestApp` must keep passing
- Deterministic TZDB builds — NZD output stable for given inputs

## 🚫 Off-limits for AI without explicit invitation

- Public API design decisions (drafts ok, ship calls are human-only)
- Calendar engine internals
- Serialization formats (binary NZD etc. are stable contracts)

## Stack

- C# / .NET, SDK version pinned in [global.json](global.json)
- Solution: [src/NodaTime.slnx](src/NodaTime.slnx)
- Test framework: NUnit (`NodaTime.Test`, `NodaTime.TzdbCompiler.Test`)

## Commands

- Build: `dotnet build src/NodaTime.sln`
- Test: `dotnet test src/NodaTime.Test`

## Layout

- `src/NodaTime` — the core library
- `src/NodaTime.Testing` — testing helpers (fakes, etc.)
- `src/NodaTime.Test` — unit tests for the core library
- `src/NodaTime.Demo` — usage examples
- `src/NodaTime.TzdbCompiler` (+ `.Test`) — compiles IANA tz data into Noda Time's binary format
- `src/NodaTime.Tools.*` — supporting CLI tools (NZD validation, dumping, etc.)
- `src/NodaTime.TzValidate.*` — time zone data validation/compatibility tools
- `src/NodaTime.Benchmarks*` — BenchmarkDotNet-based performance tests
- `src/NodaTime.AotCompatibilityTestApp` — AOT compatibility smoke test
- `data` — time zone (TZDB) and related raw data
- `build`, `packaging` — CI/build and packaging scripts

## Phase commands

- `/explore` — define what a piece of work is and why (`docs/PROJECT.md`)
- `/design` — architecture and spec (`docs/ARCHITECTURE.md`, `docs/SPEC.md`)
- `/plan` — stories and task DAG (`docs/STORIES.md`, `docs/PLAN.md`)
- `/document` — reconcile docs with reality (`README.md`, `docs/MEMORY.md`)

Each doc has one owner command — don't write into a doc owned by another phase.
