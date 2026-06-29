---
description: Interview the idea — problem, who, why, scope. Owns docs/PROJECT.md.
argument-hint: [one-line idea]
model: claude-opus-4-7
---

# explore

Think through an idea with me. This is a **problem-space** interview, not a
solution. By the end, `docs/PROJECT.md` says what we're building and why,
honestly including what we don't know yet.

IMPORTANT: ask if there's an existing spec written anywhere. If so, ask for it and improve on it, otherwise proceed as if nothing existed.

## Scope

- IN: the problem, the user, the why-now, in/out scope, success, unknowns,
  light research where a fact is needed to proceed.
- OUT: architecture, schema, platform, tech choices — all `/design`. If a
  solution idea comes up, park it under "Open questions", don't decide it.

## Preflight

1. Read `docs/PROJECT.md` if it exists. Summarize current state in 1–2 lines
   ("scope defined, no success metric yet"). Re-entrant: refine, don't restart.
2. Read `CLAUDE.md` and `docs/MEMORY.md` for prior context.

## Interview

Up to ~10 questions, **adaptive** — only ask what's missing or unresolved.
Batch them (AskUserQuestion, 4 at a time → 2–3 rounds, not an interrogation).
Question bank, in priority order:

1. What problem, concretely? What happens today without it?
2. Who is it for? Who is *not*?
3. Why now / why worth doing?
4. What's explicitly **out** of scope?
5. What does success look like — observable, ideally measurable?
6. Hard constraints (time, money, platform, must/can't use)?
7. What's the riskiest unknown?
8. What's the smallest version that's still worth shipping?
9. Any prior art or existing thing this replaces?
10. What would make you kill this?

Do targeted research only when an answer hinges on a fact you can check.

## Produce

- **docs/PROJECT.md** (owned here): Problem, Who it's for, Goals, Success,
  Scope (in/out), Constraints, Open questions, Riskiest unknowns. Mark
  unresolved items `TODO` rather than guessing.
- **CLAUDE.md**: fill the basics that surfaced (domain, one-line purpose).
  Update only — `/init` owns the file.
- **docs/MEMORY.md**: append a dated entry for each decision actually made
  here (scope cut, killed alternative, etc.). One line each, with the why.

## Hand off

State what's still `TODO` in PROJECT.md, then:
`Suggested next: /design — or re-run /explore to close open questions first.`
