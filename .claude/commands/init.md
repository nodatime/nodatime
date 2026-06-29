---
description: Scaffold the project files I like — CLAUDE.md and the /docs skeleton.
argument-hint: [project name]
model: claude-sonnet-4-6
---

# init

Stand up the working files for a project so the later phase commands have
something to fill. You **create structure, not content** — stubs with headings
and TODOs, never invented decisions.

## CLAUDE.md Template

Use this when creating CLAUDE.md

```md
# [Fill in]

[Project summary, ask if there isn't one]

## Rules

- DO NOT REPORT SOMETHING IS FIXED IF YOU HAVEN'T COMPILED THE APP
- DO NOT SEARCH node_modules for answers. GO ONLINE.
- Use emoji for markdown documents for readability.
- Get to the point, be terse, do not over explain. Tokens are water, we're in the desert. Use emoji instead of prose if you can.
- Never install a package by editing the manifest, always use a package install tool, such as `npm install`, `bun install`, etc.

```

## Scope

- IN: create CLAUDE.md, the `docs/` skeleton, a README stub, `.gitignore`.
- OUT: deciding what the project *is* (that is `/explore`), architecture
  (`/design`), tasks (`/plan`). This command does not interview the problem.

## Preflight

0. **Interview check (do this first).** If `INTERVIEW.md` still exists at the
   project root, the onboarding interview hasn't been run — the `you/` skill is
   still template defaults, so anything scaffolded here won't reflect the user's
   stack, conventions, or voice. Stop and suggest running `/interview` before
   anything else:
   `Looks like you haven't run /interview yet — that personalizes the whole toolkit. Worth doing before /init. Run it now, or want me to scaffold with defaults anyway?`
   Only proceed past this point if the user says to go ahead regardless. If
   `INTERVIEW.md` is absent (interview done, or explicitly opted out), continue
   silently.
1. Re-entrant: if a file already exists, **leave it alone**. Report what was
   already there vs. what you created. Never clobber.
2. Detect the ground:
   - Code already present → seed CLAUDE.md from what's actually there
     (language, test command, run command, layout). Be factual, no guessing.
   - Empty dir → CLAUDE.md gets a stub with those headings and `TODO`.

## Produce

- **CLAUDE.md** (root, owned here) — the **how**: stack, conventions, test &
  run commands, the phase commands available (`/explore`, `/design`, `/plan`,
  `/document`), and the rule that each doc has one owner.
- **docs/PROJECT.md** — stub, owned by `/explore`. Headings: Problem, Who it's
  for, Goals, Scope (in/out), Open questions.
- **docs/ARCHITECTURE.md** — stub, owned by `/design`.
- **docs/SPEC.md** — stub, owned by `/design`.
- **docs/STORIES.md** — stub, owned by `/plan`.
- **docs/PLAN.md** — stub, owned by `/plan`. Empty checklist.
- **docs/MEMORY.md** — the project decision log: decisions made with AI,
  preserved for Claude Code. Header + an empty dated-entry list. Curated by
  `/document`; appended to by every phase command. Distinct from the
  `~/.claude` memory system.
- **README.md** — one-line stub, owned by `/document`.

Each stub names its owner command at the top so nobody writes the wrong file.

## Interview

Almost none. Ask at most: project name (if not in `$ARGUMENTS`), and — only if
code exists and it's ambiguous — the test and run commands. Otherwise stay
silent and scaffold.

## Hand off

End with the file list (created vs. skipped) and:
`Suggested next: /explore — to define what this project is and why.`
