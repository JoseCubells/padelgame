# ADR-006 — Arquitectura de IA

- **Estado:** Aceptado
- **Fecha de investigación:** 2026-09-24
- **Decisores:** director técnico, con el informe del especialista en IA
- **Depende de / afecta a:** ADR-004 (predicción de la pelota), ADR-007 (la IA emite comandos), GAMEPLAY_SPEC

## Pregunta

¿Qué arquitectura de IA permite bots que *entiendan* el pádel (zonas, cobertura, compañero, paredes, ataque y defensa, selección de golpe, errores creíbles) con como mucho 4 agentes, sin complejidad innecesaria?

## Fuentes consultadas

[`GAMEPLAY_RESEARCH.md` §5](../research/GAMEPLAY_RESEARCH.md):

- Game AI Pro (capítulos de libre acceso en gameaipro.com): comparativa de FSM, HFSM, Behavior Trees y Utility [S].
- Jeff Orkin, "Three States and a Plan: The AI of F.E.A.R." (GOAP) [S].
- Charlas de Dave Mark sobre Utility AI [S].
- Fuentes de coaching de pádel sobre formación de pareja ("la cuerda" de 3–4 m), globo para tomar la red y defensa tras el cristal [S2].
- El estado del paquete Unity Behavior es **[NV]**: repositorio no público y documentación bloqueada.

## Alternativas evaluadas

| | FSM / HFSM | Behavior Tree | Utility AI | GOAP / HTN | Híbrido HFSM + Utility |
|---|---|---|---|---|---|
| Adecuación a un dominio con fases claras (saque, rally, red, fondo) | Alta | Media | Baja por sí sola | Baja | **Alta** |
| Decisiones con matices (qué golpe, a dónde) | Pobre (explosión de transiciones) | Media | **Alta** | Media | **Alta** |
| Depuración | Muy fácil | Media | Media (puntuaciones visibles) | Difícil | Fácil |
| Coste para 4 agentes | Mínimo | Bajo | Bajo | Planificador innecesario | Bajo |
| Dependencias | Ninguna | Paquete o implementación | Ninguna | Implementación | Ninguna |
| Veredicto | Parte | Descartado (no aporta frente a HFSM aquí) | Parte | **Descartado** por sobreingeniería | **Elegido** |

La opción del paquete **Unity Behavior** también se descarta: su estado no está verificado, añade una dependencia y no aporta nada a un dominio de 4 agentes.

## Decisión

**Híbrido en dos capas, en C# puro y dentro de la simulación:**

1. **`TeamBrain`** (uno por pareja; en 1v1, pareja de un solo jugador):
   - FSM de formación: `Net / Back / TransitionUp / TransitionDown`.
   - **Asignación de bola**: se la lleva el jugador con menor tiempo de intercepción previsto, con desempate por lado.
   - **Slots** de posición según la formación, la x prevista de la pelota y la cuerda de 3–4 m.
   - Con un humano en la pareja, el compañero IA **lee la posición del humano** y adapta su slot.
2. **`PlayerBrain`** (uno por jugador IA):
   - HFSM `Ready → SplitStep → Moving → Preparing → Striking → Recovering`, más `Covering` cuando no es su bola.
   - **Selección de golpe por Utility**: para cada `ShotDefinition` válida y cada región objetivo, puntúa probabilidad de éxito, presión, ganancia posicional, coherencia con el `TeamBrain` y riesgo. Elige con softmax y temperatura.
3. **La IA no hace trampas.** Usa el mismo `TrajectoryPredictor` (ADR-004) y degrada su información con **retardo de reacción y ruido sembrados**.
4. **Salida = `PlayerCommand`**, el mismo contrato que un humano (ADR-007). Así la IA se testea como cualquier fuente de input.
5. **La dificultad es un dato** (`AIProfile`, ScriptableObject → POCO):
   - retardo de reacción;
   - ruido de predicción;
   - probabilidad de split-step;
   - σ de ejecución;
   - temperatura;
   - pesos de utilidad ("personalidad");
   - multiplicador de error bajo presión.

   Valores iniciales en GAMEPLAY_RESEARCH §5.4, marcados [P] y pendientes de tuning.
6. Sin rubber-banding en el MVP. No hay evidencia a favor en deportes de raqueta.

## Motivo

El pádel tiene fases discretas (formación, saque, rally) que una FSM modela de forma legible, y decisiones continuas (golpe y destino) que Utility resuelve sin explosión de transiciones. Con 4 agentes, BT, GOAP o planificadores no aportan nada que compense su coste de depuración. Así lo pide el brief: "priorizar simplicidad y control".

## Compatibilidad

C# puro en el ensamblado de simulación, sin `UnityEngine`.

## Coste

Sin licencias. Implementación iterativa: la IA básica 1v1 en la Fase 3 y la de pareja en las Fases 4–5.

## Riesgos

- **La IA parece robótica.** Mitigación: ruido y personalidades como datos, split-step visible y errores bajo presión.
- **El tuning lleva mucho tiempo.** Mitigación:
  - herramienta de simulación headless (IA contra IA, N partidos con semilla) para medir el reparto de errores y la duración de los puntos;
  - tests de regresión.

## Consecuencias

- Tests en Edit Mode:
  - asignación de bola;
  - la utilidad de la bandeja supera a la del remate ante un globo profundo;
  - la dificultad Difícil tiene menos error medio que Fácil en N simulaciones;
  - reproducibilidad por semilla.
- Debug view de presentación: slots, formación, utilidades del último golpe.

## Cómo revertirla

`PlayerBrain` es una implementación de `ICommandSource`. Se puede sustituir por otra arquitectura, por ejemplo un BT, sin tocar reglas, física ni presentación.

## Historial

- 2026-09-24: creado.
