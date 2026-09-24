# Evidencia: coste por tick del núcleo (simulación + IA)

- **Fecha:** 2026-09-24
- **Entorno:** contenedor cloud (4 vCPU, sin GPU), .NET 8.0.131, build Release. **No es Unity**: en Unity (Mono/IL2CPP) hay que volver a medir con el Profiler (TEST_PLAN §5).
- **Método:** bucle de 14.400 ticks (120 s de juego a 120 Hz), Medium contra Medium, semilla 11. Se mide con `Stopwatch` por separado `GetCommand` de todos los bots (IA) y `MatchSimulation.Step`.

| Modo | IA (µs/tick) | Simulación (µs/tick) | Presupuesto por tick a 120 Hz |
|---|---|---|---|
| 1v1 | 71,2 | 7,1 | 8.333 µs |
| 2v2 | 33,1 | 3,1 | 8.333 µs |

Medido antes de limitar la evaluación de golpe de la IA a los últimos 0,9 s; el coste actual es igual o menor.

**Lectura:** el núcleo completo ocupa menos del 1 % del presupuesto de CPU de la simulación. Estos números no dicen nada sobre render, GC en Unity ni animación, que quedan pendientes de medir en la Fase 1-B.
