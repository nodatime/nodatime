---
description: Reconcile docs with reality — README, ARCHITECTURE prose, MEMORY log. Run anytime.
argument-hint: [area to focus, optional]
model: claude-sonnet-4-6
---

# document

Bring the human-facing docs in line with what actually exists. **Not a phase**
— a reconciler you run whenever code and docs have drifted. It detects drift;
it does not re-interview the project.

## Scope

- IN: README, ARCHITECTURE.md prose accuracy, curating docs/MEMORY.md.
- OUT: making product/architecture **decisions** (that's `/explore`,
  `/design`). If reconciling reveals an undecided question, log it and point
  at the owning command — don't decide it here.

## Preflight

1. Read `CLAUDE.md`, `docs/PROJECT.md`, `docs/ARCHITECTURE.md`,
   `docs/SPEC.md`, `docs/MEMORY.md`, `README.md`.
2. Read the actual code/structure. Diff **docs vs. reality**, not docs vs.
   docs. Build a short drift list (claimed but absent, present but
   undocumented, contradictions).

## Interview

Minimal and targeted. Only ask when reconciliation is genuinely ambiguous
(e.g. "ARCHITECTURE says Postgres, code uses SQLite — which is intended?").
No standing question bank. If nothing is ambiguous, ask nothing.

## Produce

- **README.md** (owned): accurate for a human arriving cold — what it is, how
  to run it, how to test it, where the docs are. Concise.
- **docs/ARCHITECTURE.md**: correct stale prose to match reality. Don't
  redesign — if reality diverged from a deliberate decision, flag it for
  `/design` rather than rewriting the decision.
- **docs/MEMORY.md** (curated here): dedupe, order by date, keep it a tight
  log of decisions-made-with-AI and why. This is the project's memory for
  Claude Code — terse, durable, not a changelog of everything.

## Hand off

Report the drift found and what you reconciled vs. flagged. Suggested next is
context-dependent — e.g. `/design` if a contradiction needs a real decision,
otherwise nothing.
