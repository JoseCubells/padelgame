# CHARACTER BIBLE

- **Estado:** 🟡 BORRADOR técnico. El diseño de los personajes (quiénes son, vestuario, paletas) espera la dirección visual.
- **Versión:** 0.1
- **Fecha:** 2026-09-24
- **Fuentes:** ADR-003, ADR-005, VISUAL_RESEARCH §B.4–B.9 y §E

## 1. Especificación técnica (vigente, provisional hasta la prueba de la Fase 2)

| Parámetro | Valor inicial | Se decide en |
|---|---|---|
| Altura del personaje en pantalla | 32–46 px (a 480×270, cámara C) | Prueba de resolución, Fase 2 |
| Lienzo del sprite | 64×64 px (margen para pala y brazos en golpes); 48×48 si la resolución final es 320×180 | Fase 2 |
| Pivote | En los pies (centro inferior del contacto con el suelo) | — |
| PPU | Derivado de la densidad objetivo en px/m (ART_BIBLE A.2) | Fase 2 |
| Direcciones | **8 para locomoción** (5 dibujadas + 3 espejadas) y **5 útiles para golpes** | Prueba ciega 4/8/16, Fase 2 |
| Pala | **Capa o sprite separado**, que no se espeja al hacer flip (evita convertir un diestro en zurdo) | Spike ADR-003 |
| Contorno | 1 px de selout horneado | — |
| Sombreado | 3–4 tonos por material, siguiendo las rampas de la paleta | Dirección visual |
| Highlights | 1 tono como máximo; nunca blanco puro salvo en brillo del cristal | Dirección visual |
| Pelo y ropa | Masas grandes y legibles con silueta distintiva por personaje; nada de detalle de 1 px aislado | — |
| Lateralidad | Diestro o zurdo es un dato del personaje (`PlayerStats`), y el arte de golpes se dibuja para cada lado | — |

## 2. Animation MVP (ADR-005)

| Clip | Frames [P] | Loop | Marcadores |
|---|---|---|---|
| idle | 6 | sí | — |
| ready (preparación) | 4 | sí | — |
| carrera | 8 | sí | pisadas (SFX, polvo) |
| lateral | 8 | sí | pisadas |
| retroceso | 6 | sí | pisadas |
| split-step | 4 | no | aterrizaje |
| drive | 8 | no | `contactFrame` |
| revés | 8 | no | `contactFrame` |
| volea (derecha y revés) | 6 + 6 | no | `contactFrame` |
| saque | 10 | no | bote, `contactFrame` |
| remate/bandeja (compartido en el MVP) | 8 | no | `contactFrame` |
| recuperación | 4 | no | — |

Después del MVP se añaden: globo propio, bandeja y víbora diferenciadas, smash x3/x4, recibir impacto, celebración y derrota.

**Regla de timing:** el `contactFrame` coincide con el tick de contacto de la simulación. El estiramiento permitido por fase es de 0,75× a 1,35× [P]. Si se supera ese margen, se elige otra variante del golpe.

## 3. Coste de producción (VISUAL_RESEARCH §E.1)

Set completo de unos 120 frames por dirección:
- 4 direcciones: 360 frames por personaje;
- 8 direcciones: 600;
- 16 direcciones: 1 080.

Con espejo. MVP: aproximadamente 104 frames por dirección (tabla §2) × 5 únicas = **~520 frames** para el primer personaje. Es la razón de hacer **un solo personaje** en el vertical slice.

## 4. Elenco (pendiente)

Cuatro personajes únicos, cada uno con:
- silueta reconocible a 32 px;
- paleta propia dentro de la general;
- lateralidad;
- personalidad de juego (`AIProfile`).

No se copian personajes, marcas ni jugadores reales.

## 5. Pipeline

Ver skills `pixel-art-pipeline` y `2p5d-character-pipeline`, y ADR-010. El flujo es:

```
.pxo (Pixelorama) → export (.aseprite o PNG + JSON) → Assets/_Project/Art/Characters/ → AnimationSet
```

Se permite usar un modelo 3D como **referencia**, siempre que cada frame se redibuje a mano y el uso se registre en ASSET_PROVENANCE.
