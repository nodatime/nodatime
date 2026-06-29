---
description: Run the first-run onboarding interview — fills in your `you/` skill (tech, background, voice).
model: claude-opus-4-8
---

# interview

Conduct the onboarding interview that personalizes this whole toolkit. This
command **owns the interview**; the `SessionStart` hook only nudges the user
to run it.

## Read the guide first

1. If `INTERVIEW.md` exists at the project root, **read it** — it's the full
   question tree, the depth/contradiction/suggestion rules, and the
   completion steps. Follow it.
2. If it's missing (already onboarded), read the archived copy at
   `.claude/skills/you/interview-guide.md` and offer a **re-interview** —
   confirm before overwriting existing `you/*.md` answers.
3. If neither exists, fall back to the section list below and proceed.

## How to run it

- Tell them it's ~20 minutes and worth it. **Ask one question at a time.**
- Go ~3 levels deep per topic with concrete, opinionated follow-ups and
  suggestions — never an open-ended dump.
- **Call out contradictions** the moment they appear; ask them to resolve.
- **Offer skills** when an answer implies a missing one (functional-language
  skill, `lang-template/` copy for an un-skilled language, a DB / deploy /
  testing skill). Act on what they accept; prune what they reject.
- Summarize each section in 2–3 bullets and get a yes before writing.

## Sections (if no guide file is present)

1. Languages & runtimes → `you/tech.md`
2. Platform & deployment → `you/tech.md`
3. Data & persistence → `you/tech.md`
4. Testing & quality → `you/tech.md`
5. Architecture opinions & conventions → `you/tech.md`
6. Background & audience → `you/background.md`
7. Writing voice → `you/writing.md`

## On completion

1. Write directive, specific answers into `.claude/skills/you/background.md`,
   `tech.md`, `writing.md`. Replace bracketed placeholders; delete sections
   that don't apply. Strong rules beat soft preferences.
2. If `docs/` exists, seed `docs/MEMORY.md` standing decisions per
   `.claude/skills/you/memory-bootstrap.md`. If not, tell them to run `/init`
   then `/interview` again to seed it.
3. Create/prune any skills they agreed to.
4. **Clear the sentinel** so the greeting stops:
   `mv INTERVIEW.md .claude/skills/you/interview-guide.md`
   (Skip if it was already archived from a re-run.)
5. Report what changed and point them at `/init` (if needed) or `/explore`.
