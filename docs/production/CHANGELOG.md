# CHANGELOG

El formato se inspira en *Keep a Changelog*. Cada milestone incluye evidencia (brief §42).

## [Fase 1-A] — 2026-09-24 — Núcleo C# sin motor (Technical Prototype, parte A)

### Añadido
- Herramientas: .NET SDK 8.0.131 y git-lfs 3.4.1 en el contenedor, con un **SessionStart hook** (`.claude/hooks/session-start.sh`) para las sesiones cloud.
- Dirección visual D1 "Sobremesa" + D2 "Luz de Mástil" aprobada (ADR-013); ART_BIBLE parte B v0.2.
- `tools/CoreTests`: compila las fuentes de `Assets/` como netstandard2.1 con C# 9.0, igual que Unity 6.3 (verificado en UnityCsReference 6000.3.24f1), y ejecuta los tests NUnit fuera de Unity.
- `Padel.Core`:
  - `Vec2`/`Vec3`;
  - `Pcg32`, que reproduce la secuencia de referencia de pcg-c;
  - `TeamId`.
- `Padel.Rules`:
  - `ScoreKeeper`: CLASSIC, GOLDEN_POINT, STAR_POINT y CUSTOM mediante `MaxAdvantages`; sets, tie-break 1-2-2, súper tie-break, set final con ventaja, orden de saque A1-B1-A2-B2, lados de resto, elección de lado en el punto decisivo y cambios de lado;
  - `PointReferee`: faltas, let y reglas del punto a partir de eventos de reglas abstractos.
- `Padel.Simulation`:
  - `Court`: geometría FIP de ambas variantes, cuadros de saque, zonas y validador;
  - `Ball`: BALL_MVP con arrastre, Magnus, rebote con fricción y `TrajectoryPredictor`;
  - `Players`: `PlayerCommand`, `ICommandSource` y movimiento propio;
  - `Shots`: taxonomía, resolver, timing y `ShotSolver`;
  - `Match`: `MatchSimulation` determinista a 120 Hz.
- `Padel.AI`:
  - `TeamBrain`, `PlayerBrain` y `AIProfile` (Easy, Medium, Hard);
  - `HeadlessMatch` para partidos IA contra IA.
- CI de nivel 1 (GitHub Actions, .NET, Release).

### Evidencia
- `dotnet test tools/CoreTests/CoreTests.sln -c Release`: **127 passed, 0 failed**.
- CI de GitHub Actions: ejecuciones 1–6 en verde.
- Calibración FIP: al soltar la pelota desde 2,54 m rebota **1,400 m** (el rango FIP es 1,35–1,45).
- Spin:
  - en vuelo, el liftado cae a 2,90 m, el plano a 3,72 m y el cortado a 4,58 m;
  - tras el bote, la velocidad horizontal es de 10,47 m/s con liftado, 7,20 con plano y 5,45 con cortado.
- `ShotSolver`: 162 casos de apex con error menor de 5 cm; remate y víbora a su velocidad exacta.
- Saque simulado: unos 58 km/h, bota en el cuadro diagonal y rebota en el cristal.
- IA contra IA (24 puntos por partido): Difícil gana a Fácil 46–2 (semillas 1 y 2); la mediana es de 11 golpes por punto en 1v1 Medium contra Medium.
- Rendimiento del núcleo: IA de 33 a 71 µs/tick y simulación de 3 a 7 µs/tick (`docs/qa/evidence/2026-09-24_core-perf/`).

### Errores encontrados y corregidos
- Signo invertido al confinar a los jugadores en su mitad (detectado por test).
- Test de puntuación con una expectativa errónea: 1-0 no cierra un set, porque hacen falta 2 juegos de diferencia.
- El rebote irregular de la malla dependía del RNG compartido, lo que rompía la predicción exacta. Ahora depende del punto de impacto.
- Buffer de golpe: una pulsación temprana adelantaba el swing antes de que llegara la pelota, lo que producía golpes al aire. Ahora la pulsación espera a la pelota y se califica como early (ADR-007).
- El alcance se evaluaba en la posición actual del jugador en lugar de donde frena, y la IA calculaba el windup con otra resolución de golpe. Ahora hay una sola fuente de verdad (`ProvisionalShot`).
- Doble golpe entre compañeros en 2v2 cuando cambiaba el dueño de la pelota. Ahora quien inicia el swing conserva la pelota.

### Riesgos pendientes
- KI-012: reglas no modeladas (contacto pelota-jugador, juego exterior…).
- KI-013: balance de la IA (rallies largos en 2v2, pocos errores no forzados).
- Nada de esto se ha ejecutado aún dentro de Unity (KI-001).

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
