# TOOLS_RESEARCH — Herramientas y pipeline de producción

- **Fecha de la investigación:** 2026-09-24
- **Autor:** Tools & Pipeline researcher (agente)
- **Ámbito:** juego de pádel en Unity, personajes 2D pixel art en mundo 3D.
- **Restricciones de presupuesto:** preferencia por herramientas gratuitas/open source. **Aseprite NO debe ser requisito** si hay alternativa gratuita adecuada.

## 0. Cómo leer este documento (nivel de evidencia)

Durante la sesión, el proxy de red **bloqueó** la mayoría de webs oficiales (aseprite.org, pixelorama.org, docs.unity3d.com, unity.com, blender.org, docs.krita.org, audacityteam.org, freesound.org, creativecommons.org, kenney.nl, sonniss.com, gnu.org, openfontlicense.org, codeberg.org, itch.io, Steam, Wikipedia), y el presupuesto de búsqueda web se agotó a mitad de la investigación. Sí eran accesibles `github.com` y `raw.githubusercontent.com` (repos oficiales, feeds `releases.atom`/`tags.atom`, ficheros LICENSE, espejos UPM de paquetes Unity). Por eso cada afirmación lleva una etiqueta:

| Etiqueta | Significado |
|---|---|
| **[V]** | Verificado en esta sesión leyendo la fuente primaria enlazada (repo oficial, LICENSE, feed de releases con timestamp ISO, fichero de docs en GitHub). |
| **[S]** | Obtenido de resultados del buscador para la URL oficial citada (fragmento indexado), sin poder abrir la página. Fiable pero pendiente de lectura directa. |
| **[NV]** | No verificable en esta sesión (dominio bloqueado). Conocimiento previo del investigador; **debe verificarse** antes de tomar decisiones contractuales. Todas aparecen también en "Lagunas de evidencia". |

Las fechas de release proceden de los timestamps ISO de los feeds Atom de GitHub (`/releases.atom`, `/tags.atom`), que son más fiables que la fecha relativa mostrada en la web.

---

## 1. Herramientas de pixel art

### 1.1 Tabla de candidatos

| Herramienta | Versión | Fecha release | Licencia | Coste | Animación | Export | Automatización | Integración Unity | Mantenimiento | Decisión |
|---|---|---|---|---|---|---|---|---|---|---|
| **Pixelorama** | 1.2.3 [V] | 2026-09-15 [V] | MIT [V] | Gratis | Timeline capas/frames, onion skin, tags, keyframes (opacidad/efectos), capas 3D [V] | PNG, APNG, GIF, vídeo, spritesheet (por filas/columnas/tags), JSON de proyecto, SVG (1.2.1), **.aseprite (desde 1.2.2)** [V] | CLI de exportación (desde 1.0): `--export`, `--spritesheet`, `--json`, `--split-layers`, `--scale`, `--frames`, `--direction`, `--output` [V/S] | Vía .aseprite → Aseprite Importer (requiere spike de compatibilidad) o PNG+JSON | Muy activo: 4 releases en jul–sep 2026 [V] | **PRIMARIA** |
| **Krita** | 6.0.4 (Qt6) [V]; línea 5.3.x (Qt5) [S] | 2026-09-10 (tag v6.0.4) [V] | GPL-3.0 [V] | Gratis | Timeline, onion skin, render de animación a secuencia/vídeo [S] | PNG, secuencias, PSD, vídeo (FFmpeg incluido desde 5.2) [S]; **sin spritesheet nativo** (plugins de terceros) [S] | Python (pykrita, plugin `batch_exporter` incluido) [V]; CLI `--export`, `--export-sequence`, `--export-filename` [V] | PNG / secuencias | Activo [V] | **SECUNDARIA** (concept art, fondos, UI pintada, key art) |
| LibreSprite | v1.2 [V] | 2025-03-06 [V] | GPL-2.0 [V] | Gratis | Heredada de Aseprite 1.1 (timeline, onion skin) [NV] | .ase/.aseprite nativo, spritesheet [NV] | JS/Lua scripting (v1.0) [V] | .ase → Aseprite Importer (formato heredado, sin tilemaps/UUID) [NV] | Commits activos sep 2026 ("Flatten layer groups when loading .ase files", 2026-09-16) [V]; releases poco frecuentes | Alternativa si se necesita .ase nativo gratis |
| Aseprite | v1.3.18.6 [V] | 2026-09-22 [V] | EULA propietaria, código fuente disponible [V] | 19,99 USD (Steam/itch) [S] | Referencia del sector [NV] | `--sheet` + `--data` JSON (hash/array), `--split-tags`, `--split-layers`, `--sheet-type packed`, `--trim` [V] | `--batch` + Lua `--script` [V] | Importer oficial Unity nativo [V] | Muy activo [V] | **Opcional por artista, nunca requisito** |
| Piskel | v0.15.0 [V] | 2018-11-25 (última release) [V] | Apache-2.0 [V] | Gratis | Básica | PNG, GIF, spritesheet [NV] | No | PNG | Sin releases desde 2018; commits de mantenimiento 2026-09-20 [V] | Descartada (limitada) |
| GrafX2 | v2.9 [V] | 2024-04-28 [V] | GPL-2.0 [V] | Gratis | Limitada, estilo Deluxe Paint [NV] | PNG/GIF [NV] | Lua [NV] | PNG | Poco activo | Descartada |
| Pixel Composer | 1.21.0 [V] | 2026-04-28 [V] | MIT (repo GitHub) [V]; versión de pago en Steam/itch [V] | Gratis (compilando) / pago (binarios) | Nodal/procedural (VFX) | PNG/secuencias [NV] | Nodos | PNG | Activo | Opcional para **VFX** (polvo, impactos), no para personajes |

### 1.2 Hallazgos

**Pixelorama**
- Última versión v1.2.3 (2026-09-15), v1.2.2 (2026-09-09), v1.2.1 (2026-08-19), v1.2 (2026-07-29) [V] — [releases.atom](https://github.com/Orama-Interactive/Pixelorama/releases.atom).
- Licencia MIT, "Copyright (c) 2019-present Orama Interactive and contributors" [V] — [LICENSE](https://github.com/Orama-Interactive/Pixelorama/blob/master/LICENSE).
- README: animación con "timeline composed of layers and frames, including features like onion skinning, audio synchronization, frame tags"; paletas prediseñadas/importadas/propias; export "PNG, animated PNG, spritesheets, GIFs and even videos"; capas 3D; CLI "for bulk exporting" [V] — [README](https://github.com/Orama-Interactive/Pixelorama).
- CHANGELOG [V] — [CHANGELOG.md](https://github.com/Orama-Interactive/Pixelorama/blob/master/CHANGELOG.md):
  - v1.0: "A basic Command Line Interface has been implemented" y "Exporting the project's data to a separate JSON file is now possible".
  - v1.1: "Importing OpenRaster (.ora) and Aseprite (.ase/.aseprite) files is now possible."
  - v1.1.5: import de Krita `.kra` con animación, `.psd` con animaciones, paletas desde .ase.
  - v1.2: "The 3D layer system has been completely re-written!"; construido con Godot 4.6.3; keyframes para opacidad/efectos; animar propiedades de objetos 3D [V] — [release v1.2](https://github.com/Orama-Interactive/Pixelorama/releases/tag/v1.2).
  - v1.2.2: "It is now possible to export projects as Aseprite files. [#1603]" [V] — [release v1.2.2](https://github.com/Orama-Interactive/Pixelorama/releases/tag/v1.2.2).
- Opciones CLI (`--export/-e`, `--spritesheet/-s`, `--output/-o`, `--scale`, `--frames/-f`, `--direction/-d`, `--json`, `--split-layers`) [S] — [docs CLI](https://pixelorama.org/user_manual/cli/). Diseño: los argumentos van tras el separador `--` para no chocar con los de Godot; formato deducido de la extensión del output [V] — [Discussion #579](https://github.com/Orama-Interactive/Pixelorama/discussions/579).
- **Headless:** en la discusión #579 el modo sin ventana (Godot server/headless) quedó como "potencial"; no hay confirmación documentada [V]. → Laguna: probar `--headless` de Godot 4 en CI.
- **JSON de Pixelorama:** contiene los datos del proyecto [S] ([docs Save & Export](https://pixelorama.org/user_manual/save_and_export/)); **no consta que sea compatible con el JSON de Aseprite** → no asumir que los importadores de JSON-Aseprite lo lean.

**Krita**
- Tags v6.0.4 (2026-09-10), v6.0.3 (2026-07-26), v6.0.0 (2026-03-19) [V] — [tags.atom](https://github.com/KDE/krita/tags.atom). Krita 6.0 = port a Qt6, publicado junto a 5.3 (Qt5) [S] — [Krita 5.3.0 released](https://krita.org/en/posts/2026/krita-5.3.0-released/).
- "Krita as a whole is licensed under the GNU Public License, Version 3" [V] — [README](https://github.com/KDE/krita).
- CLI: `--export` "Export to the given filename and exit", `--export-sequence` "Export animation to the given filename and exit", `--export-filename` [V] — [KisApplicationArguments.cpp](https://github.com/KDE/krita/blob/master/libs/ui/KisApplicationArguments.cpp).
- Plugins Python incluidos: `batch_exporter`, `exportlayers`, `scripter`, etc.; **ninguno de spritesheet** [V] — [plugins/python](https://github.com/KDE/krita/tree/master/plugins/python). El export a spritesheet requiere plugins de terceros ([kritaSpritesheetManager](https://github.com/Falano/kritaSpritesheetManager), [krita-spritesheet-generator](https://github.com/ShannonHG/krita-spritesheet-generator)) [S].
- Krita 5.3/6.0 añade "Python Painting API" y `pykrita.qt_major_version()` [S] — [Phoronix](https://www.phoronix.com/news/Krita-6.0-Released).

**LibreSprite** — releases v1.2 (2025-03-06), 1.1 (2024-09-14), 1.0 (2023-05-25) [V] ([releases.atom](https://github.com/LibreSprite/LibreSprite/releases.atom)); licencia GPL-2.0 [V] ([LICENSE.txt](https://github.com/LibreSprite/LibreSprite/blob/master/LICENSE.txt)); commits recientes 2026-09-14..18 (CI, instalador Windows, carga .ase) [V] ([commits.atom](https://github.com/LibreSprite/LibreSprite/commits/master.atom)). Mantenido pero con cadencia de releases lenta.

**Aseprite**
- v1.3.18.6 (2026-09-22), v1.3.18 (2026-07-23) [V] — [releases.atom](https://github.com/aseprite/aseprite/releases.atom).
- Licencia: "Source code and official releases/binaries are distributed under our End-User License Agreement for Aseprite (EULA)"; algunos módulos (laf, clip, undo) MIT [V] — [README](https://github.com/aseprite/aseprite).
- EULA: permite "compile and modify the source code of the SOFTWARE PRODUCT for your own personal purpose or to propose a contribution"; prohíbe "distribute copies of the SOFTWARE PRODUCT to third parties" [V] — [EULA.txt](https://github.com/aseprite/aseprite/blob/main/EULA.txt). → Compilarlo uno mismo es legal para **uso personal**; **no** se puede compilar una vez y repartir el binario al equipo. El EULA no reclama derechos sobre el arte producido [V].
- Precio 19,99 USD en Steam e itch.io [S] — [itch.io purchase](https://dacap.itch.io/aseprite/purchase).
- CLI: `--batch` ("Runs Aseprite only to process command line options"), `--sheet`, `--data` (JSON), `--format json-hash|json-array`, `--sheet-type horizontal|vertical|rows|columns|packed`, `--split-layers`, `--split-tags`, `--list-tags`, `--tag`, `--trim`, `--script` (Lua) [V] — [aseprite/docs cli.md](https://github.com/aseprite/docs/blob/main/cli.md).

**Piskel / GrafX2 / Pixel Composer** — ver tabla; fuentes: [Piskel releases.atom](https://github.com/piskelapp/piskel/releases.atom), [Piskel LICENSE](https://github.com/piskelapp/piskel/blob/master/LICENSE), [GrafX2 tags](https://gitlab.com/GrafX2/grafX2/-/tags?format=atom), [GrafX2 LICENSE](https://gitlab.com/GrafX2/grafX2/-/raw/master/LICENSE), [Pixel Composer LICENSE](https://github.com/Ttanasart-pt/Pixel-Composer/blob/main/LICENSE) ("Copyright (c) 2023 Tanasart"), [Pixel Composer releases.atom](https://github.com/Ttanasart-pt/Pixel-Composer/releases.atom) [V].

### 1.3 Integración con Unity

**Unity 2D Aseprite Importer (`com.unity.2d.aseprite`)** — fuente: espejo UPM en GitHub [needle-mirror/com.unity.2d.aseprite](https://github.com/needle-mirror/com.unity.2d.aseprite) (espejo no afiliado de los paquetes publicados por Unity).
- Versiones: 7.0.0 (2026-09-02), 6.0.0 (2026-05-19), 5.0.2 (2026-02-17) [V] — [CHANGELOG](https://github.com/needle-mirror/com.unity.2d.aseprite/blob/master/CHANGELOG.md). La 7.0.0 declara `"unity": "6000.7"` (alpha) [V] — [package.json](https://github.com/needle-mirror/com.unity.2d.aseprite/blob/master/package.json). La versión mayor del paquete va ligada a la versión del Editor; el Package Manager ofrece la adecuada (p. ej. 3.0.x para Unity 6000.3 [S], [manual 6000.3](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.2d.aseprite.html)).
- Importa `.ase/.aseprite`. Modos: **Sprite Sheet**, **Animated Sprite** (por defecto; genera prefab + Animation Clips + Animator), **Tile Set** [V] — [ImporterFeatures.md](https://github.com/needle-mirror/com.unity.2d.aseprite/blob/master/Documentation~/ImporterFeatures.md).
- "Every tag in Aseprite generates one Animation Clip"; tiempos por frame; loop según el campo Repeat del tag; solo dirección Forward [V] — [AsepriteFeatures.md](https://github.com/needle-mirror/com.unity.2d.aseprite/blob/master/Documentation~/AsepriteFeatures.md).
- Defaults de textura: PPU 100, Filter Mode Point, Compression None, Mesh Type Tight, pivote Bottom [V] (ImporterFeatures.md).
- Modos de color RGBA, Grayscale e Indexed soportados; **Slices no soportados** [V] (AsepriteFeatures.md).
- Eventos de animación desde user data de cel (`event:Nombre`); UUID de capa (Aseprite ≥ v1.3.14-beta1) para no perder datos al renombrar capas [V] — [ImporterFAQ.md](https://github.com/needle-mirror/com.unity.2d.aseprite/blob/master/Documentation~/ImporterFAQ.md).
- **¿Acepta ficheros de Pixelorama?** Pixelorama exporta `.aseprite` desde 1.2.2 (2026-09-09) [V]. **No existe evidencia pública** de que esos ficheros se hayan probado con el importer de Unity. Riesgos concretos: el importer usa campos que quizá Pixelorama no escribe (UUID de capa, user data, z-index). → **Spike obligatorio** antes de fijar el pipeline (ver §7).

**Unity Sprite Editor (PNG + JSON)** — el Sprite Editor permite cortar spritesheets por Grid By Cell Size, Grid By Cell Count o Automatic [NV] ([manual Sprite Editor](https://docs.unity3d.com/Manual/sprite/sprite-editor/use-editor.html)). Unity **no lee de forma nativa** un JSON de spritesheet externo [NV]; haría falta un script de editor (`AssetPostprocessor`/`ScriptedImporter` + `ISpriteEditorDataProvider`) o un importer de terceros. Con celdas de tamaño fijo, el corte por rejilla es suficiente y trivial de automatizar.

**Unity 2D PSD Importer (`com.unity.2d.psdimporter`)** — última 15.0.1 (2026-08-25) [S] ([changelog 15.0](https://docs.unity3d.com/Packages/com.unity.2d.psdimporter@15.0/changelog/CHANGELOG.html)); importa `.psb` generando prefab de sprites por capa, pensado para personajes con rig 2D (2D Animation) [S]. **No aporta** a un pipeline de pixel art frame-a-frame; descartado.

### 1.4 Recomendación (pixel art)

1. **Primaria: Pixelorama 1.2.3 (MIT, gratis).** Fichero fuente canónico: `.pxo`. Cubre timeline, onion skin, paletas, tags, capas 3D (útiles como referencia de perspectiva), CLI e import/export .aseprite.
2. **Secundaria: Krita 6.0.x (GPL-3.0, gratis)** para concept art, key art, fondos pintados y UI ilustrada; no para sprites animados de juego.
3. **Aseprite:** permitido como preferencia personal de un artista (licencia individual 19,99 USD), pero el pipeline **no depende de él**: el formato de intercambio `.aseprite` lo produce Pixelorama.
4. **Formato de export a Unity (dos vías, decidir tras spike):**
   - **Vía A (preferida si el spike pasa):** `.pxo` → export `.aseprite` → `Assets/` → Aseprite Importer (Animated Sprite, tags = clips, Point, sin compresión).
   - **Vía B (fallback robusto):** `.pxo` → CLI → spritesheet PNG con **celda fija** (una fila por tag) + JSON → corte por rejilla en Unity + script de editor que crea los AnimationClips a partir del JSON.
5. **Automatización:** script Python (`tools/export_sprites.py`) que recorre `art_src/**/*.pxo`, llama a la CLI de Pixelorama por fichero, valida (tamaño de celda, paleta permitida, nº de frames por tag) y copia al destino en `Assets/`. Empaquetado adicional (atlas) lo hace Unity con **Sprite Atlas**, no el script. Para validación de paleta/tamaños se puede usar Pillow [NV licencia; verificar].

---

## 2. Blender y 3D → Unity

### 2.1 Tabla de candidatos

| Herramienta | Versión | Fecha | Licencia | Coste | Export | Automatización | Integración Unity | Mantenimiento | Decisión |
|---|---|---|---|---|---|---|---|---|---|
| **Blender** (actual) | 5.2.2 [V] | 2026-09-14 [V] | GPL (código) [V] | Gratis | FBX (addon io_scene_fbx 5.15.0), glTF 2.0 (.glb/.gltf) [V] | `blender -b -P script.py -- args` [V] | FBX nativo; glTF vía glTFast; .blend directo (requiere Blender) | Muy activo | **Elegida** |
| Blender LTS | **5.2 LTS** (5.2.2) [V/S]; 4.5 LTS (4.5.14) sigue recibiendo parches [V] | 5.2.0: 2026-07-13 (tag) [V]; anuncio 2026-07-14, soporte hasta jul-2028 [S] | GPL | Gratis | — | — | — | — | **Fijar 5.2 LTS para todo el equipo** |
| Unity FBX importer | integrado | — | parte de Unity | — | — | — | Nativo, Model Import Settings completos [NV] | — | **Formato primario** |
| glTFast (`com.unity.cloud.gltfast`) | 6.20.0 estable (2026-08-25); 7.0.0-exp.1 (2026-08-28) [V] | ver izq. | Paquete oficial Unity [V] | Gratis | Import/export glTF runtime y Editor [V] | — | Requiere Unity 6.0 LTS+ desde 6.19.0 [V] | Activo | Alternativa/secundaria |

### 2.2 Hallazgos

- Tags Blender: v5.2.2 y v4.5.14 (2026-09-14), v5.2.1 (2026-08-24), v5.2.0 (2026-07-13), v4.2.23 (2026-07-20) [V] — [tags.atom](https://github.com/blender/blender/tags.atom). 5.2 es LTS y se mantendrá hasta julio 2028 [S] — [Blender 5.2 LTS release](https://www.blender.org/press/blender-5-2-lts-release/), [release notes 5.2](https://developer.blender.org/docs/release_notes/5.2/).
- Licencia: "Blender uses the GNU General Public License" y "Apart from the GNU GPL, Blender is not available under other licenses" [V] — [COPYING](https://github.com/blender/blender/blob/main/COPYING); "Blender can be used freely for any purpose, including commercial use and distribution" [V] — [readme.html v5.2.2](https://github.com/blender/blender/blob/v5.2.2/release/text/readme.html). La GPL no se extiende a los modelos/renders que produzcas (son tuyos) [NV] — [blender.org/about/license](https://www.blender.org/about/license/). Excepción práctica: scripts Python que importan `bpy` y se distribuyen son obra derivada GPL (irrelevante mientras sean herramientas internas no distribuidas) [NV].
- **Exportador FBX** (addon 5.15.0) [V] — [io_scene_fbx/__init__.py](https://github.com/blender/blender/blob/main/scripts/addons_core/io_scene_fbx/__init__.py):
  - `axis_forward='-Z'`, `axis_up='Y'` por defecto.
  - `apply_scale_options`: All Local / **FBX Units Scale** / FBX Custom Scale / FBX All.
  - `bake_space_transform` ("Apply Transform"): "experimental option, use at own risk, known to be broken with armatures/animations".
  - `colors_type` ("Vertex Colors"): None / **sRGB** (default) / Linear; `prioritize_active_color` porque "some other software can discard other color attributes besides the first one".
  - `use_mesh_modifiers` True, `add_leaf_bones` True (desactivar para Unity), `bake_anim` True, `embed_textures` False.
- **Exportador glTF** (addon 5.3.32 en main) [V] — [io_scene_gltf2/__init__.py](https://github.com/blender/blender/blob/main/scripts/addons_core/io_scene_gltf2/__init__.py): `export_yup` "+Y up" True; `export_vertex_color` Material/Active/Name/None (default Material); `export_all_vertex_colors`; formatos GLB / glTF Separate / glTF Embedded.
- **glTFast**: import y export glTF en runtime y Editor, todas las render pipelines, Draco/KTX/meshopt; Editor Export "experimental" [V] — [Documentation~/index.md](https://github.com/needle-mirror/com.unity.cloud.gltfast/blob/master/Documentation~/index.md); versiones y requisito Unity 6.0 LTS [V] — [CHANGELOG](https://github.com/needle-mirror/com.unity.cloud.gltfast/blob/master/CHANGELOG.md); 7.0.0-exp.1 rompe API (renombra ensamblados, System.Text.Json) y aplica alpha de vertex color en shaders [V].
- **.blend directo en Unity**: Unity invoca Blender instalado para convertir a FBX; si Blender no está instalado (o la versión da problemas) la importación falla [S] — [Unity manual (antiguo)](https://docs.unity3d.com/540/Documentation/Manual/HOWTO-ImportObjectBlender.html), [issue tracker](https://issuetracker.unity3d.com/issues/blender-could-not-convert-the-blend-file-to-fbx-file-error-is-thrown-when-blend-file-is-imported). El `.gitignore` oficial de Unity ya ignora `*.blend1` [V] — [Unity.gitignore](https://github.com/github/gitignore/blob/main/Unity.gitignore).
  - Pros: iteración inmediata. Contras: cada máquina/CI necesita la misma versión de Blender; import lento; reproducibilidad pobre; se cuelan objetos auxiliares (cámaras, luces, referencias). → **No usar .blend dentro de `Assets/`.**
- **Headless**: el `--` final pasa argumentos a Python ("End option processing, following arguments passed unchanged. Access via Python's 'sys.argv'"); existen `--python-exit-code` y `--factory-startup` [V] — [creator_args.cc](https://github.com/blender/blender/blob/main/source/creator/creator_args.cc). Uso: `blender -b --factory-startup archivo.blend -P tools/export_fbx.py --python-exit-code 1 -- --out Assets/Art/3D/...`.
- **Convenciones de escala/ejes** [NV salvo defaults del exportador]: Blender 1 BU = 1 m y Z-up; Unity 1 unidad = 1 m y Y-up. Receta habitual: Unit Scale 1.0 en Blender, exportar FBX con `apply_scale_options='FBX_SCALE_UNITS'`, forward -Z / up Y, "Apply Transform" desactivado; en Unity activar **Bake Axis Conversion** en el Model Importer para evitar la rotación -90° en X [NV] ([Unity manual Model tab](https://docs.unity3d.com/Manual/FBXImporter-Model.html)). Pista de pádel real = 20 × 10 m: modelar a escala 1:1.
- **Vertex colours**: FBX exporta color attributes (sRGB por defecto) [V]; Unity los importa en la malla, pero **los shaders Lit de URP no los usan**; hace falta un Shader Graph con nodo Vertex Color [NV].

### 2.3 "Render 3D → sprites" como pipeline de personajes

- **Regla del proyecto:** los personajes deben ser **pixel art intencional**, no renders reducidos. Por tanto, un pipeline donde el sprite final sea el render (aunque sea a baja resolución sin antialiasing) **queda descartado** para personajes.
- **Dead Cells** (Motion Twin, 2018) usó modelos 3D animados renderizados a baja resolución sin antialiasing con shading "toon" para producir los sprites, con retoques puntuales a mano; la ventaja principal fue la velocidad de iteración de animación y el coste bajo de cambiar equipamiento [NV] — [Game Developer: Art Design Deep Dive, Dead Cells](https://www.gamedeveloper.com/production/art-design-deep-dive-using-a-3d-pipeline-for-2d-animation-in-i-dead-cells-i-).
- **Híbrido aceptable (recomendado para poses difíciles):** 3D solo como **referencia** (rotoscopia): maniquí de jugador de pádel + pala en Blender (o capa 3D de Pixelorama), render de siluetas/guías a la resolución objetivo → capa de referencia bloqueada en Pixelorama → el artista **redibuja cada frame a mano** → la capa de referencia se borra antes de exportar.
  - Pros: proporciones y escorzos consistentes (pala en perspectiva, remate, bandeja, víbora), coherencia con la cámara del mundo 3D, facilita 8 direcciones.
  - Contras: más lento que el render directo; riesgo de "look calcado" si el artista sigue la referencia demasiado literal; requiere mantener rig de referencia.
  - Control: el `.pxo` de entrega no debe contener capas de referencia; ASSET_PROVENANCE registra "ref 3D usada: sí/no".

### 2.4 Recomendación (3D)

- **Blender 5.2 LTS** fijado para todo el equipo.
- **FBX como formato primario** a Unity (importer nativo, sin dependencia de paquete, flujo de rig/animación maduro). glTF/glTFast queda como opción secundaria (p. ej. carga runtime de contenido o si el importer FBX da problemas con vertex colors).
- `.blend` fuente fuera de `Assets/` (carpeta `art_src/3d/`), en LFS; export automatizado con `blender -b`.

---

## 3. Audio

### 3.1 Tabla de candidatos

| Herramienta | Versión | Fecha | Licencia | Coste | Uso | Automatización | Integración Unity | Mantenimiento | Decisión |
|---|---|---|---|---|---|---|---|---|---|
| **Audacity** | 4.0.0 [V]; 3.7.9 (rama 3.x) [V] | 2026-09-03 / 2026-09-02 [V] | GPL-3.0 (muchos ficheros GPL-2.0-or-later) [V] | Gratis | Edición/limpieza/normalización de SFX y foley | Macros (en 4.0 el Macro Manager aún no está disponible) [V] | Export WAV | Activo | **Elegida** (usar 3.7.9 si se necesitan Macros) |
| Tenacity | desconocida (Codeberg bloqueado) | — | GPL-2.0-or-later [V] | Gratis | Fork de Audacity | — | WAV | Desarrollo movido a Codeberg; GitHub es espejo [V] | No necesaria |
| Ardour | 9.8 (tag) [V] | 2026-08-20 [V] | GPL [NV] | Código gratis; binarios oficiales de pago/donación [NV] | DAW multipista | Lua [NV] | WAV | Activo | Opcional (música) |
| LMMS | 1.2.2 estable [V]; 1.3.0-alpha.2 [V] | 2020-06-25 (changelog 1.2.2) / 2026-09-06 [V] | GPL-2.0 [V] | Gratis | Secuenciador/música | — | WAV/OGG | Estable antigua; alpha activa | Opcional (música), con reservas |
| Reaper | [NV] | — | Propietaria [NV] | De pago (no gratis) [NV] | DAW | ReaScript [NV] | WAV | — | Fuera de presupuesto por defecto |
| sfxr (DrPetter) | — | — | [NV] (repo espejo sin licencia visible) [V] | Gratis | SFX retro | — | WAV | Histórico | Sustituido por jsfxr |
| **jsfxr** | npm `jsfxr` (commit 2026-05-05) [V] | — | Unlicense (dominio público) [V] | Gratis | SFX retro (UI, blips) | Librería npm, parámetros en JSON [V] | WAV | Activo | **Elegida para SFX retro/UI** |
| Bfxr / Bfxr2 | Bfxr2 (commit 2026-07-26) [V] | — | Bfxr: Apache-2.0 [V]; Bfxr2: MIT [V] | Gratis | SFX retro | — | WAV | Activo | Alternativa a jsfxr |
| ChipTone | [NV] | — | [NV] | Gratis (web) [NV] | SFX retro | — | WAV | — | Pendiente de verificar términos |
| **Surge XT** | 1.3.4 [V]; nightly 2026-09-18 [V] | 2024-08-11 [V] | GPL-3.0 [V] | Gratis | Sintetizador (VST3/CLAP/standalone) [NV] | — | vía DAW | Activo (nightlies) | **Elegido** para música/sintes |
| Vital | [NV] | — | Código GPL-3.0; nombre "Vital" reservado [V] | Binarios en vital.audio con cuenta [V] | Sintetizador wavetable | — | vía DAW | Repo "updated on a delay" [V] | Opcional; verificar EULA de binarios |

### 3.2 Hallazgos

- Audacity: releases 4.0.0 (2026-09-03), 3.7.9 (2026-09-02), 3.7.8 (2026-06-12) [V] — [releases.atom](https://github.com/audacity/audacity/releases.atom). "Audacity is released under the GNU General Public License version 3 (GPLv3)", documentación CC-BY 3.0 [V] — [LICENSE.txt](https://github.com/audacity/audacity/blob/master/LICENSE.txt). 4.0.0: UI Qt, formato `.aup4`, aún sin Macro Manager, MIDI, Mixer ni hosting VAMP/LADSPA [V] — [release 4.0.0](https://github.com/audacity/audacity/releases/tag/Audacity-4.0.0).
- **Telemetría de Audacity:** la polémica de 2021 (tras la compra por Muse Group se propuso telemetría con Google Analytics/Yandex; se retiró y quedó limitado a comprobación de actualizaciones y reporte de errores opt-in) [NV] — [política de privacidad](https://www.audacityteam.org/about/desktop-privacy-notice/). Las notas de 4.0.0 no mencionan telemetría ni cuentas [V]. → Laguna: revisar la privacy notice vigente antes de instalar en máquinas del estudio.
- Tenacity: "Pull requests are IGNORED", desarrollo en Codeberg; GPL-2.0-or-later [V] — [GitHub mirror](https://github.com/tenacityteam/tenacity).
- Ardour tags 9.0 (2026-02-05) … 9.8 (2026-08-20) [V] — [tags.atom](https://github.com/Ardour/ardour/tags.atom).
- LMMS: 1.3.0-alpha.2 (2026-09-06) "over 5 years of work", ARM64, sin builds 32-bit [V] — [releases.atom](https://github.com/LMMS/lmms/releases.atom); 1.2.2 con changelog 2020-06-25 [V] — [release v1.2.2](https://github.com/LMMS/lmms/releases/tag/v1.2.2); GPL-2.0 [V] — [LICENSE.txt](https://github.com/LMMS/lmms/blob/master/LICENSE.txt).
- Surge XT: GPL-3.0 [V] — [LICENSE](https://github.com/surge-synthesizer/surge/blob/main/LICENSE); releases [V] — [releases.atom](https://github.com/surge-synthesizer/surge/releases.atom).
- Vital: "The source code is licensed under the GPLv3"; prohíbe usar "Vital" para nombrar binarios propios [V] — [mtytel/vital](https://github.com/mtytel/vital).
- jsfxr: Unlicense; web en sfxr.me, npm `jsfxr`, presets (pickupCoin, hitHurt, jump, blipSelect…) y export WAV [V] — [repo](https://github.com/chr15m/jsfxr), [README](https://raw.githubusercontent.com/chr15m/jsfxr/master/README.md). Bfxr Apache-2.0 [V] — [increpare/bfxr](https://github.com/increpare/bfxr); Bfxr2 MIT [V] — [increpare/bfxr2](https://github.com/increpare/bfxr2).
- **Licencia de los sonidos generados:** ninguno de los repos (jsfxr, Bfxr, Bfxr2, sfxr) contiene una declaración explícita sobre la propiedad de los sonidos generados [V]. La licencia del software (Unlicense/MIT/Apache) regula el código, no la salida; los sonidos sintetizados a partir de parámetros propios no incorporan material de terceros. **Recomendación prudente:** guardar el JSON de parámetros de cada sonido en el repo como prueba de autoría y registrarlo en ASSET_PROVENANCE.
- **Bancos de SFX gratuitos** (todos [NV] por bloqueo de red; verificar antes de usar):
  - **freesound.org:** cada sonido lleva su propia licencia: CC0, CC-BY (atribución) o CC-BY-NC (**prohibido en un juego comercial**) — [FAQ Freesound](https://freesound.org/help/faq/). Filtrar por licencia al buscar y guardar la URL del sonido + autor.
  - **Kenney:** packs de audio bajo CC0 — [kenney.nl/support](https://kenney.nl/support). Verificar el `License.txt` incluido en cada pack.
  - **Sonniss GDC Game Audio Bundle:** royalty-free, uso comercial sin atribución, prohibida la redistribución de los ficheros sueltos; revisar cláusulas nuevas (p. ej. sobre IA) cada año — [sonniss.com/gameaudiogdc](https://sonniss.com/gameaudiogdc).

### 3.3 Recomendación: identidad sonora

El sonido del pádel es muy característico (golpe seco de pala de goma EVA/fibra, rebote en **cristal**, rebote en la **malla metálica**, bote en césped artificial con arena, pasos arrastrados). Recomendación: **grabar foley propio** en una pista real (mejor sesión temprana por la mañana, sin viento), porque:
1. Da identidad única y es 100 % propiedad del estudio (sin riesgo de licencia).
2. Los bancos genéricos rara vez tienen "pelota contra cristal de pádel".

Lista de captura mínima: golpe plano/cortado/liftado (drive, revés, volea, bandeja, víbora, remate), bote en suelo, bote en cristal (lateral y fondo, a varias alturas), golpe contra malla, pelota contra marco metálico, red, pasos/deslizamientos, bote de pelota en mano (saque), apertura de bote de pelotas (UI), público/ambiente de club. Complementar UI/menús con **jsfxr** (estética retro coherente con el pixel art) y música con **Surge XT** + DAW (LMMS/Ardour o la del compositor).

---

## 4. Control de versiones (Git + LFS)

### 4.1 Hallazgos

- **Cuotas Git LFS en GitHub** [V] — [data/variables/large_files.yml](https://raw.githubusercontent.com/github/docs/main/data/variables/large_files.yml) (fuente de docs.github.com):
  - GitHub **Free y Pro: 10 GiB de almacenamiento y 10 GiB de ancho de banda al mes** incluidos.
  - Team y Enterprise: 250 GiB + 250 GiB/mes.
  - Tamaño máximo por fichero LFS: 2 GiB; aviso de Git a partir de 50 MiB; bloqueo a 100 MiB sin LFS; subida por navegador 25 MiB.
- **Modelo de cobro**: metered (pago por uso), "Storage is billed by calculating an hourly usage rate", "Bandwidth is billed for each GiB of data downloaded"; al subir cuenta almacenamiento del propietario, al descargar cuenta ancho de banda del propietario [V] — [git-lfs.md](https://raw.githubusercontent.com/github/docs/main/content/billing/concepts/product-billing/git-lfs.md).
- **Sin método de pago y cuota agotada**: con almacenamiento excedido "You will only retrieve the pointer files" y no se puede hacer push; con ancho de banda excedido "Git LFS support is disabled on your account until the next month" [V] (misma fuente).
- **Precio extra**: 0,07 USD/GiB/mes de almacenamiento y 0,0875 USD/GiB de descarga [S] — [GitHub Docs: Git LFS billing](https://docs.github.com/billing/managing-billing-for-git-large-file-storage/about-billing-for-git-large-file-storage). Nota: la página [github.com/pricing](https://github.com/pricing) aún muestra "$5 per month for 50 GB bandwidth and 50 GB of storage" (modelo antiguo de *data packs*) [V] — **contradicción** documentada en Lagunas; prevalece la documentación de billing.
- Tamaño de repo recomendado por GitHub: < 1 GB, 5 GB como límite fuerte [V] — [about-large-files-on-github.md](https://raw.githubusercontent.com/github/docs/main/content/repositories/working-with-files/managing-large-files/about-large-files-on-github.md).
- Git LFS cliente v3.8.0 (2026-08-28), con descargas comprimidas zstd [V] — [releases.atom](https://github.com/git-lfs/git-lfs/releases.atom).
- `.gitattributes` de referencia para Unity (repo comunitario `gitattributes/gitattributes`): macros `lfs`, `unity-yaml` (`merge=unityyamlmerge eol=lf`), `unity-json`; LFS para modelos (`*.fbx`, `*.blend`, `*.obj`), audio (`*.wav`, `*.ogg`, `*.mp3`, `*.aif`), imágenes (`*.png`, `*.psd`, `*.tga`, `*.exr`), fuentes (`*.ttf`, `*.otf`), vídeo, zips, dll; `LightingData.asset binary` [V] — [Unity.gitattributes](https://raw.githubusercontent.com/gitattributes/gitattributes/master/Unity.gitattributes).
- `.gitignore` oficial de Unity (GitHub): ignora `Library/`, `Temp/`, `Obj/`, `Build(s)/`, `Logs/`, `UserSettings/`, `*.csproj`, `*.sln`, `*.blend1` [V] — [Unity.gitignore](https://github.com/github/gitignore/blob/main/Unity.gitignore).
- **Unity Smart Merge (UnityYAMLMerge)** [NV] — [Unity manual: Smart Merge](https://docs.unity3d.com/Manual/SmartMerge.html). Requisitos: Asset Serialization = *Force Text* y Version Control = *Visible Meta Files*. Configuración git:

```ini
# .git/config o ~/.gitconfig (cada desarrollador; ruta según SO y versión de Unity)
[merge]
    tool = unityyamlmerge
[mergetool "unityyamlmerge"]
    trustExitCode = false
    cmd = '<ruta>/UnityYAMLMerge' merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"
# Para que el atributo merge=unityyamlmerge de .gitattributes funcione como merge driver:
[merge "unityyamlmerge"]
    name = Unity SmartMerge
    driver = '<ruta>/UnityYAMLMerge' merge -h -p --force %O %B %A %A
    recursive = binary
```
Rutas típicas: Windows `C:\Program Files\Unity\Hub\Editor\<versión>\Editor\Data\Tools\UnityYAMLMerge.exe`; macOS `/Applications/Unity/Hub/Editor/<versión>/Unity.app/Contents/Tools/UnityYAMLMerge` [NV].

### 4.2 Pros/contras de LFS para un proyecto pequeño

| Pros | Contras |
|---|---|
| Repo Git ligero; clones rápidos del historial | 10 GiB/mes de **descarga** se agotan con pocos clones completos + CI (cada checkout con `lfs: true` descarga todo) |
| Bloqueo de ficheros (`git lfs lock`, atributo `lockable`) para binarios no fusionables (.blend, .pxo, .aseprite) | Cada versión de un binario cuenta en almacenamiento para siempre (borrar requiere reescribir historia / borrar el repo) |
| Estándar soportado por GitHub y Unity | Pasar ficheros a LFS después obliga a `git lfs migrate` (reescritura de historia) |

### 4.3 Recomendación (VCS)

1. **Usar Git LFS desde el primer commit** (evita migraciones). Tipos en LFS: `*.blend *.fbx *.obj *.glb *.wav *.aif *.aiff *.ogg *.mp3 *.flac *.psd *.psb *.kra *.png *.tga *.exr *.hdr *.ttf *.otf *.mp4 *.mov *.zip *.pxo *.aseprite *.ase`. Marcar `lockable` los fuentes no fusionables (`*.blend *.pxo *.aseprite *.kra *.psd`).
2. Unity YAML con `merge=unityyamlmerge eol=lf` (usar el `.gitattributes` de referencia como base) + `.gitignore` oficial de GitHub.
3. **Presupuesto de ancho de banda:** evitar CI que descargue LFS en cada push; si hay CI, cachear `.git/lfs` (o usar `lfs: false` en jobs que solo compilan código). Vigilar la página de uso; activar método de pago con límite de gasto para no quedar bloqueados.
4. Separar **fuentes** (`art_src/`: .pxo, .blend, .kra, WAV de sesión sin editar) de **entregables** (`Assets/`). Si `art_src/` crece mucho (sesiones de foley en bruto), mantener los brutos en almacenamiento externo con checksum en el repo.

---

## 5. Fuentes tipográficas para UI pixel

| Herramienta / recurso | Versión | Fecha | Licencia | Coste | Uso | Decisión |
|---|---|---|---|---|---|---|
| Fuentes SIL OFL 1.1 (p. ej. Press Start 2P) | — | — | OFL-1.1 [V] | Gratis | Prototipo / producción | Válidas para producción cumpliendo condiciones |
| **FontForge** | "October 2025 Release" [V] | 2025-10-09 [V] | GPL-3.0 [V] | Gratis | Crear/convertir TTF/OTF, bitmap strikes | **Elegida** para fuente propia |
| BitFontMaker2 | [NV] | — | [NV] (términos web no verificados) | Gratis (web) [NV] | Editor web de fuentes bitmap → TTF | Opcional; verificar términos |

**Hallazgos**
- OFL (texto en Google Fonts, Press Start 2P) [V] — [OFL.txt](https://raw.githubusercontent.com/google/fonts/main/ofl/pressstart2p/OFL.txt):
  - "Neither the Font Software nor any of its individual components, in Original or Modified Versions, may be sold by itself."
  - Se puede empaquetar con software "provided that each copy contains the above copyright notice and this license".
  - Las versiones modificadas no pueden usar el **Reserved Font Name** ("Press Start 2P") sin permiso.
  - "Modified Version" incluye cambiar formatos o portar la fuente "to a new environment" → convertir a atlas bitmap o a Font Asset de Unity puede considerarse versión modificada: mantener licencia, aviso de copyright y **renombrar** si hay RFN.
  - "The requirement for fonts to remain under this license does not apply to any document created using the fonts or their derivatives." → el texto renderizado en pantallas/capturas no queda bajo OFL.
- FontForge: GPL-3.0; la licencia del editor no se aplica a las fuentes creadas [V repo] — [fontforge/fontforge](https://github.com/fontforge/fontforge), [releases.atom](https://github.com/fontforge/fontforge/releases.atom).

**Recomendación:** prototipar con una fuente OFL (incluir `OFL.txt` en el build y en créditos), y **crear una fuente bitmap propia** para la versión final: identidad visual, control exacto de métrica a la resolución del juego y cobertura garantizada de caracteres españoles/catalanes/portugueses (á é í ó ú ü ñ ç ¿ ¡ · ã õ) y de marcadores (dígitos tabulares, "15-30-40", "AD"). Flujo: glifos dibujados en Pixelorama sobre rejilla → FontForge (importar/trazar píxeles como cuadrados, exportar TTF) → Unity (Font Asset de TextMeshPro/TextCore con Point filtering y tamaño de muestreo múltiplo exacto del tamaño de píxel) [NV para detalles de Unity].

---

## 6. Checklist de licencias para ASSET_PROVENANCE

> Base: textos de licencia oficiales. CC y Unity Asset Store EULA **no** pudieron abrirse en esta sesión [NV]; las URLs canónicas se indican para verificación.

| Licencia | ¿Uso comercial en el juego? | Obligaciones | ¿Modificar? | Riesgo para el proyecto | Regla ASSET_PROVENANCE |
|---|---|---|---|---|---|
| **CC0 1.0** ([texto](https://creativecommons.org/publicdomain/zero/1.0/)) [NV] | Sí | Ninguna (atribución de cortesía opcional) | Sí | Bajo (verificar que quien lo sube es el autor) | Registrar URL, autor, fecha de descarga, captura/licencia |
| **CC-BY 4.0** ([texto](https://creativecommons.org/licenses/by/4.0/)) [NV] | Sí | Atribución (autor, título, licencia, enlace, indicar cambios) | Sí | Bajo si se acredita | Añadir a créditos in-game y fichero `CREDITS` |
| **CC-BY-SA 4.0** ([texto](https://creativecommons.org/licenses/by-sa/4.0/)) [NV] | Sí | Atribución + las adaptaciones del asset deben licenciarse CC-BY-SA | Sí, con ShareAlike | Medio: el asset modificado debe ser redistribuible bajo SA; evitar en arte núcleo | Solo con aprobación explícita |
| **CC-BY-NC (cualquier versión)** ([texto](https://creativecommons.org/licenses/by-nc/4.0/)) [NV] | **NO** (juego comercial) | — | — | Alto | **Prohibido** |
| **CC-BY-ND** [NV] | Sí, sin modificar | Atribución; no se pueden distribuir adaptaciones | No | Medio | Evitar |
| **SIL OFL 1.1** [V] | Sí (empaquetada en el juego) | Incluir aviso de copyright + licencia; no vender la fuente sola; no usar el RFN en versiones modificadas | Sí (renombrar si RFN) | Bajo | Guardar `OFL.txt` junto a la fuente |
| **GPL (v2/v3)** | Herramientas GPL (Blender, Krita, Audacity…): el **output** es tuyo [NV] | Si se **incluye código o assets GPL dentro del juego**, el conjunto distribuido podría quedar sujeto a GPL | — | Alto para assets/código dentro del build | Prohibido incluir código/assets GPL en el build sin decisión legal |
| **MIT / Apache-2.0 / Unlicense** (código) [V para los repos citados] | Sí | MIT/Apache: conservar aviso de copyright y licencia (Apache: también NOTICE) | Sí | Bajo | Listar en `THIRD_PARTY_NOTICES` |
| **Standard Unity Asset Store EULA** ([texto](https://unity.com/legal/as-terms)) [NV] | Sí, integrado en el juego | Licencia no exclusiva para incorporar el asset en tu producto; **no** redistribuir el asset suelto ni en forma de fuente/editable; herramientas de editor suelen licenciarse por puesto; componentes de terceros pueden traer sus propias licencias | Sí, dentro del juego | Medio (repos públicos: no subir assets de la Store a un repo público) | Registrar factura/ID de pedido, versión, licencia por puesto si aplica |
| **Aseprite EULA** (herramienta) [V] | El arte es tuyo | No redistribuir el binario, ni compilado propio | — | Bajo | Cada artista con su licencia |

Campos mínimos por asset en ASSET_PROVENANCE: `ruta`, `tipo`, `autor`, `origen` (propio / URL), `licencia` (SPDX), `obligaciones` (atribución, NOTICE), `herramienta y versión`, `fecha`, `referencia 3D usada (sí/no)`, `IA usada (sí/no)`, `prueba` (JSON de parámetros jsfxr, sesión de grabación, factura).

---

## 7. Pipeline propuesto

### 7.1 Pixel art → Unity

1. **Paleta maestra** en Pixelorama (`.gpl`/paleta de proyecto) versionada en `art_src/palettes/`.
2. Cada personaje/animación en un `.pxo` (`art_src/sprites/<personaje>/<personaje>.pxo`) con **tags** por animación (`idle`, `run`, `drive`, `reves`, `volea`, `bandeja`, `vibora`, `remate`, `saque`…), tamaño de lienzo fijo, pivote en los pies.
3. Opcional: capa de referencia 3D (maniquí Blender o capa 3D de Pixelorama) → redibujo a mano → **borrar capa de referencia**.
4. **Spike (1 día, antes de producción):** exportar un `.pxo` con 3 tags y 2 capas a `.aseprite` (Pixelorama ≥ 1.2.2) e importarlo con `com.unity.2d.aseprite` en la versión exacta de Unity del proyecto. Criterios: clips por tag, duraciones por frame, capas/merge correctos, Point filtering, sin errores. Probar también la CLI con `--headless`.
5. **Vía A (si el spike pasa):** `tools/export_sprites.py` → Pixelorama CLI → `.aseprite` en `Assets/Art/Sprites/...` → Aseprite Importer (Animated Sprite, Merge Frames, PPU del proyecto, pivote Bottom).
6. **Vía B (si falla):** `tools/export_sprites.py` → Pixelorama CLI `--export --spritesheet --json` → PNG con celda fija (una fila por tag) + JSON → Unity: Texture Import (Sprite Multiple, Point, sin compresión, sin mipmaps) → slicing por rejilla automatizado (AssetPostprocessor) → script de editor que genera AnimationClips desde el JSON.
7. Unity **Sprite Atlas** para el empaquetado final; revisión visual pixel-perfect (cámara/escala entera).
8. Entrada en ASSET_PROVENANCE por asset.

### 7.2 Blender → Unity

1. Blender **5.2 LTS** fijado; plantilla `.blend` con unidades métricas, Unit Scale 1.0, pista a escala 1:1 (20 × 10 m).
2. Fuentes en `art_src/3d/*.blend` (LFS, `lockable`), **nunca** en `Assets/`.
3. Convenciones: +Y de Unity = arriba; frente del modelo mirando a -Y en Blender (que el exportador convierte con forward -Z/up Y) [NV]; transformaciones aplicadas (Ctrl+A) antes de exportar; nombres sin espacios; colecciones `EXPORT_*` para lo exportable.
4. `tools/export_fbx.py` ejecutado con `blender -b --factory-startup file.blend -P tools/export_fbx.py --python-exit-code 1 -- --out <ruta>`: FBX binario, `apply_scale_options='FBX_SCALE_UNITS'`, axis `-Z`/`Y`, `bake_space_transform=False`, `add_leaf_bones=False`, `colors_type='SRGB'`, `use_mesh_modifiers=True`, `embed_textures=False`.
5. Unity Model Importer: Bake Axis Conversion activado, escala 1, Generate Lightmap UVs para estáticos, materiales remapeados a materiales URP del proyecto [NV].
6. Vertex colors: Shader Graph con nodo Vertex Color si se usan para tinte/AO.
7. Alternativa: `.glb` + glTFast 6.20.x (estable) si FBX presenta problemas; no usar 7.0.0-exp en producción.

### 7.3 Audio → Unity

1. **Grabación de foley** en pista real: grabadora portátil o móvil con micro externo, WAV 48 kHz / 24-bit, varias tomas por evento; registrar fecha, lugar, micrófono y permiso del club.
2. Brutos en `art_src/audio/raw/` (o almacenamiento externo con checksum si pesan mucho).
3. Edición en **Audacity 3.7.9/4.0.0**: recorte, reducción de ruido, fades, normalización a pico consistente (p. ej. -1 dBFS) y loudness homogénea por categoría; export **WAV 48 kHz/16-bit mono** para SFX, estéreo para ambiente/música.
4. SFX retro de UI con **jsfxr**: guardar el JSON de parámetros junto al WAV.
5. Música: **Surge XT** + DAW del compositor (LMMS/Ardour); export WAV estéreo.
6. Unity: SFX cortos *Decompress On Load*, compresión Vorbis/ADPCM según plataforma; música *Streaming*; AudioMixer con grupos (SFX, UI, Ambiente, Música); variaciones aleatorias de pitch/volumen para golpes repetidos [NV para defaults de Unity].
7. Cualquier SFX externo solo si es CC0/CC-BY/Sonniss/Kenney con licencia registrada en ASSET_PROVENANCE; **CC-BY-NC prohibido**.

---

## 8. Resumen de decisiones

| Área | Decisión | Versión / licencia |
|---|---|---|
| Pixel art (primaria) | Pixelorama | 1.2.3 (2026-09-15), MIT |
| Pixel art (secundaria) | Krita (concept/fondos/UI) | 6.0.4 (2026-09-10), GPL-3.0 |
| Aseprite | Opcional por artista, no requisito | 1.3.18.6, EULA, 19,99 USD |
| Formato sprites → Unity | `.aseprite` vía Aseprite Importer (tras spike) / fallback PNG celda fija + JSON | com.unity.2d.aseprite (versión según Editor; 7.0.0 = Unity 6000.7) |
| 3D | Blender LTS → FBX | 5.2 LTS (5.2.2, 2026-09-14), GPL |
| glTF | glTFast como secundaria | 6.20.0 estable |
| Audio edición | Audacity | 4.0.0 / 3.7.9, GPL-3.0 |
| SFX retro | jsfxr | Unlicense |
| Síntesis musical | Surge XT | 1.3.4, GPL-3.0 |
| VCS | Git + LFS desde el día 1, UnityYAMLMerge | Git LFS 3.8.0; GitHub Free 10 GiB + 10 GiB/mes |
| Fuente UI | OFL para prototipo, bitmap propia con FontForge para final | FontForge (oct-2025), GPL-3.0 |

---

## 9. Lagunas de evidencia

1. **Compatibilidad Pixelorama `.aseprite` → Unity Aseprite Importer**: sin evidencia pública; requiere spike práctico (§7.1 paso 4).
2. **Pixelorama CLI headless** (sin ventana, apto para CI): no documentado; probar `--headless` de Godot 4.6.
3. **Formato del JSON de Pixelorama**: no verificado (docs bloqueadas); no asumir compatibilidad con JSON de Aseprite.
4. **Versión exacta de Unity del proyecto** desconocida → versión concreta de `com.unity.2d.aseprite` y `com.unity.2d.psdimporter` a fijar cuando se decida el Editor.
5. **Precio extra de Git LFS** (0,07 USD/GiB almacenamiento, 0,0875 USD/GiB descarga) solo vía buscador [S]; además `github.com/pricing` muestra todavía el modelo antiguo de *data packs* (5 USD por 50 GB) → contradicción a confirmar en la página de billing.
6. **Unity Smart Merge**, Model Importer (Bake Axis Conversion), Sprite Editor slicing, AudioClip import settings: docs.unity3d.com bloqueado [NV].
7. **Standard Unity Asset Store EULA**, textos **Creative Commons**, **GPL FAQ** sobre output, **OFL-FAQ**: dominios bloqueados [NV].
8. **Freesound, Kenney, Sonniss, ChipTone, BitFontMaker2**: términos no verificables en sesión [NV].
9. **Telemetría de Audacity**: estado actual de la privacy notice no verificado [NV].
10. **Tenacity**: versión actual desconocida (Codeberg bloqueado). **Ardour**: modelo de pago de binarios [NV]. **Reaper**: precio [NV]. **Vital**: términos de los binarios de vital.audio [NV].
11. **Licencia de sonidos generados** con jsfxr/Bfxr/sfxr: los repos no la declaran explícitamente [V ausencia].
12. **Dead Cells pipeline**: artículo no accesible; descripción basada en conocimiento previo [NV].
13. **Krita spritesheet nativo**: la búsqueda indica que no existe [S]; confirmar en docs.krita.org.
14. Los espejos `needle-mirror/*` son copias no oficiales de los paquetes UPM; contrastar con el Package Manager del Editor.

---

## 10. Fuentes

**Pixel art**
- Pixelorama: https://github.com/Orama-Interactive/Pixelorama · releases.atom: https://github.com/Orama-Interactive/Pixelorama/releases.atom · LICENSE: https://github.com/Orama-Interactive/Pixelorama/blob/master/LICENSE · CHANGELOG: https://github.com/Orama-Interactive/Pixelorama/blob/master/CHANGELOG.md · v1.2: https://github.com/Orama-Interactive/Pixelorama/releases/tag/v1.2 · v1.2.2: https://github.com/Orama-Interactive/Pixelorama/releases/tag/v1.2.2 · CLI discussion: https://github.com/Orama-Interactive/Pixelorama/discussions/579 · Docs CLI [S]: https://pixelorama.org/user_manual/cli/ · Save/Export [S]: https://pixelorama.org/user_manual/save_and_export/
- Krita: https://github.com/KDE/krita · tags.atom: https://github.com/KDE/krita/tags.atom · CLI: https://github.com/KDE/krita/blob/master/libs/ui/KisApplicationArguments.cpp · plugins Python: https://github.com/KDE/krita/tree/master/plugins/python · 5.3.0 [S]: https://krita.org/en/posts/2026/krita-5.3.0-released/ · Phoronix [S]: https://www.phoronix.com/news/Krita-6.0-Released · Render animation [S]: https://docs.krita.org/en/reference_manual/render_animation.html
- LibreSprite: https://github.com/LibreSprite/LibreSprite/releases.atom · https://github.com/LibreSprite/LibreSprite/blob/master/LICENSE.txt · https://github.com/LibreSprite/LibreSprite/commits/master.atom
- Aseprite: https://github.com/aseprite/aseprite · https://github.com/aseprite/aseprite/releases.atom · https://github.com/aseprite/aseprite/blob/main/EULA.txt · CLI: https://github.com/aseprite/docs/blob/main/cli.md · Precio [S]: https://dacap.itch.io/aseprite/purchase
- Piskel: https://github.com/piskelapp/piskel/releases.atom · https://github.com/piskelapp/piskel/blob/master/LICENSE
- GrafX2: https://gitlab.com/GrafX2/grafX2/-/tags?format=atom · https://gitlab.com/GrafX2/grafX2/-/raw/master/LICENSE
- Pixel Composer: https://github.com/Ttanasart-pt/Pixel-Composer · https://github.com/Ttanasart-pt/Pixel-Composer/blob/main/LICENSE · https://github.com/Ttanasart-pt/Pixel-Composer/releases.atom

**Unity**
- Aseprite Importer (espejo UPM): https://github.com/needle-mirror/com.unity.2d.aseprite · CHANGELOG: https://github.com/needle-mirror/com.unity.2d.aseprite/blob/master/CHANGELOG.md · package.json: https://github.com/needle-mirror/com.unity.2d.aseprite/blob/master/package.json · Docs: https://github.com/needle-mirror/com.unity.2d.aseprite/tree/master/Documentation~ · Manual [S]: https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.2d.aseprite.html
- glTFast (espejo UPM): https://github.com/needle-mirror/com.unity.cloud.gltfast/blob/master/CHANGELOG.md · https://github.com/needle-mirror/com.unity.cloud.gltfast/blob/master/Documentation~/index.md
- PSD Importer [S]: https://docs.unity3d.com/Packages/com.unity.2d.psdimporter@15.0/changelog/CHANGELOG.html
- Import .blend [S]: https://docs.unity3d.com/540/Documentation/Manual/HOWTO-ImportObjectBlender.html · https://issuetracker.unity3d.com/issues/blender-could-not-convert-the-blend-file-to-fbx-file-error-is-thrown-when-blend-file-is-imported
- [NV]: https://docs.unity3d.com/Manual/SmartMerge.html · https://docs.unity3d.com/Manual/FBXImporter-Model.html · https://docs.unity3d.com/Manual/sprite/sprite-editor/use-editor.html · https://unity.com/legal/as-terms

**Blender**
- Tags: https://github.com/blender/blender/tags.atom · COPYING: https://github.com/blender/blender/blob/main/COPYING · readme: https://github.com/blender/blender/blob/v5.2.2/release/text/readme.html · FBX exporter: https://github.com/blender/blender/blob/main/scripts/addons_core/io_scene_fbx/__init__.py · glTF exporter: https://github.com/blender/blender/blob/main/scripts/addons_core/io_scene_gltf2/__init__.py · CLI: https://github.com/blender/blender/blob/main/source/creator/creator_args.cc
- [S]: https://www.blender.org/press/blender-5-2-lts-release/ · https://developer.blender.org/docs/release_notes/5.2/ · [NV]: https://www.blender.org/about/license/
- Dead Cells [NV]: https://www.gamedeveloper.com/production/art-design-deep-dive-using-a-3d-pipeline-for-2d-animation-in-i-dead-cells-i-

**Audio**
- Audacity: https://github.com/audacity/audacity/releases.atom · https://github.com/audacity/audacity/blob/master/LICENSE.txt · https://github.com/audacity/audacity/releases/tag/Audacity-4.0.0 · [NV] https://www.audacityteam.org/about/desktop-privacy-notice/
- Tenacity: https://github.com/tenacityteam/tenacity · Ardour: https://github.com/Ardour/ardour/tags.atom · LMMS: https://github.com/LMMS/lmms/releases.atom · https://github.com/LMMS/lmms/releases/tag/v1.2.2 · https://github.com/LMMS/lmms/blob/master/LICENSE.txt
- Surge XT: https://github.com/surge-synthesizer/surge/releases.atom · https://github.com/surge-synthesizer/surge/blob/main/LICENSE · Vital: https://github.com/mtytel/vital
- jsfxr: https://github.com/chr15m/jsfxr · https://raw.githubusercontent.com/chr15m/jsfxr/master/README.md · Bfxr: https://github.com/increpare/bfxr · Bfxr2: https://github.com/increpare/bfxr2 · sfxr: https://github.com/grimfang4/sfxr
- [NV]: https://freesound.org/help/faq/ · https://kenney.nl/support · https://sonniss.com/gameaudiogdc

**Control de versiones**
- https://raw.githubusercontent.com/github/docs/main/data/variables/large_files.yml · https://raw.githubusercontent.com/github/docs/main/content/billing/concepts/product-billing/git-lfs.md · https://raw.githubusercontent.com/github/docs/main/content/repositories/working-with-files/managing-large-files/about-large-files-on-github.md · https://github.com/pricing · [S] https://docs.github.com/billing/managing-billing-for-git-large-file-storage/about-billing-for-git-large-file-storage
- https://github.com/git-lfs/git-lfs/releases.atom · https://raw.githubusercontent.com/gitattributes/gitattributes/master/Unity.gitattributes · https://github.com/github/gitignore/blob/main/Unity.gitignore

**Fuentes y licencias**
- OFL: https://raw.githubusercontent.com/google/fonts/main/ofl/pressstart2p/OFL.txt · FontForge: https://github.com/fontforge/fontforge · https://github.com/fontforge/fontforge/releases.atom
- [NV]: https://creativecommons.org/publicdomain/zero/1.0/ · https://creativecommons.org/licenses/by/4.0/ · https://creativecommons.org/licenses/by-sa/4.0/ · https://creativecommons.org/licenses/by-nc/4.0/ · https://openfontlicense.org/ofl-faq/
