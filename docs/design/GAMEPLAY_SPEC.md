# GAMEPLAY_SPEC

- **Versión:** 0.1 (Fase 0)
- **Fecha:** 2026-09-24
- **Fuentes:**
  - [`GAMEPLAY_RESEARCH.md`](../research/GAMEPLAY_RESEARCH.md)
  - [`PADEL_RULES_RESEARCH.md`](../research/PADEL_RULES_RESEARCH.md)
  - ADR-004/006/007/009

Los valores marcados **[P]** son propuestas iniciales de tuning sin fuente. Viven en datos (ScriptableObjects) y se ajustan en playtest. Los marcados **[FIP]** vienen del reglamento FIP 2026.

## 1. Sistema de coordenadas

El origen está en el centro de la red, a nivel del suelo.

- **X** recorre el ancho (±5 m).
- **Z** recorre el largo (±10 m); el equipo A juega en Z < 0.
- **Y** apunta hacia arriba.

Unidades: metros, segundos, kilogramos. La simulación avanza a un tick fijo de 120 Hz.

## 2. Pista (CourtDefinition)

Valores por defecto: PADEL_RULES_RESEARCH §9.

| Elemento | Valor |
|---|---|
| Pista | 20 × 10 m [FIP] |
| Red | 0,88 m en el centro y 0,92 m en los postes [FIP] |
| Líneas de saque | a 6,95 m de la red [FIP] |
| Fondos | 3 m de cristal + 1 m de malla [FIP] |
| Laterales | variante de cristal escalonado o de 4 m (configurable) |
| Puertas | según FIP |

Superficies de colisión: suelo, cristal, malla, red, postes y límite exterior.

## 3. Pelota (BallDefinition)

ADR-004.

| Parámetro | Valor |
|---|---|
| Masa | 57,7 g (centro del rango 56,0–59,4) [FIP] |
| Diámetro | 6,56 cm (centro del rango 6,35–6,77) [FIP] |
| Restitución del suelo | calibrada para que al soltarla desde 2,54 m rebote 1,35–1,45 m [FIP] (e ≈ 0,73–0,76) |
| C_D | 0,55 [P, dentro del rango de la literatura] |
| C_L(S) | ≈ 0,057 + 0,364·S [P, ajuste a revisar] |
| Cristal | rebote casi especular, restitución [P] |
| Malla | restitución baja más ruido sembrado [P] |

## 4. Movimiento del jugador (PlayerStats)

Movimiento propio en 2D (x, z) dentro de la zona de su campo. No se usa CharacterController ni NavMesh: la pista es un espacio acotado y estructurado, así que las zonas se calculan por coordenadas.

| Parámetro | Inicial [P] |
|---|---|
| v_max sprint lateral / frontal | 5,5 m/s |
| v_max retroceso | 4,0 m/s |
| 0 → v_max | 0,25 s |
| Frenada | 0,12 s |
| Cambio de sentido 180° | 0,30 s |
| Alcance normal / estirado | 0,9 / 1,3 m |
| Boost de split-step (ready en ±80 ms del golpe rival) | +30 % de aceleración durante 0,2 s |
| Asistencia de posicionamiento (imán) | 0–0,5 (accesibilidad) |

Durante un golpe, el movimiento queda bloqueado desde el windup hasta el recovery. En el windup se permiten micro-ajustes de hasta 0,3 m/s.

**Zonas:** Red (z entre 0 y 4 m del lado propio), Transición (4–7 m) y Fondo (7–10 m). Las usan la IA, la resolución de golpes y la cámara.

## 5. Golpes

### 5.1 Input → intención

Tres familias más un modificador (ADR-007): **Ataque**, **Control**, **Globo** y **Toque**.

- Mantener pulsado carga el golpe.
- La dirección se lee del stick en el instante de contacto, relativa a la pista.
- El buffer es de 120 ms [P].

### 5.2 Resolución por contexto (`ShotResolver`)

Es una tabla ordenada y gana la primera regla que encaja. La tabla es un dato y se puede testear.

| Intención | Bola baja (< 1,2 m), tras bote | Sin bote (volea) | Bola alta (> 2,2 m) cerca de la red | Tras pared de fondo |
|---|---|---|---|---|
| Ataque | Drive / revés plano | Volea de ataque | **Víbora** (sin carga), **Remate** (carga), **x3/x4** (carga completa + zona) | Bajada de pared |
| Control | Cortado | Volea cortada | **Bandeja** | Salida de pared controlada |
| Globo | Globo (top o cortado) | Globo de volea | Globo defensivo | Globo tras pared |
| Toque (mod.) | Chiquita o dejada | Dejada de volea | — | — |

El saque es un contexto propio: fase PreServe, la pelota bota detrás de la línea y el contacto se hace a la altura de la cintura o por debajo [FIP].

### 5.3 ShotDefinition (datos)

Cada golpe define:
- velocidad (rango);
- ápice o tiempo de vuelo;
- eje y rpm de spin;
- región objetivo;
- ventana de timing en ms: perfect, good y early/late;
- modelo de error: σ de dirección y de velocidad según la calidad del timing;
- fases windup, contact y recovery en s;
- id de animación, VFX y SFX.

Los rangos iniciales están en la tabla de taxonomía de GAMEPLAY_RESEARCH §3.2.

### 5.4 Calidad y feedback

- **Timing:** es la diferencia entre el tick de la pulsación (o de la liberación de la carga) y el tick de contacto ideal que da `TrajectoryPredictor`.
- **Resultado:** Perfect, Good, Early o Late. Aumenta la σ de error y reduce la velocidad efectiva.
- **Feedback:**
  - hitstop de 2 a 6 frames de presentación según la potencia [P];
  - destello o VFX en el impacto;
  - sonido que varía con la calidad;
  - texto discreto de early/late (opcional).

### 5.5 Solver

`ShotSolver` calcula la velocidad y el spin de salida para alcanzar el objetivo con el ápice pedido. Usa el método de tiro (*shooting method*) sobre `BallSimulator.Step`, con 3 a 6 iteraciones. Si el objetivo es imposible (por ejemplo, no pasa la red), sube el ápice o degrada la precisión.

## 6. Reglas y flujo del punto

Reglas por defecto FIP 2026 (PADEL_RULES_RESEARCH §5–§10):

- **Punto perdido** para quien golpea si:
  - la pelota golpea pared o malla rival antes de botar en su suelo;
  - hay doble bote;
  - toca la red o el poste propio;
  - hay doble golpe;
  - el jugador toca la red.
- **Saque:**
  - la pelota bota en el cuadro diagonal;
  - si después toca la malla, es falta;
  - si después toca el cristal, sigue en juego;
  - hay dos intentos;
  - es let si toca la red y entra sin tocar la malla antes del segundo bote.
- **Puntuación:** configurable con `CLASSIC`, `GOLDEN_POINT`, `STAR_POINT` (por defecto) y `CUSTOM_ARCADE` (con `maxAdvantages` y el resto de parámetros).
- **Tie-break:** a 7 con diferencia de 2, rotación de saque 1-2-2 y cambio de lado cada 6 puntos. El súper tie-break a 10 es opcional.

Fases del punto:

```
PreServe → ServeToss → Serve → (Fault → Serve | Let → Serve) → Rally → PointOver(motivo) → Transition → PreServe
```

- **PointOver** muestra el motivo: "doble bote", "pared antes de botar", "red"… Es una ayuda de aprendizaje.
- **Transition** dura unos 1,5 s: los jugadores se recolocan automáticamente y se actualiza el HUD.

## 7. Cámara

ADR-009.

- Cámara de juego semifija detrás de la línea de fondo del jugador local.
- Seguimiento lateral limitado.
- En el saque, encuadre del cuadro de saque.
- En el remate, un ligero *push-in*.
- La pelota y su sombra nunca salen del encuadre.

Las candidatas B, C y D se deciden en la Fase 2.

## 8. IA

Ver ADR-006. Dificultad en `AIProfile`, con 3 niveles iniciales: Fácil, Medio y Difícil. Valores en GAMEPLAY_RESEARCH §5.4.

## 9. Criterios de "el pádel es divertido" (gate del vertical slice)

Brief §36. Se evalúan en playtest con 5 a 8 personas.

1. Identifican que es pádel, en tercera persona, 2.5D, con personajes pixel art y pista 3D.
2. La pelota "se siente física": los rebotes en el cristal son predecibles.
3. Tras 5 minutos saben:
   - sacar;
   - devolver;
   - usar la pared de fondo;
   - tirar un globo.
4. Los puntos duran una mediana de 4 intercambios o más contra la IA Media [P].
5. Piden "otra partida".

Si falla cualquiera de estos puntos, **no se pasa a la Fase 4**.
