---
description: Merge the current branch into main safely — commit-check, remote sync, confirm, merge.
argument-hint: [optional branch name to merge (defaults to current)]
model: claude-haiku-4-5-20251001
---

# git-merge

Bring a working branch into `main`. Refuses to do anything destructive
or surprising — every step is checked, then confirmed.

## Preflight

1. **In a git repo?** `git rev-parse --is-inside-work-tree`. Refuse if not.
2. **Identify branches:**
   - `git branch --show-current` → source branch
   - Default target is `main` (or `master` if no `main` exists).
3. **Same-branch guard.** If the source branch equals the target,
   refuse with a clear message — there's nothing to merge.

## Working-tree check

1. `git status -s`.
2. **If there are uncommitted changes**, stop and ask:
   - Commit them first (suggest `/git-commit`).
   - Stash them (`git stash push -m "pre-merge stash"`) — note the stash
     ref in the report so the user can `git stash pop` after.
   - Discard (only if the user explicitly says so — never the default).
3. Do **not** proceed with a dirty tree.

## Remote sync check

1. `git remote -v`. If no remote:
   - Skip the remote sync; proceed with a local merge.
   - Mention in the report that no remote was found.
2. If a remote exists:
   - `git fetch origin --prune`
   - Compare `main` with `origin/main`:
     - `git rev-list --left-right --count main...origin/main`
       → tells us ahead/behind counts.
   - If local `main` is **behind** `origin/main`: tell the user, and
     after the working-tree is clean, fast-forward local `main` before
     merging (`git switch main && git pull --ff-only`).
   - If local `main` is **ahead** of `origin/main`: warn — that's
     unpushed work on main. Ask before continuing; offer to push first.
   - If they have **diverged**: stop. This is not the time for a
     `/git-merge` — recommend rebasing or pulling with `--rebase`
     first, and let the user decide.
3. Also fetch the source branch's remote tracking ref if it exists,
   and warn if `origin/<branch>` is ahead of the local branch (someone
   else pushed).

## Confirm before doing anything destructive

Use `AskUserQuestion` to confirm in one batch:

1. **Merge strategy:**
   - **No-ff merge commit** (Recommended for feature branches — keeps
     history visible).
   - **Fast-forward only** (`--ff-only`) — refuses if a merge commit
     would be needed; cleanest when the branch is just ahead.
   - **Squash** (`--squash`) — collapse to one commit on main; loses
     individual commit history but keeps main linear.
2. **After merge, delete the source branch?** local and/or remote.
3. **Push main to origin after?** Yes / No.

Show the user the exact commands you'll run before executing.

## Execute

In order (each step depends on the previous):

```bash
git switch <target>          # main (or master)
git pull --ff-only           # only if a remote exists and target was behind

# then, per chosen strategy:
git merge --no-ff <source>   # OR
git merge --ff-only <source> # OR
git merge --squash <source> && git commit -m "<conventional commit summary>"
```

For squash merges, use a Conventional Commit summary derived from the
source branch's log (`git log <target>..<source> --oneline`) — defer to
the `github` skill for the format. Include a body listing the commits
that were squashed.

After a successful merge:

- If "delete source branch" was yes:
  - Local: `git branch -d <source>` (use `-D` only if the user said so).
  - Remote: `git push origin --delete <source>`.
- If "push main after" was yes:
  - `git push origin <target>` (no `--force`, ever, on main).

## Rules

- Never force-push to `main` (or `master`).
- Never merge with a dirty working tree.
- Never proceed if local and remote `main` have diverged — punt to
  the user.
- Never silently switch branches; confirm in the report what changed.
- Never delete a branch the user didn't explicitly say to delete.
- Never use `git pull` without `--ff-only` here — a surprise merge
  commit on `main` is exactly what we're avoiding.

## Hand off

Report:
- Source → target.
- Pre-merge stash, if any (with the ref to pop).
- Strategy used.
- Merge commit SHA (or fast-forward summary).
- Whether the source branch was deleted (local / remote).
- Whether `main` was pushed.
- Final `git log -n 5 --oneline --graph` so the user can see the result.
