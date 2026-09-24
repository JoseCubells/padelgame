# ADR-002 — Rendering

- **Estado:** Aceptado (provisional). La **resolución interna final y el modo de cámara se deciden por prueba** en la Fase 2.
- **Fecha de investigación:** 2026-09-24
- **Decisores:** director técnico, con el informe del technical artist
- **Depende de / afecta a:** ADR-001, ADR-003, ADR-009 (cámara), ART_BIBLE

## Pregunta

¿Cómo renderizamos un mundo 3D (pista, cristal, metal, luz) y personajes pixel art 2D para que ambos pertenezcan al mismo lenguaje visual, sean nítidos y estables con una cámara en tercera persona y permitan leer la pelota?

## Fuentes consultadas

Detalle en [`VISUAL_RESEARCH.md` §B–C](../research/VISUAL_RESEARCH.md), [`TECHNOLOGY_RESEARCH.md` §4](../research/TECHNOLOGY_RESEARCH.md) y [`SOURCE_VERIFICATION.md`](../research/SOURCE_VERIFICATION.md).

- **[V]** URP 17.3, `PixelPerfectCamera.cs`: fija `orthographicSize` y su modelo es ortográfico. La ruta de upscale solo existe en el 2D Renderer; `UniversalRenderer.cs` no la referencia en 6000.0, 6000.2 ni master.
- **[V]** URP 17.3: bajo el 2D Renderer, el shader Lit 3D estándar dibuja las mallas sin iluminar, lo que descarta el 2D Renderer para una pista iluminada.
- **[V]** URP 17 (6000.0): `UpscalingFilterSelection` incluye `Point` ("Nearest-Neighbor"), y Render Scale está limitado a 0.1–2.0.
- **[V]** URP 17.3: Render Graph es la API de passes; el "Compatibility Mode" está deprecado.
- **[V]** Documentación de UPixelator (GitHub): la estabilización por snap de cámara solo funciona en ortográfica.
- **[S]** Paper "Texel Splatting: Perspective-Stable 3D Pixel Art" (arXiv 2603.14587, 2026): explica por qué el snap falla en perspectiva.
- **[S]/[S2]** Referencias de juegos: A Short Hike (render de baja resolución, sin AA), Cult of the Lamb (sprites que escriben profundidad, cámara con inclinación), Dead Cells (pipeline 3D → pixel), t3ssel8r (snap ortográfico, contornos por bordes).

## Alternativas evaluadas

| Alternativa | Qué es | Ventajas | Desventajas | Veredicto |
|---|---|---|---|---|
| URP + **2D Renderer** | Renderer para juegos 2D | Pixel Perfect Camera nativo, luces 2D | Las mallas 3D con Lit se ven sin iluminar [V]; luces 2D incompatibles con una pista 3D | **Descartado** |
| URP + **Universal Renderer** | Renderer 3D estándar | Luz y sombra 3D reales, Render Graph, Shader Graph | Sprites por defecto sin luz 3D ni sombra [V] → shader propio | **Elegido** |
| **Pixel Perfect Camera** | Componente 2D | Snap y upscale integrados | Solo ortográfica y solo 2D Renderer [V] | **Descartado** (tal como avisaba el brief) |
| Render Scale + filtro Point | Ajuste del URP Asset | Coste cero | Resolución relativa y variable con la ventana; mínimo 0.1 (no llega a 180p en 4K) [V] | Solo como prueba de look de un día |
| **RenderTexture de tamaño fijo + upscale entero** | Mundo y personajes a, por ejemplo, 480×270 con filtro point; blit con escala entera y letterbox | Tamaño de píxel artístico idéntico en toda pantalla; control total | Hay que gestionar input y raycast a través del RT, relación de aspecto y post a baja resolución | **Elegido como base** |
| Render Graph Renderer Feature propio | Pase que pixela capas concretas | Mezclar capas pixeladas y nativas | Más ingeniería y mantenimiento | Evolución si hace falta (Fase 6) |
| Texel Splatting | Técnica de investigación 2026 | Estabilidad en perspectiva | Experimental y costosa | **Descartado** para producción |
| MSAA / FXAA / TAA / STP / FSR en la pasada de baja resolución | Anti-aliasing / upscalers | Suavizado | Generan píxeles intermedios y rompen la paleta | **Desactivados** en la pasada de mundo |

### Sprites iluminados frente a no iluminados

| Enfoque | Veredicto |
|---|---|
| Lit + normal map por frame | Duplica el coste de arte por frame. Se descarta como base |
| **Unlit + rampa de paleta** (N·L o luz de zona cuantizada, más atenuación de sombra recibida a 2 niveles, más rim) | **Base elegida**: mantiene la paleta y da control artístico |
| Lit PBR estándar | Descartado: produce "materiales PBR genéricos sin dirección artística", algo que el brief prohíbe |

El mundo 3D usa el mismo principio: materiales estilizados (Shader Graph) con iluminación cuantizada o rampa y vertex colors, no PBR genérico.

## Compatibilidad

URP 17.3 / Shader Graph 17.3 en Unity 6000.3 [V]. Render Graph obligatorio para passes propios: se usa la skill oficial `validate-urp-render-graph-renderer-feature` para revisarlos.

## Coste

Sin licencias. Desarrollo estimado [P]:
- pipeline de RT + upscale: 1–2 días;
- shaders de rampa (personaje y mundo): 3–5 días;
- Renderer Feature opcional: 3–5 días.

## Riesgos

| Riesgo | Mitigación |
|---|---|
| Pixel crawl en perspectiva al mover la cámara | Cámara semifija con FOV estrecho (20–30°) y snap al plano focal (ADR-009). Prueba con ortográfica oblicua como alternativa |
| Pelota ilegible: 0,7–1,5 px en el campo lejano a tamaño físico (cálculo del informe visual) | Tamaño mínimo en píxeles, contorno, sombra blob siempre visible y color exclusivo. Es una "trampa" de legibilidad aceptada |
| Cristal transparente y ordenación con sprites | Tres opciones a probar (VISUAL_RESEARCH B.6). Preferencia inicial: cristal que no escribe profundidad, dibujado tras los opacos, con reflejo y suciedad pintados |
| UI borrosa | HUD a escala entera del píxel del mundo; texto largo y menús a resolución nativa |
| Relación de aspecto (16:10, ultrapanorámico) | Letterbox al entero inferior; se evalúa en Fase 2 |

## Decisión

1. **URP con Universal Renderer.** No se usa el 2D Renderer ni Pixel Perfect Camera.
2. **Mundo y personajes se renderizan a una RenderTexture de resolución fija** con filtro point y escala entera a pantalla, con letterbox.
3. **La resolución interna final se decide por prueba** en la Fase 2 (Visual Prototype):
   - candidata principal: **480×270**;
   - candidata de detalle: **640×360** (escala entera en 720p, 1080p, 1440p y 4K);
   - control de estilo extremo: **320×180**;
   - opcional: 384×216.

   Protocolo en `docs/qa/TEST_PLAN.md` §Visual: misma escena graybox × 3 cámaras × 3 estrategias de escala de sprite, con capturas y prueba ciega de lectura (golpe del jugador lejano y altura de la pelota).
4. **Sin anti-aliasing** en la pasada de mundo.
5. **Personajes:** shader propio unlit con rampa de paleta, alpha clip, `ShadowCaster` y `DepthOnly` (ADR-003).
6. **Mundo:** materiales estilizados propios (rampa o cuantizado, vertex color, decals propios) y contornos por shader solo en siluetas clave (red, marcos, líneas).
7. **Pelota:** billboard o esfera con tamaño mínimo en píxeles, contorno de 1 px, sombra blob siempre visible y color reservado.
8. **Post-proceso:** solo el que defina la ART_BIBLE (por ejemplo, gradación a paleta), implementado como pase propio con Render Graph. No se usa bloom "porque sí".

## Motivo

Es la única combinación que da luz y sombra 3D coherentes para la pista y los personajes, píxel estable e idéntico en cualquier pantalla y control artístico de paleta. Además, respeta los límites verificados en el código de URP 17.3. La prueba de resoluciones cumple el mandato del brief de decidir por evidencia y no por intuición.

## Consecuencias

- Se crea `Padel.Presentation.Rendering`: controlador de RT, blit y letterbox, conversión de input de pantalla a RT y perfiles `VisualProfile` (ScriptableObject con resolución, escala de sprite y parámetros de rampa).
- Las capturas de validación se toman de la RT a resolución interna **y** del frame final.
- `VisualProfile` permite cambiar la resolución interna en caliente para la prueba A/B.

## Validación pendiente

Fase 2:
- matriz de capturas y prueba ciega;
- estabilidad al mover la cámara;
- lectura de la pelota;
- rendimiento (60 FPS estables medidos con el Profiler).

Con esos resultados se actualiza este ADR a "Aceptado" con la resolución elegida.

## Cómo revertirla

Todo el pipeline cuelga de `VisualProfile` y de un único controlador de RT. Para volver a render nativo basta con desactivar la RT (render directo a pantalla). Los shaders de rampa siguen siendo válidos.

## Historial

- 2026-09-24: creado.
