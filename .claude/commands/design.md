---
description: Interview the solution — architecture, schema, platform. Owns ARCHITECTURE.md + SPEC.md.
argument-hint: [area to focus, optional]
model: claude-opus-4-7
---

# design

> 🎯 **Design for change.** Every architectural decision here is judged by
> one question: when this changes, how big is the diff? Pick the boundaries,
> seams, and data shapes that make the *next* change small and local.

Decide *how* to build what `/explore` defined. This is a **solution-space**
interview. Output: an architecture, a data model, and a behavioral spec.

## Scope

- IN: components & boundaries, data model, database (schema **and** platform),
  runtime, sync vs async, build-vs-buy, key tradeoffs, behavioral requirements.
- OUT: the problem/why (that's `/explore` — read it, don't redo it), task
  breakdown (`/plan`), prose docs/README (`/document`).

## Preflight

1. Read `docs/PROJECT.md` (the brief — design must serve it), plus
   `docs/ARCHITECTURE.md`, `docs/SPEC.md`, `CLAUDE.md`, `docs/MEMORY.md`.
   Summarize current state in 1–2 lines. Re-entrant: refine, don't restart.
2. If PROJECT.md still has open scope questions, surface them — designing on
   an undefined problem is wasted work; offer to bounce back to `/explore`.

## Skills

Pull in the relevant skill rather than improvising:
- DB schema → `postgres-dba` (or `sqlite-dev` if local/Bun + SQLite).
- Module/class boundaries → `solid-principles`, `design-principles`.
- Pattern choice → `gof-patterns`. TS conventions → `typescript-best-practices`.

## Interview

Up to ~10 questions, adaptive, batched (4 at a time). Bank:

1. Data model — core entities and relationships?
2. Database platform, and why (durability, scale, ops)?
3. Runtime / language / framework — and any locked constraints?
4. Major components and their boundaries?
5. Sync or async? Where are the seams?
6. External services / APIs — build or buy?
7. Auth & trust boundaries, if any?
8. Hardest technical risk, and the fallback?
9. What are we explicitly **not** building for now (YAGNI)?
10. Non-functionals that bite: scale, latency, offline, cost ceiling?

## Produce

- **docs/ARCHITECTURE.md** (owned): components, data flow, the data model,
  database platform + schema, key decisions **with their rationale and the
  alternative rejected**.
- **docs/SPEC.md** (owned): behavioral requirements in prose — what the system
  must do, observably. This is what `/plan` slices into stories; keep it
  testable, not vague.
- **docs/PROJECT.md**: update the decisions log only (don't rewrite it).
- **docs/MEMORY.md**: append dated entries for each architectural decision —
  the choice, the why, the rejected alternative.

## Hand off

Note any `TODO` in ARCHITECTURE.md / SPEC.md, then:
`Suggested next: /plan — or /document to refresh README/ARCHITECTURE prose.`
