# GDD — Game Design Document

- **Versión:** 0.1 (Fase 0)
- **Fecha:** 2026-09-24
- **Estado:** borrador de visión. Las mecánicas se detallan en `GAMEPLAY_SPEC.md` y el arte en `docs/art/`.
- **Título de trabajo:** *padelgame* (TEMP: el nombre final se decide junto con la identidad visual)

## 1. Visión

Un juego de pádel en tercera persona con **personajes pixel art dibujados a mano dentro de un club 3D con luz propia**. Cada punto debe sentirse como pádel de verdad: las paredes forman parte del juego, la pareja se mueve como una unidad y un globo bien tirado te devuelve la red.

> **Frase de captura:** "Se reconoce en una captura: un club de pádel a última hora de la tarde, sombras violetas larguísimas sobre la pista y cuatro jugadores pixel art que parecen dibujados para ese sitio."
> (Depende de la dirección visual que se elija; ver §7.)

## 2. Pilares

1. **Pádel auténtico, entendible.** Reglas FIP 2026 por defecto (Star Point). Paredes y cristal legibles. La profundidad sale del pádel real (pared, cobertura, presión, definición), no de mecánicas inventadas.
2. **Golpear se siente bien.** Timing preciso, *hitstop* breve, sonido de foley real, una pelota con física creíble y trayectorias que se leen.
3. **Tu pareja importa.** Un compañero IA que entiende la formación, cubre y sube a la red contigo. Es el hueco de mercado detectado (GAMEPLAY_RESEARCH §1).
4. **Identidad visual propia.** Mundo, personajes, cámara, materiales, luz, UI, VFX y audio pertenecen al mismo universo (brief §45).
5. **Poco contenido, muy pulido.** Una pista excelente mejor que diez mediocres (brief §47).

## 3. Público y plataforma

- **Público:** jugadores de pádel aficionados (España, Argentina y mercados en expansión) y jugadores de juegos deportivos arcade o de simulación ligera.
- **Plataforma inicial:** PC (Windows/macOS/Linux), con mando y teclado. Se tiene en cuenta Steam Deck (1280×800) en la UI y la resolución (ADR-002). Consolas: fuera de alcance por ahora.

## 4. Bucle de juego

```
Punto:   saque → resto → intercambio (fondo ⇄ red, paredes) → definición → punto
Juego:   puntos → juego (Star Point en iguales) → cambio de saque / lado
Partido: juegos → sets → tie-break → partido
Meta (post-MVP): torneo corto de club, desbloqueo de personajes del club
```

Aprendizaje (brief §46): movimiento → posicionamiento → preparación → golpe → recuperación, y después pared → cobertura → presión → ataque → defensa → definición.

## 5. Modos

| Modo | Fase | Jugadores |
|---|---|---|
| 1v1 contra IA | Vertical slice (Fase 3) | 1 humano |
| 2v2 con compañero IA contra IA | Fase 4 | 1 humano |
| 2v2 local | Fase 4 | 2–4 humanos + IA en los huecos |
| Online | **Fuera del MVP** (ADR-008) | — |

### Formato 1v1 (decisión de diseño abierta)

No se encontró un formato individual oficial de la FIP (PADEL_RULES_RESEARCH §8).

- **Vertical slice:** pista oficial de 20×10 m con las reglas de dobles aplicadas a un jugador por lado. Es la única geometría con dimensiones verificadas. El saque sigue siendo en diagonal.
- **Playtest de la Fase 3:** se compara con una variante estrecha "individual", marcada como no oficial y `TEMP` hasta encontrar una fuente. `CourtDefinition` admite la variante sin tocar código.

## 6. Alcance del MVP

**Dentro:**
- 1 pista (el club);
- 4 personajes jugables únicos (Fase 6; 1 en el vertical slice);
- 1v1 y 2v2;
- IA en 3 niveles;
- reglas FIP 2026 (Star Point), punto de oro y ventaja clásica;
- HUD de marcador;
- menú mínimo;
- foley propio.

**Fuera:** tienda, skins, online, carrera, editor de personajes, más de una pista y más de 4 personajes (brief §35).

## 7. Identidad visual

**Pendiente de la elección del propietario** entre las direcciones de `VISUAL_RESEARCH.md` §D. Recomendación del equipo:

- **D1 "Sobremesa"** como identidad principal;
- **D2 "Luz de Mástil"** como variante nocturna del mismo club.

Hasta la elección, `ART_BIBLE.md` es un borrador y no se produce arte definitivo.

## 8. Identidad sonora

El pádel suena distinto al tenis: el "toc" seco de la pala, el golpe grave del cristal, el traqueteo metálico de la malla y el chirrido de la zapatilla en el césped. Sonidos propios grabados en pista (ADR-010). Ambiente de club: chicharras, conversación lejana, piscina y focos zumbando de noche. Lista de sonidos en `docs/art/ART_BIBLE.md` §Audio (pendiente).

## 9. Referencias de diseño (analizar, no copiar)

GAMEPLAY_RESEARCH §1:
- Mario Tennis: familias de golpe y carga.
- TopSpin 2K25: feedback de timing.
- Virtua Tennis: accesibilidad.
- Lethal League: lectura de velocidad y hitstop.
- Golf Story / Sports Story: lo que conviene evitar (exigencia de posicionamiento).
- Críticas a Tennis World Tour: golpes sin contacto visible.

## 10. Riesgos de diseño

| Riesgo | Mitigación |
|---|---|
| Las paredes no se entienden | Sombra de la pelota siempre visible, predicción visual opcional en dificultad fácil, cámara que encuadra la pared de fondo rival |
| El 1v1 en 20×10 es agotador o aburrido | Variante de pista y ajuste de `PlayerStats` en playtest |
| La IA compañera frustra | Debug views, métricas de cobertura, personalidades por datos |
| Competidores lanzan antes | Diferenciarse por el compañero IA y la identidad visual, no por la cantidad de modos |
