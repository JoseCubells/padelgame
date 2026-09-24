---
name: padel-research
description: Research-and-decide protocol for padelgame. Use BEFORE choosing any technology, package, architecture, physics/rendering/animation/AI/input/networking approach, tool, or production method, and whenever writing or updating an ADR in docs/decisions/. Enforces primary-source research, alternative comparison, evidence levels and ADR format.
---

# Protocolo de investigación y decisión

No se toma ninguna decisión importante por intuición (CLAUDE.md, regla 1).

## Pasos

1. **Formular la pregunta**, concreta y verificable.
2. **Revisar lo ya decidido:** `docs/decisions/README.md` y los ADR relacionados. Si ya hay un ADR vigente, no se vuelve a discutir salvo que haya evidencia nueva.
3. **Investigar siguiendo esta jerarquía de fuentes:**
   1. documentación oficial;
   2. release notes;
   3. repositorios oficiales;
   4. documentación técnica primaria;
   5. papers;
   6. fuentes especializadas;
   7. comunidad.

   Una decisión crítica nunca se apoya solo en comunidad.
   - Unity: docs, release notes, GitHub de Unity-Technologies (código fuente de paquetes, `package.json`, tags de `UnityCsReference`) y skills oficiales en `.claude/skills/`.
   - Pádel: FIP (padelfip.com), reglamento vigente, federaciones nacionales.
   - Herramientas: web oficial, GitHub oficial (releases y LICENSE).
4. **Anotar la fecha de cada fuente** y comprobar que es actual. Verificar la compatibilidad **con Unity 6000.3.x**.
5. **Si la web oficial está bloqueada** (ocurre en el contenedor cloud, ver ENVIRONMENT_AUDIT): usar el código fuente o el repositorio oficial en GitHub (`git clone --depth 1 --filter=blob:none --sparse`) y documentarlo en `docs/research/SOURCE_VERIFICATION.md`. No inventar lo que no se pudo leer.
6. **Comparar al menos 2 alternativas:** ventajas, desventajas, compatibilidad, coste, licencia, mantenimiento y riesgo.
7. **Escribir o actualizar el ADR** con la plantilla de `docs/decisions/README.md`. Secciones obligatorias:
   - pregunta, fecha, fuentes, alternativas, ventajas y desventajas, compatibilidad, coste, riesgos;
   - decisión, motivo, consecuencias, validación pendiente, cómo revertirla, historial.
8. **Hacer commit** del ADR antes de implementar.

## Niveles de evidencia (obligatorio marcarlos)

| Nivel | Significado |
|---|---|
| `[V]` | Leído en fuente primaria |
| `[S]` | Extracto oficial visto vía buscador |
| `[S2]` | Secundaria |
| `[NV]` | No verificado |
| `[I]` / `[P]` | Inferencia o propuesta propia |

## Frases prohibidas

"Es la mejor opción" (sin evidencia), "normalmente se hace así", "es común", "debería funcionar", "supongo", "probablemente". Si falta evidencia, se escribe: **"No hay evidencia suficiente: <qué falta> — <cómo cerrarlo>"**.

## Cuándo preguntar al propietario

Solo en estos casos:
- cambio de alcance;
- gasto o contratación de un servicio;
- dos alternativas válidas que cambian la experiencia;
- información imprescindible que no se puede inferir.
