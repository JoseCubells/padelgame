# ARCHITECTURE

- **Versión:** 0.1 (Fase 0, antes de escribir código)
- **Fecha:** 2026-09-24
- **Base:** ADR-001 a ADR-012

## 1. Principios

1. **Separación estricta en capas:** GAME RULES / GAME STATE / PLAYER INPUT / SIMULATION / PRESENTATION (brief §15). Se impone **en compilación** mediante Assembly Definitions: las capas de núcleo no referencian `UnityEngine`.
2. **La simulación es una función:** `MatchState(t+1) = MatchSimulation.Step(MatchState(t), PlayerCommand[t])`. Es determinista, avanza con tick fijo de **120 Hz** y el RNG sembrado vive dentro del estado.
3. **La presentación solo lee.** Interpola entre el estado anterior y el actual, y consume los eventos del tick. Nunca escribe en la simulación.
4. **Datos antes que código.** Todo valor de balance vive en ScriptableObjects (`*Definition`, `*Profile`), que se convierten a configuraciones inmutables (POCO) al empezar el partido.
5. **Un solo modelo para 1v1 y 2v2.** Un partido tiene 2 equipos con 1 o 2 jugadores cada uno. Nada se escribe dos veces.
6. **Sin patrones por moda.** No se usan Singleton, Service Locator, frameworks de DI, event bus global, ECS/DOTS ni estado global mutable. Si un caso concreto los necesitara, requeriría un ADR. La composición se hace por referencias serializadas en la escena de arranque y por constructores en el núcleo.
7. **Interfaces solo donde aportan un punto de variación real:** `ICommandSource` (humano, IA, script de test, red futura). Nada de "una interfaz por clase".
8. **Depurable:** estado inspeccionable, *debug views* en la capa de presentación y *replays* reproducibles (semilla + comandos).

## 2. Ensamblados y dependencias

```
                     ┌───────────────────────────────┐
                     │   Padel.Core   (sin UnityEngine)│  Vec2/Vec3, Rng (PCG32), FixedTick, ids, SimEvent
                     └──────────────┬────────────────┘
                                    │
                     ┌──────────────▼────────────────┐
                     │   Padel.Rules  (sin UnityEngine)│  Scoring (Classic/Golden/Star/Custom), PointRules, ServeRules
                     └──────────────┬────────────────┘
                                    │
                     ┌──────────────▼────────────────┐
                     │ Padel.Simulation (sin Unity)   │  Court, Ball, Players, Teams, Shots, Match, Commands
                     └──────────────┬────────────────┘
                                    │
                     ┌──────────────▼────────────────┐
                     │   Padel.AI     (sin UnityEngine)│  TeamBrain, PlayerBrain, Utility, AICommandSource
                     └──────────────┬────────────────┘
      ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ┼ ─ ─ ─ ─ ─ ─ ─ ─  frontera: por debajo, solo C# puro
                     ┌──────────────▼────────────────┐
                     │   Padel.Data   (UnityEngine)   │  ScriptableObjects → configuración POCO
                     └──────────────┬────────────────┘
                     ┌──────────────▼────────────────┐
                     │ Padel.Presentation (Unity)     │  Rendering, Camera, Animation, Audio, UI, VFX, Input (adaptadores)
                     └──────────────┬────────────────┘
                     ┌──────────────▼────────────────┐
                     │ Padel.Infrastructure (Unity)   │  MatchRunner (bucle de tick), arranque, flujo de escenas
                     └───────────────────────────────┘
   Padel.Editor (solo editor): validadores de datos, herramientas de pista y de sprites, debug windows
   Tests: Padel.Core.Tests, Padel.Rules.Tests, Padel.Simulation.Tests, Padel.AI.Tests (Edit Mode, NUnit puro)
          Padel.PlayMode.Tests (Play Mode, integración y presentación)
```

- `Padel.Core`, `Padel.Rules`, `Padel.Simulation` y `Padel.AI` tienen `"noEngineReferences": true` en su `.asmdef`.
- **Doble ejecución de tests:** el código de esas cuatro capas y sus tests (NUnit puro, sin `UnityEngine.TestTools`) se compila también desde un proyecto .NET 8 en `tools/CoreTests/`, que referencia los mismos `.cs` por glob. Así se testea en el contenedor de Claude Code y en CI sin licencia de Unity (ADR-011, nivel 1).
- **Matemáticas:** `Vec2`/`Vec3` propios en `Padel.Core`, en lugar de `System.Numerics`, para que el comportamiento sea el mismo con cualquier runtime (Mono de Unity o CoreCLR de .NET). Así los tests de hash comparan resultados reproducibles dentro de cada runtime. El determinismo *entre runtimes* no se promete (ADR-008).

### Mapa de namespaces (brief §27)

| Área del brief | Namespace / ensamblado |
|---|---|
| Core | `Padel.Core` |
| Rules | `Padel.Rules` |
| Gameplay, Players, Teams, Ball, Court, Physics | `Padel.Simulation.{Players, Teams, Ball, Court, Shots, Match}`. La física de la pelota es `Padel.Simulation.Ball` (ADR-004) |
| AI | `Padel.AI` |
| Data | `Padel.Data` |
| Presentation, Animation, Audio, UI | `Padel.Presentation.{Rendering, Camera, Animation, Audio, UI, Vfx, Input}` |
| Infrastructure | `Padel.Infrastructure` |
| Editor | `Padel.Editor` |
| Tests | `Padel.*.Tests` |

## 3. Modelo de partido (Team / Match / Player / Court)

```
MatchConfig (inmutable)
 ├─ CourtConfig        ← CourtDefinition (dimensiones FIP, variante lateral, superficies)
 ├─ BallConfig         ← BallDefinition (masa, radio, C_D, C_L(S), decaimiento de spin)
 ├─ RulesConfig        ← MatchRules (CLASSIC | GOLDEN_POINT | STAR_POINT | CUSTOM; sets, tie-break, super tie-break)
 ├─ ShotCatalog        ← ShotDefinition[] (familia, contexto, velocidad, ápice, spin, ventanas, fases)
 └─ TeamConfig[2]
      └─ PlayerConfig[1..2]  ← PlayerStats (velocidad, aceleración, alcance, lateralidad)
            └─ Controller: Human(deviceSlot) | AI(AIProfile)

MatchState (mutable, pequeño, serializable)
 ├─ tick, rng
 ├─ BallState { pos, vel, spin, lastHitter, bouncesSinceHit, lastSurface }
 ├─ PlayerState[n] { pos, vel, facing, phase(Idle/Moving/Windup/Contact/Recovery), currentShot, phaseTime }
 ├─ PointState { phase(PreServe/Serve/Rally/PointOver), server, receiver, faultCount, serveSide }
 └─ ScoreState { sets[], games, points, deuceCount, tiebreak… }
```

- En 1v1, cada `TeamConfig` tiene un jugador; en 2v2, dos. Las reglas de orden de saque y de resto se escriben una sola vez sobre "jugadores del equipo".
- Combinaciones de control (1 humano + bots, 2 humanos + bots, 4 humanos): solo cambian los `Controller` de los `PlayerConfig`.

## 4. Bucle de ejecución

```
Unity Update (MatchRunner, Padel.Infrastructure)
  acumulador += Time.deltaTime (con límite)
  mientras acumulador >= 1/120:
      commands[i] = commandSources[i].GetCommand(state, tick)   // humano: InputSystem buffer; IA: PlayerBrain
      events.Clear()
      MatchSimulation.Step(ref state, commands, config, events)
          ├─ PlayerSim.Step          (movimiento 2D en la pista, fases de golpe)
          ├─ ShotSystem.TryResolve   (ventana de contacto → ShotResolver → ShotSolver → nueva vel/spin)
          ├─ BallSimulator.Step      (fuerzas + colisiones barridas → BallBounced/HitNet/HitWall…)
          ├─ PointRules.Evaluate     (eventos de pelota → punto ganado/fallo/let)
          └─ Scoring.Apply           (punto → juego/set/partido, cambio de saque y de lado)
      presentationQueue.Push(events); prevState = state
      acumulador -= 1/120
  alpha = acumulador / (1/120)
  views.Render(prevState, state, alpha)   // BallView, PlayerView, CameraRig, HUD, Audio, VFX
```

- Los eventos (`SimEvent`: bote, golpe, red, punto, fallo…) son una **lista por tick** que devuelve `Step`. Esto **no es un event bus global**: la presentación los recorre una vez y los descarta.
- `Time.timeScale` y la pausa solo afectan al acumulador. La simulación nunca lee el tiempo de Unity.

## 5. Estructura de carpetas

```
/                               (raíz del repo = raíz del proyecto Unity)
├─ Assets/
│  └─ _Project/
│     ├─ Scripts/
│     │  ├─ Core/            Padel.Core.asmdef
│     │  ├─ Rules/           Padel.Rules.asmdef
│     │  ├─ Simulation/      Padel.Simulation.asmdef
│     │  ├─ AI/              Padel.AI.asmdef
│     │  ├─ Data/            Padel.Data.asmdef
│     │  ├─ Presentation/    Padel.Presentation.asmdef
│     │  ├─ Infrastructure/  Padel.Infrastructure.asmdef
│     │  └─ Editor/          Padel.Editor.asmdef
│     ├─ Tests/
│     │  ├─ EditMode/        Padel.*.Tests.asmdef
│     │  └─ PlayMode/        Padel.PlayMode.Tests.asmdef
│     ├─ Data/               *.asset (Definitions/Profiles)
│     ├─ Art/                entregables importados (sprites, FBX, materiales, shaders)
│     ├─ Audio/
│     ├─ Scenes/             Boot, Court_Graybox, VisualLab, ...
│     └─ Prototype/          TEMP/GRAYBOX: todo lo de aquí se reemplaza antes del art lock
├─ ArtSource/                fuentes (.pxo, .blend, .kra, WAV de sesión), fuera de Assets
├─ Packages/  ProjectSettings/
├─ tools/
│  ├─ CoreTests/             proyecto .NET 8 que compila las capas de núcleo y sus tests fuera de Unity
│  └─ pipeline/              scripts (export de Pixelorama/Blender, validadores)
├─ docs/                     research, decisions, design, art, architecture, qa, production
└─ .claude/skills/           skills oficiales copiadas + skills del proyecto
```

## 6. Datos (ScriptableObjects)

| Asset | Contenido | Consumidor |
|---|---|---|
| `CourtDefinition` | Dimensiones FIP, variante de lateral, puertas, materiales de superficie | Simulation.Court, generador de graybox |
| `BallDefinition` | Masa, radio, C_D, curva C_L(S), decaimiento de spin, restituciones y fricciones por superficie | Simulation.Ball |
| `ShotDefinition` | Familia, reglas de contexto, velocidad, ápice o tiempo de vuelo, spin, ventana de timing, modelo de error, fases (windup/contact/recovery), id de animación, VFX, SFX | Simulation.Shots, Presentation |
| `PlayerStats` | Velocidades, aceleraciones, alcance, lateralidad | Simulation.Players |
| `MatchRules` | Sistema de puntuación, sets, tie-break, súper tie-break, lets, juego exterior | Rules |
| `AIProfile` | Reacción, ruido, split-step, error, temperatura, pesos | AI |
| `CameraProfile` | FOV, altura, distancia, pitch, límites, amortiguación | Presentation.Camera |
| `VisualProfile` | Resolución interna, escala de sprite, rampas, rim, sombras | Presentation.Rendering |
| `AnimationSet` | Frames por dirección, duraciones, marcadores de fase, eventos | Presentation.Animation |

- Conversión: cada asset expone `ToConfig()`, que devuelve un `readonly struct` o una clase inmutable del núcleo.
- Un validador de editor comprueba rangos (por ejemplo, que la red sea de 0,88 m ± 0,005 en el centro).

## 7. Reglas de código

- C# con `namespace` por archivo, un tipo público por archivo, nombres descriptivos. Nada de `PlayerEverythingManager`.
- Los MonoBehaviour son **delgados**: vistas y adaptadores. La lógica vive en clases C# puras.
- Sin `Find*`/`GetComponent` en caliente, sin `static` mutable, sin `UnityEngine.Random` ni `Time` en el núcleo.
- Los placeholders llevan el prefijo `TEMP_`/`GRAYBOX_` y una etiqueta `REPLACE BEFORE ART LOCK`, y se listan en `docs/qa/KNOWN_ISSUES.md`.
- Presupuesto de rendimiento: **60 FPS estables**. Solo se optimiza con medición del Profiler (TEST_PLAN §Performance).

## 8. Estrategia de tests

Ver `docs/qa/TEST_PLAN.md`. Resumen:

- **Edit Mode (NUnit puro)** para Core, Rules, Simulation y AI: puntuación, cambio de saque, juegos, sets, tie-break, Star Point, rebotes, validación de pista, estados del partido y del jugador, IA básica, comandos y estado de la pelota.
- **Play Mode** para la integración: el MatchRunner avanza, las vistas siguen al estado, las capturas de validación visual.
- **Determinismo:** hash de `MatchState` tras N ticks con semilla y comandos fijos.

## 9. Evolución prevista

- Si llega el online: `NetworkCommandSource` + transporte (ADR-008). El núcleo no cambia.
- Si el tamaño lo exige, `Padel.Simulation` puede partirse en más ensamblados sin cambiar las dependencias hacia arriba.
