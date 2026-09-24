# Vendored official Unity skills

These skill folders are copied verbatim (no edits) from the official repository
[Unity-Technologies/skills](https://github.com/Unity-Technologies/skills).

| Field | Value |
|---|---|
| Upstream commit | `a851b67` (2026-09-22, "fix: quote four skill descriptions that break YAML frontmatter, add a CI check (#74)") |
| Fetched | 2026-09-24 |
| License | Unity Companion License for Unity-dependent projects (see `UNITY_SKILLS_LICENSE.md`) |
| Vendored skills | `unity-cli`, `physics-3d-collision`, `unity-package-management`, `validate-urp-render-graph-renderer-feature` |

Why vendored instead of `npx skills add` or the plugin: cloud sessions of Claude Code
read project skills from `.claude/skills/`, and pinning to a commit makes the content
reviewable and reproducible. Selection rationale and rejected candidates:
`docs/research/MCP_SKILLS_RESEARCH.md` and `docs/research/RESEARCH_REPORT.md` (SKILLS).

Security review (2026-09-24): the folders contain Markdown only, plus one C# debug
MonoBehaviour (`physics-3d-collision/resources/CollisionDebugger.cs`) that only logs
collision events. `unity-cli/SECURITY.md` documents the CLI's accepted risks
(local C# eval in the Editor, `curl | bash` installer with SHA-256 pin).

To update: re-clone upstream, diff each folder, review, copy, and update the commit above.
Project-specific skills (non-vendored) live next to these and are prefixed `padel-`
or named after the project pipelines.
