---
description: Slice SPEC into stories and an ordered task DAG. Owns STORIES.md + PLAN.md.
argument-hint: [scope to plan, optional]
model: claude-opus-4-7
---

# plan

> 🎯 **Design for change.** Slice tasks so each one touches one seam.
> A task that edits five unrelated files is a coupling smell — re-slice
> before you ship it to `/build-loop`.

Turn the design into something the build process can execute. This command
**transforms** SPEC.md — it does not re-elicit requirements.

## Scope

- IN: derive user stories from SPEC, order tasks into a dependency-aware
  checklist, mark what can run concurrently, **then generate executable specs
  from the finalized stories via the `bdd-specs` skill — always, no asking**.
- OUT: gathering requirements (`/explore`/`/design`), building (`/build-loop`).
  This authors a plan and its specs; it does not consume them.

## Preflight

1. Read `docs/SPEC.md` and `docs/PROJECT.md`. If SPEC.md is a stub or full of
   `TODO`, **stop** — there's nothing to plan. Send the user back to `/design`.
2. Read existing `docs/STORIES.md` / `docs/PLAN.md`. Re-entrant: reconcile and
   extend; never silently drop or renumber completed (`- [x]`) tasks.

## Stories

Delegate to the **`user-stories`** skill to produce/refine `docs/STORIES.md`
(it already formats Story→Feature with Given/When/Then so `bdd-specs` can
consume it). Don't hand-roll the format.

## Specs (always, no asking)

Once `docs/STORIES.md` is finalized and `docs/PLAN.md` is written, **always**
delegate to the **`bdd-specs`** skill to generate executable spec files from
the stories. Do not ask the user whether to run it — it is part of `/plan`'s
contract. The generated specs should fail until `/build-loop` turns them green.

## Interview

Light — this is confirmation, not elicitation. ~3–5 questions, batched:

1. Here's how I sliced SPEC into stories — anything mis-cut or missing?
2. Priority/sequence right? What's the first shippable slice?
3. Any task I marked parallel that actually shares state?
4. Hard external dependencies that gate ordering?

## Produce

- **docs/STORIES.md** (owned, via `user-stories` skill).
- **docs/PLAN.md** (owned): a checklist `build-loop` can execute —
  - every task is `- [ ]`, top-to-bottom in dependency order;
  - each task line carries: a short id, the story it implements
    (`story:`), explicit `depends-on:` ids, and a `parallel-group:` tag for
    tasks with no ordering between them;
  - one task = one reviewable, committable unit of work;
  - no task references a file or decision not in ARCHITECTURE.md/SPEC.md.
- **Wire tasks are first-class.** For every feature that touches a deployed
  entry point (exported handler, route file, `default.fetch`, Next route),
  include an explicit `wire: <feature> into <entry>` task whose acceptance is
  a real request through the production binary producing the spec's side
  effect. A task whose only acceptance criterion is "unit tests pass" is not
  allowed for code on a production path — that's how stubs ship.
- **Executable specs** (via `bdd-specs` skill): generated automatically
  from finalized STORIES.md + PLAN.md. Not optional.
- **docs/MEMORY.md**: append dated entries for sequencing decisions that
  weren't obvious (why X blocks Y, why a slice was deferred).

## Hand off

Summarize: N stories, M tasks, S spec files generated, the first parallel
group, then:
`Suggested next: /build-loop docs/PLAN.md — specs are in place and red.`
