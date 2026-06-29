---
description: Create the GitHub remote for this project and make it look sharp — name, license, real README, contributing, security, issue templates.
argument-hint: [optional repo name]
model: claude-sonnet-4-6
---

# git-remote

Stand up the GitHub remote for this project and treat it like a calling
card. A scrappy first-push leaves a scrappy first impression. Don't ship
a stub. Use the **`github`** skill for any commit / PR work along the way.

## Preflight

1. **Refuse if a remote already exists.** `git remote -v` — if `origin`
   is set and points at github.com, stop and tell the user. Suggest
   `gh repo view` or `gh repo edit`.
2. **`gh auth status`** — refuse if not logged in. Tell the user to run
   `gh auth login`.
3. **Read context:** `CLAUDE.md`, `docs/PROJECT.md`, existing
   `README.md`, any `LICENSE`, `CONTRIBUTING.md`, `SECURITY.md`,
   `.github/` — never overwrite without consent.

## Interview (ask in one batch)

Use `AskUserQuestion` to settle the choices that need a human:

1. **Repo name** — propose one derived from the project (CLAUDE.md /
   PROJECT.md), kebab-case, short. Offer 2–3 options.
2. **Visibility** — public, private. Default: public (it's a calling card).
3. **License** — propose one with a one-line rationale each:
   - **MIT** — most permissive, easiest to adopt (Recommended for libs/tools).
   - **Apache-2.0** — same permissions + explicit patent grant; good for
     anything corporates will touch.
   - **AGPL-3.0** — strong copyleft; use when you want network-use
     modifications shared back.
   - **MPL-2.0** — file-level copyleft; middle ground.
   - **Unlicense / CC0** — public domain dedication.
   Recommend based on project type (CLI/lib → MIT; service/SaaS-y →
   Apache-2.0; opinionated stack you want kept open → AGPL).
4. **Owner** — personal account vs an org (only ask if `gh auth status`
   shows the user belongs to orgs).

## Calling-card requirements (the bar)

The repo gets pushed **only after all of these are in place**:

### README.md — must be real, never boilerplate

A boilerplate README is **anything** matching:

- Just the project name and a one-liner.
- Contains `TODO`, `Lorem ipsum`, "your project here", or the literal
  scaffold text (e.g. the `/init` stub `TODO — one-line description.`).
- No installation, no usage, no example.
- Auto-generated `create-foo-app` content untouched.

If the current README is boilerplate, **refuse to push**. Offer to
write a real one with this shape:

```markdown
# <Project name>

> <One-line elevator pitch — what it does and who it's for.>

[badges row — build, license, version, etc. — only ones that are real]

## Why

<2–4 sentences. The problem, why it matters, what makes this different.
Not features — motivation.>

## Quick start

```bash
# install
<install command>

# run
<run command>
```

## Usage

<Smallest possible useful example. Real code, real output.>

## How it works

<2–4 sentences or a small diagram. The architecture in plain English.
Link to docs/ARCHITECTURE.md for depth.>

## Status

<Pre-release / beta / stable. Honest about what works and what doesn't.>

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## Security

See [SECURITY.md](SECURITY.md).

## License

<SPDX id> — see [LICENSE](LICENSE).
```

Read from `docs/PROJECT.md` for the "Why" — never invent it. If
PROJECT.md is empty, send the user back to `/explore` first.

### CONTRIBUTING.md — short, honest, opinionated

Minimum sections:
- How to get the project running locally (one command if possible).
- How to run tests.
- Coding style / what we care about (point at `CLAUDE.md`).
- How to propose a change (issue first vs PR first).
- Conventional Commits (link the `github` skill rules — or restate them
  briefly: `<type>(<scope>): <imperative>`).

Don't pad. 60–150 lines is plenty.

### SECURITY.md — supported versions + how to report

- A `Supported versions` table (latest minor at minimum).
- A `Reporting a vulnerability` section with an email or
  [GitHub Security Advisories](https://github.com/<owner>/<repo>/security/advisories/new)
  link. Promise a response window (e.g. "within 5 business days").
- A "please don't open public issues for security bugs" line.

### LICENSE — real SPDX text

Use `gh repo create … --license <id>` so GitHub drops in the canonical
text, or write it from the chosen SPDX id (MIT/Apache-2.0/AGPL-3.0/etc.).
Don't fudge the year or holder.

### .github/ISSUE_TEMPLATE/ — keep it to two

Don't overload contributors. Default set:

- `bug_report.md` — what happened, what you expected, repro steps,
  environment.
- `feature_request.md` — problem you're trying to solve, proposed
  solution, alternatives considered.

Plus `config.yml` with `blank_issues_enabled: false` and a contact link
to discussions or email if relevant.

### Optional but encouraged

- `.github/pull_request_template.md` mirroring the `/git-pr` body
  template (Summary / Why / How to verify / Risk).
- A `CODE_OF_CONDUCT.md` if the project expects outside contributors —
  the Contributor Covenant is the standard pick. Skip if it would just
  be noise on a solo project.

## Behavior

1. Run the interview. Get name, visibility, license, owner.
2. Audit existing files (README, LICENSE, CONTRIBUTING, SECURITY,
   `.github/`). For each one missing or boilerplate, **stop and
   draft it** with the user — never push first and fix later.
3. **README boilerplate gate.** Re-read README before any push. If it
   still trips the boilerplate detector, refuse and offer to rewrite.
4. Create the remote:
   ```bash
   gh repo create <owner>/<name> \
     --<visibility> \
     --license <spdx-id> \
     --source . \
     --remote origin \
     --description "<one-line from README>" \
     --push
   ```
   Use `--push` only after the calling-card gate passes.
5. After push: verify topics make sense (`gh repo edit --add-topic …`),
   confirm the description on github.com matches the README hook, and
   print the URL.

## Rules

- **Never push a boilerplate README.** Refuse, draft, then push.
- Don't enable issues/wiki/projects features the user didn't ask for.
- Don't add badges that aren't real (no fake "build: passing" before CI
  exists).
- Don't fabricate a description — pull it from the README's one-liner.
- Never set `origin` to a different remote without explicit consent.
- Never make a repo public if the interview said private — and vice versa.

## Hand off

Report:
- Remote URL.
- License + visibility + owner.
- Files created (README, CONTRIBUTING, SECURITY, LICENSE,
  `.github/ISSUE_TEMPLATE/*`, optional extras).
- Anything skipped and why.
- Suggested next: `/git-pr` once the first feature branch is ready,
  or `/git-issue` to file the first piece of work.
