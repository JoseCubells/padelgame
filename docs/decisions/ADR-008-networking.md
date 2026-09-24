# ADR-008 — Networking y modos de jugadores del MVP

- **Estado:** Aceptado
- **Fecha de investigación:** 2026-09-24
- **Decisores:** director técnico
- **Depende de / afecta a:** ADR-004, ADR-007, ARCHITECTURE

## Pregunta

¿El MVP debe ser un jugador contra IA, multijugador local u online? ¿Cómo diseñamos el núcleo para que un online futuro sea posible sin construirlo ahora?

## Fuentes consultadas

- [`GAMEPLAY_RESEARCH.md` §7](../research/GAMEPLAY_RESEARCH.md): GDC 2018 Rocket League. La física en red exige que cliente y servidor simulen de forma idéntica [V].
- **[V]** [`TECHNOLOGY_RESEARCH.md` §4.5](../research/TECHNOLOGY_RESEARCH.md) y [`SOURCE_VERIFICATION.md` §5](../research/SOURCE_VERIFICATION.md):
  - Netcode for GameObjects 2.13.x requiere Unity 6000.0 o superior; la rama `develop-2.0.0` está en `2.13.4`.
  - **NGO 3.0.x requiere 6000.7.**
  - Netcode for Entities 1.12.0 en un proyecto oficial 6.3.
  - Multiplayer Services 2.1.1.
- **[NV]** Precios de Relay y Multiplayer Services.
- Mercado ([`GAMEPLAY_RESEARCH.md` §1](../research/GAMEPLAY_RESEARCH.md), [S]): los dos competidores directos anunciados (Padel Rivals, KorrPadel) prometen local de 4 y online. El hueco detectado es **un modo individual profundo con un compañero IA creíble**.

## Alternativas evaluadas

| | Un jugador + IA | Multijugador local (1–4, pantalla compartida) | Online |
|---|---|---|---|
| Valida "el pádel es divertido" | **Sí**, directamente | Sí | No añade nada a esa validación |
| Coste marginal con la capa de comandos (ADR-007) | Base | **Bajo**: la pista entra entera en una cámara y no hace falta pantalla dividida | Alto (servidor o relay, predicción, reconciliación, matchmaking, QA de red) |
| Coste de servicio | 0 | 0 | Relay/servicios (precio [NV]) |
| Riesgo | Bajo | Bajo | Alto; además NGO 3.x obliga a pasar a 6.7 |
| Diferenciación de mercado | **Alta** (hueco detectado) | Media (los competidores también lo tienen) | Baja (los competidores lo prometen) |

Candidatos de solución online, a comparar **solo** si se aborda: NGO 2.x/3.x (servidor autoritativo o Distributed Authority), Netcode for Entities (exige ECS: descartado por sobreingeniería, ver brief §27), y soluciones externas deterministas o de rollback.

## Decisión

1. **MVP (Fases 3–5): un jugador contra IA** en 1v1 y en 2v2 con compañero IA.
2. **Multijugador local de hasta 4 jugadores en pantalla compartida**: la arquitectura lo soporta desde el día 1 (comandos). Se **activa y valida en la Fase 4**, con el 2v2.
3. **No hay online en el MVP. No se instala ningún paquete de Netcode.**
4. Reglas de diseño para no cerrar el online:
   - tick fijo de simulación (120 Hz);
   - `MatchState(t+1) = Simulation.Step(MatchState(t), Commands(t))`;
   - separación estricta **GAME RULES / GAME STATE / PLAYER INPUT / SIMULATION / PRESENTATION**;
   - estado pequeño y serializable (una pelota, 4 jugadores, marcador y RNG);
   - RNG propio sembrado dentro del estado (nunca `UnityEngine.Random` en la simulación);
   - sin PhysX en la simulación.
5. Si en el futuro se decide hacer online, se crea un ADR nuevo que compare las soluciones anteriores. Tendrá en cuenta que el float no garantiza determinismo entre plataformas [NV]: un lockstep o rollback entre plataformas exigiría evaluar punto fijo, mientras que un servidor autoritativo con predicción del cliente no lo exige.

## Motivo

El objetivo de las Fases 3–5 es demostrar que **el pádel es divertido**. El online no ayuda a demostrarlo, multiplica el coste de QA y ataría el motor a 6.7 por NGO 3.x. El multijugador local sale casi gratis con la capa de comandos. Y el hueco de mercado está en el modo individual con compañero IA.

## Compatibilidad

No aplica: no se instala nada.

## Coste

0 €.

## Riesgos

- **El mercado exige online** para competir. Mitigación: el núcleo queda preparado; la decisión se revisa al cerrar la Fase 5 con datos de playtest.
- **Se rompe la disciplina de separación de capas.** Mitigación: el ensamblado de simulación sin `UnityEngine` lo impide a nivel de compilación, y hay tests de determinismo por hash.

## Consecuencias

Test obligatorio: dos ejecuciones con la misma semilla y los mismos comandos producen el mismo hash de estado.

## Cómo revertirla

Añadir el online más adelante es aditivo: `NetworkCommandSource` + transporte. No hay que deshacer nada.

## Historial

- 2026-09-24: creado.
