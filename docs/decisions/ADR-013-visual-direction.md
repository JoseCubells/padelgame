# ADR-013 — Dirección visual

- **Estado:** Aceptado. Paleta y valores provisionales hasta el VisualLab de la Fase 2.
- **Fecha:** 2026-09-24
- **Decisores:** propietario del proyecto (aprobó la recomendación del equipo con "hazlo, instala lo necesario y continua" el 2026-09-24) y director técnico
- **Depende de / afecta a:** ART_BIBLE (parte B), CHARACTER_BIBLE, COURT_BIBLE, UI_BIBLE, ADR-002, ADR-003

## Pregunta

¿Cuál de las direcciones visuales propuestas define la identidad del juego?

## Fuentes consultadas

[`VISUAL_RESEARCH.md` §A y §D](../research/VISUAL_RESEARCH.md): análisis de referencias y cinco direcciones originales con paleta, tratamiento, ventajas, problemas, dificultad y diferenciación.

## Alternativas evaluadas

| Dirección | Dificultad | Diferenciación | Riesgo principal |
|---|---|---|---|
| **D1 "Sobremesa"** | 3/5 | 4/5 | Sombras de billboard con luz rasante |
| **D2 "Luz de Mástil"** | 3/5 | 3/5 | Parecer synthwave; monótona si es la única |
| D3 "Polígono" | 2/5 | 4/5 | Aspecto gris |
| D4 "Tinta Riso" | 4/5 | 5/5 | Trama frente a legibilidad de la pelota |
| D5 "Vitrina" | 3/5 | 4/5 | Transparencia y ordenación |

## Decisión

- **D1 "Sobremesa"** es la identidad principal.
- **D2 "Luz de Mástil"** es su variante nocturna: mismo club y mismos assets.
- De D5 se toman las **marcas persistentes de pelota en el cristal**.
- D3 se usa solo como **graybox de control** con luz uniforme, para separar los problemas de píxel de los de luz.
- D4 queda como posible prueba de shader de un día, fuera del plan.

## Motivo

- Representa la cultura de club de pádel sin tópicos.
- La identidad se apoya en la luz y no en la cantidad de assets, lo que escala bien para un equipo pequeño.
- Las sombras violetas contrastan con la pelota amarilla.
- La variante nocturna da variedad a coste casi cero.
- Evita el look HD-2D de diorama.

(VISUAL_RESEARCH §D.7)

## Riesgos

- Las sombras largas de billboards con luz rasante son el caso técnico más difícil. Mitigación: spike de ADR-003 con blob de sombra como garantía.
- A contraluz, la pelota puede perder contraste. Mitigación: contorno, tamaño mínimo y color reservado (ADR-002), validados en el VisualLab.

## Consecuencias

- La parte B de ART_BIBLE queda completa en v0.2.
- La escena VisualLab de la Fase 2 se monta con la luz de la tarde.
- KI-011 queda cerrado.

## Cómo revertirla

Hasta el COURT_ART_PASS (Fase 6), cambiar de dirección solo afecta a la paleta, las rampas y los `VisualProfile`. Después, habría que rehacer el arte de la pista.

## Historial

- 2026-09-24: aceptado por delegación del propietario.
