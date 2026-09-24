---
name: pixel-art-pipeline
description: Pixel-art production and import pipeline (ADR-010) - Pixelorama as primary tool (Krita secondary, Aseprite optional), ArtSource/ layout, palette and canvas rules, export as .aseprite or PNG+JSON grid, Unity import settings (point filter, no mipmaps, no compression artifacts), AnimationSet generation, automated validation. Use when creating, exporting, importing or validating sprites, tiles, UI pixel art or palettes.
---

# Pipeline de pixel art

Reglas de estilo: `docs/art/ART_BIBLE.md` (parte A vigente; parte B cuando se elija la dirección). Personajes: `docs/art/CHARACTER_BIBLE.md`.

## Herramientas (ADR-010)

| Rol | Herramienta | Licencia |
|---|---|---|
| Principal | **Pixelorama 1.2.3+** | MIT |
| Concept, fondos, UI pintada | Krita 6.x | GPL-3.0; lo que se crea con ella es nuestro |
| Opcional por artista | Aseprite | EULA de pago; **nunca obligatorio** |

## Estructura

```
ArtSource/characters/<personaje>/<personaje>.pxo      fuente (LFS, lockable)
ArtSource/palettes/<direccion>.gpl|.png               paleta cerrada
Assets/_Project/Art/Characters/<personaje>/           entregables importados
```

## Reglas de pixel art

- **Dibujar a la resolución final.** Nunca dibujar grande y reducir, ni filtrar un render (brief §6).
- Solo se usa la paleta cerrada de la dirección. El amarillo de la pelota está reservado.
- Lienzo y pivote según CHARACTER_BIBLE. El pivote está en los pies y es idéntico en todos los frames.
- Contorno de 1 px con selout. Sin píxeles huérfanos ni *jaggies* no intencionados.
- En Pixelorama, un tag por clip (`idle_S`, `run_SE`, …) con la convención `<clip>_<dir>`. Las direcciones son S, SE, E, NE y N; las restantes se espejan cuando se permite.

## Export → Unity

**Plan A** (pendiente del spike KI-007): export `.aseprite` desde Pixelorama y el importer `com.unity.2d.aseprite` convierte los tags en clips.

**Plan B**: PNG con celda fija más JSON. Se corta por grid en Unity y un script de editor genera el `AnimationSet`.

Import settings obligatorios:
- Filter Mode = **Point**;
- Mip Maps **desactivados**;
- Compression = **None**;
- PPU según ART_BIBLE A.2;
- pivote Bottom Center (o el personalizado del frame).

## Automatización (`tools/pipeline/`)

Un script Python:
1. exporta cada `.pxo` con el CLI de Pixelorama. Queda por verificar que el CLI funcione sin ventana; si no, se exporta a mano con un checklist;
2. valida:
   - paleta (ningún color fuera de ella);
   - tamaño de lienzo;
   - pivote;
   - número de frames por tag;
3. copia el resultado a `Assets/`.

Cualquier fallo de validación bloquea el commit.

## Procedencia

Si se usa referencia externa (fotos, un modelo 3D de guía), se registra con la skill `asset-provenance`. Los *placeholders* llevan prefijo `TEMP_`.
