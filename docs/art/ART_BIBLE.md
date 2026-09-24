# ART BIBLE

- **Estado:** 🟡 **BORRADOR — dirección visual pendiente de elección del propietario.** Sin esa elección no se produce arte definitivo (brief §5).
- **Versión:** 0.1
- **Fecha:** 2026-09-24
- **Fuentes:** [`VISUAL_RESEARCH.md`](../research/VISUAL_RESEARCH.md), ADR-002, ADR-003, ADR-005, ADR-009, ADR-010

Este documento tiene dos partes:
- **Parte A — Reglas técnicas del lenguaje visual.** Valen para cualquier dirección y ya están vigentes.
- **Parte B — Dirección seleccionada.** Se completará cuando se elija.

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

## Parte B — Dirección seleccionada

**Pendiente.** Opciones propuestas (detalle, paletas hex y análisis en VISUAL_RESEARCH §D):

| Dirección | Pilar | Dificultad | Diferenciación |
|---|---|---|---|
| D1 "Sobremesa" | Luz rasante mediterránea a última hora, sombras violetas, cal y terracota | 3/5 | 4/5 |
| D2 "Luz de Mástil" | Partido nocturno de club de barrio, focos LED frente a farolas de sodio | 3/5 | 3/5 |
| D3 "Polígono" | Pádel indoor en una nave industrial | 2/5 | 4/5 |
| D4 "Tinta Riso" | Estética de cartel risográfico, 4 tintas y trama | 4/5 | 5/5 |
| D5 "Vitrina" | La pista como vitrina de cristal flotante | 3/5 | 4/5 |

**Recomendación del equipo:** D1, con D2 como variante horaria (mismo club y mismos assets) y, de D5, las marcas persistentes en el cristal. Motivos: VISUAL_RESEARCH §D.7.

Al elegirse, esta sección se completa con:
- mood;
- paleta definitiva;
- tratamiento de escenario, personajes, UI y VFX;
- iluminación por hora;
- láminas de referencia propias;
- una captura objetivo del VisualLab.
