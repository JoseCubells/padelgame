# ROADMAP

- **Versión:** 0.1
- **Fecha:** 2026-09-24
- **Principio:** vertical slices pequeños. Cada paso termina con código, compilación, tests, validación, evidencia, documentación y commit (brief §41). **Si falla un gate, no se pasa de fase.**

## Estado

| Fase | Nombre | Estado |
|---|---|---|
| 0 | Research + Setup | Research ✅ completado · Setup ⏳ bloqueado por entorno (0-B) |
| 1 | Technical Prototype | **1-A ✅ completado** (núcleo C#, 127 tests, CI verde) · 1-B ⏳ requiere Unity |
| 2 | Visual Prototype | Pendiente (requiere Unity y la dirección visual) |
| 3 | 1v1 Vertical Slice | Pendiente |
| 4 | 2v2 | Pendiente |
| 5 | AI Polish | Pendiente |
| 6 | Visual Identity Pass | Pendiente |
| 7 | Audio Pass | Pendiente |
| 8 | UX/UI | Pendiente |
| 9 | QA | Pendiente |
| 10 | Optimization | Pendiente |
| 11 | Build | Pendiente |

## Fase 0 — Research + Setup

**0-A Research: ✅ completado.**
- Auditoría del entorno.
- 6 investigaciones.
- Verificación en fuentes primarias.
- ADR-001 a ADR-012.
- RESEARCH_REPORT con matriz.
- Biblias en borrador.
- Arquitectura.
- Skills.

**0-B Setup: requiere la máquina del propietario con Unity.** Procedimiento verificado en la skill oficial `unity-cli`:

1. Instalar el Unity CLI (beta): `curl -fsSL https://public-cdn.cloud.unity3d.com/hub/prod/cli/install.sh | UNITY_CLI_CHANNEL=beta bash` en macOS/Linux, o `install.ps1` en Windows. Comprobar con `unity --version` y `unity doctor`.
2. Ejecutar `unity auth login` y `unity license activate`. Comprobar con `unity auth status` y `unity license status`.
3. Instalar el editor 6.3 LTS: `unity releases --stream lts` y después `unity install <6000.3.x> --yes --accept-eula`.
4. Crear el proyecto **en la raíz de este repositorio** con la plantilla URP (`com.unity.template.urp-blank`). Antes, consultar `unity projects create --help`, porque crear dentro de una carpeta ya existente puede requerir un paso intermedio.
5. Project Settings:
   - Force Text;
   - Visible Meta Files;
   - Active Input Handling = Input System Package;
   - Linear.
6. Paquetes, con las versiones que resuelva 6000.3 congeladas en el manifest: Cinemachine 3.1.x, Input System 1.20.x y Test Framework.
7. Configurar UnityYAMLMerge en git (TOOLS_RESEARCH §4.1).
8. MCP: ejecutar `unity pipeline install`, después `unity mcp configure claude-code --project-path <repo> --dry-run` y, cuando la vista previa sea correcta, sin `--dry-run`. Luego **validación real** (MCP_SKILLS_RESEARCH §8), con evidencia en `docs/qa/evidence/mcp/`.
9. Commit: "Create Unity 6.3 URP project with project settings".

**Gate 0:** el proyecto abre y compila, las pruebas del MCP pasan con evidencia y el repositorio se puede clonar y reconstruir.

## Fase 1 — Technical Prototype

**1-A Núcleo C#, sin Unity: ✅ completado el 2026-09-24** (ver CHANGELOG). Incluye además `MatchSimulation` y la IA básica (`Padel.AI`), que se adelantaron desde las Fases 3 y 5 porque son C# puro y testeable.

1. `tools/CoreTests` (.NET 8 + NUnit) y el esqueleto de los ensamblados `Padel.Core` / `Padel.Rules` / `Padel.Simulation`, con los `.asmdef` preparados para Unity.
2. `Padel.Rules`: sistemas de puntuación (CLASSIC, GOLDEN, STAR, CUSTOM), sets, tie-break, orden de saque y cambio de lado, **con tests**.
3. `Padel.Simulation.Court`: `CourtConfig` a partir de los datos FIP y su validación, con tests.
4. **BALL_MVP:** `BallSimulator` (gravedad, arrastre, Magnus y rebote con fricción contra suelo, cristal, malla y red), con tests de calibración FIP.
5. `PointRules`: eventos de pelota → resultado del punto, con tests.
6. `PlayerSim` (movimiento 2D, fases de golpe), `ShotResolver` y `ShotSolver`, con tests.
7. CI de nivel 1 (GitHub Actions + .NET).

**1-B Integración Unity (máquina local o MCP):**

8. `MatchRunner` más vistas graybox (COURT_GRAYBOX generado desde `CourtDefinition`, pelota, jugadores cápsula `GRAYBOX_`).
9. Adaptador de Input System.
10. `ReplayTestCamera`.
11. Spikes:
    - personaje (ADR-003): A' frente a B;
    - importación de sprites (ADR-010).

**Gate 1:**
- se puede sacar, golpear, rebotar en la pared y el cristal y puntuar en graybox;
- los tests del núcleo pasan en .NET y en Unity;
- los spikes están decididos.

## Fase 2 — Visual Prototype

Requiere la **dirección visual elegida**.
- `VisualLab`: prueba de resolución, cámara y direcciones (TEST_PLAN §4).
- Shader de rampa para personaje y mundo.
- Primer personaje `TEMP` con locomoción en 8 direcciones.
- Sombras blob y reales.
- Pelota legible.

**Gate 2:** ADR-002, ADR-003 y ADR-009 cerrados con evidencia; ART_BIBLE parte B completa.

## Fase 3 — 1v1 Vertical Slice

- Una pista (graybox más un primer pase de materiales).
- Un personaje con el Animation MVP.
- Un oponente IA (3 niveles).
- Cámara de juego.
- Saque, golpes de las 3 familias, rebote, pared, cristal, puntuación Star Point y reset del punto.
- HUD mínimo.

**Gate 3:** GAMEPLAY_SPEC §9 ("el pádel es divertido") y brief §36.

## Fase 4 — 2v2

- `TeamBrain`: formación, cuerda y asignación de bola.
- Compañero IA.
- Multijugador local de 2 a 4 jugadores con `PlayerInputManager`.
- Transición ataque/defensa.
- Se decide `SwitchPlayer`.

## Fases 5–11

| Fase | Objetivo |
|---|---|
| 5 | Pulido de IA: personalidades, errores creíbles, simulación headless para el tuning |
| 6 | Identidad visual: COURT_ART_PASS, los 4 personajes, VFX. Se reevalúa Unity 6.7 LTS antes de empezar |
| 7 | Audio: foley propio, ambiente, música |
| 8 | UX/UI: menús, fuente bitmap propia, ADR de tecnología de UI |
| 9 | QA: regresión completa, playtests |
| 10 | Optimización, medida con el Profiler |
| 11 | Build: plataformas, CI de nivel 2 |
