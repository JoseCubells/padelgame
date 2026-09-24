---
name: 2p5d-character-pipeline
description: Technical pipeline for 2D pixel-art characters inside the 3D court (ADR-003/005) - quad billboard (cylindrical + camera-pitch tilt, feet pivot, depth bias), direction selection with hysteresis (8 locomotion / 5 shot directions, mirroring with separate racket layer), palette-ramp shader with alpha clip + ShadowCaster + DepthOnly, blob + light-facing shadows, sim-driven SpriteAnimationPlayer. Use when implementing or debugging character rendering, sprites in 3D, shadows, sorting against net/glass.
---

# Pipeline de personajes 2.5D

Decisiones en ADR-003 (renderer) y ADR-005 (animación); especificación en `docs/art/CHARACTER_BIBLE.md`. Hechos verificados en código de URP 17.3 (`docs/research/SOURCE_VERIFICATION.md`):

- `Sprite-Lit-Default` va en la cola **Transparent**, con **ZWrite 0**, **sin pase ShadowCaster** y **sin luz 3D** en el Universal Renderer.
- **Pixel Perfect Camera no sirve**: es ortográfica y exclusiva del 2D Renderer.
- El inspector de SpriteRenderer no tiene opciones de sombra.

## Implementación por defecto (hipótesis B, hasta que decida el spike)

1. **`PixelSpriteRenderer`**: `MeshRenderer` sobre un quad con pivote en los pies. Lee `Sprite.textureRect`, `pivot` y UV del frame actual y los pasa por `MaterialPropertyBlock`.
2. **Billboard**: rotación sobre el eje Y hacia la cámara, más una inclinación en X igual al pitch de la cámara, con pivote en la base. Tiene un *depth bias* configurable (`VisualProfile`).
3. **Dirección**: `atan2` en XZ del ángulo entre el *facing* y el vector cámara→personaje, cuantizado a N sectores con **histéresis de ±6°**. **Durante un golpe la dirección queda bloqueada.**
4. **Espejo**: solo en clips simétricos o con la **pala en una capa aparte** que no se espeja. Un diestro nunca debe verse zurdo.
5. **Shader** (Shader Graph Unlit o HLSL):
   - rampa de paleta de 3–5 tonos;
   - atenuación de sombra recibida cuantizada a 2 niveles;
   - rim;
   - Alpha Clipping;
   - pases `ShadowCaster` y `DepthOnly`.

   Si se escribe un Renderer Feature, validarlo con la skill oficial `validate-urp-render-graph-renderer-feature`.
6. **Sombras**: **blob siempre** (contacto con el suelo). Además, sombra real de la luz principal; hay que probar a orientar el quad hacia la luz en el pase ShadowCaster (hipótesis sin fuente primaria que el spike debe validar).
7. **Animación**: `SpriteAnimationPlayer` elige el frame a partir de la fase y el progreso que dicta la simulación. `contactFrame` = tick de contacto.

## Spike de la semana 1 (ADR-003)

Construir A' (SpriteRenderer con Shader Graph 3D) y B con el mismo sprite `TEMP_`. Medir:

- luz;
- sombra con luz rasante;
- orden frente a la red y el cristal;
- espejo con la pala;
- integración con el importer;
- draw calls (Profiler).

Registrar el resultado en ADR-003.

## Checklist visual

`visual-quality-review` + ART_BIBLE A.7. Se rechaza si el personaje flota, si los pies atraviesan el suelo, si la sombra es incorrecta, si hay parpadeo de dirección o si el píxel se ve borroso.
