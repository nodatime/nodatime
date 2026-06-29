---
description: Stage and commit current changes using Conventional Commits, branch-or-trunk by size. Uses the `github` skill.
argument-hint: [optional summary or scope hint]
model: claude-haiku-4-5-20251001
---

# commit

Create a clean, well-described commit for the current working changes.
Defers to the **`github`** skill for branching and message format.

## Behavior

1. **Inspect first.** Run in parallel:
   - `git status` (no `-uall`)
   - `git diff` (staged + unstaged)
   - `git log -n 10 --oneline` to match the project's commit style
   - `ls .gitignore` to confirm a `.gitignore` exists
2. **Require a `.gitignore`.** If one is missing, **stop and offer to
   create one** scaffolded to the project (Bun/Node/TS by default —
   `node_modules/`, `dist/`, `.env*`, `.DS_Store`, IDE folders, build
   artifacts). Do not proceed with the commit until the user accepts or
   provides their own. If `.gitignore` exists, glance at it and warn if
   anything obviously sensitive in the diff isn't covered.
3. **Check change size first.** Roughly: count files changed and lines
   touched. If it looks like **a lot** (rule of thumb: >10 files OR
   >300 lines OR touches >2 distinct concerns), **stop and ask** whether
   a short-lived branch would be better. Recommend a name in
   `<type>/<slug>` form derived from the diff (e.g.
   `feat/order-fulfillment-tx`). Wait for the user before continuing.
4. **Decide branch vs trunk** per `github` skill:
   - Trunk → commit straight to `main` for tiny, obvious changes.
   - Branch → create `<type>/<short-slug>` if the change is non-trivial,
     spans multiple files meaningfully, or implements a story task.
5. **Stage everything with `-am`.** Use `git commit -am "<summary>"`
   (with `-F` for the long body via a HEREDOC file when needed) so
   tracked modifications and deletions are all included — nothing gets
   left behind. For brand-new untracked files, `git add <file>` them
   explicitly first (since `-a` won't pick those up).
   **Always re-scan for likely secrets** (`.env*`, `*.pem`,
   `*credentials*`, API tokens in the diff) before staging — refuse
   the commit if any are found, name the file, and ask the user.
6. **Write the message** per the `github` skill:
   - Conventional Commit summary (`<type>(<scope>): <imperative>`).
   - Detailed body: what changed, why, trade-offs. Wrap ~72 cols.
   - Footers: `Refs <story-id>` / `Closes #<n>` where applicable.
7. **Commit via HEREDOC** so formatting is preserved. Append:
   ```
   Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>
   ```
8. **Verify** with `git status` after the commit. If a hook fails, fix
   the underlying issue and create a NEW commit — never `--amend` without
   being asked.

## Rules

- Always require a `.gitignore`. Offer to create one if missing; do not
  commit without it.
- Always use `-am` (plus explicit `git add` for new files) so nothing
  tracked is left out of the commit.
- If the change is large (see step 3 thresholds), stop and recommend a
  branch name before committing.
- Never push as part of `/git-commit` unless the user asks.
- Never `--no-verify` or `--no-gpg-sign`.
- Never force-push.
- If the working tree is clean, say so and stop — don't create empty commits.
- If the diff contains a likely secret, refuse and tell the user what
  was found.

## Hand off

Report: branch (created or trunk), files staged, summary line, and the
new commit SHA.
