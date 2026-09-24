# CLAUDE.md — padelgame

Juego de pádel 2.5D: mundo, pista y pelota en 3D, personajes pixel art 2D, cámara en tercera persona, modos 1v1 y 2v2. Claude Code actúa como director técnico y como equipo de desarrollo. Este archivo es el contrato de trabajo; se lee al inicio de cada sesión.

## Estado actual

- **Fase:** 0-A (Research) ✅. **1-A (núcleo C# sin motor) ✅**: reglas, pista, pelota, jugadores, golpes, partido e IA básica, con 127 tests y CI verde. **0-B / 1-B (Unity) bloqueadas**: hace falta la máquina del propietario (KI-001). Los problemas abiertos están en `docs/qa/KNOWN_ISSUES.md` (KI-012 y KI-013).
- **Stack** ([ADRs](docs/decisions/README.md)):
  - Unity **6.3 LTS** (6000.3.x) con URP y Universal Renderer.
  - Pelota: simulación propia determinista en C# puro.
  - Personajes: quad billboard con shader de rampa.
  - Render: RenderTexture de baja resolución con escalado entero.
  - Cinemachine 3.1.x.
  - Input System 1.20.x detrás de una capa de comandos.
  - IA: HFSM + Utility.
  - MVP: un jugador contra IA.
- **Dirección visual:** D1 "Sobremesa" + D2 "Luz de Mástil" como variante nocturna (ADR-013).
- **Pendiente del propietario:** instalar Unity y validar el MCP (ROADMAP, Fase 0-B). En el contenedor la licencia no puede activarse: los hosts de licencias de Unity están bloqueados.

## Reglas no negociables (resumen del brief)

1. **No decidir por intuición.** Investigar en fuentes primarias, comparar, documentar en un ADR y solo entonces implementar. Nunca borrar un ADR: se sustituye por otro. Sin frases del tipo "esto debería funcionar", "supongo" o "probablemente". Si falta evidencia, se dice.
2. **Nada genérico como solución final:** ni Mixamo, ni humanoides genéricos, ni assets de tienda "porque existen", ni filtros pixel, ni PBR genérico. Los placeholders llevan `TEMP_`, `GRAYBOX_` o `PLACEHOLDER_`, viven en `Assets/_Project/Prototype/` y se listan en KNOWN_ISSUES como **REPLACE BEFORE ART LOCK**.
3. **Pasos pequeños.** Cada paso termina con código, compilación, tests, validación, evidencia, documentación y **commit**. Nada de "big bang".
4. **Evidencia:** no decir "funciona" sin demostrarlo con salida de tests, capturas o logs.
5. **Sin sobreingeniería.** No usar Singleton, Service Locator, DI framework, event bus global, ECS/DOTS, estado global mutable ni Netcode sin un ADR que lo justifique.
6. **Licencias:** todo asset externo va a `docs/production/ASSET_PROVENANCE.md` **antes** del commit. NC y ND están prohibidas.
7. **Comunicación durante la implementación:** usar la estructura **ESTADO / DECISIÓN / EVIDENCIA / PRÓXIMO PASO**.
8. **Preguntar al propietario solo** si la decisión cambia el alcance, implica gasto o contratar un servicio, hay dos alternativas válidas que cambian la experiencia, o falta información imprescindible. Todo lo demás se investiga, se decide, se documenta y se continúa.

## Arquitectura (ver docs/architecture/ARCHITECTURE.md)

- Capas: **Rules / State / Input / Simulation / Presentation**. `Padel.Core`, `Padel.Rules`, `Padel.Simulation` y `Padel.AI` tienen `noEngineReferences: true`: **no pueden usar `UnityEngine`**.
- `MatchState(t+1) = MatchSimulation.Step(MatchState(t), commands)` a **120 Hz**, con RNG sembrado dentro del estado. La presentación solo lee e interpola.
- La IA y los humanos producen `PlayerCommand` mediante `ICommandSource`. En 1v1 y 2v2 se usa el mismo código.
- Datos en ScriptableObjects (`*Definition`, `*Profile`) que se convierten a configuración inmutable.
- Coordenadas: origen en el centro de la red; X a lo ancho (±5), Z a lo largo (±10), Y hacia arriba; metros.

## Comandos

```bash
# Tests del núcleo fuera de Unity. En sesiones cloud, .NET 8 y git-lfs los instala el hook .claude/hooks/session-start.sh
dotnet test tools/CoreTests/CoreTests.sln -c Release
# Formato
dotnet format tools/CoreTests/CoreTests.sln --verify-no-changes

# Unity (solo en la máquina del propietario; ver la skill unity-cli y el ROADMAP de la Fase 0-B)
unity --version && unity doctor
unity test <ruta-proyecto> --mode EditMode --report-format junit --output ./test-results.xml
```

No inventar comandos de Unity: consultar `.claude/skills/unity-cli/`.

## Entorno cloud (limitaciones conocidas)

- En este contenedor no hay Unity, GPU, Blender ni Pixelorama. La red bloquea los hosts de Unity, FIP y Blender (ENVIRONMENT_AUDIT).
- `.NET 8` y `git-lfs` los instala el SessionStart hook (`.claude/hooks/session-start.sh`, registrado en `.claude/settings.json`).
- Los tests del núcleo deben compilar como **C# 9 / netstandard2.1** y usar solo la API clásica de NUnit 3, porque es lo que compila Unity 6.3.
- **No se versiona `.mcp.json`** (ADR-012): el MCP de Unity se registra en scope local en la máquina del propietario.

## Git

- Rama de trabajo indicada por la sesión. Commits pequeños que describan la intención (asunto en inglés, en imperativo). Nunca "misc", "stuff" ni "update".
- LFS para binarios según `.gitattributes`. Las fuentes de arte van en `ArtSource/`, fuera de `Assets/`.

## Skills del proyecto (`.claude/skills/`)

| Skill | Cuándo usarla |
|---|---|
| `padel-research` | Antes de cualquier decisión importante: protocolo de investigación y ADR |
| `padel-rules` | Implementar o testear reglas y puntuación (Star Point, tie-break, saque) |
| `padel-physics` | Pelota: integrador, rebotes, calibración, ShotSolver |
| `padel-gameplay` | Movimiento, golpes, input, flujo del punto |
| `padel-ai` | TeamBrain y PlayerBrain, Utility, dificultad |
| `2p5d-character-pipeline` | Billboards, direcciones, sombras y shader de personaje |
| `pixel-art-pipeline` | Sprites: Pixelorama → Unity, paleta, validación |
| `visual-quality-review` | Validar cualquier cambio visual con capturas |
| `asset-provenance` | Antes de añadir cualquier asset externo o de referencia |
| `gameplay-qa` | Tests, playtests, gates de fase |
| Oficiales de Unity (copiadas en el repo) | `unity-cli`, `physics-3d-collision`, `unity-package-management`, `validate-urp-render-graph-renderer-feature` |

## Documentos clave

- Decisiones: `docs/decisions/` (ADR-001 a ADR-012).
- Investigación: `docs/research/RESEARCH_REPORT.md` (resumen y matriz).
- Diseño: `docs/design/GDD.md`, `docs/design/GAMEPLAY_SPEC.md`.
- Arte: `docs/art/` (biblias en borrador hasta elegir la dirección).
- QA: `docs/qa/TEST_PLAN.md`, `docs/qa/KNOWN_ISSUES.md`.
- Producción: `docs/production/ROADMAP.md`, `CHANGELOG.md`, `ASSET_PROVENANCE.md`.
