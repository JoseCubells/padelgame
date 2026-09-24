# ADR-009 — Cámara

- **Estado:** Aceptado (provisional). La cámara concreta se decide en la prueba visual de la Fase 2, junto con ADR-002.
- **Fecha de investigación:** 2026-09-24
- **Depende de / afecta a:** ADR-002, ADR-003, GAMEPLAY_SPEC

## Pregunta

¿Qué cámara en tercera persona refuerza el 2.5D (no top-down, no 2D plano, no 3D tradicional), mantiene la pelota, las paredes y la trayectoria legibles y convive con el pixel art? ¿La construimos con Cinemachine o a mano?

## Fuentes consultadas

- **[V]** `com.unity.cinemachine@main` `package.json` → `3.1.8-pre.2`; el informe tecnológico verifica el release **3.1.7** (2026-06-08).
- **[V]** URP 17.3 incluye `CinemachineUniversalPixelPerfect.cs` (extensión para Pixel Perfect Camera, que es ortográfica y de 2D Renderer, así que **no aplica**).
- [`VISUAL_RESEARCH.md` §B.2 y §C](../research/VISUAL_RESEARCH.md): cálculo de tamaño proyectado con tres cámaras. **A** está cerca y detrás del jugador (FOV 40°), **B** es tipo *broadcast* (FOV 30°) y **C** es teleobjetivo (FOV 20°). El cociente de tamaño entre el jugador cercano y el lejano es 2,75× en A, 1,75× en B y 1,45× en C. El snap al grid de píxeles solo es exacto en ortográfica [V UPixelator].
- Referencias [S/S2]: la cámara de retransmisión de tenis, Mario Tennis y Cult of the Lamb (ortográfica con inclinación).

## Alternativas evaluadas

| | Cámara propia (script) | **Cinemachine 3.1.x + extensión propia** |
|---|---|---|
| Seguimiento suave, límites, encuadre de grupo | Hay que escribirlo | Incluido: `CinemachineCamera`, confiner, group framing, impulse |
| Mezclas entre cámaras (juego, saque, repetición, test) | Hay que escribirlas | Blends nativos |
| Snap al grid de píxeles | Fácil | Se añade como `CinemachineExtension` en la etapa final |
| Riesgo | Mantener código propio | Dependencia de un paquete oficial y maduro |

| Tipo de proyección y encuadre | Lectura de la pelota | Pixel art | Sensación 2.5D |
|---|---|---|---|
| A: cercana, detrás del jugador (FOV 40°) | Mala en el campo lejano | Mala (2,75× de escala) | 3D tradicional |
| **B: broadcast (FOV 30°)** | Buena | Aceptable | **Buena** |
| **C: teleobjetivo (FOV 20°)** | Buena | **Buena** (1,45×) | Buena, algo plana |
| D: ortográfica inclinada | Buena | **Perfecta** (snap exacto) | Se pierde la escala con la distancia |

## Decisión

1. **Cinemachine 3.1.x**, con una `CinemachineExtension` propia para el snap al grid de píxeles del plano focal.
2. **Cámaras iniciales**:
   - `GameplayCamera`: semifija, detrás de la línea de fondo del jugador local, con seguimiento lateral limitado y encuadre que prioriza pelota, jugador local y pared de fondo rival.
   - `ReplayTestCamera`: libre y orbital, para depurar y capturar evidencias.
3. **Parámetros en `CameraProfile`** (ScriptableObject): FOV, altura, distancia, pitch, límites, amortiguación, peso de la pelota en el encuadre y comportamiento en saque y remate.
4. **Candidatas en prueba en la Fase 2**: B y C en perspectiva y D ortográfica inclinada, combinadas con las resoluciones de ADR-002. Gana la que mejor puntúe en la prueba ciega de lectura sin romper la estabilidad del píxel.
5. En 2v2 local, una sola cámara compartida (la pista entera cabe en el encuadre). La **pantalla dividida no se construye** y solo se reconsideraría con un ADR nuevo.

## Motivo

Cinemachine ya resuelve blends, límites, encuadre de grupo y sacudidas. Escribirlo a mano no aporta nada. La cámara semifija con FOV estrecho es la que menos castiga al pixel art en perspectiva y la que mejor lee la pista, según el cálculo del informe visual.

## Coste

Gratis (paquete oficial). Extensión de snap: 1 día [P].

## Riesgos

La cámara semifija se siente estática. Mitigación: pequeños desplazamientos en saque y remate, e impulse en los impactos fuertes, todo configurado por datos.

## Consecuencias

Hay que decidir si el jugador lejano (el rival) se ve siempre de frente. Eso afecta a las direcciones que se dibujan (ADR-003).

## Cómo revertirla

La cámara solo lee el estado de la simulación y no afecta a nada más. Sustituir Cinemachine por un script propio es un cambio local a la capa de presentación.

## Historial

- 2026-09-24: creado.
