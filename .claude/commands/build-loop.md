---
description: Drive a PLAN.md to completion task-by-task — builder builds, reviewer gates, commit only on pass.
argument-hint: [path-to-PLAN.md]
model: claude-sonnet-4-6
---

# build-loop

> 🎯 **Design for change.** Each task's diff should be small, local, and
> behind a stable seam. The reviewer fails work that couples modules or
> bloats blast radius — builder should pre-empt that.

Execute the build plan in `$ARGUMENTS` (default: `docs/PLAN.md`) one task at a
time. You are the orchestrator (the lead thread). You do **not** write feature
code or review it yourself — you dispatch and gate.

## Scope

- IN: build each unchecked task, get it through review, commit it.
- OUT: planning, writing PLAN.md, authoring specs, refactor/refinement passes.
  This command **consumes** a plan; it does not author one.

## Preflight

1. Resolve the plan: use the path in `$ARGUMENTS`; else `docs/PLAN.md`; else
   **stop and ask** — do not infer a plan.
2. Read it. It must be a checklist of tasks (`- [ ]` / `- [x]`). If it has no
   checkboxes, stop and report — it is not a build plan.
3. Confirm a clean git working tree. If dirty, stop and report; do not build on
   top of uncommitted changes.

## The loop

For each task still unchecked (`- [ ]`), in order, top to bottom:

1. **Build.** Dispatch a `builder` subagent (Agent tool,
   `subagent_type: builder`, model **sonnet**). Give it: the exact task text,
   the relevant section of PLAN.md, the files it owns, and an instruction to
   invoke the project's TypeScript + DB skills and run the existing specs
   (`bun test` / project test command) before reporting back. The builder
   **does not commit**.

2. **Review.** Dispatch a `reviewer` subagent (Agent tool,
   `subagent_type: reviewer`, model **opus** — reviewer must be ≥ builder).
   It invokes `security-web` and `simplify`, reads the builder's diff, and
   returns a verdict: `PASS` or `FAIL` with specific findings.

3. **Recurse.** If `FAIL`: send the findings back to a fresh `builder` for the
   same task. Repeat build → review until `PASS`. No cap — a task is not done
   until it passes code **and** security review. Nothing reaches git history
   before `PASS`.

4. **Smoke the entry point.** Before commit, build/start the production
   artifact and drive **one real request through the deployed entry point**
   (one `curl` against `wrangler dev`, one `bun build` + invoke, one route
   fetch — whatever applies to the deploy target). Fake adapters (DB, email,
   storage) are fine, but the request must travel through the exported
   handler, not an internal function. Assert the spec's side effect actually
   occurred. If the smoke fails, recurse to step 3 with a finding — unit-test
   green is not enough to ship.

5. **Commit.** On `PASS` + smoke green, have the builder commit *only this
   task's changes*
   with a message naming the task. One commit per task.

6. **Check the box.** Edit PLAN.md: `- [ ]` → `- [x]` for the completed task.
   Commit that PLAN.md change with the task commit or immediately after.

7. Next task.

## Finish

When every box is checked: run the full test suite once more, then report a
summary (tasks completed, commits, anything still red). Architect / final
design check is a separate step — not part of this loop.

## Rules

- Sequential, dependency-ordered. This is a pipeline, not a parallel team — use
  the Agent tool (subagents), not Agent Teams.
- Builder owns code; reviewer owns the gate; you own sequencing and the
  checkbox state. Never collapse these roles.
- If a gate can't pass after repeated attempts and the builder is stuck, stop
  and report the task + findings rather than committing degraded code.
