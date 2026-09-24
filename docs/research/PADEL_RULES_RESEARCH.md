# Investigación de reglas y datos físicos del pádel

- **Fecha de la investigación:** 2026-09-24
- **Objetivo:** valores por defecto de `CourtDefinition` / `BallDefinition` y reglas de puntuación configurables (`CLASSIC_SCORING`, `GOLDEN_POINT`, `STAR_POINT`, `CUSTOM_ARCADE_SCORING`) según el reglamento FIP **vigente** (revisión con aplicación desde el 01.01.2026).
- **Autor:** investigador de reglas y física (agente), para el estudio de videojuego.

---

## 0. Advertencia metodológica (leer primero)

Durante esta sesión el proxy de salida **bloqueó la descarga directa** de todos los dominios consultados (`www.padelfip.com`, `web.archive.org`, `padelfederacion.es`, `knltb.nl`, `padeladdict.com`, `padelnuestro.com`, `padelandia.com`, `wikipedia.org`, `pmc.ncbi.nlm.nih.gov`, `padel.how`). Con `curl` el túnel CONNECT devolvió 403 y WebFetch devolvió `EGRESS_BLOCKED`. Después se agotó el cupo de búsquedas web de la sesión.

Por eso **no se ha leído completo ningún PDF oficial**. Los datos de la FIP de este documento proceden de **fragmentos del texto de los PDF oficiales** (dominio `padelfip.com`) que devolvía el motor de búsqueda, que indexa esos PDF. Cada regla lleva un nivel de verificación:

| Código | Significado |
|---|---|
| **V1** | Texto del PDF oficial de la FIP 2026 recuperado mediante un extracto del buscador restringido a `padelfip.com`. El PDF no se ha leído completo y el número de artículo no está confirmado. |
| **V2** | Nota de prensa o noticia oficial de la FIP o de Premier Padel (extracto del buscador). |
| **V3** | Prensa especializada o sitios de terceros. |
| **NV** | **No verificado en esta sesión.** Es conocimiento previo del investigador o una práctica habitual. Hay que contrastarlo con el PDF antes de congelarlo en el código. |
| **DER** | Valor derivado por cálculo a partir de un dato V1. |

**Acción recomendada:** que una persona con acceso descargue `FIP_Reglas-del-Padel-1.pdf` / `FIP_Rules-of-Padel.pdf` (enlaces abajo) y sustituya cada NV por la cita literal con su número de artículo.

---

## 1. Jerarquía de fuentes y versiones de los documentos

1. **FIP: Reglas del Pádel / Rules of Padel, revisión con aplicación desde el 01.01.2026** (documento base de este informe)
   - ES: <https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Reglas-del-Padel-1.pdf> (y variante `FIP_Reglas-del-Padel.pdf`)
   - EN: <https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf> (portada: *"Review of application 01.01.2026"*) y `FIP_Rules-of-Padel-1.pdf`
   - Existe además <https://www.padelfip.com/wp-content/uploads/2024/11/FIP-Rules-of-Padel.pdf> con el título *"In force as of 1.01.2026"*. Hay que confirmar cuál de las dos versiones de 2026 es la definitiva. La de diciembre de 2025 es la posterior y es la que incorpora el Star Point.
   - Índice de documentos de la FIP: <https://www.padelfip.com/documents/>
2. **Comunicados oficiales de la FIP sobre el Star Point:**
   - Artículo: <https://www.padelfip.com/2025/12/between-innovation-and-tradition-introducing-the-star-point-the-scoring-system-that-appeals-to-everyone/> (diciembre de 2025)
   - Nota de prensa conjunta FIP–Premier Padel (18-12-2025): <https://www.padelfip.com/2025/12/premier-padel-and-international-padel-federation-unveil-2026-qatar-airways-premier-padel-tour-calendar-and-innovative-new-star-point-system-launched/>. PDF: <https://www.padelfip.com/wp-content/uploads/2025/12/18-december_Star-Point_ENG.pdf>. Copia en Premier Padel: <https://premierpadel.com/en/news/premier-padel-and-international-padel-federation-unveil-2026-qatar-airways-premier-padel-tour-calendar-and-innovative-new-star-point-system-launched>
   - FIP Promises 2026: <https://www.padelfip.com/2025/12/fip-promises-new-regulations-published-effective-from-2026/>
3. **Reglamentos de circuito (FIP / Premier Padel):**
   - Premier Padel, Official Rulebook Men's Tournaments, versión 2026: <https://www.padelfip.com/wp-content/uploads/2025/03/Premier-Padel-Rulebook-Men%C2%B4s_EN.pdf>
   - CUPRA FIP Tour, Official Rulebook, versión 2026: <https://www.padelfip.com/wp-content/uploads/2025/03/Cupra-FIP-Tour-Rulebook_EN-1-1-2.pdf>
   - FIP Beyond Rulebook (diciembre de 2025): <https://www.padelfip.com/wp-content/uploads/2026/02/FIP-Beyond-Rulebook-EN-1.pdf>
   - FIP Promises, reglamento ES (versiones 19-12-25, 09-01-26, 10-02-26 y 19-03-26): p. ej. <https://www.padelfip.com/wp-content/uploads/2025/12/ES-Reglamento-Circuito-FIP-Promises-19_03_2026.pdf>
   - Pelotas homologadas por la FIP (2026): <https://www.padelfip.com/wp-content/uploads/2024/04/pelotas-2026.pdf> y el proceso de certificación <https://www.padelfip.com/wp-content/uploads/2026/04/Game-Ball-Certification-Process-2.pdf>
4. **Federaciones nacionales:** Reglamento de juego de la FEP <https://www.padelfederacion.es/refs/docs/REGLAMENTO_JUEGO_FEP.pdf>. **No accesible** (bloqueado). No se ha consultado ningún documento de la Asociación Pádel Argentino.
5. **Prensa (V3):** padeladdict.com, padelnuestro.com, padelspain.net, padelbiz.it y otros. Solo se usa para dar contexto y siempre se marca.

---

## 2. Pista

| # | Regla | Verif. | Fuente |
|---|---|---|---|
| 2.1 | Rectángulo de **10 m de ancho × 20 m de largo** (medidas interiores), con una **tolerancia del 0,5 %**. La red lo divide por la mitad. | V1 | [FIP Reglas 2026](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Reglas-del-Padel-1.pdf) |
| 2.2 | **Fondos:** altura total de **4 m**. Los **primeros 3 m son pared** (de cualquier material transparente u opaco: cristal, ladrillo, etc., que cumpla lo exigido para las paredes) y **el último metro es malla metálica**. | V1 | [FIP Rules 2026](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf) |
| 2.3 | **Laterales, variante 1 (escalonada):** en ambos extremos, un primer escalón de pared de **3 m de alto × 2 m de largo** y un segundo escalón de **2 m de alto × 2 m de largo**. La malla metálica completa el cerramiento **hasta 3 m en los 6 m centrales**. En el tramo de 2 m pegado al fondo, la malla completa hasta 4 m, en continuidad con el fondo (este último dato es NV). | V1 / NV | [FIP Rules 2026](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf) |
| 2.4 | **Laterales, variante 2 (cristal sin escalón):** en cada extremo, una pared de **3 m de alto × 4 m de largo** sin escalón. La malla completa el cerramiento **hasta 4 m en los 2 m extremos**. La altura de la malla en el resto del lateral (lo habitual es 3 m) es NV. | V1 / NV | idem |
| 2.5 | **Malla metálica:** rombos soldados. La diagonal del hueco mide **como mínimo 5 cm y como máximo 7,08 cm**. Se recomienda un alambre de **2 a 3 mm** de grosor. | V1 | idem |
| 2.6 | **Cristal:** vidrio templado conforme a las normas aplicables. Remite al *Anexo de Homologación de Pistas* de la FIP. El grosor (10/12 mm) y el tamaño de los paneles (el panel típico mide 2 × 3 m o 2 × 1 m en el escalón) **no se han podido extraer** y son NV. | V1 (remisión) / NV | idem |
| 2.7 | **Red:** **10 m de largo**, **0,88 m de alto en el centro** y hasta **0,92 m como máximo en los extremos**, con una **tolerancia máxima de 0,005 m**. | V1 | [FIP Rules 2026](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf) |
| 2.8 | La red cuelga de un **cable metálico de 0,01 m de diámetro como máximo**. Sus extremos se sujetan a **dos postes laterales de 1,05 m de altura como máximo**. | V1 | idem |
| 2.9 | La red lleva en la parte superior una **banda blanca de 5,0 a 6,3 cm** que cubre el cable. La malla de la red es de fibra sintética y con un tejido lo bastante tupido para que **la pelota no pueda atravesarla**. | V1 | idem |
| 2.10 | **Líneas de saque:** paralelas a la red y a **6,95 m** de ella, a cada lado. La zona entre la red y la línea de saque se divide en dos mitades con la **línea central de saque**, perpendicular a la red. | V1 | idem |
| 2.11 | **Todas las líneas miden 5 cm de ancho.** Son preferiblemente **blancas o negras**, para que contrasten con el suelo. | V1 | idem |
| 2.12 | La inclusión del ancho de las líneas en las medidas (si los 6,95 m se miden hasta el borde exterior o hasta el eje) y la prolongación de la línea central más allá de la línea de saque (en pádel suele cruzar 0,20 m hacia el fondo) son **NV**. | NV | — |
| 2.13 | **Accesos:** en los dos laterales o en uno solo, y siempre **simétricos respecto al centro** (la red). **Un acceso por lado:** hueco mínimo de **1,05 × 2,00 m** y máximo de **2,20 × 2,20 m**. **Dos accesos por lado:** cada hueco mide como mínimo **0,72 × 2,00 m** y como máximo **1,10 × 2,20 m**. | V1 | [FIP Rules 2026](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf) |
| 2.14 | Los accesos y el poste de la red se protegen por sus tres lados con material amortiguador de **≥ 2 cm**. Si hay puertas, las manillas van por fuera y no sobresalen hacia dentro. | V1 | idem |
| 2.15 | **Juego exterior:** la pista necesita **2 accesos por lado**. Fuera de la pista no puede haber obstáculos en una zona de **al menos 3 m de ancho (se recomiendan 4 m) × 4 m de largo** a cada lado y **al menos 3 m de alto**. **Novedad de 2026:** antes el ancho era de 2 m. | V1 + V3 ([padeladdict](https://www.padeladdict.com/nuevas-normas-padel-2026-reglamento-fip/)) | idem |
| 2.16 | **Altura libre e iluminación:** los proyectores deben estar a **6 m del suelo como mínimo** (medidos hasta su parte inferior). En instalaciones nuevas se sugieren **8 m** si los proyectores quedan dentro de la proyección vertical de los laterales. La luz artificial debe ser uniforme y no deslumbrar. Para retransmisiones de televisión hacen falta **≥ 1000 lux verticales**. | V1 | idem |
| 2.17 | **Altura mínima de techo (obstáculos cenitales):** la búsqueda solo devolvió la altura de los proyectores (6 m). La cifra del techo o de otros obstáculos sobre la pista (es habitual citar ≥ 6 m, y 7 m u 8 m como recomendación) es **NV**. | NV | — |
| 2.18 | **Superficie:** hormigón poroso, cemento, sintético o césped artificial, entre otras, según el *Anexo de Homologación de Pistas* de la FIP. Debe dar un bote uniforme. Los colores del suelo (verde, azul o tierra) son **NV**: la FIP remite al anexo, que no se ha podido consultar. | V1 (remisión) / NV | idem |

## 3. Pelota

| # | Regla | Verif. | Fuente |
|---|---|---|---|
| 3.1 | Esfera de goma con la superficie exterior uniforme, de color **blanco o amarillo**, o de otro color que contraste con el suelo. | V1 | [FIP Rules 2026](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf) |
| 3.2 | **Diámetro:** de **6,35 a 6,77 cm**. | V1 | idem |
| 3.3 | **Masa:** de **56,0 a 59,4 g**. | V1 | idem |
| 3.4 | **Bote:** de **135 a 145 cm** al dejarla caer sobre una superficie dura desde **2,54 m**. | V1 | idem |
| 3.5 | **Presión interna:** de **4,6 a 5,2 kg por 2,54 cm²** (así figura en el extracto, que equivale a kgf/in²). Convertido: **≈ 69,9 a 79,1 kPa**, o ≈ 10,1 a 11,5 psi. La conversión es DER. Hay que confirmar con el PDF si es presión manométrica o absoluta. | V1 / DER | idem |
| 3.6 | **Altitud:** a **más de 800 m** sobre el nivel del mar puede usarse otra pelota, idéntica salvo en el bote, que debe ser **> 121,92 cm y < 135 cm**. | V1 | idem |
| 3.7 | En la competición oficial solo se usan pelotas homologadas (listado de 2026). | V1 | [pelotas-2026.pdf](https://www.padelfip.com/wp-content/uploads/2024/04/pelotas-2026.pdf) |

### 3.8 Datos físicos (simulación)

| Parámetro | Valor | Verif. | Nota |
|---|---|---|---|
| Coeficiente de restitución vertical contra suelo rígido, e = √(h_bote / h_caída) | **0,729 a 0,756** (valor central 0,742) | DER a partir de 3.4 | Medido contra una superficie rígida en laboratorio, **no** contra césped artificial con arena. |
| e (pelota de altitud) | 0,693 a 0,729 | DER a partir de 3.6 | |
| Sección transversal A | 31,67 a 36,00 cm² (valor central ≈ 33,8 cm², d = 6,56 cm) | DER | Para calcular F_d = ½·ρ·C_d·A·v² |
| Coeficiente de restitución contra el cristal | **Sin dato** | Laguna | No se ha podido consultar ninguna publicación. Ver §11. |
| Coeficiente de arrastre C_d | **Sin dato verificado** | Laguna | La literatura de la pelota de tenis suele situarlo en torno a 0,5 a 0,6 (NV). Úsese como punto de partida para calibrar, nunca como dato de pádel. |
| Coeficiente de sustentación por Magnus | **Sin dato verificado** | Laguna | Igual que el anterior. |
| Fricción en el bote (efecto del giro), contra el suelo y contra el cristal | **Sin dato** | Laguna | |

## 4. Pala

| # | Regla | Verif. | Fuente |
|---|---|---|---|
| 4.1 | Dos partes, cabeza y mango. **Longitud total (cabeza + mango) ≤ 45,5 cm. Anchura ≤ 26 cm.** | V1 | [FIP Rules 2026](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf) |
| 4.2 | **Mango:** longitud de **20 cm como máximo** y anchura de **50 mm como máximo** (en la garganta, sin contar el hueco). | V1 | idem |
| 4.3 | **Grosor:** en el control de medidas se admite una **tolerancia del 2,5 %** en el grosor. El valor máximo (lo habitual es 38 mm) **no aparecía en el extracto** y es NV. | V1 / NV | idem |
| 4.4 | **Agujeros:** en la zona central, un número ilimitado de agujeros cilíndricos de **9 a 13 mm**. En una franja perimetral de **≤ 4 cm** desde el borde pueden tener un diámetro mayor u **otra forma**, con largo y ancho variables, siempre que **no superen los 20 mm** (en 2026 se admiten formas no circulares). | V1 + V3 | idem; [padeladdict](https://www.padeladdict.com/nuevas-normas-padel-2026-reglamento-fip/) |
| 4.5 | **Cordón de seguridad:** no elástico, de **35 cm como máximo**, fijado al mango y **de uso obligatorio** alrededor de la muñeca. | V1 | idem |
| 4.6 | Si el cordón se rompe o la pala se vuelve peligrosa, puede cambiarse la pala, pero nunca durante un punto (V1). Según la prensa, en 2026 se pierde el punto si el cordón se rompe durante el punto (V3). La cita literal de la FIP no se ha podido confirmar. | V1 / V3 | [padelspain](https://www.padelspain.net/padel-profesional/826075/cambios-en-el-reglamento-fip-conoce-las-nuevas-reglas/) |

## 5. Reglas de juego

### 5.1 Saque

| # | Regla | Verif. |
|---|---|---|
| S1 | **Saque por abajo:** al golpear, la pelota debe estar **a la altura de la cintura o por debajo**, y el jugador debe tener **al menos un pie en contacto con el suelo**. | V1 [FIP Rules 2026](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel-1.pdf) |
| S2 | Hasta golpear la pelota, el sacador **no puede pisar con los pies la línea de saque, la prolongación imaginaria de la línea central ni el cuadro de recepción de su propio campo**. | V1 idem |
| S3 | Antes de golpear, el sacador bota la pelota en el suelo por detrás de la línea de saque (bote previo). No se ha extraído el texto literal. | NV |
| S4 | **Diagonal:** desde la mitad derecha, la pelota debe pasar en diagonal por encima de la red y **botar dentro del cuadro de saque del receptor** situado en diagonal. **Las líneas son buenas.** Después se alterna entre la mitad derecha y la izquierda en cada punto. | V1 idem |
| S5 | **Dos oportunidades:** si el primer saque es falta, hay un segundo. Si el segundo también es falta, es **doble falta** y el punto se pierde. | V1 |
| S6 | **Falta de saque** en estos casos: bota fuera del cuadro correcto; **bota en el cuadro y toca la malla metálica antes del segundo bote**; antes de botar en el cuadro del receptor toca la pared, la malla o una luz **del lado del sacador**; golpea al sacador, a su compañero o algo que lleven. | V1 |
| S7 | Si la pelota **bota en el cuadro y luego toca el cristal o la pared** (no la malla), el saque es **bueno** y la pelota sigue en juego. Esto se deduce de S6, que solo penaliza la malla. | V1 (por deducción) |
| S8 | En pistas **sin zona de seguridad** (donde no se autoriza el juego exterior), el saque que bota en el cuadro y sale de la pista directamente por una puerta figura en la lista de supuestos del extracto (**falta de saque**). Hay que confirmar con el texto íntegro en qué artículo está. | V1 parcial |
| S9 | **Let (se repite el saque):** la pelota toca la red o los postes, cae en el cuadro correcto y **no toca la malla antes del segundo bote**. Si el receptor demuestra que no estaba preparado, no puede cantarse falta. Hay que verificar la variante en la que la pelota toca la red y después golpea al receptor o a su compañero (let) (NV). | V1 / NV |
| S10 | **Resto:** el receptor debe dejar botar la pelota. No puede restar de volea. Si el saque golpea al receptor o a su compañero antes de botar, el extracto indica que se aplica una sanción al receptor. La redacción exacta está **por confirmar**. | V1 parcial |
| S11 | **Cambio de 2026 (según la prensa):** basta con tener un pie detrás de la línea sin invadir el cuadro, y se prohíbe explícitamente golpear dentro de la proyección del cuadro o en la proyección lateral. **No se ha confirmado en el extracto primario.** | V3 [padelnuestro](https://www.padelnuestro.com/blog/nuevas-reglas-padel-2026) |
| S12 | Entre puntos se permiten como máximo **20 s**. | V1 |

### 5.2 Orden de saque y de resto (dobles)

| # | Regla | Verif. |
|---|---|---|
| O1 | La elección de campo y de saque o resto se decide por sorteo (moneda). La pareja que gana el sorteo elige. | V1 |
| O2 | Las parejas comunican al árbitro quién saca primero. El orden de saque se mantiene durante todo el set. La secuencia es A1, B1, A2, B2, y así sucesivamente. | V1 parcial + NV (secuencia) |
| O3 | Cada jugador de la pareja receptora resta siempre desde el mismo lado (derecha, «drive», o izquierda, «revés») durante todo el set. La pareja puede cambiar el reparto al empezar un set nuevo. | NV |
| O4 | En el punto decisivo (de oro o Star Point), **la pareja receptora elige si resta por la derecha o por la izquierda**, pero **sus jugadores no pueden intercambiar las posiciones** establecidas. En un **partido mixto**, en ese punto resta **un jugador del mismo sexo que el sacador**. | V1 |
| O5 | Durante el punto no hay restricción de posición: los compañeros del sacador y del receptor pueden colocarse en cualquier parte de su campo. | NV |

### 5.3 Pelota en juego, punto ganado y punto perdido

| # | Regla | Verif. |
|---|---|---|
| J1 | Si, **después de botar en el campo contrario**, la pelota toca cualquier elemento de la pista (cara interior de las paredes, malla, suelo, red o postes), **sigue en juego** y hay que devolverla antes de que bote por segunda vez. | V1 |
| J2 | El jugador puede devolver la pelota **directamente o haciéndola rebotar en las paredes de su propio campo**. | V1 |
| J3 | **Se pierde el punto** si la devolución, directamente o tras tocar las paredes propias, **golpea las paredes del campo contrario, la malla o cualquier elemento ajeno sin haber botado antes en el suelo contrario**. | V1 |
| J4 | Se pierde el punto si la pelota **bota dos veces** antes de devolverla. | V1 |
| J5 | Se pierde el punto si un jugador, su pala o cualquier cosa que lleve **toca la red, los postes, el cable tensor o cualquier parte del campo contrario** con la pelota en juego. | V1 |
| J6 | Se pierde el punto si hay **doble golpe**. | V1 |
| J7 | Se pierde el punto si, tras el golpe, la pelota toca al propio jugador, a su compañero o algo que lleven. | V1 |
| J8 | Se pierde el punto si la pelota golpeada por el rival **toca a un jugador o su equipo en cualquier parte que no sea la pala**, aunque el jugador esté fuera de la pista. | V1 |
| J9 | Se pierde el punto si el jugador golpea la pelota y esta toca la **malla metálica o el suelo de su propio campo** (antes de pasar la red). | V1 |
| J10 | **La pelota toca la red o un poste y cae en el campo contrario:** la devolución es **buena**. | V1/V3 |
| J11 | **La pelota queda enganchada en la malla, sale por un agujero o desperfecto de la malla, o se queda quieta sobre la superficie horizontal de un muro, tras botar en el campo contrario:** **punto ganado** para quien la golpeó (regla 15, «Punto ganado», en la numeración del reglamento de juego anterior). | V1 (versión 2023) |
| J12 | **Sale de la pista o toca el techo, las luces u otro objeto ajeno tras botar en el campo contrario:** **punto ganado** para quien la golpeó, salvo que se autorice el juego exterior y la pelota salga por la zona permitida. | V1/V3 |
| J13 | **Juego exterior:** solo después de un **bote válido en el suelo** del campo del jugador. Se sale por los accesos laterales y hay que golpear la pelota **antes de su segundo bote en el suelo exterior**. Exige la zona de seguridad descrita en 2.15. La trayectoria de vuelta (si debe entrar directamente en el campo contrario sin tocar elementos exteriores) es NV. | V3 + V1 (requisitos) |
| J14 | **La pelota bota en el campo contrario y vuelve a pasar la red** por efecto o por viento: el rival puede **pasar el brazo o la pala por encima de la red para golpearla**, sin tocar la red ni el campo contrario. | V3 |
| J15 | **Let durante el punto:** la pelota se rompe, entra un objeto ajeno, se produce un imprevisto externo, o un objeto ajeno en el campo contrario supone un peligro. | V3 |
| J16 | Volea: está permitida en todo el juego **salvo en el resto del saque** (ver S10). | V1 parcial |

### 5.4 Puntuación, sets y tie-break

| # | Regla | Verif. |
|---|---|---|
| P1 | Un juego se cuenta **15, 30, 40 y juego**. | V1 |
| P2 | **Iguales / ventaja (reglamento 2026, Star Point):** con 3 puntos cada pareja se canta **«deuce 1»** (iguales 1). El punto siguiente da la **«ventaja 1»** a quien lo gana. Si esa pareja gana el siguiente, gana el juego; si lo pierde, se pasa a **«deuce 2»**. Luego la **«ventaja 2»** funciona igual; si se pierde, se llega a **«deuce 3»** y se juega un punto decisivo llamado **«Star Point»**. | V1 [FIP Rules 2026](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf) |
| P3 | **Punto de oro (sin ventaja):** el reglamento lo mantiene como sistema alternativo. Con 3 puntos cada pareja se canta «deuce» y se juega un punto decisivo llamado **«punto de oro»**. Se aplica lo dicho en O4 (elección de lado y regla mixta). | V1 |
| P4 | **Set:** lo gana la primera pareja que llega a **6 juegos con 2 de diferencia**. Con **6-6** se juega un **tie-break** y el set termina **7-6**. Como opción fijada de antemano, el tercer set puede jugarse sin tie-break, hasta lograr 2 juegos de diferencia. | V1 |
| P5 | **Partido:** al **mejor de 3 sets**. | V1 |
| P6 | **Tie-break:** lo gana quien llega antes a **7 puntos con 2 de diferencia**. Si hace falta, se sigue hasta que haya esa diferencia. | V1 |
| P7 | **Saque en el tie-break:** empieza el jugador al que le toca sacar según el orden del set, que hace **un saque desde la derecha**. Los **dos puntos siguientes** los saca la pareja contraria, respetando su orden, **empezando por la izquierda**. Después, cada jugador saca **dos puntos seguidos**, siempre en el orden de saque. | V1 |
| P8 | **Súper tie-break (tie-break de partido):** opción que **sustituye al tercer set**. Lo gana quien llega antes a **10 puntos con 2 de diferencia**. El reglamento también contempla una variante a 7 puntos. Qué circuito o fase lo usa lo decide cada reglamento de circuito (no se ha confirmado para cada caso). | V1 |
| P9 | **Cambio de lado:** tras los juegos **1.º, 3.º y todos los impares** del set. En el tie-break, **cada 6 puntos**. | V1 |
| P10 | El cambio de lado al terminar un set (cuándo se hace según si el total de juegos es par o impar) y quién saca primero en el set siguiente a un tie-break (habitualmente, la pareja que restó el primer punto del tie-break) son **NV**. | NV |

## 6. Star Point, punto de oro y estado en 2026

1. **Aprobación:** la **Asamblea General de la FIP** (100 federaciones) aprobó el Star Point **por unanimidad el 28-11-2025** como modificación de las Reglas del Pádel (V2, [nota de prensa del 18-12-2025](https://www.padelfip.com/wp-content/uploads/2025/12/18-december_Star-Point_ENG.pdf)).
2. **Anuncio en el circuito:** el **Steering Committee de Premier Padel** (Barcelona, 11-12-2025) acordó aplicarlo **a partir del Riyadh Season P1** (9 a 14 de febrero de 2026). También se aplica en el CUPRA FIP Tour desde 2026 (V2, [misma nota](https://www.padelfip.com/2025/12/premier-padel-and-international-padel-federation-unveil-2026-qatar-airways-premier-padel-tour-calendar-and-innovative-new-star-point-system-launched/)).
3. **Ámbito en 2026:** **Premier Padel, CUPRA FIP Tour, FIP Promises y FIP Beyond** (amateur). El primer torneo con Star Point fue el **FIP Bronze Melbourne (5 a 11 de enero de 2026)**. En el circuito FIP Promises, todos los partidos del World Tour y del Continental Tour lo usan (V2, [FIP](https://www.padelfip.com/2025/12/between-innovation-and-tradition-introducing-the-star-point-the-scoring-system-that-appeals-to-everyone/), [FIP Promises](https://www.padelfip.com/2025/12/fip-promises-new-regulations-published-effective-from-2026/)).
4. **Regla exacta** (reglamento FIP 2026, V1): *deuce 1 → ventaja 1 → (la gana: juego / la pierde: deuce 2) → ventaja 2 → (la gana: juego / la pierde: deuce 3) → Star Point* (un único punto decisivo). La pareja receptora elige el lado de resto y no puede intercambiar posiciones. En mixto, el punto es entre jugadores del mismo sexo.
5. **Estado del punto de oro:** sigue en el reglamento 2026 como **modalidad alternativa** («sin ventaja»). En los circuitos profesionales de la FIP y Premier Padel lo ha **sustituido el Star Point** en 2026. Según fuentes V3, el Star Point sigue vigente en la temporada 2026 de Premier Padel (etapa de París / Roland-Garros en septiembre). No se ha encontrado ningún anuncio oficial de que se haya revertido, aunque esto está verificado solo de forma indirecta.
6. **Ambigüedad pendiente:** la nota de la FIP describe el Star Point como una «opción alternativa» para los juegos que llegan a iguales. El extracto del reglamento lo presenta en la redacción principal del recuento. **No se ha confirmado** si el reglamento general de 2026 fija como sistema por defecto el Star Point, la ventaja clásica o deja la elección al organizador.
7. Dato de prensa (V3, sin fuente primaria): en las pruebas de 2025 la duración media de un juego con iguales habría bajado de unos 4:30 a unos 2:45 min.

## 7. Posición de los jugadores en dobles

- Los lados de juego (derecha o «drive», izquierda o «revés») los elige cada pareja libremente. El reglamento no los define; solo obliga a mantener el orden de resto (O3, NV).
- En el saque, el sacador cumple S1–S2. El resto de jugadores pueden colocarse donde quieran dentro de su campo (NV). En los puntos decisivos se aplica O4 (V1).

## 8. Pádel individual (1 contra 1)

- **No se ha encontrado ninguna fuente oficial de la FIP** que regule una modalidad individual con reglas o circuito propios. Las búsquedas de esta sesión no llegaron a cubrir este punto porque se agotó el cupo. **Estado: laguna (NV).**
- Es conocimiento previo, no verificado, que existen pistas individuales de **20 × 6 m** comercializadas por fabricantes. Además, algunos documentos técnicos nacionales las mencionan. Hay que buscarlas en el *Anexo de Homologación* de la FIP y en la normativa técnica de la FEP antes de usar esta medida como «oficial».

---

## 9. Datos para el juego (valores por defecto)

`CourtDefinition` (en metros, con el origen en el centro de la red, X a lo ancho, Z a lo largo y Y hacia arriba):

| Parámetro | Valor oficial | Tolerancia / rango | Verif. | Fuente |
|---|---|---|---|---|
| `length` | 20,00 | ±0,5 % (±0,10) | V1 | FIP Rules 2026 |
| `width` | 10,00 | ±0,5 % (±0,05) | V1 | idem |
| `backWallSolidHeight` | 3,00 | — | V1 | idem |
| `backWallTotalHeight` (con malla) | 4,00 | — | V1 | idem |
| `sideWallStep1` (variante 1) | 3,00 de alto × 2,00 de largo | — | V1 | idem |
| `sideWallStep2` (variante 1) | 2,00 de alto × 2,00 de largo | — | V1 | idem |
| `sideWallGlass` (variante 2) | 3,00 de alto × 4,00 de largo | — | V1 | idem |
| `sideMeshHeightCenter` | 3,00 (en los 6 m centrales, variante 1) | — | V1 | idem |
| `sideMeshHeightEnds` | 4,00 (en los 2 m extremos) | — | V1 / NV | idem |
| `meshHoleDiagonal` | 0,05 a 0,0708 | — | V1 | idem |
| `netLength` | 10,00 | — | V1 | idem |
| `netHeightCenter` | 0,88 | ±0,005 | V1 | idem |
| `netHeightPosts` | 0,92 (máximo) | ±0,005 | V1 | idem |
| `netPostMaxHeight` | 1,05 | máximo | V1 | idem |
| `netTopBand` | 0,050 a 0,063 | — | V1 | idem |
| `serviceLineFromNet` | 6,95 | — | V1 | idem |
| `lineWidth` | 0,05 | — | V1 | idem |
| `doorSingle` | 1,05 × 2,00 como mínimo y 2,20 × 2,20 como máximo | — | V1 | idem |
| `doorDouble` (cada una) | 0,72 × 2,00 como mínimo y 1,10 × 2,20 como máximo | — | V1 | idem |
| `outOfCourtZone` | ≥ 3 de ancho (se recomiendan 4) × 4 de largo × 3 de alto | mínimo | V1 | idem |
| `lightMinHeight` | 6,0 (se recomiendan 8,0 en obra nueva) | mínimo | V1 | idem |
| `ceilingMinHeight` | ≥ 6,0 (supuesto; no se ha confirmado que sea la regla de techo) | — | NV | — |
| `tvVerticalLux` | ≥ 1000 lux | mínimo | V1 | idem |

`BallDefinition`:

| Parámetro | Valor | Rango | Verif. | Fuente |
|---|---|---|---|---|
| `diameter` | 0,0656 m | 0,0635 a 0,0677 | V1 | FIP Rules 2026 |
| `mass` | 0,0577 kg | 0,0560 a 0,0594 | V1 | idem |
| `dropTestHeight` | 2,54 m | — | V1 | idem |
| `reboundHeight` | 1,40 m | 1,35 a 1,45 | V1 | idem |
| `restitutionFloorRigid` | 0,742 | 0,729 a 0,756 | DER | calculado |
| `internalPressure` | ≈ 74,5 kPa | 69,9 a 79,1 kPa | V1 / DER | idem |
| `altitudeReboundHeight` (> 800 m) | — | 1,2192 a 1,35 | V1 | idem |
| `restitutionGlass`, `dragCoefficient`, `liftCoefficient`, `bounceFriction` | **calibrar** | — | Laguna | — |

`RacketDefinition`: longitud ≤ 0,455 m; anchura ≤ 0,26 m; mango ≤ 0,20 m; grosor ≤ 0,038 m (NV) con tolerancia del 2,5 %; agujeros centrales de Ø 9 a 13 mm; franja perimetral de 4 cm con agujeros de ≤ 20 mm; cordón de ≤ 0,35 m.

---

## 10. Máquina de estados de puntuación

### 10.1 Estructura común

```
Match{ config, sets[], currentSet, serverOrder[4], receiverSide{A:{right,left},B:{...}}, endsSwapped }
Set  { gamesA, gamesB, mode: NORMAL | TIEBREAK | SUPER_TIEBREAK, firstServerOfSet }
Game { pA, pB, deuceCount, phase }
```

Parámetros de `ScoringConfig`:
- `deuceMode`: `ADVANTAGE` (clásico), `GOLDEN_POINT`, `STAR_POINT(maxAdvantages=2)` o `CUSTOM`.
- `gamesPerSet` = 6; `setsToWin` = 2.
- `tiebreakAt` = 6-6; `tiebreakPoints` = 7.
- `finalSetMode`: `TIEBREAK`, `ADVANTAGE_SET` (sin tie-break) o `SUPER_TIEBREAK(10)`.

### 10.2 Juego: CLASSIC (ventaja indefinida)

```
0/15/30/40 normales.
Si pA==pB>=3 → IGUALES
IGUALES --gana X--> VENTAJA_X
VENTAJA_X --gana X--> JUEGO_X
VENTAJA_X --gana Y--> IGUALES   (bucle sin límite)
```

### 10.3 Juego: GOLDEN_POINT

```
Si pA==pB==3 → PUNTO_DECISIVO (receptor elige lado; no intercambia posiciones; mixto = mismo sexo)
PUNTO_DECISIVO --gana X--> JUEGO_X
```

### 10.4 Juego: STAR_POINT (FIP 2026)

```
pA==pB==3           → DEUCE_1
DEUCE_1  --X--> ADV_1(X)
ADV_1(X) --X--> JUEGO_X
ADV_1(X) --Y--> DEUCE_2
DEUCE_2  --X--> ADV_2(X)
ADV_2(X) --X--> JUEGO_X
ADV_2(X) --Y--> DEUCE_3 = STAR_POINT (receptor elige lado, igual que el punto de oro)
STAR_POINT --X--> JUEGO_X
```

Duración máxima de un juego: **11 puntos** (6 hasta el 40-40, 2·2 de las dos ventajas perdidas y 1 del Star Point). En `CUSTOM_ARCADE_SCORING` hay que parametrizar `maxAdvantages` (0 = punto de oro, ∞ = clásico, 2 = Star Point).

**Casos límite:**
- En el punto decisivo, el lado que elige el receptor rompe la alternancia derecha/izquierda solo para ese punto. El jugador que resta es el que ocupa ese lado.
- En el tie-break y el súper tie-break **no hay iguales ni ventajas**: basta con la diferencia de 2 puntos. Allí no se aplica el Star Point.
- Tras un let el punto se repite y no cambia el estado.

### 10.5 Set y tie-break

```
Tras cada juego:
  si games>=6 y diff>=2 → SET ganado
  si 6-6 y (no es set final o finalSetMode==TIEBREAK) → TIEBREAK
  si set final y finalSetMode==SUPER_TIEBREAK y sets 1-1 → SUPER_TIEBREAK en lugar del 3.er set
TIEBREAK(n=7 | 10):
  gana quien llegue a n con diff>=2
  saque: punto 1 → S0 (el que le tocaba), desde la DERECHA
         puntos 2-3 → siguiente sacador del orden (rival), empieza por la IZQUIERDA
         luego bloques de 2 puntos por sacador siguiendo el orden; el primero de cada bloque desde la izquierda
         (el lado en cada punto es: total de puntos jugados impar → izquierda; par → derecha)
  cambio de lado: cuando (pA+pB) % 6 == 0
  tie-break de set: el set se anota 7-6
```

**Cambio de lado (juegos):** cuando `(gamesA+gamesB)` es impar, tras terminar el juego. Hay que decidir la regla de fin de set (NV, ver P10). Para el prototipo se propone la convención del tenis de la ITF: si el total de juegos del set es impar, se cambia al acabarlo; si es par, se cambia tras el primer juego del set siguiente. Después de un tie-break (13 juegos, impar) se cambia de lado.

**Rotación del saque (juegos):** el orden es A1 → B1 → A2 → B2. En cada set nuevo la pareja puede reordenar quién saca primero. El primer sacador del set siguiente a un tie-break es NV (P10).

### 10.6 CUSTOM_ARCADE_SCORING (propuesta, no oficial)

Todos los parámetros de 10.1 son configurables, además de `maxAdvantages`, `gamesPerSet` (por ejemplo, 4), `noLetServe`, `singleServe` y `finalSetMode`. Por defecto, la interfaz debe ofrecer **STAR_POINT** como «Reglas oficiales FIP 2026».

---

## 11. Decisiones abiertas para el diseño

1. **Formato 1 contra 1:** no hay formato oficial de la FIP verificado. Opciones: (a) 20 × 10 m con reglas de dobles y un solo jugador por lado; (b) una pista «individual» de 20 × 6 m, marcada como no oficial hasta localizar una fuente. Hay que definir cómo se escalan las puertas, los laterales y la línea de saque en (b).
2. **Variante de lateral por defecto:** escalonada (variante 1) o cristal de 4 m (variante 2). Recomendación: variante 2, la habitual en la competición profesional, con la variante 1 como opción.
3. **Juego exterior:** activarlo por defecto (pista con zona de 3 × 4 × 3 m) o desactivarlo. Afecta a S8 y J12–J13.
4. **Tercer set:** set completo con tie-break (Premier Padel) o súper tie-break a 10 (circuitos amateur).
5. **Referencia de las líneas** (si se mide al borde exterior o al eje) y **grosor de la pala**: pendientes de cita literal.
6. **Física del cristal y la malla:** la malla debe dar un rebote atenuado e irregular y el cristal uno casi especular. Hacen falta coeficientes calibrados; sin datos, ajustarlos a ojo con vídeo de referencia.

## 12. Lagunas de evidencia

| Laguna | Motivo | Cómo cerrarla |
|---|---|---|
| Texto íntegro y numeración de artículos del reglamento FIP 2026 | El proxy bloqueó `padelfip.com`. Solo hay extractos del buscador. | Descargar manualmente los PDF de §1 |
| Grosor máximo de la pala (38 mm) | No aparecía en el extracto | PDF de la FIP, capítulo de la pala |
| Paneles de cristal (grosor y medidas), anexo de homologación, colores del suelo y altura del techo | No se ha podido acceder al anexo | *Anexo de Homologación de Pistas* de la FIP |
| Redacción del cambio de 2026 en la posición de los pies al sacar | Solo hay prensa (V3) | PDF de la FIP, capítulo del saque |
| Saque que golpea al receptor o a su compañero; let con contacto en la red y luego en el jugador | Extracto parcial | PDF de la FIP |
| Cambio de lado al terminar un set; primer sacador tras un tie-break; orden de resto por set | No extraído | PDF de la FIP |
| Si el Star Point es el sistema por defecto del reglamento general o solo de los circuitos | Discrepancia entre la nota y el extracto del reglamento | PDF de la FIP, artículo de puntuación |
| Formato del partido de Premier Padel 2026 por fase (sets completos o súper tie-break) | Extracto ambiguo | Premier Padel Rulebook 2026 |
| Modalidad individual y pista de 20 × 6 m | Cupo de búsquedas agotado | FIP, FEP y fabricantes homologados |
| **Datos científicos**: coeficiente de restitución de la pelota contra el cristal y el césped artificial, C_d, Magnus y fricción | Cupo agotado y bases científicas bloqueadas (PMC). **No se ha verificado ningún artículo.** | Buscar en *Sports Engineering* (Springer), *Sensors* / *Applied Sciences* (MDPI) y *Journal of Sports Engineering and Technology* los términos «padel ball rebound», «padel glass wall coefficient of restitution» y «padel ball aerodynamics» |
| Federación Argentina / APA | No consultada | — |

## 13. Fuentes

- FIP, *Reglas del Pádel* (revisión de aplicación del 01.01.2026): <https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Reglas-del-Padel-1.pdf>, <https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Reglas-del-Padel.pdf>
- FIP, *Rules of Padel* (Review of application 01.01.2026): <https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf>, <https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel-1.pdf>
- FIP, *Rules of Padel* (In force as of 1.01.2026): <https://www.padelfip.com/wp-content/uploads/2024/11/FIP-Rules-of-Padel.pdf>
- FIP, *Reglamento de Juego* (2023, versión anterior; regla 15 «Punto ganado»): <https://www.padelfip.com/wp-content/uploads/2023/09/2-Reglamento-Juego.pdf>
- FIP, Star Point (diciembre de 2025): <https://www.padelfip.com/2025/12/between-innovation-and-tradition-introducing-the-star-point-the-scoring-system-that-appeals-to-everyone/>
- FIP y Premier Padel, nota de prensa del 18-12-2025: <https://www.padelfip.com/wp-content/uploads/2025/12/18-december_Star-Point_ENG.pdf>, <https://www.padelfip.com/2025/12/premier-padel-and-international-padel-federation-unveil-2026-qatar-airways-premier-padel-tour-calendar-and-innovative-new-star-point-system-launched/>, <https://premierpadel.com/en/news/premier-padel-and-international-padel-federation-unveil-2026-qatar-airways-premier-padel-tour-calendar-and-innovative-new-star-point-system-launched>
- FIP Promises 2026: <https://www.padelfip.com/2025/12/fip-promises-new-regulations-published-effective-from-2026/>
- Premier Padel Rulebook Men's 2026: <https://www.padelfip.com/wp-content/uploads/2025/03/Premier-Padel-Rulebook-Men%C2%B4s_EN.pdf>
- CUPRA FIP Tour Rulebook 2026: <https://www.padelfip.com/wp-content/uploads/2025/03/Cupra-FIP-Tour-Rulebook_EN-1-1-2.pdf>
- FIP Beyond Rulebook (diciembre de 2025): <https://www.padelfip.com/wp-content/uploads/2026/02/FIP-Beyond-Rulebook-EN-1.pdf>
- Pelotas homologadas por la FIP en 2026: <https://www.padelfip.com/wp-content/uploads/2024/04/pelotas-2026.pdf>
- Prensa (V3): <https://www.padeladdict.com/nuevas-normas-padel-2026-reglamento-fip/>, <https://www.padelnuestro.com/blog/nuevas-reglas-padel-2026>, <https://www.padelspain.net/padel-profesional/826075/cambios-en-el-reglamento-fip-conoce-las-nuevas-reglas/>, <https://www.padelnuestro.com/int/blog/star-point-padel>, <https://padelbiz.it/en/2025/12/18/premier-padel-tappe-inedite-nel-calendario-2026-e-il-nuovo-sistema-star-point/>
