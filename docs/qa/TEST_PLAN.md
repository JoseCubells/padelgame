# TEST PLAN

- **Versión:** 0.1
- **Fecha:** 2026-09-24
- **Base:** ARCHITECTURE §8, ADR-004/006/007/008/011

## 1. Niveles

| Nivel | Herramienta | Dónde corre | Qué cubre |
|---|---|---|---|
| **Unit (núcleo)** | NUnit puro | Unity Edit Mode **y** .NET 8 (`tools/CoreTests`), en el contenedor y en CI de nivel 1 | Core, Rules, Simulation, AI |
| **Integración** | Unity Test Framework, Play Mode | Unity local; CI de nivel 2 (GameCI, opcional) | MatchRunner, vistas, input, escenas |
| **Visual** | Capturas y checklist (ART_BIBLE A.7) | Unity local o MCP | Rendering, legibilidad, identidad |
| **Rendimiento** | Unity Profiler y Frame Debugger | Unity local | 60 FPS estables |
| **Playtest** | Protocolo §6 | Personas | Diversión, comprensión |

## 2. Suites obligatorias del núcleo (brief §29)

| Suite | Casos mínimos |
|---|---|
| **Puntuación** | 0-15-30-40; iguales y ventaja (CLASSIC); punto de oro; **Star Point** (iguales 1, ventaja 1, iguales 2, ventaja 2, iguales 3 = punto decisivo; máximo 11 puntos por juego); CUSTOM (`maxAdvantages` = 0/1/2/∞) |
| **Cambio de servicio** | Rotación A1→B1→A2→B2; 1v1; alternancia derecha/izquierda; en el punto decisivo el receptor elige lado |
| **Juegos y sets** | 6 con diferencia de 2; 7-5; 6-6 → tie-break; 7-6 |
| **Tie-break** | A 7 con diferencia de 2; saque 1-2-2 (el primer punto desde la derecha); cambio de lado cada 6 puntos; súper tie-break a 10 |
| **Cambio de lado** | Tras los juegos impares; comportamiento al terminar un set (convención documentada, NV en FIP) |
| **Rebotes (pelota)** | Parábola analítica sin arrastre (ε); **calibración FIP**: soltada desde 2,54 m rebota 1,35–1,45 m; topspin frente a backspin; energía no creciente; sin tunneling a 40 m/s; cristal casi especular; malla atenuada |
| **Validación de pista** | Las dimensiones de `CourtDefinition` están dentro de las tolerancias FIP; la red mide 0,88 m en el centro; las líneas de saque están a 6,95 m |
| **Reglas del punto** | Doble bote; pared o malla rival antes de botar = fallo; saque: bote en el cuadro + malla = falta, + cristal = en juego; let; red; toque de red del jugador |
| **Estados del partido** | PreServe → Serve → Rally → PointOver → Transition; transición entre puntos, juegos y sets |
| **Estados del jugador** | Idle → Moving → Windup → Contact → Recovery; bloqueo de movimiento; split-step |
| **ShotSolver** | Error menor de 5 cm hasta el objetivo; pasa la red en el 100 % de una rejilla de casos con semilla |
| **IA básica** | Asignación de bola por tiempo de intercepción; bandeja mejor que remate ante un globo profundo en la red; en N partidos con semilla, Difícil comete menos errores que Fácil |
| **Input** | Buffer de 120 ms; la carga; la dirección se muestrea en el contacto; comandos sintéticos conducen un partido completo |
| **Determinismo** | Misma semilla y mismos comandos dan el mismo hash de `MatchState` tras N ticks; `TrajectoryPredictor` coincide con la simulación |
| **Ball state** | Transiciones: en juego, botada, fuera, red, muerta |

**Criterio de aceptación de cada paso de implementación (brief §41):** compila, pasan todas las suites afectadas, hay evidencia (salida de tests) y se hace commit.

## 3. Play Mode (Unity)

- `MatchRunner` avanza N ticks con IA contra IA sin excepciones.
- Las vistas siguen al estado: la posición de la pelota renderizada coincide con el estado interpolado (±1 px interno).
- El input real llega como `PlayerCommand` (dispositivo simulado con Input System test fixtures).

## 4. Validación visual (brief §30)

Tras cada cambio visual importante:

1. Abrir la escena, ejecutar y capturar la Game View (RT interna + frame final) y la Scene View.
2. Guardar las capturas en `docs/qa/evidence/<fecha>_<tema>/`, siguiendo la política de LFS para PNG.
3. Revisar el checklist de ART_BIBLE A.7 y anotar el resultado.

### Protocolo de la prueba de resolución, cámara y direcciones (Fase 2; ADR-002, ADR-003, ADR-009)

- **Matriz:**
  - resoluciones: 320×180, 480×270 y 640×360 (384×216 opcional);
  - cámaras: B, C y D;
  - estrategias de escala de sprite: P1, P2 y P3;
  - direcciones: 4, 8 y 16 (solo con la mejor combinación anterior).
- **Capturas:** fijas y un vídeo corto con movimiento de cámara para ver el pixel crawl.
- **Prueba ciega** con 5–8 personas:
  - identificar el golpe del jugador lejano;
  - estimar la altura de la pelota;
  - decir de qué pared viene.
- **Métricas:** % de aciertos y tiempo de respuesta.
- El resultado se documenta en ADR-002, ADR-003 y ADR-009.

## 5. Rendimiento (brief §31)

- **Objetivo:** 60 FPS estables en el hardware objetivo, que se definirá en la Fase 0-B con la máquina del propietario como referencia mínima.
- **Medir** con el Profiler y el Frame Debugger:
  - CPU (simulación y presentación);
  - GPU;
  - draw calls y batches;
  - memoria;
  - asignaciones GC por frame (objetivo: 0 B en la simulación durante el rally);
  - física (debe ser mínima: sin PhysX en la simulación);
  - animación;
  - shaders.
- **Cuándo:** al cerrar las Fases 3, 4, 6 y 10. Nunca se afirma que algo "está optimizado" sin medición.

## 6. Playtest

Gate del vertical slice: GAMEPLAY_SPEC §9. Protocolo:
- 5–8 personas;
- sesiones de 20 minutos;
- observación sin ayuda los primeros 5 minutos;
- cuestionario;
- métricas registradas (duración del punto, errores no forzados, uso de la pared).

## 7. Evidencias

Cada milestone documenta en `docs/production/CHANGELOG.md`:
- archivos modificados;
- tests ejecutados y resultado;
- capturas;
- errores encontrados y corregidos;
- riesgos pendientes (brief §42).
