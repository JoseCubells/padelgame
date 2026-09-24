---
name: visual-quality-review
description: Visual validation procedure after any significant visual change (brief §30) - open scene, run, capture Game View (internal RT + final frame) and Scene View via Unity MCP or CLI, store evidence, and review against ART_BIBLE checklist (pixel consistency, scale, contrast, composition, depth, player and ball readability, shadows, materials, UI). Use after changing shaders, rendering, camera, sprites, court art, lighting, VFX or UI.
---

# Revisión de calidad visual

Un cambio visual no está terminado hasta pasar esta revisión con evidencia.

## Procedimiento

1. **Preparar**: árbol de trabajo limpio y commit previo. Abrir la escena afectada (`VisualLab`, `Court_Graybox`…).
2. **Ejecutar** en Play Mode con un estado reproducible: semilla fija y, si aplica, un replay de comandos.
3. **Capturar**:
   - Game View a **resolución interna** (la RenderTexture);
   - **frame final** escalado;
   - Scene View;
   - si hay movimiento, un clip corto para detectar *pixel crawl*.

   Con el MCP oficial: `capture_game_view` y `capture_scene_view`. Con el CLI: `unity command screenshot --output <ruta> --width <w> --height <h>` (ver skill `unity-cli`). Comprobar que la captura es de la vista y no del escritorio.
4. **Guardar** en `docs/qa/evidence/<AAAA-MM-DD>_<tema>/` (los PNG van por LFS).
5. **Revisar** contra `docs/art/ART_BIBLE.md` A.7:
   - consistencia del píxel (un único tamaño, sin blur);
   - escala;
   - contraste;
   - composición;
   - profundidad;
   - **lectura del jugador y de la pelota** (tamaño mínimo, contorno, sombra blob);
   - sombras (sin personajes flotando);
   - materiales con dirección artística;
   - UI.
6. **Rechazar** si aparece:
   - blur;
   - sprites borrosos;
   - clipping evidente;
   - z-fighting;
   - iluminación inconsistente;
   - personaje flotando;
   - sombra incorrecta;
   - cámara que pierde la pelota;
   - un elemento sin identidad (genérico).
7. **Registrar**: una línea en el CHANGELOG del milestone con capturas, resultado y problemas encontrados. Los problemas no resueltos van a `KNOWN_ISSUES.md`.

## Si no hay Unity en el entorno

No se puede validar visualmente. Hay que decirlo explícitamente, dejar el paso pendiente en KNOWN_ISSUES y **no** declarar el cambio como terminado.
