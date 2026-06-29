---
description: Open a pull request with a clear summary, verification plan, and risk note via `gh pr create`. Uses the `github` skill.
argument-hint: [optional base branch or PR title hint]
model: claude-haiku-4-5-20251001
---

# pr

Open a GitHub pull request for the current branch using the **`github`**
skill's PR template.

## Behavior

1. **Preflight.**
   - `gh auth status` to confirm CLI is logged in.
   - Refuse if the current branch is `main` — make a branch first
     (`git switch -c <type>/<slug>`).
   - Refuse if the working tree is dirty — commit or stash first
     (suggest `/git-commit`).
2. **Gather context** in parallel:
   - `git status`
   - `git log <base>..HEAD --oneline` (default base = the repo's default
     branch; `main` unless `--base` arg says otherwise)
   - `git diff <base>...HEAD` to understand the full set of changes
3. **Push the branch** if not already tracking a remote:
   `git push -u origin HEAD`.
4. **Draft per the `github` skill PR template:**
   - **Title** — Conventional Commit style, ≤70 chars.
   - **Summary** — 2–4 bullets covering all the commits, not just the
     latest. The headline change first.
   - **Why** — one paragraph; link the story id / issue.
   - **How to verify** — concrete checklist (commands, URLs, manual
     steps). Reviewer should be able to run these in order.
   - **Risk / rollback** — one or two lines; be honest if it's low risk.
   - `Closes #N` footer if it resolves an issue.
5. **Show the draft** to the user before creating. Confirm `--draft` if
   work is still in progress.
6. **Create** with a HEREDOC body:
   ```bash
   gh pr create --title "<title>" --body "$(cat <<'EOF'
   ...
   EOF
   )"
   ```
   Use `--base <branch>` only when not targeting the default.

## Rules

- Title must reflect *all* the commits on the branch, not just HEAD.
- Don't push to `main`; don't force-push to a branch with an open PR
  without saying so.
- Don't include local-only paths, secrets, or `.env` references in the body.

## Hand off

Return the PR URL `gh` prints.
