---
name: csharp-best-practices
description: C#/.NET conventions for this user's projects — multi-targeting for libraries, NUnit/xUnit testing, warnings-as-errors, immutability, exception-based error handling, and dependency injection used selectively. Use when writing or reviewing C# code, .csproj/Directory.Packages.props files, or test suites in a .NET project.
---

# C# Best Practices

## Why: Design for Change

The goal is code that looks deceptively simple — simple enough that a
reader asks how it took so long to write — but is easy to change, test,
and use. Anything unusual gets a comment explaining *why*, not what.

## Compiler / runtime settings

- SDK pinned via `global.json`.
- Dependency versions centralized via `Directory.Packages.props` (Central
  Package Management) rather than per-project version strings.
- Compiler warnings are strict — treat as effectively warnings-as-errors.
  Analyzer (code-style) warnings are looser; use judgement rather than
  blocking on every one.
- **Libraries** (anything with broad external consumers, like Noda Time):
  multi-target the same broad TFM spread the project already targets —
  don't drop old TFMs casually.
- **Internal tools**: fine to target the latest LTS only — no need to
  multi-target.

## Type system & modeling

Lean toward immutable types for new code. OO by default; reach for
functional approaches (pure functions, expression-bodied members, LINQ
pipelines) where they make the code clearer, not as a rule unto itself.

## Error handling

Exceptions, in almost all cases — not Result/Either types, not error
tuples. Don't introduce a Result type pattern unless explicitly asked.

## Dependency injection

Use DI for web projects. Skip it for smaller tools and libraries — don't
wire up a DI container where it isn't earning its keep.

## External API clients

When consuming an external API, prefer an existing dedicated client
library over hand-rolling HTTP calls, unless none exists or it's
unsuitable.

## Testing conventions

- Tests are written alongside production code, in the same commit — not
  strictly tests-first, not tests-after.
- Runner: NUnit or xUnit depending on the project. **Never MSTest.**
- Don't be dogmatic about unit testing specifically — higher-level
  (integration/smoke) tests often carry more value than exhaustive unit
  coverage. Match the project's existing test shape rather than imposing
  one.
- Coverage targets are project-specific: Noda Time targets ~99%; most
  other projects carry no coverage target. Don't impose a coverage bar
  that isn't already there.

## Patterns to use with judgement, not by default

- **Singleton:** commonly overused by teams, but occasionally the right
  tool. Don't reject it on sight, and don't reach for it by default
  either.

## Standard rules for every type / class / module

No fixed project-wide template (e.g. no blanket "every class must
override ToString()" rule) — follow the conventions already established
in the specific project being worked on.
