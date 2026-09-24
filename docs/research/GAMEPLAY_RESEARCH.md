# Investigación de Gameplay, Física e IA — Juego de Pádel

- **Fecha:** 2026-09-24
- **Rol:** Gameplay Engineer + Physics Specialist + AI Specialist
- **Hipótesis de motor:** Unity 6.x. Modos 1v1 y 2v2. Primero un jugador contra IA, con un máximo de 4 jugadores.
- **Objetivo:** gameplay deportivo preciso y satisfactorio, dirigido por datos, testeable y sin sobreingeniería.

---

## 0. Metodología y límites de esta investigación (leer primero)

1. **WebFetch estuvo bloqueado** durante toda la sesión por la política de salida de red (`EGRESS_BLOCKED`), y eso incluyó `docs.unity3d.com`, `store.steampowered.com`, `en.wikipedia.org` y `padelfip.com`. Tampoco pude acceder por `curl` a `docs.unity3d.com` (403 del proxy). **Por eso ninguna página se ha leído completa.** Las afirmaciones marcadas con fuente se apoyan en los extractos que devolvió el buscador (WebSearch) para esa URL concreta.
2. **Se agotó el cupo de WebSearch de la sesión** (200/200, compartido con otros agentes) antes de terminar los temas 5, 6, 7 y 8. En esos apartados las afirmaciones sobre APIs de Unity salen de conocimiento previo **sin verificar en esta sesión**. Van marcadas como **[NV]** (no verificado) y enlazan a la documentación oficial que hay que confirmar.
3. Convenciones de marcado:
   - **[V]**: verificado mediante el extracto del buscador para la URL citada.
   - **[NV]**: no verificado en esta sesión. Hay que confirmarlo antes de tomar decisiones irreversibles.
   - **[D]**: valor derivado por cálculo propio a partir de datos con fuente (se muestra la cuenta).
   - **[P]**: propuesta de diseño o valor de tuning. No es un dato de la literatura.
4. Cuando la fuente es una web comunitaria (wiki de fans, Grokipedia, blog), se indica como **fuente secundaria**.

---

## 1. Mercado: juegos de pádel existentes y referentes de deportes de raqueta

### 1.1 Hallazgos: juegos de pádel (estado a sept. 2026)

| Título | Estudio / plataforma | Estado y fecha | Modelo de juego (según la fuente) | Recepción |
|---|---|---|---|---|
| **Padel Rivals** | Krokanti Games (España). PC, Mac, Linux, Switch, PlayStation, Xbox | Early access previsto entre Q3 y Q4 de 2026 [V] ([FAQ](https://www.playpadelrivals.com/en/faq), [Padelbiz](https://padelbiz.it/en/2025/12/04/padel-rivals/), [actu-padel](https://actu-padel.com/en/padel-rivals-padel-comes-to-the-console-mid-2026/)) | Arcade, centrado en "timing y estrategia", uso de paredes y remate. Mando o teclado y ratón. Local para 4 jugadores con un mando cada uno. Liga online y modo carrera narrativo [V] ([Steam](https://store.steampowered.com/app/4056780/Padel_Rivals/), [krokanti.com](https://www.krokanti.com/en/games/padel-rivals), [PadelStar](https://padelstar.es/noticias-padel/padel-rivals-el-primer-videojuego-de-padel-arcade-llegara-en-2026-para-pc-y-consolas/)) | Sin datos de recepción (sin lanzar o sin reseñas accesibles) |
| **KorrPadel** | Frozax Games. PC primero, consolas después; también en Epic | Wishlist en Steam y Epic ("Coming soon") [V] ([korrpadel.com](https://korrpadel.com/en/), [Epic](https://store.epicgames.com/p/korrpadel-02411a)) | "Simulación moderna" donde posicionamiento y timing deciden. Bandeja y víbora. CPU, local para 4, online y mixto local+online. Carrera. Formatos 1v1 en diagonal o en pista estrecha [V] ([korrpadel.com](https://korrpadel.com/en/)) | Sin datos |
| **Padel Simulator** | Steam (app 4915770), estudio no identificado | No lanzado (solo wishlist) [V] ([Steam](https://store.steampowered.com/app/4915770/Padel_Simulator/)) | Tercera persona, "responsive movement, precise racket control, smart AI, dynamic ball physics". 2v2 con compañero IA [V] | Sin datos |
| **Padel Impact Pro** | "lazar", Steam | Página en Steam. Fecha no confirmada | Solo contra IA, lobbies privados y online. "Precision physics". Smash, globo y juego de cristal [V] ([Steam](https://store.steampowered.com/app/4510950/Padel_Impact_Pro/)) | Sin datos |
| **Padel Pro World Tour** | Steam (app 4809310) | El buscador muestra "Save 10%", lo que sugiere que ya se vende. **Sin detalles** [V parcial] ([Steam](https://store.steampowered.com/app/4809310/Padel_Pro_World_Tour/)) | Desconocido | Sin datos |
| **Red Bull Padel: Court Legends** | Red Bull. iOS y Android, free-to-play | Lanzado el 27-may-2026. **Primer juego con licencia oficial de Premier Padel** [V] ([padelfip.com](https://www.padelfip.com/2026/05/mobile-gaming-meets-professional-padel-in-red-bull-padel-court-legend-the-premier-padel-game/), [Padel Magazine](https://padel-magazine.co.uk/red-bull-lance-le-premier-jeu-video-officiel-de-premier-padel/)) | Se toca la pista para mover al jugador más cercano. El swipe decide el golpe (largo y suelto = globo, corto y seco = plano, potente = smash). **Se controlan los dos jugadores de la pareja.** PvP en tiempo real con gestión de club [V] ([redbull.com](https://www.redbull.com/us-en/red-bull-padel-court-legends-mobile-game-preview), [Google Play](https://play.google.com/store/apps/details?id=com.redbull.padel)) | Feedback de usuarios: "matches are way too short" [V, extracto del resumen del buscador] |
| **PadelVR Game / PadelVR Training** | Meta Quest | Game: "en desarrollo, el motor de física se está mejorando". Training en Early Access con suscripción [V] ([Meta](https://www.meta.com/experiences/padelvr-game/5701882323175143/), [Meta Training](https://www.meta.com/experiences/padelvr-training/8067583823340990/)) | Simulación VR con adaptador físico de pala | Training: 3,4/5 con 33 reseñas en la Meta Store [V] |
| Móvil variado: *Padel 3D: Nations League* (1v1), *Padel Battle* (arcade), *Padel Simulator 3D*, *Heroes of Padel* | Google Play | Publicados | 1v1 y partidas rápidas | Sin valoraciones recuperadas [V] ([Nations League](https://play.google.com/store/apps/details?id=com.Prelogos.Padel), [Padel Battle](https://play.google.com/store/apps/details?id=com.ikermedia.hitpadelworld)) |

Notas:
- "**Padel Rush**" no es un videojuego. Es un club y app de reservas en Riad [V] ([Google Play](https://play.google.com/store/apps/details?id=com.padelrush.bookandgo&hl=en_US)).
- "**PadelVerse**" es una app de reservas y deporte ("We Make Sports Easier") y una plataforma de clubes, no un juego [V] ([Google Play](https://play.google.com/store/apps/details?id=com.sportyfriends.padelVerse&hl=en), [padelverse.net](https://padelverse.net/)).
- No encontré un juego llamado "Padel Pro" más allá de *Padel Impact Pro* y *Padel Pro World Tour*.

### 1.2 Hallazgos: referentes de deportes de raqueta

| Juego | Modelo de input | Asistencia de movimiento | Tipos de golpe y counters | Feedback | Fuente |
|---|---|---|---|---|---|
| **Mario Tennis Aces / Fever** | **Un botón por spin**: A = topspin (trayectoria alta, bote alto), B = slice (bajo, bote bajo), Y = plano (el más rápido). Globo y dejada con combinaciones o stick. **Mantener pulsado = golpe cargado** | Trick Shot: desplazamiento rápido hacia la bola, con riesgo | Aces: Zone Shot en primera persona apuntando (cuesta ≥1/3 de energía), rotura de raqueta (3 Zone Shots o 1 Special). Energía que se rellena con rallies. Fever (2026): 30 "Fever Rackets" con efectos al botar; se contrarrestan volando la bola | Reseñas de Fever mixtas o positivas: "tight to play", aunque "quantity over quality" | [Shacknews](https://www.shacknews.com/article/105756/controls-and-basic-button-configuration-in-mario-tennis-aces), [MarioWiki Zone Shot](https://www.mariowiki.com/Zone_Shot), [Wikipedia Aces](https://en.wikipedia.org/wiki/Mario_Tennis_Aces), [GameSpot Fever](https://www.gamespot.com/reviews/mario-tennis-fever-review-bringing-the-heat/1900-6418460/), [Nintendo Life](https://www.nintendolife.com/reviews/nintendo-switch-2/mario-tennis-fever) [V] |
| **TopSpin 2K25** | **Timing Meter**: soltar el botón en la zona verde. Tap = golpe de control; hold y soltar = golpe potente. Soltar pronto o tarde lleva a red o fuera | Limitación de posición en el saque | Los golpes perfectos cuestan más con la **Rally Energy** baja o en rallies largos | **Nota de timing tras cada golpe** (pronto, tarde o perfecto) | [2K Centre Court Report](https://topspin.2k.com/2k25/centre-court-report/gameplay/), [Tom's Guide](https://www.tomsguide.com/gaming/topspin-2k25-review) [V] |
| **Virtua Tennis** | Mantener = cargar potencia. La liberación en la ventana óptima reduce errores. Slice = atrás en el stick + golpe | Desconocido | Slice, dejada y globo | — | [Grokipedia](https://grokipedia.com/page/Virtua_Tennis_(video_game)) (**fuente secundaria, fiabilidad baja**) |
| **Wii Sports Tennis** | Swing con movimiento. **El timing del swing decide la dirección**: pronto = cruzado, tarde = paralelo | **Movimiento totalmente automático** | Spin según el gesto | — | [Wii Sports Wiki](https://wiisports.fandom.com/wiki/Tennis_(sport)), [StrategyWiki](https://strategywiki.org/wiki/Wii_Sports/Tennis) (**fuentes comunitarias**) |
| **Windjammers 2** | Pulsación temporizada = lanzamiento rápido o lento. El stick curva la trayectoria o la manda diagonal contra la pared | — | Curva, globo, recto, slapshot, dropshot, salto, smash, EX move | "Easy to learn, hard to master" | [Engadget](https://www.engadget.com/2019-08-23-windjammers-2-demo-impressions-gamescom.html), [Dotemu](https://www.dotemu.com/games/windjammers-2/) [V] |
| **Lethal League (Blaze)** | Golpe único con dirección | — | La bola acelera con cada golpe | **El hitstop crece con la velocidad de la bola** y puede durar segundos, lo que "upped the hype" | [Game Developer, entrevista a Dion Koster](https://www.gamedeveloper.com/design/developing-the-stylish-indie-hit-fighting-game-i-lethal-league-blaze-i-), [TV Tropes](https://tvtropes.org/pmwiki/pmwiki.php/VideoGame/LethalLeague) [V] |
| **Sports Story (tenis)** | — | — | — | Críticas: "far too precise character placement and swing timing" y confusión al apuntar | [Game Informer](https://gameinformer.com/review/sports-story/a-series-of-unforced-errors), [RPGamer](https://rpgamer.com/review/sports-story-review/) [V] |
| **Tennis World Tour (2018)** | — | — | — | Metacritic 4,5 (crítica) y 3,1 (usuarios). "Clunky and heavy", la bola se devuelve sin contacto visible con la raqueta y las animaciones dan tirones | [Metacritic](https://www.metacritic.com/game/tennis-world-tour/critic-reviews/) [V, extracto] |

Hitstop y "juice":
- Sakurai define el hitstop como congelar a atacante y víctima unos frames para subrayar la potencia del impacto [V] ([Source Gaming, trad. de su columna en Famitsu](https://sourcegaming.info/2015/11/11/thoughts-on-hitstop-sakurais-famitsu-column-vol-490-1/)).
- En "Juice It or Lose It" (GDC 2012, Jonasson y Purho) se añaden flash, shake, partículas y sonido a un Breakout. El juice se suma encima de algo que ya funciona [V] ([GDC Vault](https://www.gdcvault.com/play/1016487/Juice-It-or-Lose)).

### 1.3 Evaluación

- **Laguna de mercado.** No he encontrado ningún juego de pádel para PC o consola lanzado, consolidado y con recepción medible a 24-sep-2026. Los dos competidores directos con más visibilidad (**Padel Rivals** y **KorrPadel**) están sin lanzar o en fase de wishlist, y los dos prometen local para 4 más online. El único producto con licencia oficial (Red Bull Court Legends) es **móvil, con controles táctiles simplificados** (tap para mover, swipe para golpear, control de los dos jugadores de la pareja).
- **Qué parece faltar** (inferencia a partir de lo que declaran las fichas, no de reseñas):
  1. Un **modo individual profundo con un compañero IA creíble en 2v2**. Solo *Padel Simulator* lo nombra, y no está lanzado.
  2. Un **juego de paredes legible**: bola visible tras rebotes en cristal y malla, con predicción clara.
  3. Una **taxonomía de golpes específica de pádel** (bandeja, víbora, x3, x4, chiquita, contrapared) accesible con pocos botones.
- **Riesgo.** Hay una ventana competitiva a corto plazo: dos estudios independientes lanzan entre 2026 y 2027 con propuestas parecidas. La diferenciación no puede ser "el primer juego de pádel".
- **Lecciones de los referentes:**
  - El castigo más claro de la crítica llega por (a) **desincronía visual entre raqueta y bola** (Tennis World Tour) y (b) **exigencia excesiva de colocación y timing sin ayuda** (Sports Story).
  - Los modelos que funcionan separan **la elección del golpe (botón)**, **la dirección (stick)** y **la calidad (timing o carga)**, como Mario Tennis y TopSpin.

### 1.4 Recomendación

- **Posicionamiento [P]:** "Arcade-sim". Física creíble de paredes y spin, con input de pocos botones y asistencia de posicionamiento. Lo diferencial es **un jugador contra IA en 2v2 con un compañero IA que juega "en pareja"**.
- **Modelo de input recomendado [P]** (detalle en §6):
  - **Stick izquierdo:** movimiento, con imán suave al punto de golpeo ideal.
  - **3 botones de familia de golpe:** *Ataque* (plano o top), *Control* (cortado o slice) y *Globo*, más un modificador para dejada o chiquita.
  - **El contexto resuelve el golpe concreto:** volea, bandeja, víbora o remate según la altura y la zona.
  - **La dirección se toma del stick en el momento del contacto.**
  - **Calidad por timing implícito:** error respecto al instante ideal de contacto, **sin medidor en pantalla por defecto**.
  - **Hold para cargar potencia**, como en Mario Tennis y Virtua Tennis.
  - **Nota de timing tras el golpe** (pronto, perfecto o tarde), tomada de TopSpin 2K25.
- **Feedback [P]:**
  - Hitstop de 2 a 6 frames que escala con la velocidad o la calidad del golpe (idea de Lethal League, con escala mucho menor).
  - Shake de cámara solo en remates.
  - El contacto visual pala-bola tiene que ser exacto: la animación se adapta a la bola con IK o una ventana de contacto, nunca al revés.
- **Cámara [P, sin fuente]:** fija, elevada y detrás del fondo del jugador humano, con cristal translúcido. La pista (20×10 m) cabe entera en pantalla, así que **4 jugadores locales comparten una sola cámara sin split-screen**.

---

## 2. Física de la bola

### 2.1 Hallazgos: Unity PhysX

- **Bounce Threshold:** por defecto vale **2** (m/s). Los contactos con velocidad relativa inferior **no rebotan**, sea cual sea la `bounciness`. Es una fuente típica de botes "muertos" a baja velocidad [V] ([Unity Discussions](https://forum.unity.com/threads/ball-doesnt-bounce-much-even-though-bounciness-at-1.315043/), [Manual 6000.2 "Collider surface bounciness"](https://docs.unity3d.com/6000.2/Documentation/Manual/collider-surface-bounce.html)).
- **Bounciness** (0–1) es el coeficiente de restitución.
- **Modos de combinación** de fricción y rebote: Average, Minimum, Multiply y Maximum, con prioridad **Average < Minimum < Multiply < Maximum** cuando los dos materiales difieren [V] ([Manual 6000.0 "How collider surface values combine"](https://docs.unity3d.com/6000.0/Documentation/Manual/collider-surfaces-combine.html)).
- **CCD (detección continua de colisiones):**
  - Discrete puede producir tunneling.
  - Continuous: contra colliders estáticos.
  - ContinuousDynamic: también contra otros rigidbodies en modo continuo.
  - ContinuousSpeculative: más barato, maneja mejor la rotación y sirve en cinemáticos.
  - Unity recomienda empezar en Discrete, pasar a Speculative si hay tunneling y usar Dynamic "como último recurso" [V] ([Scripting API](https://docs.unity3d.com/ScriptReference/CollisionDetectionMode.ContinuousSpeculative.html), [Manual 2022.3](https://docs.unity3d.com/2022.3/Documentation/Manual/physics-optimization-cpu-rigidbody-collision-modes.html)).
- **Simulación manual:** con `Physics.simulationMode = SimulationMode.Script` se llama a `Physics.Simulate(dt)`. Se recomienda un paso fijo, y pasos mayores de 0,03 s "likely inaccurate". Para predecir trayectorias con PhysX hay que crear una **escena de física paralela** y simular en ella [V] ([Manual 6000.3](https://docs.unity3d.com/6000.3/Documentation/Manual/physics-optimization-cpu-manual-simulation.html), [PhysicsScene.Simulate](https://docs.unity3d.com/6000.2/Documentation/ScriptReference/PhysicsScene.Simulate.html)).
- **Determinismo de PhysX:**
  - Es determinista con paso fijo y "Enhanced Determinism" (orden de contactos no aleatorio, añadido en 2018.3), siempre que se mantenga el orden de creación de actores.
  - El determinismo **entre plataformas o binarios** es "extremely hard".
  - Photon Quantum y otros usan punto fijo y motor propio [V, fuentes secundarias] ([Manual 2018.3](https://docs.unity.cn/cn/2018.3/Manual/class-PhysicsManager.html), [Kimbatt/unity-deterministic-physics](https://github.com/Kimbatt/unity-deterministic-physics)).
- **PhysX no modela** ni la fuerza Magnus ni el acoplamiento spin↔fricción de una bola hueca que "agarra" la superficie. Esto es una inferencia: la API de `PhysicsMaterial` solo expone fricción estática y dinámica, restitución y modos de combinación [V para la API; la inferencia es mía].

### 2.2 Hallazgos: qué hacen otros juegos

- **Rocket League** integró **Bullet** en UE3 para tener física de red determinista: cliente y servidor simulan igual. Jared Cone (GDC 2018) explica el diseño de física y red, y el problema de desincronía cuando el buffer de inputs del servidor se vacía y repite el último input [V] ([GDC Vault](https://www.gdcvault.com/play/1024972/It-IS-Rocket-Science-The), [slides PDF](https://media.gdcvault.com/gdc2018/presentations/Cone_Jared_It_Is_Rocket.pdf), [pybullet.org](https://pybullet.org/wordpress/index.php/2018/03/15/rocket-league-using-bullet-physics-in-unreal-engine-4/)). La frecuencia de simulación de 120 Hz y el tuning a medida de la bola aparecen en esa charla según mi conocimiento previo **[NV]**; confirmar en las slides.
- **Tenis y golf:** no encontré ningún postmortem primario de un juego de tenis o golf comercial que describa su integración de la bola. **Laguna** (ver §9).

### 2.3 Hallazgos: aerodinámica (literatura)

**Fuerzas** (modelo estándar de Štěpánek 1988: gravedad + arrastre + Magnus, con coeficientes que dependen del parámetro de spin S) [V] ([Štěpánek, Am. J. Phys. 56, 138](https://ui.adsabs.harvard.edu/abs/1988AmJPh..56..138S/abstract), [Cross, "Ball Trajectories"](https://physics.usyd.edu.au/~cross/TRAJECTORIES/42.%20Ball%20Trajectories.pdf)):

```
F_drag   = -½ · ρ · C_D · A · |v| · v                 (opuesta a la velocidad)
F_magnus =  ½ · ρ · C_L · A · |v|² · (ω̂ × v̂)          (perpendicular a v y a ω)
S        =  r · |ω| / |v|                              (parámetro de spin)
a        =  g + (F_drag + F_magnus) / m
```

Forma compacta para código, con `k = ½ρA/m`:
`a = g − k·C_D·|v|·v + k·C_L·|v|·(ω̂ × v)`. Se usa |v|·(ω̂×v) porque |ω̂×v̂|·|v|² = |ω̂×v|·|v| cuando ω ⟂ v; si hay componente de ω paralela a v, esa parte no genera sustentación.

**Tabla de coeficientes (literatura, pelota de tenis):**

| Parámetro | Valor | Condiciones | Fuente |
|---|---|---|---|
| C_D, en vuelo real | **0,507 ± 0,024**, independiente de velocidad y spin | Lanzador a 15–30 m/s, spin ≤ 2500 rpm | Cross & Lindsey 2014, *Sports Eng.* 17:89–96 [V] ([Springer](https://link.springer.com/article/10.1007/s12283-013-0144-9)) |
| C_D, túnel de viento, bola nueva | **0,6–0,7** | Re 85–250·10³ (20–60 m/s) | Goodwill, Chin & Haake 2004, *J. Wind Eng.* [V] ([SHU](https://shura.shu.ac.uk/634/), [ScienceDirect](https://www.sciencedirect.com/science/article/abs/pii/S0167610504000807)) |
| C_D, bola nueva | ≈ **0,62**. La felpa añade hasta un 40 % de arrastre frente a una esfera lisa (0,5) | Re 80–300·10³ | Mehta & Pallis 2001; revisión Mehta 2008 [V] ([Wiley 2001](https://onlinelibrary.wiley.com/doi/abs/10.1046/j.1460-2687.2001.00083.x), [Wiley 2008](https://onlinelibrary.wiley.com/doi/full/10.1002/jst.11)) |
| C_D, bola usada | 0,61 frente a 0,67 de la nueva con S = 0,15 | 1500 impactos | Mehta 2008 [V] |
| C_D, rango | 0,55–0,75 | S ≈ 0,05–0,6 | Štěpánek 1988 [V] |
| C_L | **0,075–0,275**, crece con S | S ≈ 0,05–0,6 | Štěpánek 1988 [V] ([TWU/Cross](https://twu.tennis-warehouse.com/learning_center/aerodynamics2.php)) |
| C_L con spin bajo | Sustentación no nula incluso con spin bajo, por asimetrías de la felpa | — | Goodwill et al. 2004 [V] |
| Forma de la trayectoria | "Determined primarily by C_L, not C_D" | — | TWU/Cross [V] |
| ρ del aire | 1,2 kg/m³ (≈20 °C, nivel del mar) | Valor estándar | [NV] (física estándar) |

**Fórmula de C_L(S) para el MVP [D/P].** No pude recuperar la regresión exacta de Štěpánek. Propongo una interpolación lineal entre sus extremos publicados:

```
C_L(S) = clamp(0.075 + (S − 0.05) · (0.200 / 0.55), 0, 0.30)     para S ≥ 0
       ≈ 0.057 + 0.364·S
```

Es un ajuste propio y tiene que quedar como parámetro en un ScriptableObject. Hay que validarlo contra la regresión original ([AJP PDF](https://pubs.aip.org/aapt/ajp/article-pdf/56/2/138/11547952/138_1_online.pdf)).

**Pelota de pádel (FIP)** [V] ([FIP Rules of Padel 2026](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf), [resumen padel.how](https://padel.how/rules/official-fip-padel-rules/)):

| Parámetro | Valor FIP | Valor MVP |
|---|---|---|
| Diámetro | 6,35–6,77 cm | r = 0,033 m [P] |
| Masa | 56,0–59,4 g | m = 0,0575 kg [P] |
| Presión interna | 4,6–5,2 kg por 2,54 cm² | — |
| Bote (caída desde 2,54 m sobre superficie dura) | 135–145 cm | **e_n ≈ 0,73–0,76** [D: e = √(h/H) → √(1,35/2,54) = 0,729 y √(1,45/2,54) = 0,756] |
| Área frontal | — | A = πr² ≈ 3,42·10⁻³ m² [D] |
| k = ½ρA/m | — | ≈ 0,0357 m⁻¹. Con C_D = 0,55, k·C_D ≈ **0,0196 m⁻¹** [D] |

Magnitud del arrastre con C_D = 0,55 [D]:
- 10 m/s → 2,0 m/s²
- 20 m/s → 7,9 m/s²
- 30 m/s → 17,7 m/s²
- 37 m/s (133 km/h) → 26,9 m/s² (≈2,7 g)

**El arrastre no es despreciable en remates.** Velocidades de remate medidas con radar: semiprofesionales 133,1 ± 8,2 km/h sin oposición y 120,7 ± 9,8 con oposición; amateurs 124,6 ± 9,2 y 104,5 ± 10,0 km/h [V] ([Kinesiology, "Influence of the opposition on overhead smash velocity"](https://ojs.srce.hr/index.php/kinesiology/article/view/5656)).

No hay datos específicos de C_D o C_L de la pelota de pádel (presión menor que la de tenis). **Laguna.** Uso los valores de tenis como aproximación.

### 2.4 Hallazgos: rebote con spin (fricción)

- Cross, "Grip-slip behavior of a bouncing ball" (*Am. J. Phys.* 70, 1093, 2002) [V] ([PDF](https://www.physics.usyd.edu.au/~cross/PUBLICATIONS/GripSlip.pdf)):
  - Si la bola desliza durante todo el bote, e_x < 0.
  - Si entra en rodadura, e_x = 0.
  - Si "muerde" (grip), e_x > 0.
  - El e horizontal de una pelota de tenis va de **0,51 a 0,24** según el ángulo y el coeficiente de fricción.
  - Hay un desplazamiento D ≈ 2–3 mm de la normal que añade spin.
- Cross, "Measurement of the speed and bounce of tennis courts" [V] ([PDF](https://www.physics.usyd.edu.au/~cross/PUBLICATIONS/52.%20SpeedAndBounce.pdf)):
  - Las pistas se clasifican por el **coeficiente de fricción de deslizamiento (COF)**.
  - Con ángulos de incidencia bajos, la pérdida de velocidad horizontal es **proporcional al COF**.
  - **Las bolas reales no ruedan: agarran.**
  - La relación de velocidad vertical puede bajar hasta **0,4** en algunas pistas.
  - Las pistas lentas (tierra batida) botan alto y las rápidas (hierba) botan bajo.
- Momento de inercia: para una capa esférica fina, I = ⅔·m·r² [V] ([Brody 2005, *Phys. Teach.* 43:503](https://pubs.aip.org/aapt/pte/article/43/8/503/274508/The-Moment-of-Inertia-of-a-Tennis-Ball)). El valor medido para pelotas de tenis (≈0,55·m·r² según mi recuerdo) es **[NV]**.
- Pared de pádel:
  - El cristal da botes "faster, longer and more accelerated" que el hormigón. El tipo de bola influye más en la velocidad y la pared más en el ángulo [V] ([Int. J. Perf. Anal. Sport 2021, doi 10.1080/24748668.2021.1875778](https://doi.org/10.1080/24748668.2021.1875778)).
  - Un divulgador indica que la bola conserva ≈65–70 % de la velocidad al rebotar en el cristal [V extracto, **fuente secundaria**, atribución incierta] ([The Padel Brief](https://thepadelbrief.com/en/blog/how-to-play-off-walls-padel)).

**Modelo de rebote de impulso con fricción (rígido, Coulomb) [D, a partir del modelo clásico de impulso que Cross toma como referencia].**

Notación: n = normal unitaria de la superficie, r = radio, I = α·m·r², con α ≈ 0,55–0,667.

```
v_n  = (v·n)                       // < 0 si entra
v_t  = v − v_n·n
u    = v_t + ω × (−r·n)            // velocidad tangencial del punto de contacto
// Normal
v_n' = −e_n(|v_n|) · v_n
J_n  = m · (1 + e_n) · |v_n|
// Tangencial: impulso necesario para anular u (grip/rodadura)
J_grip = m · |u| · α / (1 + α)
J_t    = min(μ · J_n, J_grip)      // desliza si μ·J_n < J_grip
û      = u / |u|
v_t'   = v_t − (J_t / m) · û
ω'     = ω + (J_t / (α·m·r)) · (n × û)
v'     = v_t' + v_n'·n
```

Casos límite [D]:
- **Sin spin, grip completo, α = ⅔:** v_t' = (3·v_t + 2·r·ω_fwd)/5, es decir, la bola sale al 60 % de su v_t y enrollada hacia delante.
- **Topspin:** el punto de contacto va más lento, así que hay menos fricción de frenado y la bola "salta" hacia delante.
- **Backspin (bandeja o cortado):** u es grande, así que J_t es grande y la bola frena y se queda baja. **Es justo lo que describen los entrenadores de la bandeja** ("stays low… dies into the back glass") [V] ([LTA Padel](https://www.ltapadel.org.uk/play/padel-tips-and-techniques/overheads-bandeja-and-the-smash/)).
- Para capturar el e_x > 0 ("overspin" por deformación) observado por Cross, añadir un factor `gripOvershoot` de 0 a 0,1 **[P]**. Se puede dejar para después del MVP.

**Parámetros de superficie para el MVP [P, a tunear; ninguno medido para pádel]:**

| Superficie | e_n | μ (COF) | Notas |
|---|---|---|---|
| Suelo (césped artificial con arena) | 0,73–0,76 a baja velocidad [D del bote FIP]. Bajar a ≈0,65 a alta velocidad [P] | 0,5–0,7 [P] | e_n dependiente de la velocidad: curva en datos. Que el COR baje con la velocidad es conocido pero **[NV]** aquí |
| Cristal | 0,65–0,75 [P, anclado al 65–70 % de conservación, fuente secundaria] | 0,2–0,35 [P] | Cristal más "rápido" que el hormigón [V] |
| Malla metálica | 0,3–0,5 [P] | 0,5 [P] | Añadir perturbación de la normal con **RNG sembrado** (±10–20°) [P]. Determinista pero "irregular" |
| Red | e_n ≈ 0,1–0,2 [P] | alta | Tratar como plano vertical con altura h(x) |

### 2.5 Integración numérica y predicción

- **Semi-implícito de Euler** (v += a·dt; x += v·dt) a paso fijo. Con solo gravedad, el error de posición vertical al final de un vuelo de duración T es ½·g·T·dt [D]:
  - T = 1,5 s y dt = 1/120 → **6,1 cm**.
  - Con dt = 1/60 → 12,3 cm.
- **Velocity Verlet** es exacto con aceleración constante. Con arrastre (que depende de v) necesita una evaluación extra, pero sigue siendo barato.
- **Clave para la IA:** si la IA predice **con exactamente el mismo integrador, dt y parámetros**, el error de integración **no importa**, porque la predicción coincide al 100 % con la simulación (misma máquina, mismo binario). Por eso **el determinismo es más importante que la precisión absoluta**. Con PhysX habría que simular en una escena paralela ([V], §2.1). Con un integrador propio basta con **clonar un struct y avanzarlo N pasos**.
- **Colisiones analíticas:** la pista es un conjunto de **planos alineados con los ejes** (suelo, 2 fondos, 2 laterales con tramos de cristal y malla, red). Colisión esfera-plano con barrido: se calcula el tiempo de impacto t* = (d − r − p·n)/(v·n) dentro del paso. Luego se sub-paso: avanzar hasta t*, resolver el rebote y consumir el resto de dt, **iterando hasta 3 contactos por paso** (esquinas). Sin tunneling a ninguna velocidad: a 37 m/s y 120 Hz la bola avanza 0,31 m por paso [D].

### 2.6 Evaluación: PhysX frente a integrador propio

| Criterio | Rigidbody + PhysX | Integrador propio (C# puro) |
|---|---|---|
| Magnus y arrastre | Hay que añadirlos con `AddForce` en FixedUpdate igualmente | Nativos en la fórmula |
| Rebote con spin (grip/slip) | No lo modela. Solo fricción y restitución combinadas | Modelo de impulso de §2.4 |
| Botes a baja velocidad | Umbral de 2 m/s por defecto que mata botes [V] | Sin umbral, o uno explícito |
| Predicción para la IA | Escena paralela + `PhysicsScene.Simulate` [V]. Caro y con sincronización de escenas | Copiar struct y avanzar N pasos. Barato |
| Determinismo | Solo con Enhanced Determinism, paso fijo y el mismo orden; no entre plataformas [V] | Determinista en la misma plataforma y binario. Entre plataformas con float: **[NV]**, requeriría punto fijo |
| Test en Edit Mode | Requiere escena o PlayMode | NUnit puro, sin MonoBehaviour |
| Coste de implementación | Bajo al inicio y alto en tuning y bugs de borde | ~300–600 LOC [P]: 1 esfera y ~10 planos |
| Geometría compleja | Gratis | No hace falta: la pista es una caja |

### 2.7 Recomendación: BALL_MVP

**BALL_MVP = integrador propio determinista en C# puro, sin PhysX para la bola.**

1. `BallState { Vector3 pos, vel, spin; }` como struct.
2. `BallSimulator.Step(ref BallState, in BallParams, in CourtGeometry, float dt, ref Rng)` es una función pura.
3. Paso fijo de **1/120 s** [P], con subpasos si el `fixedDeltaTime` del proyecto es mayor. El valor por defecto de Unity de 0,02 s es **[NV]**.
4. Integrador: semi-implícito de Euler **o** Velocity Verlet [P: empezar con semi-implícito por simplicidad; la precisión absoluta da igual porque la IA usa el mismo código].
5. Fuerzas: gravedad + arrastre (C_D = 0,55 constante [P], dentro del rango 0,507–0,62 de la literatura) + Magnus (C_L(S) lineal de §2.3). El spin decae con τ ≈ unos segundos **[P, sin fuente]**.
6. Colisiones: barridas y analíticas contra planos. Rebote con el modelo de impulso de §2.4 y tabla de materiales en un ScriptableObject.
7. **La interacción pala-bola no es física.** Es un **evento de gameplay**: cuando el golpe es válido, `ShotSolver` fija la nueva `vel` y `spin` (ver §3).
8. La presentación (el `Transform` de la bola) **interpola** entre los dos últimos estados de simulación.
9. `TrajectoryPredictor` reutiliza `Step` para producir botes, cruces de pared y el punto de golpeo óptimo para la IA y la asistencia de movimiento.

**Justificación:** el pádel se juega con las paredes y el spin (bandeja que muere en el cristal, x3 con liftado). PhysX no modela el acoplamiento spin↔fricción ni Magnus, trae umbrales de rebote que hay que desactivar y hace cara la predicción. Un integrador propio con ~10 planos es pequeño, testeable en Edit Mode y determinista, que es la base de la IA y de un futuro online. Es el mismo principio que aplicó Rocket League: simulación idéntica en todas partes [V].

---

## 3. Sistema de golpes

### 3.1 Hallazgos: técnica de pádel (fuentes de coaching)

- **Bandeja:**
  - Golpe controlado, **cortado (slice) con algo de lateral**, trayectoria descendente.
  - Sirve para **mantener la red**; es "el antídoto del globo".
  - Busca profundidad o esquinas y que la bola **quede baja tras el cristal**.
  - No busca ganar el punto [V] ([LTA Padel](https://www.ltapadel.org.uk/play/padel-tips-and-techniques/overheads-bandeja-and-the-smash/), [PadelStar](https://padelstar.es/videos-de-padel/diferencias-tecnica-bandeja-vibora/), [Padel Point](https://www.tiendapadelpoint.com/en/difference-bandeja-vibora-padel-en)).
  - **El punto de impacto es más bajo que el del remate** [V] ([PadelStar, bandeja o remate](https://padelstar.es/tactica-padel/hago-bandeja-o-remate-diferencias-tecnica-tactica/)).
- **Víbora:**
  - Más agresiva, con **efecto lateral** marcado y cortado, y más velocidad.
  - Se golpea más por el lado de la bola, con el codo más flexionado y alto.
  - "Chasquido de muñeca" en el contacto. Busca el punto o una devolución difícil [V] (mismas fuentes; [SimplePadel/ThePadelBrief](https://thepadelbrief.com/en/blog/how-to-do-bandeja-padel)).
- **Remate (smash):**
  - Potencia, con extensión completa, para cerrar el punto.
  - **Remate x3:** tras botar sale por encima de la **pared lateral** (3 m); se hace con **liftado (topspin)** [V] ([PadelStar x3](https://padelstar.es/tecnica-padel/el-remate-por-tres/)).
  - **Remate x4:** sale por encima de la **pared de fondo**. Es un golpe de timing, con **contacto muy adelantado** y la cara de la pala hacia el suelo [V] ([PadelStar x4](https://padelstar.es/tecnica-padel/remate-4-sacarla-fondo-4-metros/), [Padel World Press](https://padelworldpress.es/la-tecnica-del-padel-sacar-la-bola-x3-x4/)).
- **Chiquita:** bola lenta y corta desde el fondo que cae a los pies del que está en la red y le obliga a volear desde abajo. Después se sube a la red [V] ([Zona de Padel](https://www.zonadepadel.es/blog/2013/12/los-contraataques-el-globo-la-chiquita-y-la-bajada-de-pared/)).
- **Globo:** eleva la bola por encima de la pareja que está en la red para hacerla retroceder y ganar tiempo [V] (misma fuente).
- **Salida de pared, bajada de pared y contrapared:**
  - Golpe tras el rebote en la pared propia.
  - La bajada es agresiva tras el fondo.
  - La contrapared golpea hacia la propia pared para que la bola pase la red [V] ([Zona de Padel](https://www.zonadepadel.es/blog/2013/12/los-contraataques-el-globo-la-chiquita-y-la-bajada-de-pared/)).
- **Saque (FIP):**
  - Botar la bola dentro del área propia.
  - Golpear **a la altura de la cintura o por debajo**, con al menos un pie en el suelo.
  - En diagonal al cuadro de saque contrario [V] ([FIP Rules 2026](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf), [sportrules.org](https://www.sportrules.org/padel/rules-scoring-serving-and-walls/)).
- **Regla de paredes:** en el lado rival la bola tiene que **botar primero en el suelo** antes de tocar paredes o malla, salvo volea [V] (resumen del buscador sobre las reglas FIP).

### 3.2 Taxonomía de golpes (MVP)

Alturas, velocidades y ápices son **[P]** (propuestas de tuning) salvo que lleven fuente.

| Golpe | Contexto | Altura de contacto | Spin | Objetivo táctico | Trayectoria y velocidad [P] |
|---|---|---|---|---|---|
| **Saque** | Inicio del punto, tras botar la bola | ≤ cintura (≈0,9–1,0 m) [V regla] | Cortado o plano | Iniciar y subir a la red | Diagonal al cuadro. 50–80 km/h. Ápice bajo (≈1,3–1,6 m) |
| **Resto** | Recibe el saque | 0,4–1,0 m | Cortado o plano | Neutralizar y sacar al sacador de la red | Bajo a los pies o globo |
| **Drive / revés de fondo** | Fondo o tras la pared | 0,3–1,2 m | Plano o top | Presionar o pasar | 50–90 km/h. Ápice de 1,2 a 2 m |
| **Globo** | Fondo, rivales en la red | 0,3–1,5 m | Top (ofensivo) o cortado (defensivo) | Hacer retroceder y tomar la red [V] | Ápice de 5–8 m. Bote cerca del fondo rival |
| **Chiquita** | Fondo, rivales en la red | 0,3–1,0 m | Cortado suave | Bola a los pies, volea desde abajo [V] | 25–45 km/h. Cae entre 0 y 3 m de la red |
| **Volea** | Red o transición, sin bote | 0,5–1,8 m | Cortado | Profundidad y control | 40–80 km/h, plana y baja |
| **Bandeja** | Red, globo no muy profundo | Por encima de la cabeza, **más baja que el remate** [V] (≈2,2–2,6 m [P]) | **Slice + algo de lateral** [V] | **Mantener la red**, bote bajo tras el cristal [V] | 50–80 km/h descendente, a esquina o fondo |
| **Víbora** | Red, bola algo más alta o lateral | Similar a la bandeja, contacto lateral [V] (≈2,3–2,7 m [P]) | **Lateral marcado + cortado** [V] | Ataque o punto directo, bote incómodo en la pared lateral | 70–100 km/h |
| **Remate plano** | Red, bola alta y corta | Máxima extensión (≈2,6–3,0 m [P]) | Plano | Cerrar el punto | 100–135 km/h [V, rango medido] |
| **Remate x3** | Bola alta y cerca de la red, jugador bien colocado | Máxima extensión, ligeramente detrás de la cabeza [V] | **Topspin (liftado)** [V] | Punto: la bola sale por la **lateral** [V] | Bote fuerte y cerca del lateral |
| **Remate x4** | Bola alta y muy adelantada | Máxima extensión, **contacto adelantado** [V] | Plano o top, cara hacia abajo [V] | Punto: la bola sale por el **fondo** [V] | Bote muy vertical |
| **Bajada de pared** | Tras el rebote en el fondo propio | 0,8–1,6 m | Plano o top | Pasar de defensa a ataque [V] | 60–90 km/h |
| **Contrapared** | La bola ha superado al jugador | Variable | — | Supervivencia [V] | Golpe contra la pared propia; alto y lento |

### 3.3 Representación en datos [P]

- `ShotDefinition` como ScriptableObject y datos inmutables. Campos:
  - `family` (Attack, Control, Lob, Touch)
  - `contextRule`: rangos de altura de contacto, zona de pista, bote previo o volea y rebote en pared
  - `speedRange`
  - `apexRange` o `timeOfFlightRange`
  - `spinAxis` y `spinRate` (rpm)
  - `targetRegion`
  - `timingWindow` (ms)
  - `errorModel` (σ de dirección y de velocidad según la calidad del timing)
  - `staminaCost` (opcional)
- **Resolución:** `ShotResolver(inputFamily, charge, aimDir, BallState, PlayerState) → ShotDefinition` elegido por reglas de contexto. Es una tabla ordenada, y **gana la primera regla que encaja**. Es testeable.
- Luego `ShotSolver(ShotDefinition, contactPoint, target, quality) → (vel, spin)`.

### 3.4 Balística inversa

**Sin arrastre, dado el ápice H (altura absoluta):** p0 = contacto, p1 = objetivo (en el suelo, y = 0), con H ≥ max(y0, y1) + margen.

```
v_y0   = sqrt(2·g·(H − y0))
t_up   = v_y0 / g
t_down = sqrt(2·(H − y1) / g)
T      = t_up + t_down
v_xz   = (p1.xz − p0.xz) / T
```

**Sin arrastre, dado el tiempo de vuelo T:** `v0 = (p1 − p0 − ½·g·T²) / T`, con g vectorial. Es útil para voleas y remates rápidos, donde se fija T a partir de la velocidad deseada.

**Comprobación de red:** evaluar y(t) en el instante en que x cruza la red (z = 0) y exigir y ≥ h_red(x) + r + margen. Si falla, subir H o T y repetir.

**Con arrastre y Magnus (recomendado): método de disparo (shooting) con el mismo integrador [P].**
1. Semilla: la solución sin arrastre.
2. Simular con `BallSimulator` hasta el primer contacto con el suelo, lo que da un punto de caída p̂.
3. Corregir v0 con el error e = p1 − p̂:
   - Horizontal: escalar v_xz con un factor |Δ_obj|/|Δ_sim| y rotar en yaw para corregir la dirección.
   - Vertical: ajustar v_y por secante para mantener el ápice o el tiempo de vuelo.
4. Repetir entre 3 y 6 iteraciones hasta que |e| < 5 cm. El coste es de ≈N_iter × (T/dt) ≈ 6 × 240 pasos, trivial.
5. Como la IA y el jugador usan el mismo solver, **"apuntar" y "predecir" son la misma función**.

Método de disparo e iteración secante son técnicas numéricas estándar **[NV, no hay fuente de juego específica]**.

**Error del jugador:** aplicar el ruido **después** de resolver: perturbar v0 (yaw ± σ_dir, módulo × (1 ± σ_spd)) con una σ que depende de la calidad del timing y de la dificultad de la bola. Queda determinista con un RNG sembrado.

---

## 4. Movimiento del jugador

### 4.1 Hallazgos

- **CharacterController [NV]:** cápsula cinemática movida con `Move()`, con `slopeLimit`, `stepOffset` y `skinWidth`. No recibe fuerzas y resuelve colisiones por sweep ([Manual](https://docs.unity3d.com/Manual/class-CharacterController.html)). Está pensado para terreno arbitrario y escalones, que **no existen** en una pista plana.
- **Rigidbody cinemático [NV]:** `MovePosition` interpolado. Sirve para empujar otros rigidbodies, cosa que no necesitamos.
- **Split-step:**
  - Estudio de campo (Filipčič et al., *J. Hum. Kinet.* 2017): mide el timing del split-step respecto al impacto del rival y encuentra diferencias entre profesionales y juniors [V] ([PMC](https://pmc.ncbi.nlm.nih.gov/articles/PMC5304278/)).
  - Estudio complementario sobre la velocidad antes y después del split-step [V] ([PMC](https://pmc.ncbi.nlm.nih.gov/articles/PMC5304280/)).
  - Coaching: el despegue empieza ≈0,08 s antes del impacto rival y la **cima del salto coincide con el golpe rival**. Se cita que un split-step bien cronometrado da ≈25 % más de tiempo, o ≈60 cm más de alcance en la red [V extracto, **fuentes de coaching secundarias** ([Feel Tennis](https://www.feeltennis.net/split-step/), [My Tennis HQ](https://mytennishq.com/tennis-split-step-a-comprehensive-guide-with-videos/)); el origen de las cifras de 25 % y 60 cm **no está verificado**].
- **Movimiento en pareja:** "la pareja se mueve como atada por una cuerda de 3–4 m". Nunca uno en la red y el otro al fondo [V] ([The Padel Brief](https://thepadelbrief.com/en/blog/padel-doubles-positioning-guide), [The Padel School](https://thepadelschool.com/padel-tips/moving-as-a-pair)).
- **Wii Sports** automatiza el movimiento por completo [V, fuente comunitaria]. **Sports Story** recibe críticas por exigir una colocación demasiado precisa [V].

### 4.2 Evaluación

- La pista es un **rectángulo plano conocido** (20×10 m [V] ([BNL Italy Major](https://www.bnlitalymajorpremierpadel.com/en/News/Padel-QA/The-padel-court-measurements-and-materials))) sin obstáculos salvo la red, las paredes y el compañero.
- **NavMesh no aporta nada**: no hay rutas, solo un punto objetivo en un plano sin obstáculos. Los límites se resuelven con un clamp en X y Z, y el choque entre jugadores con separación de círculos.
- El movimiento tiene que formar parte de la **simulación determinista** (misma razón que la bola: la IA, los tests y el online futuro). CharacterController y Rigidbody lo sacan de ella.

### 4.3 Recomendación [P]

- **Movimiento totalmente propio en 2D (x, z)** dentro de `PlayerSim` en C# puro. El Transform solo presenta: interpola y anima.
- **Modelo de "sporty feel":**
  - `v_target = input · v_max · f(estado)`
  - `v += clamp(v_target − v, −a·dt, +a·dt)`
  - Con `a_accel` ≠ `a_decel` ≠ `a_turn`, porque cambiar de sentido cuesta más que arrancar.
  - Valores de partida, a tunear y sin fuente:

| Parámetro | Valor inicial [P] |
|---|---|
| v_max (sprint lateral) | 5,5 m/s |
| v_max (retroceso) | 4,0 m/s |
| Tiempo 0 → v_max | 0,25 s |
| Frenada v_max → 0 | 0,12 s |
| Cambio de sentido 180° | 0,30 s |
| Radio de alcance (sin estirarse / estirándose) | 0,9 m / 1,3 m |

  - No encontré datos biomecánicos de velocidades de desplazamiento en pádel (**laguna**). Hay que tunear con playtest.
- **Split-step como mecánica implícita:** si el jugador está en "ready" (casi parado) en el instante del golpe rival ±80 ms, recibe un **boost de arranque** (p. ej. +30 % de aceleración durante 0,2 s). Se refleja con una animación de salto. La IA lo aplica según su nivel de dificultad.
- **Asistencia de posicionamiento:**
  - `TrajectoryPredictor` calcula el **punto de golpeo ideal** (según la familia de golpe elegida, p. ej. la altura de bandeja).
  - Mientras el jugador se mueve hacia él, se aplica un **imán suave**: se suma un vector hacia el punto ideal con un peso de 0 a 0,5, según el ajuste de accesibilidad y solo en los últimos ~1,5 m.
  - Opción "movimiento automático" (estilo Wii Sports) como ajuste de accesibilidad.
- **Bloqueo durante el golpe:**
  - El movimiento se bloquea desde que empieza el swing (windup) hasta el recovery.
  - Durante el windup se permiten micro-ajustes de hasta 0,3 m/s para corregir.
  - Las fases se definen en datos por golpe: windup, contact, recovery.

---

## 5. IA

### 5.1 Hallazgos: arquitecturas

- **Game AI Pro, cap. 4 "Behavior Selection Algorithms: An Overview"** [V] ([PDF](https://www.gameaipro.com/GameAIPro/GameAIPro_Chapter04_Behavior_Selection_Algorithms.pdf)):
  - Las FSM se complican al crecer y la HFSM lo mitiga anidando máquinas.
  - Utility asigna una puntuación en coma flotante a cada opción y elige.
  - Los planificadores (GOAP, HTN) construyen secuencias hacia un estado objetivo.
  - El Behavior Tree es "más un framework que una arquitectura", porque admite cualquier selector.
- **Utility (Dave Mark y Kevin Dill, GDC 2010):** response curves para normalizar entradas y weighted random. Las transiciones duras de un if/then producen comportamientos toscos [V] ([GDC Vault](https://www.gdcvault.com/play/1012410/Improving-AI-Decision-Modeling-Through), [slides](https://media.gdcvault.com/gdc10/slides/MarkDill_ImprovingAIUtilityTheory.pdf)).
- **Utility dentro de un BT:** hay un capítulo que muestra cómo añadir decisiones de utilidad a un BT existente [V] ([Game AI Pro cap. 10, Merrill](https://www.gameaipro.com/GameAIPro/GameAIPro_Chapter10_Building_Utility_Decisions_into_Your_Existing_Behavior_Tree.pdf)).
- **Modular AI:** consideraciones reutilizables, a cargo de Dill y Dragert [V] ([Game AI Pro 3 cap. 8](https://www.gameaipro.com/GameAIPro3/GameAIPro3_Chapter08_Modular_AI.pdf)).
- **GOAP (Orkin, GDC 2006, F.E.A.R.):** FSM de **solo 3 estados** (Goto, Animate, UseSmartObject) con A* para planificar secuencias de acciones [V] ([PDF](https://pages.cs.wisc.edu/~dyer/cs540/handouts/gdc2006_orkin_jeff_fear.pdf), [GDC Vault](https://gdcvault.com/play/1013282/Three-States-and-a-Plan)).
- **Unity Behavior** (`com.unity.behavior`, BT gráfico para Unity 6): **estado actual [NV]**. No pude verificar si sigue mantenido o si ha cambiado de estado en 2026. Solo como nota: no conviene depender de él para la lógica central porque ata la IA a assets de editor y dificulta los tests en Edit Mode.

### 5.2 Hallazgos: táctica de pádel para la IA

- **3 zonas:** red, transición y fondo [V] ([Babolat](https://www.babolat.com/us/news-articles-blog-padel-tactics/padel-tactics.html)).
  - La transición (≈3–7 m de la red) es "la más peligrosa"; hay que cruzarla rápido y **no quedarse** [V] ([The Padel Brief](https://thepadelbrief.com/en/blog/padel-doubles-positioning-guide)).
- **Posiciones base:**
  - Fondo: ≈2 m (6 ft) de las paredes de fondo y lateral, detrás de la línea de saque.
  - Red: uno o dos pasos detrás de la red, a la altura del segundo poste.
  - Siempre en paralelo [V] ([SimplePadel](https://simplepadel.com/ultimate-guide-to-positions-in-padel/)).
- **Pareja como unidad:** si uno sube, sube el otro. Cuerda de 3–4 m [V] ([The Padel Brief](https://thepadelbrief.com/en/blog/padel-doubles-positioning-guide), [The Padel School](https://thepadelschool.com/padel-tips/moving-as-a-pair)).
- **Patrones:**
  - **Globo profundo → ambos avanzan a la línea de saque → si la bola sube, a la red** [V] ([Padeligo](https://padeligo.com/blog/tips/articles/padel-doubles-tactics)).
  - **Bandeja para mantener la red** frente a globos no profundos [V] (LTA).
  - **Chiquita y subir** [V] (Zona de Padel).
  - **Defensa desde el cristal** (salida o bajada de pared) [V].

### 5.3 Evaluación

- Un punto de pádel es un ciclo corto y repetitivo: **posicionarse → preparar → golpear → recuperar**. Las decisiones interesantes son **qué golpe y a dónde** (múltiples criterios) y **dónde colocarse** (depende de la pareja).
  - Para el ciclo sirve una FSM o HFSM, que es clara, testeable y depurable.
  - Para elegir golpe y colocación sirve Utility, que maneja bien varios criterios con transiciones suaves [V, Mark y Dill].
- **GOAP o HTN son sobreingeniería.** La secuencia útil tiene 1 o 2 pasos ("globo y subir") y se codifica mejor como una intención de pareja que como un plan A*.
- **Un BT no aporta frente a HFSM + Utility** con 4 agentes y un ciclo tan estructurado. Añade framework y depuración visual que no necesitamos al principio.

### 5.4 Recomendación: arquitectura de IA [P]

**Dos capas: `TeamBrain` (pareja) + `PlayerBrain` (HFSM + selección de golpe por Utility). Todo en C# puro y consumiendo solo `MatchState` y `BallPrediction`.**

```
TeamBrain (1 por pareja, decide cada vez que cambia la posesión o cada 100 ms)
 ├─ Formation: {Net, Back, TransitionUp, TransitionDown}   ← FSM de pareja
 │     Back → TransitionUp   : nuestro último golpe fue globo profundo / chiquita buena
 │     TransitionUp → Net    : rival golpea desde abajo (contacto < altura red)
 │     Net → TransitionDown  : globo rival supera punto de bandeja alcanzable
 │     TransitionDown → Back : bola va a pasar; defender tras el cristal
 ├─ BallOwner: quién va a por la bola
 │     score_i = tiempoIntercepción_i + penalización_lado(revés/centro) + preferencia_"drive/revés"
 │     (el que llega antes; empate en el centro → el que tiene la derecha hacia el centro [P])
 └─ Slots: posición objetivo de cada jugador = f(formación, x de la bola prevista, “cuerda” 3–4 m)

PlayerBrain (1 por jugador)
 HFSM:
  Ready ──(rival golpea)──► SplitStep ──► Moving
  Moving ──(bola alcanzable & ventana)──► Preparing(shotChoice) ──► Striking ──► Recovering ──► Ready
  Moving ──(no es mi bola)──► Covering (ir a slot del TeamBrain)
 Selección de golpe (al entrar en Preparing) — Utility:
  para cada ShotDefinition válida en contexto × cada TargetRegion candidata:
    U = w1·P(éxito | calidad, dificultad bola)            // prob. no fallar
      + w2·Presión(target, posición rivales)             // distancia rival a punto de golpeo, altura de contacto rival
      + w3·GananciaPosicional(formación resultante)      // p.ej. globo → podemos subir
      + w4·Coherencia(intención del TeamBrain)
      − w5·Riesgo(red, fuera, pared lateral directa)
  elección = softmax(U / temperatura)  // temperatura baja = élite
```

**Modelo de dificultad (todo en datos, `AIDifficulty` en un ScriptableObject) [P]:**

| Parámetro | Fácil | Medio | Difícil | Efecto |
|---|---|---|---|---|
| Retardo de reacción tras el golpe rival | 350 ms | 250 ms | 150 ms | Hasta entonces usa la predicción de la bola del frame anterior |
| Ruido en la predicción del punto de golpeo (σ) | 0,6 m | 0,3 m | 0,1 m | Colocación imperfecta |
| Probabilidad de split-step | 0,2 | 0,6 | 0,95 | Arranque |
| σ de ejecución (dirección / velocidad) | 8° / 12 % | 5° / 8 % | 2,5° / 4 % | Error de golpe |
| Temperatura softmax | 1,0 | 0,5 | 0,15 | Calidad de la decisión |
| Pesos de Utility | Presión baja, riesgo alto | — | Presión alta | "Personalidad" |
| Error forzado bajo presión | ×2 | ×1,5 | ×1,2 | Multiplica σ cuando la bola es difícil |

Principios:
- **La IA no hace trampas con la física.** Usa el mismo `TrajectoryPredictor` y degrada su información con ruido y retardo sembrados, así que un partido es reproducible con su semilla.
- **El compañero IA del humano:** mismo `TeamBrain`, con el humano como un miembro cuyo slot no se controla. El compañero **lee la posición del humano** y ajusta la cuerda de 3–4 m.
- En el MVP no hay rubber-banding. No encontré ninguna fuente sobre el uso de rubber-banding en deportes de raqueta (**laguna**).

---

## 6. Input

### 6.1 Hallazgos (todo [NV]: la búsqueda se agotó y la documentación de Unity estaba bloqueada)

- **Input System** ([Manual del paquete](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest)):
  - Assets `.inputactions` con Action Maps y Actions.
  - Opción **"Generate C# Class"** que genera una clase tipada.
  - Componente **`PlayerInput`**: un jugador por instancia, emparejamiento de dispositivos y envío de mensajes, UnityEvents o eventos C#.
  - **`PlayerInputManager`**: *join* de jugadores locales, con `maxPlayerCount` y split-screen opcional.
  - *Interactions* (Press, Hold, Tap, SlowTap, MultiTap).
  - `InputAction.CallbackContext.time` da la marca de tiempo del evento.
  - En Unity 6 existen las "project-wide actions".
- **Buffering en juegos de timing:** no encontré ninguna fuente primaria de juegos de raqueta que documente ventanas de buffer (**laguna**). TopSpin 2K25 evalúa la liberación del botón respecto a la zona verde [V], lo que implica medir el instante del input y no el frame.

### 6.2 Recomendación [P]

1. **Abstracción por comandos:** `PlayerCommand { int tick; Vector2 move; ShotFamily pressed; bool held; float chargeMs; Vector2 aim; bool modifier; }`. **La simulación solo consume comandos.** La IA produce los mismos comandos, y un "bot" de test también.
2. **Fuente de input:**
   - `PlayerInputManager` + `PlayerInput` (Behavior = Invoke C# Events) para el join local de hasta 4 jugadores. Cada `PlayerInput` alimenta un `HumanCommandSource`.
   - Usar la clase generada (o el `InputActionAsset` directamente) **solo en la capa adaptadora**, nunca en la simulación.
3. **Timestamps:** registrar `context.time` al pulsar y al soltar y convertirlo a tick de simulación. El timing del golpe se evalúa con el tiempo real del evento, no con el frame de render.
4. **Buffer de golpe:**
   - Una pulsación queda "viva" **120 ms** [P].
   - Si en ese intervalo la bola entra en la ventana de contacto, el golpe se ejecuta con la calidad calculada respecto al instante ideal.
   - Pulsar antes de tiempo (hasta 400 ms [P]) equivale a **cargar** (hold), como en Mario y Virtua Tennis.
5. **Dirección:** el stick se muestrea en el tick de contacto (o como media de los últimos 50 ms [P]) y se interpreta respecto a la pista, no a la cámara, porque la cámara es fija.
6. **Mapeo por defecto (mando) [P]:**

| Acción | Botón |
|---|---|
| Ataque (plano o top; en alto: víbora o remate) | Sur (A/✕) |
| Control (cortado; en alto: bandeja) | Oeste (X/□) |
| Globo | Este (B/○) |
| Modificador de toque (dejada o chiquita) | R1 |
| Remate especial x3/x4 | Ataque cargado con bola alta y cerca de la red |

---

## 7. Red: decisión de MVP

### 7.1 Hallazgos

- Rocket League: la física en red exige que **cliente y servidor simulen idénticamente**. Los problemas de buffer de input causan desincronías [V] ([GDC Vault](https://www.gdcvault.com/play/1024972/It-IS-Rocket-Science-The)).
- PhysX no es determinista entre plataformas [V, secundaria].
- **Netcode for GameObjects:** el estado del paquete en Unity 6 (versiones, Distributed Authority) es **[NV]** y lo cubre otro agente. Solo como nota: NGO es un modelo de replicación con autoridad de servidor o distribuida, **no** un framework de rollback determinista **[NV]**.

### 7.2 Recomendación

- **MVP = un jugador contra IA (1v1 y 2v2 con compañero IA). Si el coste es bajo, añadir local para hasta 4 jugadores en una sola pantalla.** El coste marginal de local es bajo si todo pasa por `PlayerCommand` y la cámara es única.
- **Online fuera del MVP.** Para no cerrarlo:
  1. **Tick fijo** (120 Hz de simulación [P]) y simulación como función pura: `MatchState(t+1) = Step(MatchState(t), Commands(t))`.
  2. **Separación estricta** estado/simulación ↔ input ↔ presentación. La presentación solo lee e interpola.
  3. **Estado pequeño y serializable:** 1 bola, 4 jugadores, marcador y RNG, en menos de 1 KB [D aproximado], lo que permite snapshots baratos y un rollback futuro.
  4. **RNG propio sembrado** (xorshift o PCG) dentro del estado. Nunca `UnityEngine.Random` en la simulación.
  5. **Sin PhysX en la simulación** (§2.7).
  6. El determinismo **entre plataformas** con float **no está garantizado** [NV]. Si en el futuro se elige lockstep o rollback entre plataformas, habrá que evaluar punto fijo. Si se elige servidor autoritativo con predicción del cliente, basta con tener un float "casi determinista" y reconciliar.

---

## 8. Testing

### 8.1 Hallazgos [NV]

- **Unity Test Framework** ([docs](https://docs.unity3d.com/Packages/com.unity.test-framework@latest)):
  - Tests en **Edit Mode** (NUnit, sin entrar en Play) y en Play Mode.
  - Los tests viven en una assembly definition de test que referencia la assembly bajo prueba.
  - Una asmdef puede marcar **"No Engine References"** (`noEngineReferences: true`) para prohibir `UnityEngine` y garantizar C# puro.

### 8.2 Recomendación [P]

**Estructura de assemblies:**

```
Assets/_Project/
  Scripts/
    Sim/                 Padel.Sim.asmdef        (noEngineReferences = true)
      Math/              Vec3 propio o System.Numerics.Vector3
      Ball/              BallState, BallParams, BallSimulator, SurfaceMaterial, TrajectoryPredictor
      Court/             CourtGeometry (planos, red h(x)), Zones
      Shots/             ShotDefinition (datos POCO), ShotResolver, ShotSolver
      Players/           PlayerState, PlayerSim (movimiento 2D)
      Rules/             PointRules (paredes, dobles botes, saque), Scoring (juegos/sets/tie-break)
      AI/                TeamBrain, PlayerBrain, UtilityScorer, AIDifficulty (POCO)
      Match/             MatchState, MatchSim.Step(state, commands), Rng
    Unity/               Padel.Unity.asmdef  (refs Sim + UnityEngine + InputSystem)
      Config/            ScriptableObjects → convierten a POCO de Sim
      Input/             HumanCommandSource, PlayerInputManager glue
      Presentation/      BallView, PlayerView (interpolación, IK, VFX, hitstop)
  Tests/
    EditMode/            Padel.Sim.Tests.asmdef  (refs Padel.Sim, nunit; Editor only)
```

**Tests clave (Edit Mode, NUnit, deterministas):**

1. **Física:**
   - Sin arrastre ni spin, el integrador está a menos de ε del parábola analítica (ε = ½·g·T·dt con semi-implícito).
   - La caída desde 2,54 m sobre el suelo rebota entre 1,35 y 1,45 m (**test de calibración FIP**).
   - Topspin frente a backspin: el bote con topspin sale más rápido en horizontal y el de backspin más lento.
   - Energía no creciente en cada rebote.
   - No hay tunneling a 40 m/s contra ningún plano.
2. **Balística inversa:** `ShotSolver` alcanza el objetivo con |e| < 5 cm y pasa la red en el 100 % de una rejilla de casos (property-based con semillas).
3. **Determinismo:**
   - Dos ejecuciones de `MatchSim` con la misma semilla y los mismos comandos dan el mismo hash de estado tras N ticks.
   - `TrajectoryPredictor` coincide con la simulación real bit a bit.
4. **Reglas:**
   - Bola que toca la malla o la pared rival sin botar antes en el suelo = **fallo del golpeador** (punto para el rival).
   - Doble bote.
   - Saque por encima de la cintura = falta.
   - Tabla de marcador (deuce, punto de oro u otro formato según configuración).
5. **IA:**
   - Con una semilla fija y un estado dado, `TeamBrain` asigna la bola al jugador con menor tiempo de intercepción.
   - La utilidad de la bandeja es mayor que la del remate ante un globo profundo con la formación en la red.
   - La dificultad Difícil tiene menos error medio que Fácil en 1000 simulaciones.
6. **Golden tests:** grabar rallies (comandos + semilla) y comparar el estado final para detectar regresiones de tuning.

---

## 9. Lagunas de evidencia

1. **Recepción de los juegos de pádel competidores** (Padel Rivals, KorrPadel, Padel Impact Pro, Padel Pro World Tour): no se pudieron leer las páginas de Steam por el bloqueo de red, ni hay reseñas indexadas. Tampoco se recuperó el número de wishlists ni la fecha exacta de Padel Pro World Tour.
2. **Postmortems primarios de física de bola en juegos de tenis o golf comerciales:** no encontrados.
3. **Regresión exacta de C_D(S) y C_L(S) de Štěpánek:** solo tengo los rangos. La fórmula lineal de §2.3 es un ajuste propio.
4. **Aerodinámica específica de la pelota de pádel** (presión menor): sin datos. Se usan los de tenis.
5. **COF y e_n del césped artificial con arena, del cristal y de la malla en pádel:** solo hay datos cualitativos (cristal más rápido que el hormigón, IJPAS 2021) y una cifra del 65–70 % de fuente secundaria. **Recomiendo medir con vídeo a 240 fps** siguiendo el método de Cross (§2.4).
6. **Momento de inercia medido de la pelota** (α ≈ 0,55): [NV].
7. **Velocidades y aceleraciones de desplazamiento de jugadores de pádel:** no encontradas.
8. **Origen de las cifras del split-step** (80 ms, 25 %, 60 cm): vienen de webs de coaching. El paper de *J. Hum. Kinet.* no se pudo leer entero.
9. **Cámara en juegos de raqueta:** no se encontraron fuentes primarias (GDC o entrevistas) sobre el diseño de cámara en rallies.
10. **Dificultad de IA en juegos de raqueta** (reacción, ruido, rubber-banding): sin charla GDC específica encontrada.
11. **APIs de Unity 6** (Input System, PlayerInputManager, CharacterController, Test Framework, `fixedDeltaTime` por defecto, estado del paquete Unity Behavior y de NGO): **[NV]**, porque la búsqueda se agotó y la documentación estaba bloqueada.
12. **Geometría fina de la pista** (escalonado de las paredes laterales de cristal, alturas de malla, puerta, línea de saque a 6,95 m): solo verifiqué 20×10 m, red de 0,88/0,92 m y fondo de 3 m de cristal con referencia a 4 m totales. **Hay que confirmarlo con las reglas FIP de 2026** ([PDF](https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf)).
13. **Formato de puntuación vigente en 2026** (punto de oro o variantes): no verificado. Lo cubre la investigación de reglas.

---

## 10. Fuentes

**Mercado y juegos**
- Padel Rivals: https://store.steampowered.com/app/4056780/Padel_Rivals/ · https://www.playpadelrivals.com/en/faq · https://www.krokanti.com/en/games/padel-rivals · https://padelbiz.it/en/2025/12/04/padel-rivals/ · https://actu-padel.com/en/padel-rivals-padel-comes-to-the-console-mid-2026/ · https://padelstar.es/noticias-padel/padel-rivals-el-primer-videojuego-de-padel-arcade-llegara-en-2026-para-pc-y-consolas/
- KorrPadel: https://korrpadel.com/en/ · https://store.epicgames.com/p/korrpadel-02411a · https://store.steampowered.com/app/4181770/KorrPadel/
- Padel Simulator: https://store.steampowered.com/app/4915770/Padel_Simulator/
- Padel Impact Pro: https://store.steampowered.com/app/4510950/Padel_Impact_Pro/
- Padel Pro World Tour: https://store.steampowered.com/app/4809310/Padel_Pro_World_Tour/
- Red Bull Padel Court Legends: https://www.padelfip.com/2026/05/mobile-gaming-meets-professional-padel-in-red-bull-padel-court-legend-the-premier-padel-game/ · https://www.redbull.com/us-en/red-bull-padel-court-legends-mobile-game-preview · https://play.google.com/store/apps/details?id=com.redbull.padel · https://padel-magazine.co.uk/red-bull-lance-le-premier-jeu-video-officiel-de-premier-padel/
- PadelVR: https://www.meta.com/experiences/padelvr-game/5701882323175143/ · https://www.meta.com/experiences/padelvr-training/8067583823340990/
- Móvil: https://play.google.com/store/apps/details?id=com.Prelogos.Padel · https://play.google.com/store/apps/details?id=com.ikermedia.hitpadelworld
- No son juegos: https://play.google.com/store/apps/details?id=com.padelrush.bookandgo&hl=en_US · https://play.google.com/store/apps/details?id=com.sportyfriends.padelVerse&hl=en

**Referentes de diseño**
- Mario Tennis Aces: https://www.shacknews.com/article/105756/controls-and-basic-button-configuration-in-mario-tennis-aces · https://www.mariowiki.com/Zone_Shot · https://en.wikipedia.org/wiki/Mario_Tennis_Aces
- Mario Tennis Fever: https://www.gamespot.com/reviews/mario-tennis-fever-review-bringing-the-heat/1900-6418460/ · https://www.nintendolife.com/reviews/nintendo-switch-2/mario-tennis-fever
- TopSpin 2K25: https://topspin.2k.com/2k25/centre-court-report/gameplay/ · https://www.tomsguide.com/gaming/topspin-2k25-review
- Virtua Tennis (secundaria): https://grokipedia.com/page/Virtua_Tennis_(video_game)
- Wii Sports (comunitaria): https://wiisports.fandom.com/wiki/Tennis_(sport) · https://strategywiki.org/wiki/Wii_Sports/Tennis
- Windjammers 2: https://www.engadget.com/2019-08-23-windjammers-2-demo-impressions-gamescom.html
- Lethal League: https://www.gamedeveloper.com/design/developing-the-stylish-indie-hit-fighting-game-i-lethal-league-blaze-i-
- Sports Story: https://gameinformer.com/review/sports-story/a-series-of-unforced-errors · https://rpgamer.com/review/sports-story-review/
- Tennis World Tour: https://www.metacritic.com/game/tennis-world-tour/critic-reviews/
- Hitstop: https://sourcegaming.info/2015/11/11/thoughts-on-hitstop-sakurais-famitsu-column-vol-490-1/
- Juice: https://www.gdcvault.com/play/1016487/Juice-It-or-Lose

**Física**
- Unity: https://docs.unity3d.com/6000.2/Documentation/Manual/collider-surface-bounce.html · https://docs.unity3d.com/6000.0/Documentation/Manual/collider-surfaces-combine.html · https://docs.unity3d.com/ScriptReference/CollisionDetectionMode.ContinuousSpeculative.html · https://docs.unity3d.com/2022.3/Documentation/Manual/physics-optimization-cpu-rigidbody-collision-modes.html · https://docs.unity3d.com/6000.3/Documentation/Manual/physics-optimization-cpu-manual-simulation.html · https://docs.unity3d.com/6000.2/Documentation/ScriptReference/PhysicsScene.Simulate.html · https://docs.unity.cn/cn/2018.3/Manual/class-PhysicsManager.html · https://forum.unity.com/threads/ball-doesnt-bounce-much-even-though-bounciness-at-1.315043/
- Rocket League: https://www.gdcvault.com/play/1024972/It-IS-Rocket-Science-The · https://media.gdcvault.com/gdc2018/presentations/Cone_Jared_It_Is_Rocket.pdf · https://pybullet.org/wordpress/index.php/2018/03/15/rocket-league-using-bullet-physics-in-unreal-engine-4/
- Aerodinámica: https://link.springer.com/article/10.1007/s12283-013-0144-9 · https://shura.shu.ac.uk/634/ · https://www.sciencedirect.com/science/article/abs/pii/S0167610504000807 · https://onlinelibrary.wiley.com/doi/abs/10.1046/j.1460-2687.2001.00083.x · https://onlinelibrary.wiley.com/doi/full/10.1002/jst.11 · https://ui.adsabs.harvard.edu/abs/1988AmJPh..56..138S/abstract · https://twu.tennis-warehouse.com/learning_center/aerodynamics2.php · https://physics.usyd.edu.au/~cross/TRAJECTORIES/42.%20Ball%20Trajectories.pdf
- Rebote: https://www.physics.usyd.edu.au/~cross/PUBLICATIONS/GripSlip.pdf · https://www.physics.usyd.edu.au/~cross/PUBLICATIONS/31.%20Spin.pdf · https://www.physics.usyd.edu.au/~cross/PUBLICATIONS/52.%20SpeedAndBounce.pdf · https://pubs.aip.org/aapt/pte/article/43/8/503/274508/The-Moment-of-Inertia-of-a-Tennis-Ball · https://doi.org/10.1080/24748668.2021.1875778 · https://thepadelbrief.com/en/blog/how-to-play-off-walls-padel
- Pelota y pista FIP: https://www.padelfip.com/wp-content/uploads/2025/12/FIP_Rules-of-Padel.pdf · https://padel.how/rules/official-fip-padel-rules/ · https://www.bnlitalymajorpremierpadel.com/en/News/Padel-QA/The-padel-court-measurements-and-materials
- Velocidad de remate: https://ojs.srce.hr/index.php/kinesiology/article/view/5656

**Golpes y táctica**
- https://www.ltapadel.org.uk/play/padel-tips-and-techniques/overheads-bandeja-and-the-smash/ · https://padelstar.es/videos-de-padel/diferencias-tecnica-bandeja-vibora/ · https://padelstar.es/tactica-padel/hago-bandeja-o-remate-diferencias-tecnica-tactica/ · https://www.tiendapadelpoint.com/en/difference-bandeja-vibora-padel-en · https://padelstar.es/tecnica-padel/el-remate-por-tres/ · https://padelstar.es/tecnica-padel/remate-4-sacarla-fondo-4-metros/ · https://padelworldpress.es/la-tecnica-del-padel-sacar-la-bola-x3-x4/ · https://www.zonadepadel.es/blog/2013/12/los-contraataques-el-globo-la-chiquita-y-la-bajada-de-pared/ · https://www.sportrules.org/padel/rules-scoring-serving-and-walls/ · https://thepadelbrief.com/en/blog/how-to-do-bandeja-padel
- Posicionamiento: https://www.babolat.com/us/news-articles-blog-padel-tactics/padel-tactics.html · https://thepadelbrief.com/en/blog/padel-doubles-positioning-guide · https://thepadelschool.com/padel-tips/moving-as-a-pair · https://simplepadel.com/ultimate-guide-to-positions-in-padel/ · https://padeligo.com/blog/tips/articles/padel-doubles-tactics
- Split-step: https://pmc.ncbi.nlm.nih.gov/articles/PMC5304278/ · https://pmc.ncbi.nlm.nih.gov/articles/PMC5304280/ · https://www.feeltennis.net/split-step/ · https://mytennishq.com/tennis-split-step-a-comprehensive-guide-with-videos/

**IA**
- https://www.gameaipro.com/GameAIPro/GameAIPro_Chapter04_Behavior_Selection_Algorithms.pdf · https://www.gameaipro.com/GameAIPro/GameAIPro_Chapter10_Building_Utility_Decisions_into_Your_Existing_Behavior_Tree.pdf · https://www.gameaipro.com/GameAIPro3/GameAIPro3_Chapter08_Modular_AI.pdf · https://www.gdcvault.com/play/1012410/Improving-AI-Decision-Modeling-Through · https://media.gdcvault.com/gdc10/slides/MarkDill_ImprovingAIUtilityTheory.pdf · https://pages.cs.wisc.edu/~dyer/cs540/handouts/gdc2006_orkin_jeff_fear.pdf · https://gdcvault.com/play/1013282/Three-States-and-a-Plan

**Unity (a verificar, [NV])**
- https://docs.unity3d.com/Packages/com.unity.inputsystem@latest · https://docs.unity3d.com/Manual/class-CharacterController.html · https://docs.unity3d.com/Packages/com.unity.test-framework@latest
