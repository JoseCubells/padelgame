# UI BIBLE

- **Estado:** 🟡 BORRADOR técnico. El estilo gráfico espera la dirección visual.
- **Versión:** 0.1
- **Fecha:** 2026-09-24
- **Fuentes:** ADR-002 (§UI), VISUAL_RESEARCH §B.12, TOOLS_RESEARCH §5

## 1. Capas de UI

| Capa | Resolución | Contenido |
|---|---|---|
| **HUD de juego** | Escala entera del píxel del mundo: mismo tamaño de píxel que la RT interna | Marcador, indicador de saque, feedback de timing, motivo del punto |
| **Menús y texto largo** | Resolución nativa | Menús, opciones, tutoriales, textos localizados |

## 2. Reglas

- Tipografía:
  - En el prototipo, una fuente **OFL**, que se registra en ASSET_PROVENANCE junto con su licencia.
  - En la versión final, una **fuente bitmap propia** hecha con FontForge, con cobertura completa de español, catalán y portugués.
- **Legibilidad antes que estilo:**
  - el marcador se lee en 1 segundo;
  - contraste mínimo WCAG AA en los textos de menú.
- **Sin UI genérica:** nada de paneles grises por defecto, iconos de stock ni plantillas de Unity UI.
- **Marcador:** muestra sets, juegos y puntos, más el indicador de **Star Point** (iguales 1, 2 y 3) cuando se usa ese sistema. Es un elemento de identidad. En D1, por ejemplo: un cartel de chapa rotulado a mano.
- **Feedback de golpe:** Early / Perfect / Late, discreto y cerca del jugador, y opcional.
- **Accesibilidad:** escala de UI, opción para reducir los destellos y remapeo completo de controles (Input System).

## 3. Tecnología

**Pendiente de ADR** en la Fase 8. Opciones:
- **UI Toolkit**: es la skill oficial `ui-uitk` y está disponible en el repositorio de Unity.
- **uGUI.**

Se decide con evidencia sobre el renderizado pixel-perfect del HUD a escala entera.
