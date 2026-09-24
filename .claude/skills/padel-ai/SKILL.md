---
name: padel-ai
description: Padel AI architecture (ADR-006) - TeamBrain formation FSM (Net/Back/TransitionUp/TransitionDown), ball ownership by interception time, partner rope 3-4 m slots, PlayerBrain HFSM (Ready/SplitStep/Moving/Preparing/Striking/Recovering/Covering), Utility-based shot and target selection with softmax, seeded reaction delay and noise, AIProfile difficulty data. Use when implementing, tuning or testing Padel.AI or AI partner behaviour.
---

# IA de pádel (ADR-006)

Detalle y valores iniciales: `docs/research/GAMEPLAY_RESEARCH.md` §5.

## Arquitectura

- **`TeamBrain`**, uno por pareja:
  - FSM de formación;
  - asignación de la bola al jugador con menor tiempo de intercepción previsto;
  - *slots* según formación, x prevista de la pelota y la "cuerda" de 3–4 m.

  Con un humano en la pareja, el compañero IA **lee al humano** y adapta su *slot*.
- **`PlayerBrain`**, uno por jugador IA:
  - HFSM de fases;
  - al entrar en `Preparing`, puntúa cada `ShotDefinition` válida × región objetivo:
    ```
    U = w1·P(éxito) + w2·Presión + w3·GananciaPosicional + w4·Coherencia(TeamBrain) − w5·Riesgo
    ```
  - elige con softmax(U / temperatura).
- **Salida: `PlayerCommand`**, el mismo contrato que un humano.

## Reglas

1. **Sin trampas:** usa el mismo `TrajectoryPredictor` que la física, degradado con retardo y ruido **sembrados** (`Rng` del estado). Un partido es reproducible con su semilla.
2. **La dificultad y la personalidad son datos** (`AIProfile`):
   - retardo de reacción: Fácil, Medio y Difícil = 350, 250 y 150 ms;
   - ruido de predicción;
   - probabilidad de split-step;
   - σ de ejecución;
   - temperatura;
   - pesos;
   - error bajo presión.
3. No se usan Behavior Trees, GOAP, Unity Behavior, ML ni rubber-banding sin un ADR nuevo.
4. La IA es C# puro dentro de `Padel.AI` (`noEngineReferences`).

## Depuración

La capa de presentación muestra una debug view con formación, *slots*, bola asignada y el top 3 de utilidades del último golpe. Para ajustar la IA se usa simulación headless: IA contra IA, N partidos con semilla, y se miden la duración de los puntos, el reparto de errores y el uso de la pared.

## Tests

TEST_PLAN §2, fila "IA básica". Añadir un test por cada comportamiento táctico nuevo (por ejemplo: "tras un globo profundo propio, la formación pasa a TransitionUp").
