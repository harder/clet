# CI/CD Workflows

## Branching model

| Branch | Purpose | Default? |
|--------|---------|----------|
| `develop` | Day-to-day work. PRs target here. | Yes |
| `main` | Releases only. Merge `develop` → `main` to ship. | No |

## Versioning (D-023)

Version is controlled by `<Version>` in `src/Clet/Clet.csproj`. The release workflow auto-increments a build number on each run.

| Phase | csproj `<Version>` | main produces |
|-------|--------------------|---------------|
| RC | `1.0.0-rc` | `v1.0.0-rc.1`, `.2`, `.3` ... |
| Stable | `1.0.0` | `v1.0.0`, `v1.0.1`, `v1.0.2` ... |

To move between phases, change `<Version>` in the csproj and merge to main.

Build numbers auto-increment by finding the latest matching git tag (`v1.0.0-rc.*`, etc.).

## Workflows

### `ci.yml` — Continuous Integration

Runs on every push and every PR targeting `develop` or `main`.

- Restore, build, unit tests, integration tests, smoke tests
- No publishing, no tagging

### `release.yml` — Build, Test, Tag, Publish

**Triggers:**

| Trigger | When | Channel |
|---------|------|---------|
| Push to `main` (changes in `src/` or `tests/`) | Merge develop → main | main |
| `repository_dispatch` from Terminal.Gui | TG main-branch publish (`tg-main-published`) | main |
| `repository_dispatch` from Terminal.Gui.Editor | Editor main-branch publish (`editor-main-published`) | main |
| `workflow_dispatch` (manual) | Rollback patches, dry-runs | main |

> **Branch guard:** Manual dispatch from a non-`main` branch is rejected unless `version_override` is provided. This prevents accidental prereleases from feature branches.

**Pipeline:**

```
resolve-version → build (3 RIDs) → tag → publish-nuget
                                       → publish-homebrew (stable only)
                                       → publish-winget (stable only)
                                       → notify-failure (on error)
```

**Build matrix:** `osx-arm64`, `linux-x64`, `win-x64`. Each RID builds AOT, runs unit + integration + smoke tests, uploads artifacts.

**Tagging:** Every successful build is tagged (`v1.0.0-rc.3`, `v1.0.0`, etc.) so future runs can find the latest build number.

**NuGet:** Main publishes to package id `clet` (see [D-024](../../specs/decisions.md)). Prerelease versions (`-rc`) are hidden from default `dotnet tool install -g clet`; consumers opt in with `--prerelease`.

**Homebrew / WinGet:** Only on stable main releases (version has no `-` suffix). Both are placeholders until `tui-cs/homebrew-tap` exists and WinGet tooling is wired (D-012).

## Terminal.Gui versions

The TG dependency versions are set in `Directory.Build.props` as `<TerminalGuiVersion>` and `<TerminalGuiEditorVersion>`. The release workflow can override them via:

- `repository_dispatch` payload: `client_payload.tg_version`
- `repository_dispatch` payload: `client_payload.tge_version`
- `workflow_dispatch` input: `tg_version`
- `workflow_dispatch` input: `tge_version`
- MSBuild property: `-p:TerminalGuiVersion=2.0.3`
- MSBuild property: `-p:TerminalGuiEditorVersion=2.2.5`

Release builds reject Terminal.Gui or Terminal.Gui.Editor versions older than the latest stable NuGet release.

## Secrets and variables

| Name | Type | Used by | Purpose |
|------|------|---------|---------|
| `NUGET_API_KEY` | Secret | `publish-nuget` | Push packages to nuget.org |
| `HOMEBREW_TAP_TOKEN` | Secret | `publish-homebrew` | Push to `tui-cs/homebrew-tap` |
| `HOMEBREW_TAP_ENABLED` | Variable | `publish-homebrew` | Set to `true` to enable |
| `WINGET_ENABLED` | Variable | `publish-winget` | Set to `true` to enable |

## Rollback

See `docs/runbooks/release-rollback.md`.

## Related decisions

- **D-023** — Two-branch versioning (historical context; release publishing is now main-only)
- **D-022** — Independent versioning from TG (superseded by D-023, principle retained)
- **D-020** — TG dispatch types and MSBuild version variable
- **D-012** — Code signing deferred post-1.0
