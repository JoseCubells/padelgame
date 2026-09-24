# ART BIBLE

- **Estado:** 🟢 **Dirección elegida: D1 "Sobremesa" + D2 "Luz de Mástil"** (ADR-013). Los valores de la parte B son **v0.2 provisional** hasta el VisualLab de la Fase 2, que los validará con capturas.
- **Versión:** 0.2 (0.1 → 0.2: dirección elegida)
- **Fecha:** 2026-09-24
- **Fuentes:** [`VISUAL_RESEARCH.md`](../research/VISUAL_RESEARCH.md), ADR-002, ADR-003, ADR-005, ADR-009, ADR-010

Este documento tiene dos partes:
- **Parte A — Reglas técnicas del lenguaje visual.** Valen para cualquier dirección y ya están vigentes.
- **Parte B — Dirección seleccionada** (ADR-013).

---

## Parte A — Reglas vigentes (independientes de la dirección)

### A.1 Principio

WORLD + CHARACTERS + CAMERA + MATERIALS + LIGHTING + UI + VFX + AUDIO pertenecen al mismo universo. Una captura tiene que bastar para reconocer el juego.

**Prohibido como solución final:**
- modelos, animaciones y texturas genéricas;
- Mixamo;
- assets de tienda usados "porque existen";
- filtros que conviertan en pixel art un render normal;
- PBR genérico;
- bloom sin intención;
- primitivas como aspecto final.

### A.2 Pixel grid

- Mundo y personajes se renderizan a una **resolución interna fija**. Candidatas: 480×270 y 640×360, con 320×180 como control. Se escalan a pantalla con **factor entero** y filtro point (ADR-002).
- **Una sola densidad de píxel en el mundo:** personajes, texturas de pista y decals se crean para la misma densidad objetivo en px/m a la distancia media de la pista. Referencia inicial: unos 21 px/m a 480×270 con la cámara C (VISUAL_RESEARCH §C.3). El valor final se fija tras la prueba de resolución.
- Sin anti-aliasing en la pasada de mundo. Sin blur, sin mipmaps en sprites y con filtro point en todas las texturas pixel.
- Ningún elemento puede mostrar píxeles de distinto tamaño en el mismo plano. **Excepción documentada:** texto largo y menús a resolución nativa (UI_BIBLE).

### A.3 Paleta

- Paleta cerrada por dirección, con un máximo de 48 colores de uso general y rampas de 3 a 5 tonos por material.
- **Color reservado para la pelota** (amarillo óptico): no aparece en ningún otro elemento del juego.
- Las sombras no son negras: son un tono de la paleta (por ejemplo, un violeta o un azul) definido por la dirección.

### A.4 Luz y sombra

- Personajes y mundo usan **iluminación cuantizada o por rampa** (unlit + rampa + atenuación de sombra de 2 niveles + rim). No se usa PBR estándar.
- **Toda entidad que pisa el suelo tiene sombra blob**, y la pelota la tiene siempre. La sombra real de la luz principal se añade como ambiente.
- Un personaje nunca flota: pivote en los pies y blob de contacto.

### A.5 Contornos

- **Personajes:** contorno horneado de 1 px con selout (el tono oscuro del propio color, no negro).
- **Mundo:** contornos por shader solo en siluetas clave (red, marcos, líneas de pista).
- **Pelota:** contorno de 1 px garantizado y tamaño mínimo en pantalla de 3 a 4 px a 270p.

### A.6 Placeholders

Todo placeholder:
- lleva el prefijo `TEMP_`, `GRAYBOX_` o `PLACEHOLDER_`;
- vive en `Assets/_Project/Prototype/`;
- se lista en `docs/qa/KNOWN_ISSUES.md` con la etiqueta **REPLACE BEFORE ART LOCK**.

### A.7 Checklist de validación visual

Se aplica en cada cambio visual importante (brief §30; skill `visual-quality-review`):

1. Abrir la escena, ejecutar y capturar la Game View (RT interna + frame final).
2. Comparar con esta biblia:
   - consistencia del píxel;
   - escala;
   - contraste;
   - composición;
   - profundidad;
   - lectura del jugador y de la pelota;
   - sombras;
   - materiales;
   - UI.
3. **Rechazar si aparece:**
   - blur;
   - sprites borrosos;
   - clipping;
   - z-fighting;
   - luz inconsistente;
   - personaje flotando;
   - sombra incorrecta;
   - cámara que pierde la pelota;
   - elementos sin identidad.

---

## Parte B — Dirección seleccionada: "Sobremesa" (tarde) / "Luz de Mástil" (noche)

**Decisión:** ADR-013. Las opciones descartadas y su análisis se conservan en VISUAL_RESEARCH §D.

### B.1 Concepto

**Un solo club, dos horas.** Club de urbanización mediterráneo o rioplatense (ficticio, con nombre y branding propios), a dos horas del día:

- **Tarde, "Sobremesa":** las 18:30 de julio. La luz rasante y dorada deja sombras violetas larguísimas. Nostalgia cálida, sin postal turística. **Es el escenario principal del vertical slice.**
- **Noche, "Luz de Mástil":** la pista es una isla de luz bajo cuatro mástiles LED; al fondo, farolas viejas de sodio. Íntimo y tenso. Se añade en la Fase 6 con los **mismos assets**.

El pilar de identidad es **la luz y el urbanismo de club**, no el folclore. Prohibido: toros, flamenco, banderas y postal turística.

### B.2 Paleta v0.2 (provisional)

| Rol | Tarde | Noche |
|---|---|---|
| Cal / tapia | `#F1E6D2`, sombra `#D8C7A8` | iluminado `#E9EEF2` |
| Terracota | `#C4553B`, sombra `#8E3B2E` | — (silueta) |
| Pista | `#2F6F8F`, sombra `#1F4A63` | `#1E4F8A` |
| Sombra / ambiente | **violeta `#4B3A6B`** | noche `#0E1430`, relleno `#1C2750` |
| Vegetación | `#7A9A3A` | silueta |
| Luz | rasante `#E7A04A` | LED `#9FE8FF`, sodio `#FFB45A` |
| Metal de estructura | (se deriva de la rampa de la cal) | `#3B3F4A` |
| **Pelota (reservado)** | **`#E4F53A`** | **`#E4F53A`** |

Reglas:
- en la noche no se usa magenta (riesgo "synthwave");
- el amarillo de la pelota no aparece en ningún otro elemento;
- la paleta final (≤ 48 colores) se cierra en la Fase 2 como `ArtSource/palettes/sobremesa.gpl`.

### B.3 Escenario

Pista FIP (COURT_BIBLE) rodeada de:
- tapias encaladas;
- pinos y adelfas;
- toldo de lona;
- banco de obra;
- fuente de agua;
- marcador físico;
- una **piscina fuera de campo** que devuelve reflejos de luz.

Los cristales conservan **marcas persistentes de la pelota** (rasgo tomado de D5).

### B.4 Personajes

- Ropa de club: polos, gorras, viseras, cintas.
- Paleta propia por equipo.
- Luz: **rim cálido lateral** por la tarde y **rim frío** con sombras múltiples por la noche.
- Contorno de selout (ver A.5).

### B.5 UI

- Tarde: marcador como **cartel de chapa rotulado a mano** (tipografía pixel de rotulista), con estadísticas a tiza.
- Noche: la misma información en **marcador LED de matriz de puntos**, diegético.

### B.6 VFX

Todo en pixel art, a la resolución interna:
- polvo dorado al frenar;
- destello seco en el impacto;
- polillas al anochecer;
- halos de foco por la noche;
- marcas de pelota en el cristal.

### B.7 Iluminación

- **Tarde:** luz principal cálida a **15–20° de elevación** y ambiente violeta. **Las sombras largas son el sello.** Es el caso técnico más difícil para billboards, y por eso se hace el spike de sombras en ADR-003.
- **Noche:** 4 focales en mástiles y ambiente casi negro. Hay que medir el coste de las sombras múltiples en URP (Fase 10).

### B.8 Pixel art

- Resolución interna candidata: 480×270 (la final se decide en la Fase 2, ADR-002).
- 3–4 tonos por material con rampa.
- Contorno de selout, nunca negro.

### B.9 Captura objetivo (criterio de aceptación de la Fase 2)

Una captura del VisualLab al atardecer tiene que mostrar a la vez:
- pista azul con sombras violetas largas;
- tapia encalada al fondo;
- 2 personajes pixel art con rim cálido y sombra;
- la pelota amarilla perfectamente legible con su sombra.

Tiene que reconocerse sin logo.
