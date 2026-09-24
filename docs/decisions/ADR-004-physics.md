# ADR-004 — Física de la pelota

- **Estado:** Aceptado
- **Fecha de investigación:** 2026-09-24
- **Decisores:** director técnico (Claude Code), con informe del especialista en física
- **Depende de / afecta a:** ADR-001 (motor), ADR-006 (IA, usa la predicción), ADR-008 (red), `docs/architecture/ARCHITECTURE.md`

## Pregunta

¿Cómo simulamos la pelota (vuelo, rebotes en suelo, cristal, malla y red, golpe de pala, spin) para que se sienta física, sea configurable y testeable, sirva a la IA y no cierre la puerta a un futuro modo online?

## Fuentes consultadas

Detalle completo, con URLs, en [`GAMEPLAY_RESEARCH.md` §2](../research/GAMEPLAY_RESEARCH.md) y [`PADEL_RULES_RESEARCH.md` §3](../research/PADEL_RULES_RESEARCH.md).

- Documentación de Unity sobre PhysX: `bounceThreshold` (2 m/s por defecto), modos de combinación de materiales, CCD, `PhysicsScene.Simulate` para escenas paralelas y Enhanced Determinism [V/S según el punto].
- GDC 2018, "It IS Rocket Science! The Physics of Rocket League Detailed" (Psyonix). Para el juego en red, cliente y servidor deben simular de forma idéntica [V].
- Literatura aerodinámica sobre pelotas de tenis, el análogo más cercano con datos publicados:
  - Cross & Lindsey 2014: C_D ≈ 0,507 en vuelo real.
  - Goodwill 2004 y Mehta: 0,55–0,7 en túnel de viento.
  - Štěpánek 1988: C_L 0,075–0,275 para un parámetro de spin S de 0,05 a 0,6.
- Cross, "Bounce of a spinning ball": modelo de rebote con fricción (grip frente a slide) [S].
- Reglamento FIP 2026: bola de 56,0–59,4 g y 6,35–6,77 cm, que rebota 135–145 cm al soltarla desde 2,54 m [V1: extracto del PDF oficial].

## Alternativas evaluadas

| | A. Rigidbody + PhysX | B. Integrador propio determinista (C# puro) | C. Trayectorias analíticas "sobre raíles" (A→B scriptado) |
|---|---|---|---|
| Arrastre + Magnus | Se añaden a mano con `AddForce` | Nativos en la fórmula | No existen: la curva es predefinida |
| Rebote con spin (grip/slide) | No se modela; solo fricción y restitución combinadas | Modelo de impulso tangencial | Falso |
| Botes lentos | `bounceThreshold` de 2 m/s los elimina (configurable a nivel global) | Umbral explícito en datos | N/A |
| Predicción para la IA | Escena paralela + `PhysicsScene.Simulate`: caro y con estado duplicado | Copiar un struct y llamar a `Step` N veces: barato y exacto | Trivial |
| Determinismo | Solo con Enhanced Determinism, en la misma plataforma | Determinista en el mismo binario y plataforma. Entre plataformas, no garantizado con float [NV] | Total |
| Tests | Requieren escena o Play Mode | NUnit en Edit Mode sin MonoBehaviour | Fáciles |
| Coste inicial | Bajo | Medio: estimación de 300–600 líneas [P] | Bajo |
| Coste de tuning y bugs de borde | Alto (tunneling, jitter, umbrales) | Bajo: la geometría es ~10 planos y una red | Bajo, pero se ve falso |
| Sensación física | Buena para objetos genéricos | Buena y ajustable a pádel | **No cumple el brief** ("no basta con mover una esfera de A a B") |

## Ventajas y desventajas (resumen)

- **A (PhysX):** gratis en el motor y fiable para geometría arbitraria. Pero el pádel es una caja de ~10 planos, y lo que PhysX no ofrece (Magnus, acoplamiento spin-fricción, predicción barata, determinismo) es justo lo que el pádel necesita.
- **B (propio):** control total y testeable. Además, la IA usa **la misma función** que el juego, así que el error de predicción es cero por construcción. A cambio, hay que escribir y mantener el integrador y las colisiones.
- **C (raíles):** se descarta porque contradice el brief y no permite rebotes emergentes en pared.

## Compatibilidad

B es C# puro sin dependencias del motor: compila en Unity (cualquier versión con C# 9) y en un proyecto .NET de test fuera de Unity. Es independiente de ADR-001.

## Coste

Cero en licencias. Desarrollo estimado de 3 a 5 días para BALL_MVP con tests [P].

## Riesgos

| Riesgo | Mitigación |
|---|---|
| Faltan coeficientes medidos para una bola de pádel contra cristal, malla y césped | Calibrar el suelo con el test FIP (caída de 2,54 m → 1,35–1,45 m). Para cristal y malla, ajustar con vídeo de referencia a 240 fps (tarea de QA) y dejar todo en `SurfaceMaterial` (datos) |
| Tunneling a velocidades de remate (~37 m/s) | Colisiones barridas (swept) contra planos analíticos y paso de 1/120 s. Hay un test explícito a 40 m/s |
| El determinismo entre plataformas no está garantizado con float | Solo afecta a un online lockstep/rollback entre plataformas. Se reevalúa (punto fijo) en ADR-008 si se aborda el online |
| La sensación no convence | Todos los parámetros son datos, y un modo de depuración dibuja la trayectoria prevista. El diseño se itera en la Fase 1 |

## Decisión

**B: integrador propio determinista en C# puro para la pelota. PhysX no participa en la simulación de la bola.**

Especificación del BALL_MVP:

1. `BallState { position, velocity, spin }` es un struct serializable.
2. `BallSimulator.Step(ref BallState, in BallParams, in CourtGeometry, float dt)` es una función pura, sin estado oculto.
3. Paso fijo de **1/120 s** para la simulación de partido, desacoplado del framerate. La presentación interpola.
4. Integrador semi-implícito de Euler. Se reevalúa Velocity Verlet si los tests de precisión lo piden.
5. Fuerzas:
   - gravedad;
   - arrastre `F = -½ρ·C_D·A·|v|·v` (C_D por defecto 0,55);
   - Magnus `F = ½ρ·C_L(S)·A·|v|²·(ω̂×v̂)`;
   - decaimiento de spin.

   Todos los coeficientes están en `BallDefinition` (datos).
6. Colisiones analíticas y barridas contra `CourtGeometry`: suelo, paredes de cristal, malla, red (altura variable por x), postes y límites exteriores. Cada superficie usa un `SurfaceMaterial` (restitución normal, fricción, aleatoriedad controlada con semilla para la malla).
7. Rebote con modelo de impulso normal + tangencial (grip/slide con límite de Coulomb), que acopla spin y velocidad.
8. **El golpe de pala no es una colisión física.** Es un evento de gameplay: `ShotSolver` calcula la velocidad y el spin de salida para alcanzar un objetivo (balística inversa por *shooting method* reutilizando `Step`).
9. `TrajectoryPredictor` reutiliza `Step` para anticipar botes, cruces de pared y puntos de golpeo (IA y asistencia de movimiento).
10. Eventos de simulación (`BallBounced(surface)`, `BallHitNet`, `BallOut`…) alimentan a las reglas (`Rules`) y a la presentación (audio/VFX) sin acoplarlas.

Expansión posterior, por fases: tabla de spin más rica, efecto de la malla irregular, calibración con vídeo.

## Motivo

El pádel se define por las paredes y el spin: una bandeja que muere tras el cristal, un x3 que sale por la lateral. PhysX no modela el acoplamiento spin-fricción ni Magnus, trae umbrales que hay que desactivar y hace cara la predicción. Un integrador propio sobre una geometría de ~10 planos es pequeño, testeable en Edit Mode y determinista. Eso es a la vez la base de una IA justa, que ve la misma física que el jugador, y de un posible online.

## Consecuencias

- Se crea el ensamblado `Padel.Simulation` sin referencias a `UnityEngine` (ver ARCHITECTURE). Tipos matemáticos propios o `System.Numerics`.
- Los jugadores tampoco usan PhysX para moverse (ADR-007/ARCHITECTURE).
- PhysX puede usarse solo en presentación: confeti, objetos decorativos. Nunca en reglas.
- Tests obligatorios:
  - parábola analítica;
  - calibración FIP del bote;
  - topspin frente a backspin;
  - energía no creciente;
  - no-tunneling a 40 m/s;
  - determinismo por hash;
  - precisión del `ShotSolver`.

## Validación pendiente

- Tests de calibración y sensación en la Fase 1 (Technical Prototype).
- Coeficientes de cristal y malla: campaña de vídeo de referencia (KNOWN_ISSUES).

## Cómo revertirla

La simulación de la bola vive detrás de `BallSimulator.Step`. Para pasar a PhysX habría que implementar un adaptador que avance una `PhysicsScene` y copie el estado al struct. Reglas, IA y presentación consumen `BallState` y los eventos, no PhysX, así que no cambiarían. Coste estimado: 1–2 días más la pérdida de la predicción exacta.

## Historial

- 2026-09-24: creado y aceptado.
