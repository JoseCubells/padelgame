# ADR-005 — Sistema de animación

- **Estado:** Aceptado (provisional, depende del spike de ADR-003)
- **Fecha de investigación:** 2026-09-24
- **Decisores:** director técnico
- **Depende de / afecta a:** ADR-003, ADR-004 (fases del golpe), GAMEPLAY_SPEC, CHARACTER_BIBLE

## Pregunta

¿Cómo reproducimos las animaciones pixel art de los personajes para que sean precisas con el gameplay (frame de contacto = instante de golpe), sean data-driven y no condicionen la simulación?

## Fuentes consultadas

- [`GAMEPLAY_RESEARCH.md` §3.3, §4.3](../research/GAMEPLAY_RESEARCH.md): fases de golpe (windup, contact, recovery) definidas en datos; bloqueo de movimiento.
- [`VISUAL_RESEARCH.md` §E](../research/VISUAL_RESEARCH.md): lista mínima de animaciones (~120 frames por dirección) con animación limitada al estilo de Lethal League [S].
- [`TOOLS_RESEARCH.md` §1.3](../research/TOOLS_RESEARCH.md): el importer de Aseprite convierte tags en clips y conserva la duración de cada frame [S].
- [`SOURCE_VERIFICATION.md`](../research/SOURCE_VERIFICATION.md) y ADR-003: el renderer por defecto de la hipótesis es un quad propio, no `SpriteRenderer`.

## Alternativas evaluadas

| | A. Mecanim (Animator + AnimationClips de sprites) | B. Reproductor flipbook propio, dirigido por el estado de la simulación | C. Animación esqueletal 2D (2D Animation / Spine) |
|---|---|---|---|
| Sincronía con el gameplay | El Animator avanza con su propio reloj; sincronizar el frame de contacto exige eventos o normalizedTime | **La simulación dicta la fase** (`windup`/`contact`/`recovery` y su progreso) y la presentación elige el frame | Igual que A |
| Compatibilidad con ADR-003 B (quad) | Anima `SpriteRenderer.sprite`; con un quad hace falta un puente | Nativa | Otro pipeline de arte |
| Data-driven | Asset de Animator (grafo) | `AnimationSet` (ScriptableObject): frames por dirección, duración y eventos | Rig |
| Pixel art intencional | Sí | Sí | **No**: la deformación por huesos rompe el pixel grid, lo que va contra el brief |
| Depuración | Ventana de Animator | Se inspecciona como datos y se testea | — |
| Coste | Bajo al inicio | 1–2 días [P] | Alto |

## Decisión

**B: reproductor flipbook propio (`SpriteAnimationPlayer`) dirigido por el estado de la simulación.**

- `AnimationSet` (ScriptableObject) por personaje. Para cada clip guarda:
  - `id` (Idle, Run, Strafe, Backpedal, SplitStep, Ready, Drive, Backhand, Volley, Serve, Overhead, …);
  - frames por **dirección**;
  - duración de cada frame;
  - `loop`;
  - **marcadores de fase** (`contactFrame`, `recoveryStartFrame`);
  - eventos de presentación (sonido de pisada, polvo).
- Los golpes se reproducen **por fase normalizada**. El frame de contacto coincide siempre con el tick de contacto de la simulación: la animación se estira o comprime dentro de límites que marca la CHARACTER_BIBLE.
- La locomoción usa la velocidad simulada para elegir el clip y la cadencia.
- La simulación **nunca lee la animación**. Ver ARCHITECTURE: la presentación consume el estado y no escribe en él.
- Los frames se importan como `Sprite` de Unity (ADR-003), así que el formato de arte sigue siendo estándar. Si el spike elige A' (SpriteRenderer), el reproductor asigna `sprite` en lugar de las UV.

### Animation MVP

Primero se hace la lista mínima para que la Fase 3 sea jugable. Sale de las 17 animaciones del brief; el resto va a fases posteriores.

| MVP (Fase 2–3) | Después (Fase 5–6) |
|---|---|
| idle, ready (preparación), carrera, lateral, retroceso, split-step, drive, revés, volea (derecha/revés), saque, remate/bandeja (compartido en el MVP), recuperación | globo (pose propia), bandeja y víbora diferenciadas, smash x3/x4, recibir impacto, celebración, derrota |

## Motivo

En un juego de timing, la animación es presentación de un estado que decide la simulación. Invertir esa dependencia, con Mecanim moviendo el reloj, es la causa típica de golpes que "no coinciden". Un reproductor flipbook es poco código, se testea y encaja con el quad de ADR-003. La animación esqueletal se descarta porque rompe el pixel art intencional.

## Compatibilidad

C# puro sobre Unity 6000.3. No depende de ningún paquete salvo el importer de sprites elegido en ADR-003.

## Coste

1–2 días de código [P]. El coste dominante es el arte: unos 120 frames por dirección para el set completo.

## Riesgos

- Estirar demasiado la animación para cuadrarla con el contacto se nota. Mitigación: límites de estiramiento en datos y, si se superan, se elige otra variante de golpe.
- Esfuerzo de arte: el MVP se limita a la tabla anterior.

## Consecuencias

- `Padel.Presentation.Animation` se crea con `SpriteAnimationPlayer`, `AnimationSet` y `DirectionSelector`.
- La CHARACTER_BIBLE define los frames por clip y los límites de estiramiento.

## Cómo revertirla

`SpriteAnimationPlayer` es la única pieza que decide qué frame se muestra. Para pasar a Mecanim, se sustituye por un adaptador que escriba parámetros en un Animator. El contrato de fase de la simulación no cambia.

## Historial

- 2026-09-24: creado.
