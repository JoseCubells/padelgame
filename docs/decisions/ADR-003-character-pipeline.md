# ADR-003 — Pipeline técnico de personajes 2.5D

- **Estado:** Aceptado (provisional). La hipótesis por defecto se confirma o se cambia mediante el **spike de la semana 1** de la Fase 1.
- **Fecha de investigación:** 2026-09-24
- **Decisores:** director técnico, con los informes del technical artist y el especialista en pixel art
- **Depende de / afecta a:** ADR-002, ADR-005, CHARACTER_BIBLE, `pixel-art-pipeline` y `2p5d-character-pipeline` (skills)

## Pregunta

¿Cómo existe técnicamente un personaje pixel art 2D dentro del espacio 3D? Abarca: billboard, orientación a cámara, selección de dirección, profundidad y ordenación, sombras, contacto con el suelo, recepción de luz y separación visual respecto a la pista. ¿Y cuántas direcciones dibujamos?

## Fuentes consultadas

[`VISUAL_RESEARCH.md` §B.4–B.8 y §E](../research/VISUAL_RESEARCH.md), [`TECHNOLOGY_RESEARCH.md` §4.3](../research/TECHNOLOGY_RESEARCH.md), [`SOURCE_VERIFICATION.md` §3](../research/SOURCE_VERIFICATION.md), [`TOOLS_RESEARCH.md` §1.3](../research/TOOLS_RESEARCH.md).

- **[V]** `Sprite-Lit-Default` (URP 17.3): cola Transparent, `ZWrite` 0 por defecto, sin pase `ShadowCaster`, sin iluminación en el pase `UniversalForward`.
- **[V]** Subtarget de Shader Graph "Sprite Lit": su pase `UniversalForward` no ilumina.
- **[V]** El inspector de `SpriteRenderer` (6000.3.24f1, 6000.6.2f1, 6000.7.0b1) no expone opciones de sombra. La API `Renderer.shadowCastingMode` y `receiveShadows` sí existe.
- **[S]/[S2]** Cult of the Lamb (inclinación con la cámara, escritura de profundidad), Doom (8 rotaciones, de ellas 5 únicas con espejo), Dead Cells (3D → pixel con normales).
- **[V]** Pixelorama 1.2.2+ exporta `.aseprite`. Paquete `com.unity.2d.aseprite`: importa tags como clips. **Compatibilidad real Pixelorama → importer: [NV]**.

## Alternativas evaluadas

| | A. SpriteRenderer + material por defecto | A'. SpriteRenderer + Shader Graph 3D propio | B. Quad (MeshRenderer) + material propio | C. 2D Renderer híbrido | D. Shader de sprite HLSL propio | E. Pipeline 3D low-poly → render a sprites |
|---|---|---|---|---|---|---|
| Luz 3D | No [V] | Sí (por verificar) | Sí | Mallas 3D sin iluminar [V] | Sí | Sí (horneada) |
| Sombra proyectada | No (sin ShadowCaster) [V] | Sí por script (por verificar) | Sí (alpha clip + ShadowCaster) | No | Sí | Sí |
| Profundidad | Transparente sin ZWrite [V] | Configurable | Opaco con alpha clip, ZWrite | 2D sorting | Configurable | Configurable |
| Animación | AnimationClip de sprites (importer) | Igual | Flipbook propio (UV del atlas) | Igual que A | Igual que B | Igual que B |
| Espejo (flipX) | Nativo | Nativo (por verificar con shader propio) | Escala negativa o UV | Nativo | En shader | En render |
| Billboard / inclinación | Script | Script | Script o shader | — | Shader | Shader |
| Riesgo técnico | — | **Medio:** compatibilidad de Shader Graph Lit 3D con SpriteRenderer (vertex color, flip, pivot) no verificada | **Bajo:** todas las piezas son 3D estándar | — | Medio (mantenimiento de HLSL frente a cambios de URP) | Contradice la regla del brief si el render es el arte final |
| Veredicto | Descartado | **Candidato del spike** | **Hipótesis por defecto** | Descartado | Solo si Shader Graph no basta | Solo como **referencia** para dibujar encima; nunca como arte final |

## Decisión

**Hipótesis por defecto: B + material propio (Shader Graph Unlit con rampa de paleta, Alpha Clipping, `ShadowCaster` y `DepthOnly`).** Se completa con:

1. **Datos de animación:** los frames se importan como `Sprite` de Unity (importer de Aseprite o spritesheet con JSON). Un componente propio `PixelSpriteRenderer` lee `sprite.textureRect`, `pivot` y UV y los aplica al quad mediante `MaterialPropertyBlock`. El formato de arte sigue siendo estándar.
2. **Billboard cilíndrico (eje Y) inclinado según el pitch de la cámara, con pivote en los pies** y *depth bias* configurable. Los pies nunca atraviesan el suelo y el sprite no se ve escorzado.
3. **Selección de dirección:** `atan2` del ángulo entre la orientación del personaje y el vector cámara→personaje en XZ, cuantizado a N sectores con **histéresis de ±6°**. **La dirección queda bloqueada durante un golpe.**
4. **Direcciones para el prototipo:**
   - **8 para locomoción**: 5 dibujadas y 3 espejadas;
   - **5 útiles según la cámara para golpes**;
   - la **pala en capa o sprite separado** que no se espeja, para no convertir a un diestro en zurdo;
   - **prueba ciega 4 frente a 8 frente a 16** en la Fase 2, que decide el número final.
5. **Sombras:** **blob siempre** (lectura de juego, contacto con el suelo) más sombra real proyectada por la luz principal. Se probará un **billboard de sombra orientado a la luz** en el pase ShadowCaster, sin fuente primaria: la hipótesis se valida en el spike.
6. **Luz:** rampa de paleta de 3–5 tonos, sombra recibida cuantizada a 2 niveles y *rim* configurable para separar la silueta de la pista. Todo en `VisualProfile` y en el material.
7. **Contorno** horneado en el sprite (selout: tono oscuro del propio color, no negro). No se hace contorno por shader en personajes.

### Spike de la semana 1 (criterio de decisión)

Se construyen **A'** y **B** con el mismo sprite de prueba (marcado `TEMP`) sobre la pista graybox. Se miden:

- iluminación coherente con la pista;
- sombra proyectada con luz rasante;
- ordenación contra red y cristal;
- flip sin romper la pala;
- coste de integración con el importer de Aseprite;
- draw calls y batching (Profiler).

Gana la opción que cumpla todo con menos código propio. Si A' cumple, se prefiere A' (menos código); si no, B. El resultado queda registrado en este ADR.

## Motivo

- Los límites de los sprites por defecto están verificados en código: no dan luz 3D, no proyectan sombra y son transparentes sin profundidad. Cualquier solución necesita un material propio.
- B usa solo piezas 3D estándar, con el riesgo técnico más bajo.
- A' puede ahorrar código, pero su compatibilidad no está verificada, y por eso el spike decide.
- La elección de direcciones sale de un cálculo de coste: 360, 600 o 1080 frames por personaje según 4, 8 o 16 direcciones con espejo. El problema del espejo en un deporte de lateralidad y la legibilidad en diagonal quedan como hipótesis validables.

## Compatibilidad

URP 17.3 y Shader Graph 17.3 (Unity 6000.3). El importer `com.unity.2d.aseprite` requiere comprobar qué versión resuelve el Package Manager para 6000.3: el informe de herramientas indica que la 7.0.0 exige 6000.7.

## Coste

Sin licencias. El spike lleva 2–3 días; el componente y el shader definitivos, 3–5 días [P].

## Riesgos

| Riesgo | Mitigación |
|---|---|
| El `.aseprite` de Pixelorama no importa bien | Plan B verificado en el informe de herramientas: PNG con celda fija + JSON, cortado por grid y generación de datos de animación por script de editor |
| La sombra de un billboard con luz rasante degenera en una línea | Billboard de sombra orientado a la luz (spike) con el blob como garantía |
| La inclinación del quad hace que intersecte con la red o el cristal cercanos | Depth bias o profundidad plana por sprite ("depth de los pies") |
| Batching pobre con `MaterialPropertyBlock` | Medir. Con 4 personajes el impacto es despreciable; se revisa en la Fase 10 |

## Consecuencias

- La skill de proyecto `2p5d-character-pipeline` documenta este pipeline.
- El número de frames del Animation MVP se calcula con 8/5 direcciones (ADR-005).
- Todo sprite de prueba lleva el prefijo `TEMP_` y la marca REPLACE BEFORE ART LOCK.

## Cómo revertirla

`PixelSpriteRenderer` es la única pieza que conoce el tipo de renderer. Cambiar entre A' y B consiste en sustituir ese componente y el material; la animación y los datos de sprites no cambian.

## Historial

- 2026-09-24: creado.
