# COURT BIBLE

- **Estado:** 🟡 La geometría (COURT_GRAYBOX) está vigente. El arte (COURT_ART_PASS) espera la dirección visual.
- **Versión:** 0.1
- **Fecha:** 2026-09-24
- **Fuentes:** [`PADEL_RULES_RESEARCH.md` §2 y §9](../research/PADEL_RULES_RESEARCH.md) (FIP 2026; extractos del PDF oficial, V1), ADR-010

## 1. Geometría oficial (CourtDefinition por defecto)

| Elemento | Valor | Fuente |
|---|---|---|
| Superficie de juego | 20,00 × 10,00 m (±0,5 %) | FIP 2026 (V1) |
| Pared de fondo | 3 m de pared o cristal + 1 m de malla = 4 m | FIP 2026 (V1) |
| Lateral, variante 1 (escalonada) | Tramo de 3 m × 2 m de cristal, tramo de 2 m × 2 m y malla hasta 3 m en los 6 m centrales | FIP 2026 (V1) |
| Lateral, variante 2 | 3 × 4 m de cristal en cada extremo y malla hasta 4 m en los 2 m exteriores | FIP 2026 (V1) |
| Rombo de la malla | Abertura de 5 a 7,08 cm | FIP 2026 (V1) |
| Red | 10 m de largo; 0,88 m en el centro y 0,92 m como máximo en los postes (±0,005); cinta blanca de 5 a 6,3 cm | FIP 2026 (V1) |
| Postes | ≤ 1,05 m | FIP 2026 (V1) |
| Líneas de saque | A 6,95 m de la red, con línea central | FIP 2026 (V1) |
| Líneas | 5 cm, blancas o negras | FIP 2026 (V1) |
| Puertas | Una por lado (de 1,05×2,00 a 2,20×2,20 m) o dos por lado (de 0,72×2,00 a 1,10×2,20 m), simétricas respecto a la red | FIP 2026 (V1) |
| Espacio exterior (juego fuera de pista) | ≥ 3 m de ancho (se recomiendan 4) × 4 m de largo × 3 m de alto | FIP 2026 (V1) |
| Iluminación | ≥ 6 m de altura; ≥ 1000 lux verticales para TV | FIP 2026 (V1) |
| Grosor del cristal, colores del suelo y altura libre del techo | **Laguna** (anexo de homologación no leído) | — |

**Decisión de diseño:** la variante 2 de lateral (cristal de 4 m) va por defecto, porque es la habitual en competición según PADEL_RULES_RESEARCH §11. La variante 1 queda como opción.

## 2. COURT_GRAYBOX (Fase 1)

- Generada **por código** a partir de `CourtDefinition`, con ProBuilder o mallas procedurales. Garantiza proporciones exactas y que la simulación y la vista usen los mismos datos.
- Materiales de graybox con código de color por superficie: suelo, cristal, malla, red, líneas y zona exterior. Todos llevan el prefijo `GRAYBOX_`.
- Un validador de editor compara la malla con `CourtDefinition` (dimensiones ±1 mm).

## 3. COURT_ART_PASS (Fase 6; después de elegir la dirección)

Requisitos del brief §8, que se concretan según la dirección elegida:

- **Estructura propia:** perfiles metálicos con uniones y tornillería legible a la densidad de píxel del juego.
- **Cristal:** marco, reflejos pintados y suciedad o marcas. Si se adopta el rasgo D5, marcas persistentes de pelota. El tratamiento de transparencia y ordenación se decide en la Fase 2 (ADR-002, riesgos).
- **Malla:** un patrón de rombo que no produzca moiré a la resolución interna. Esto requiere prueba: probablemente una textura pintada a la densidad del píxel, no geometría real.
- **Suelo:** césped o moqueta con variación de color propia, líneas como decal propio y desgaste en las zonas de saque.
- **Red y postes:** cinta, tensores y logo propio del club (**branding original**).
- **Iluminación:** mástiles de focos, que a la vez forman parte de la identidad nocturna si se elige D2.
- **Arquitectura del club:** muros, toldos, bancos, fuente de agua, marcador físico y vegetación.
- **Pequeños detalles:** botes de pelotas, toallas, bolsas de pala, una botella, hojas…
- **Herramienta:** Blender 5.2 LTS → FBX. Vertex colors y texturas pintadas a la densidad de píxel. **No se usan texturas de stock** (ADR-010).
- **Todo** asset externo se registra en ASSET_PROVENANCE. Objetivo: 0 externos en la pista.
