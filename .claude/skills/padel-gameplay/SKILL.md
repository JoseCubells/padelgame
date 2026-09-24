---
name: padel-gameplay
description: Padel gameplay implementation guide - custom 2D court movement (no CharacterController/NavMesh), court zones, shot families and context-based ShotResolver, ShotDefinition data, timing windows and input buffer, PlayerCommand/ICommandSource, point flow state machine, 1v1/2v2 single model. Use when implementing or tuning Padel.Simulation.Players/Shots/Match or the Input System adapter.
---

# Gameplay de pádel

Especificación: `docs/design/GAMEPLAY_SPEC.md`. Decisiones: ADR-004 (golpe como evento), ADR-007 (input) y ADR-008 (modos).

## Reglas de implementación

1. **Todo el gameplay está en la simulación** (C# puro). Los MonoBehaviour solo presentan y adaptan.
2. **Un solo modelo de partido.** Un `TeamConfig` tiene 1 o 2 `PlayerConfig`. No se escribe lógica aparte para 1v1 y 2v2, ni una por cada combinación de humanos y bots.
3. **El input llega como `PlayerCommand`.** No se usan `Input.GetKey`, `Input.GetAxis` ni acciones de Input System dentro de la simulación. El adaptador `HumanCommandSource` convierte eventos (con su timestamp) en comandos por tick.
4. **Movimiento propio en 2D** (x, z) con aceleración, frenada y giro separados. Split-step implícito. Movimiento bloqueado desde el windup hasta el recovery. Zonas por coordenadas: Red, Transición y Fondo. No se usa NavMesh ni CharacterController.
5. **Golpes por familia más contexto.** La intención (Ataque, Control, Globo o Toque) y el contexto (altura de contacto, bote o volea, pared, zona) se resuelven en `ShotResolver`, que es una tabla ordenada en datos donde gana la primera regla que encaja. La tabla es testeable.
6. **`ShotDefinition` es un dato.** Contiene:
   - velocidad y ápice;
   - spin;
   - región objetivo;
   - ventanas de timing;
   - modelo de error;
   - fases;
   - animación, VFX y SFX.

   Nunca va en código.
7. **Timing:** la calidad es la diferencia entre la pulsación y el tick ideal de contacto que da `TrajectoryPredictor`. Buffer de 120 ms y carga manteniendo pulsado [P].
8. **Flujo del punto:** `PreServe → Serve → Rally → PointOver(motivo) → Transition`. Mostrar el motivo del punto es una ayuda de aprendizaje.

## Anti-patrones

- `PlayerEverythingManager`: clases que mezclan input, movimiento, golpe, animación y audio.
- Leer el estado de la animación para decidir el gameplay (ADR-005: es al revés).
- Valores de tuning en el código.

## Verificación

Tests de estados del jugador, de input (comandos sintéticos) y de flujo del punto (TEST_PLAN §2). El gate del vertical slice está en GAMEPLAY_SPEC §9.
