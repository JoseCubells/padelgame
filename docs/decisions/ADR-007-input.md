# ADR-007 — Input y modelo de control

- **Estado:** Aceptado. El mapeo concreto de botones es provisional hasta el playtest de la Fase 3.
- **Fecha de investigación:** 2026-09-24
- **Decisores:** director técnico, con el informe del gameplay engineer
- **Depende de / afecta a:** ADR-006 (la IA emite comandos), ADR-008 (red), GAMEPLAY_SPEC

## Pregunta

¿Cómo llega el input al gameplay para que el gameplay no dependa del dispositivo, soporte teclado y mando, juego local de hasta 4 jugadores, IA y un posible online, y permita un sistema de timing preciso? ¿Y qué modelo de control (botones y golpes) usamos?

## Fuentes consultadas

- **[V]** Repositorio oficial [Unity-Technologies/InputSystem](https://github.com/Unity-Technologies/InputSystem): `package.json` en `develop` indica `1.20.1` con `unity: 6000.0`. El informe tecnológico sitúa el release publicado en 1.20.0 (2026-07-21).
- **[NV]** Documentación de `PlayerInput` y `PlayerInputManager`, y si Input System es el valor por defecto en proyectos nuevos de Unity 6: la documentación de Unity estaba bloqueada. Por eso se fija explícitamente en Project Settings.
- [`GAMEPLAY_RESEARCH.md` §1 y §6](../research/GAMEPLAY_RESEARCH.md), con fuentes [S]/[S2]:
  - Mario Tennis y Virtua Tennis: carga manteniendo el botón.
  - TopSpin 2K25: feedback de timing (early / perfect / late).
  - Críticas a Tennis World Tour: golpes sin contacto visible.
  - Críticas a Sports Story: exigía un posicionamiento demasiado preciso.

## Alternativas evaluadas

**Capa técnica**

| | Input Manager clásico (`Input.GetKey`) | Input System con código directo en gameplay | **Input System en un adaptador + comandos** |
|---|---|---|---|
| Multi-dispositivo y rebinding | Pobre | Bueno | Bueno |
| Local de 4 jugadores | Manual | `PlayerInputManager` | `PlayerInputManager` |
| Gameplay testeable sin dispositivos | No | No | **Sí** |
| IA y red con el mismo contrato | No | No | **Sí** |
| Brief | Prohibido | Prohibido ("no escribir gameplay contra Input") | Cumple |

**Modelo de control**

| | Un botón por golpe (8 botones) | **Tres familias + contexto** | Un botón + timing |
|---|---|---|---|
| Aprendizaje | Difícil | Medio | Fácil |
| Profundidad | Alta | **Alta** (el contexto elige bandeja, víbora o remate) | Baja |
| Encaje con el pádel | Artificial | Natural: el jugador real elige la *intención* y la situación define la técnica | Pobre |

## Decisión

1. **Input System 1.x** (1.20.x en Unity 6000.3). Active Input Handling = *Input System Package* (se fija explícitamente).
2. **Capa de comandos.** La simulación solo consume:
   ```
   PlayerCommand { int tick; Vec2 move; ShotIntent pressedIntent; bool held; float chargeTime; Vec2 aim; bool touchModifier; bool switchPlayer; }
   ```
   Las fuentes implementan `ICommandSource`: `HumanCommandSource` (adaptador de Input System), `AICommandSource` (ADR-006), `ScriptedCommandSource` (tests y replays) y, en el futuro, `NetworkCommandSource`.
3. **Juego local:** `PlayerInputManager` + `PlayerInput` (comportamiento *Invoke C# Events*) crean un `HumanCommandSource` por dispositivo. Pausa y menús son acciones de UI fuera de la simulación.
4. **Timing:**
   - las pulsaciones se registran con la marca de tiempo del evento (`context.time`) y se convierten a tick de simulación;
   - **buffer de 120 ms**, un valor [P] que se ajusta en playtest;
   - pulsar antes, hasta 400 ms, equivale a cargar.
5. **Modelo de control por defecto** (mando; el teclado equivale, y todo es rebindable):
   - Stick izquierdo: movimiento.
   - **Ataque** (sur): plano o top; con la bola alta: víbora o remate.
   - **Control** (oeste): cortado; con la bola alta: bandeja.
   - **Globo** (este).
   - **Toque** (modificador R1): dejada o chiquita.
   - La dirección se toma del stick en el instante de contacto, relativa a la pista.
   - La calidad depende del timing respecto al contacto ideal, **sin medidor en pantalla por defecto**, con feedback early / perfect / late después del golpe.
   - El remate especial x3/x4 se hace con Ataque cargado, bola alta y cerca de la red.
   - `SwitchPlayer` queda reservado (2v2 con control del compañero, por decidir en la Fase 4).
6. **Asistencia de posicionamiento** configurable: "imán" suave hacia el punto de golpeo ideal, con peso de 0 a 0,5. Es un ajuste de accesibilidad.

## Motivo

La capa de comandos es la pieza que hace posible "1 jugador + bots, 2 jugadores + bots, 4 jugadores" sin duplicar lógica. También permite tests deterministas, replays e IA con el mismo contrato, y deja preparado el online. El modelo de tres familias más contexto representa bien el pádel real: se elige la intención y la situación define la técnica.

## Compatibilidad

Input System 1.20.x requiere Unity 6000.0 o superior [V package.json], así que es compatible con 6000.3.

## Coste

Gratis (paquete oficial). Adaptador: 1 día [P].

## Riesgos

- **Tres familias quizá no bastan** para distinguir bandeja y víbora con intención clara. Mitigación: el playtest de la Fase 3 decide; la alternativa es un modificador.
- **Latencia de input.** Mitigación: timestamps de evento y procesado en el tick de simulación; se mide en QA.

## Consecuencias

- `Padel.Simulation` define `PlayerCommand` e `ICommandSource`. `Padel.Presentation.Input` contiene el `.inputactions` y los adaptadores.
- Tests: comandos sintéticos conducen partidos completos en Edit Mode.

## Cómo revertirla

El adaptador es la única pieza que conoce Input System. Cambiar de sistema de input afecta solo a ese adaptador.

## Historial

- 2026-09-24: creado.
- 2026-09-24: **ajuste del buffer, por evidencia de los partidos IA contra IA**. El diseño inicial ("una pulsación viva 120 ms") adelantaba el contacto antes de que llegara la pelota y producía golpes al aire. Semántica actual:
  - una pulsación hasta 0,4 s temprana (`MaxEarlyPress`) mantiene el swing hasta el contacto ideal y se califica según su adelanto (early, good o perfect);
  - una pulsación tardía golpea tarde, con 0,25 m de alcance extra [P], o falla;
  - el windup lo decide la simulación (`ProvisionalShot`), que es la única fuente de verdad también para la IA.
