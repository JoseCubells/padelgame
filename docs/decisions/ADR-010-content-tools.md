# ADR-010 — Herramientas de contenido (pixel art, 3D, audio, tipografía)

- **Estado:** Aceptado (provisional en la ruta Pixelorama → Unity, pendiente de un spike)
- **Fecha de investigación:** 2026-09-24
- **Depende de / afecta a:** ADR-003, ASSET_PROVENANCE, skills `pixel-art-pipeline` y `asset-provenance`

## Pregunta

¿Con qué herramientas, preferiblemente gratuitas y con licencia clara, producimos pixel art, 3D, audio y tipografía, y con qué formatos llegan a Unity?

## Fuentes consultadas

El detalle, con versiones, fechas y licencias de cada herramienta, está en [`TOOLS_RESEARCH.md`](../research/TOOLS_RESEARCH.md). Casi todas las versiones se verificaron **[V]** en los releases oficiales de GitHub. Las webs oficiales estaban bloqueadas, así que algunas condiciones de licencia de servicios son [S] o [NV].

## Alternativas evaluadas (resumen)

| Área | Candidatas | Decisión |
|---|---|---|
| Pixel art | Pixelorama 1.2.3 (MIT), Krita 6.0.4 (GPL-3.0), Aseprite 1.3.18.6 (EULA, 19,99 USD), LibreSprite (GPL-2.0, poca actividad), Piskel (sin release desde 2018), GrafX2 (última versión de 2024), Pixel Composer (MIT) | **Pixelorama** principal; **Krita** para concept, fondos y UI pintada; Aseprite opcional para cada artista, **nunca obligatorio** |
| Formato de sprites hacia Unity | `.aseprite` → `com.unity.2d.aseprite`; PNG con celda fija + JSON | **Plan A:** `.aseprite` exportado por Pixelorama (desde 1.2.2). **Plan B:** PNG + JSON. Decide un spike de un día |
| 3D | Blender 5.2 LTS (5.2.2; GPL; soporte hasta julio de 2028 [S]); ProBuilder | **Blender** para el arte de la pista; **ProBuilder** solo para el graybox dentro de Unity |
| Formato 3D | FBX, glTF (glTFast 6.20.0), `.blend` directo | **FBX**. No se importa `.blend` directo porque obliga a tener Blender en todas las máquinas y en CI. glTFast queda como alternativa |
| Audio | Audacity 3.7.9 / 4.0.0 (GPL-3.0), jsfxr (Unlicense), Surge XT 1.3.4 (GPL-3.0), LMMS 1.2.2, Ardour 9.8 | **Audacity** para edición, **grabación de foley propia** para golpes, cristal, malla y suelo, **jsfxr** para la UI retro y **Surge XT** para síntesis |
| Tipografía | Fuentes OFL; fuente bitmap propia hecha con FontForge (GPL-3.0) | OFL en el prototipo y **fuente bitmap propia** para el arte final |

## Decisión

- La tabla anterior es vigente.
- Las **fuentes de arte** se versionan en `ArtSource/` (fuera de `Assets/`): `.pxo`, `.blend`, `.kra` y WAV de sesión. Los **entregables** van en `Assets/`.
- Automatización:
  - script Python que llama al CLI de Pixelorama por cada `.pxo`, valida paleta y tamaños y copia el resultado a `Assets/`;
  - exportación de Blender con `blender -b --factory-startup f.blend -P export.py --python-exit-code 1`.

  Queda pendiente verificar que el CLI de Pixelorama funciona sin ventana.
- Un modelo 3D renderizado a sprites **nunca es arte final**. Solo puede usarse como capa de referencia que se redibuja a mano. Cuando se use, se registra en ASSET_PROVENANCE.
- **Prohibidas** las licencias NonCommercial (NC) y NoDerivatives (ND). CC-BY-SA solo con aprobación explícita. Todo asset externo se registra en ASSET_PROVENANCE.

## Motivo

Son herramientas gratuitas, mantenidas activamente (releases de septiembre de 2026 [V]) y con licencias que no afectan a lo que producimos con ellas. Cumplen además el requisito de que Aseprite no sea obligatorio.

## Coste

0 €. Aseprite, si algún artista lo quiere, cuesta 19,99 USD por su cuenta.

## Riesgos

- **El `.aseprite` de Pixelorama podría no importar bien en Unity.** Mitigación: Plan B.
- **El CLI de Pixelorama podría no funcionar sin ventana.** Mitigación: exportar a mano con un checklist.
- **La versión del importer de Aseprite depende del editor.** La 7.0.0 exige 6000.7, así que hay que verificar cuál resuelve 6000.3.

## Cómo revertirla

Todas las herramientas producen formatos estándar (PNG, FBX, WAV), así que cambiar de herramienta no afecta a Unity.

## Historial

- 2026-09-24: creado.
