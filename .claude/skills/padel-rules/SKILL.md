---
name: padel-rules
description: Official FIP 2026 padel rules and the decoupled scoring system (CLASSIC, GOLDEN_POINT, STAR_POINT, CUSTOM_ARCADE). Use when implementing or testing Padel.Rules - scoring, deuce/advantage, Star Point, golden point, games, sets, tie-break, super tie-break, serve order, serve faults/lets, change of ends, point-won/point-lost conditions (walls, glass, mesh, net, double bounce).
---

# Reglas de pádel (FIP 2026) para `Padel.Rules`

Fuente de verdad: `docs/research/PADEL_RULES_RESEARCH.md`. Consultar allí el nivel de evidencia de cada regla: V1 es un extracto del PDF FIP 2026; NV es no verificado.

## Principios de implementación

- `Padel.Rules` es C# puro (`noEngineReferences`). La entrada son **eventos de pelota y de golpe**; la salida es el **resultado del punto** y el nuevo `ScoreState`. No conoce la física.
- El sistema de puntuación se configura como **datos** (`MatchRules` → `RulesConfig`) y nunca va en código condicional repartido:
  - `deuceMode`: `ADVANTAGE` | `GOLDEN_POINT` | `STAR_POINT`
  - `maxAdvantages`: 0 = punto de oro, 2 = Star Point, ∞ = clásico
  - `gamesPerSet`, `setsToWin`, `tiebreakAt`, `tiebreakPoints`
  - `finalSetMode`: `TIEBREAK` | `ADVANTAGE_SET` | `SUPER_TIEBREAK(10)`
- **Por defecto**: `STAR_POINT` ("Reglas oficiales FIP 2026").

## Star Point (aprobado por la FIP el 28-11-2025 y vigente en 2026)

```
40-40 → DEUCE_1 → ADV_1(X) → gana X: juego | pierde: DEUCE_2 → ADV_2(X) → gana: juego | pierde: DEUCE_3 = STAR POINT (punto decisivo)
```

- En el punto decisivo (Star Point o punto de oro) **la pareja receptora elige el lado** y no puede intercambiar posiciones. En mixto, el punto se juega entre jugadores del mismo sexo.
- Un juego tiene un máximo de 11 puntos.
- No se aplica en tie-break ni en súper tie-break.

## Tie-break

- Se juega a 7 puntos con diferencia de 2.
- **Orden de saque:** el primer punto lo saca S0 desde la derecha. Después, bloques de 2 puntos por sacador siguiendo el orden; el primer punto de cada bloque se saca desde la izquierda.
- **Cambio de lado** cada 6 puntos.
- El set se anota 7-6.
- El súper tie-break (a 10) es opcional y sustituye al tercer set.

## Reglas del punto (resumen)

**Pierde el punto quien golpea si:**
- la pelota golpea la pared o la malla rival antes de botar en el suelo rival;
- hay doble bote;
- hay doble golpe;
- el jugador toca la red o el poste.

**Gana quien golpea si,** tras botar en el campo rival, la pelota:
- se queda en la malla;
- atraviesa un agujero;
- se queda encima de una pared.

**Saque:**
- de abajo, a la cintura o por debajo, en diagonal, con dos intentos;
- si bota en el cuadro y después toca la malla, es falta;
- si bota en el cuadro y después toca el cristal, sigue en juego;
- es let si toca la red o el poste, entra en el cuadro y no toca la malla antes del segundo bote.

## Lagunas (KI-003): no inventar

No hay fuente verificada para:
- el cambio de lado al terminar un set;
- el primer sacador después de un tie-break;
- la posición exacta del pie en el saque.

Usar la convención documentada en PADEL_RULES_RESEARCH §10.5, marcada `[P]`, y dejarla configurable.

## Tests obligatorios

`docs/qa/TEST_PLAN.md` §2: puntuación, cambio de saque, juegos y sets, tie-break, cambio de lado y reglas del punto. Cada regla nueva debe llevar al menos un test positivo y uno negativo.
