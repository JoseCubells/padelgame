# CHANGELOG

El formato se inspira en *Keep a Changelog*. Cada milestone incluye evidencia (brief §42).

## [Fase 0-A] — 2026-09-24 — Research + decisiones

### Añadido
- Auditoría del entorno (`docs/production/ENVIRONMENT_AUDIT.md`).
- Investigaciones:
  - `TECHNOLOGY_RESEARCH`
  - `MCP_SKILLS_RESEARCH`
  - `VISUAL_RESEARCH`
  - `GAMEPLAY_RESEARCH`
  - `PADEL_RULES_RESEARCH`
  - `TOOLS_RESEARCH`
  - `SOURCE_VERIFICATION`
  - `RESEARCH_REPORT` (con la matriz de decisión)
- ADR-001 a ADR-012 e índice de decisiones.
- GDD, GAMEPLAY_SPEC y ARCHITECTURE.
- ART_BIBLE, CHARACTER_BIBLE, COURT_BIBLE y UI_BIBLE en borrador (dirección visual pendiente).
- TEST_PLAN, KNOWN_ISSUES, ROADMAP y ASSET_PROVENANCE.
- `.gitignore` (plantilla oficial de GitHub más añadidos) y `.gitattributes` (plantilla de referencia para Unity con LFS más añadidos).
- Skills oficiales de Unity copiadas al repo (commit `a851b67`): `unity-cli`, `physics-3d-collision`, `unity-package-management`, `validate-urp-render-graph-renderer-feature`.
- Skills propias del proyecto en `.claude/skills/` y `CLAUDE.md`.

### Evidencia
- Versiones de Unity verificadas con `git ls-remote --tags` sobre `Unity-Technologies/UnityCsReference`: 6000.3.24f1, 6000.6.2f1 y 6000.7.0b1.
- Código fuente de URP 17.3 inspeccionado: Pixel Perfect Camera es ortográfica y los shaders de sprite no tienen ShadowCaster (`SOURCE_VERIFICATION.md`).
- `git-lfs` 3.4.1 instalado en el contenedor (`git lfs version`).

### Errores o bloqueos encontrados
- KI-001 y KI-002: el entorno no puede ejecutar Unity y bloquea webs oficiales. No se ha escrito código de gameplay (brief §33).

### Riesgos pendientes
- Ver `docs/qa/KNOWN_ISSUES.md` y `RESEARCH_REPORT.md` §5.
