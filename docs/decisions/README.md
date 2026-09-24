# Architecture Decision Records (ADR)

Registro de decisiones importantes del proyecto. **Nunca se borra un ADR.** Si una decisión cambia, se crea un ADR nuevo que la sustituye y el antiguo pasa a `Estado: Sustituido por ADR-XXX`.

## Estados

- **Propuesto:** redactado, pendiente de validación práctica.
- **Aceptado:** decisión vigente.
- **Aceptado (provisional):** vigente, pero condicionado a una prueba o verificación listada en el propio ADR. Si la prueba falla, se reabre.
- **Sustituido por ADR-XXX** / **Rechazado**.

## Índice

| ADR | Pregunta | Estado |
|---|---|---|
| [ADR-001](ADR-001-engine.md) | ¿Qué motor? | Aceptado (provisional) |
| [ADR-002](ADR-002-rendering.md) | ¿Cómo se renderiza un mundo 3D con personajes pixel art? | Aceptado (provisional) |
| [ADR-003](ADR-003-character-pipeline.md) | ¿Cómo existen técnicamente los personajes 2D en el espacio 3D? | Aceptado (provisional) |
| [ADR-004](ADR-004-physics.md) | ¿Cómo se simula la pelota? | Aceptado |
| [ADR-005](ADR-005-animation.md) | ¿Cómo se animan los personajes? | Aceptado (provisional) |
| [ADR-006](ADR-006-ai.md) | ¿Qué arquitectura de IA? | Aceptado |
| [ADR-007](ADR-007-input.md) | ¿Cómo llega el input al gameplay? | Aceptado |
| [ADR-008](ADR-008-networking.md) | ¿Online, local o un jugador en el MVP? | Aceptado |

## Plantilla

```
# ADR-NNN — Título
- Estado:
- Fecha de investigación:
- Decisores:
- Depende de / afecta a:

## Pregunta
## Fuentes consultadas
## Alternativas evaluadas
## Ventajas y desventajas (por alternativa)
## Compatibilidad
## Coste
## Riesgos
## Decisión
## Motivo
## Consecuencias
## Validación pendiente
## Cómo revertirla
## Historial
```

## Niveles de evidencia usados

La red del entorno de investigación bloqueó muchas webs oficiales (ver `docs/production/ENVIRONMENT_AUDIT.md`). Cada ADR indica el nivel de las fuentes en las que se apoya:

- **[V]** leído en fuente primaria: documentación oficial, repositorio oficial o código fuente.
- **[S]** extracto de una fuente oficial visto a través de un buscador.
- **[S2]** fuente secundaria.
- **[NV]** no verificado.
- **[I]/[P]** inferencia o propuesta propia.
