---
description: Make a small, obvious fix directly on the current branch. No sprint, no PLAN, no stories.
argument-hint: <describe the fix>
model: claude-sonnet-4-6
---

# quick-fix

For the trivial stuff: a typo, a reversed condition, a dead `try/catch`, a
misnamed variable, a stray import. The kind of bug where opening
`/explore` → `/design` → `/plan` → `/build-loop` would be absurd.

## Scope

- IN: one-spot fixes that a competent engineer would commit straight to
  `main` without ceremony. Single concern. Usually < ~20 lines changed.
- OUT: anything that needs a design call, touches a public API contract,
  or changes behavior across modules. If it smells like that, stop and
  suggest `/plan` or `/build-loop`.
- Note: needing a new unit test does **not** push a fix out of scope — a
  quick-fix always gets a test (see Loop step 2). Only escalate if the
  *fix itself* is non-trivial.

## Argument

`$ARGUMENTS` describes the fix in the user's words (e.g. "remove the
unneeded try/catch in fetchUser", "fix the typo in the login button").
If empty, ask one short clarifying question.

## Loop

1. **Reproduce first.** Before changing anything, pin down the bug:
   - If the user hasn't given repro steps (input, command, failing test),
     ask for them. Don't guess at a bug you can't observe.
   - Try to reproduce it yourself — run the relevant code path or existing
     tests — and show the actual failure/output. For a typo or obviously
     static issue (wrong string, wrong constant) a visual confirmation in
     the source is enough; say so explicitly instead of skipping the step
     silently.
2. **Write a failing test first.** Be sure to use the
   nodatime-test-conventions skill. Add a test that
   captures the bug and currently fails for the right reason. This applies
   even to "trivial" fixes — the test is what proves the bug existed and
   stays fixed. Skip only if an existing test already reproduces it
   (point to it instead of duplicating).
3. **Locate.** Find the exact fix site. If the description is ambiguous or
   matches multiple places, ask before guessing.
4. **Confirm it's actually small.** If reading the surrounding code reveals
   the fix is non-trivial (touches a class boundary, changes a contract,
   needs new branches of test coverage beyond the one repro test),
   **stop and escalate** — tell the user this isn't a quick-fix and
   suggest `/plan`.
5. **Consult skills selectively.** Only load a code-quality skill if the
   fix genuinely touches its territory:
   - `solid-principles` / `design-principles` / `gof-patterns`: only if the
     fix sits at a class or module boundary, or changes a control-flow
     pattern. Skip for typos, comments, log strings, dead code removal.
   - `agent-teams`: skip — quick-fixes are single-threaded by definition.
   - `csharp-best-practices`: consult if the fix involves nullability,
     exceptions, multi-targeting, or public surface area.
6. **Apply the edit.** Use `Edit`, not `Write`. Match surrounding style.
7. **Verify.** Run the new test and confirm it now passes (it must have
   failed in step 2 and pass now — if it passed before the fix, it isn't
   testing the bug). Then run the rest of the fast suite if feasible to
   confirm nothing else broke.
8. **Report and ask before committing.** Show the diff summary (fix +
   test) and propose a one-line commit message. Do **not** commit unless
   the user says go — committing straight to `main` is a shared-state
   action.

## Commit message style

Imperative, lowercase, no body unless genuinely needed:

- `fix: remove unreachable try/catch in fetchUser`
- `fix: correct typo in login button label`
- `fix: invert empty-state condition in Sidebar`

## Hand off

One sentence: what changed, what you verified, awaiting commit confirmation
(or confirming the commit if the user pre-authorized).
