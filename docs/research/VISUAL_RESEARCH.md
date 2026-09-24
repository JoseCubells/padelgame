# Investigación visual: juego de pádel con personajes pixel art en un mundo 3D

**Fecha:** 2026-09-24
**Rol:** Technical Artist + Art Director (investigación)
**Hipótesis de motor:** Unity 6.x + URP (Universal Renderer, no 2D Renderer)
**Premisa:** pista, mundo y bola en 3D; personajes 2D pixel art como billboards dentro del 3D; cámara en tercera persona (ni cenital, ni 2D plano, ni 3D tradicional). Objetivo: una identidad visual **original** y reconocible. No vale "plantilla de Unity + filtro de píxel + bloom". No se copian personajes, logos ni assets de otros juegos.

---

## 0. Método y nivel de confianza

Durante esta sesión el proxy de salida **bloqueó WebFetch** para casi todos los dominios (gamedeveloper.com, unrealengine.com, docs.unity3d.com, blog.playstation.com, wikipedia, arxiv, davidhol.land…). Solo pudo leerse **GitHub** entero (incluido el código fuente de URP en `Unity-Technologies/Graphics`). Además, se agotó el presupuesto de búsquedas web de la sesión. Por eso cada afirmación lleva una etiqueta de confianza:

| Etiqueta | Significado |
|---|---|
| **[V]** | Verificado: leí la fuente completa o el código fuente (GitHub / raw.githubusercontent). |
| **[S]** | Extracto del buscador sobre esa URL. La URL es la fuente citada, pero **no pude leer la página completa**. Conviene releerla antes de tomar decisiones irreversibles. |
| **[S2]** | Fuente secundaria (foro, blog de terceros, reseña, vídeo divulgativo), no el desarrollador. |
| **[I]** | Inferencia o cálculo propio (razonamiento de TA/AD), sin fuente externa. Se explica el razonamiento. |
| **[?]** | Dato dudoso o contradictorio: tratarlo como hipótesis. |

---

## A. Análisis de referencias (analizar, no copiar)

### A.1 Tabla resumen

| Juego | Personajes en 3D | Cámara | Rejilla de píxel | Luz y sombras | Paleta / UI / VFX / profundidad | Qué aprendemos para el pádel |
|---|---|---|---|---|---|---|
| **Octopath Traveler / HD-2D** (Square Enix + Acquire, UE4) | Sprites pixel art como billboards en entornos 3D [S] | Fija, baja, "mirando hacia dentro" del escenario en lugar de desde arriba [S2]; efecto tilt-shift [S] | Sprites nativos a baja resolución; el entorno 3D no está pixelado de forma estricta [S] | Luz dinámica, DOF, tilt-shift → efecto diorama [S]; los sprites proyectan sombra [S2] | La DOF y el bloom ocultan una resolución sub-HD en Switch (720p máx.) [S2] | La profundidad de campo y la luz dan identidad, pero el "look HD-2D" es muy reconocible: **copiarlo nos convierte en un clon**. |
| **Triangle Strategy** (HD-2D) | Igual que Octopath | Rotación de 360° en combate, lo que exigió mapas presentables desde todos los ángulos [S] | Igual | Igual | "Diorama" [S] | Una cámara que rota dispara el coste (bordes del mapa, direcciones de sprite). **En el pádel conviene una cámara semifija.** |
| **Paper Mario** (Intelligent Systems) | Personajes 2D de "papel recortado" en mundos 3D de papel [S] | 3/4 lateral | Ilustración, no píxel | — | Cada entrega se apoya en un lenguaje de material (papel, pegatinas, pintura) [S] | **El lenguaje de material como identidad** (idea clave para la dirección D4). |
| **Don't Starve** (Klei) | Animaciones 2D planas en un mundo 3D, con billboards [S2] | No ortogonal, ligeramente inclinada [S2] | Ilustración | Oscura, gótica | Influencia de Tim Burton [S2] | Una cámara en perspectiva suave con billboards funciona con muy pocos medios. |
| **Cult of the Lamb** (Massive Monster, Unity) | Arte 2D en un mundo 3D; **los sprites escriben en depth**; shaders 3D sobre sprite renderers para proyectar sombras; Spine deforma mallas para dar volumen [S] | **Ortográfica a unos 45°**; los sprites se inclinan con el mismo ángulo que la cámara [S2] | Ilustración (no píxel) | Sombras de sprites con shaders 3D [S] | Textura de papel superpuesta, DOF, bloom, color grading [S2]; la cámara relativamente fija limitó el diseño [S] | **Esta es la receta técnica más cercana en Unity.** Depth write, sprites inclinados y sombras 3D. |
| **Eastward** (Pixpil, motor propio) | Assets de Aseprite divididos en partes, reconstruidos en 3D, con **bump maps pintados a mano** [S] | Cenital 3/4 | Pixel art estricto | Iluminación 3D, niebla, rayos de sol, SSAO simulado, LUT [S] | Calidez por la luz, no por la saturación [S] | Normal/bump map pintado a mano = luz 3D sin romper el pixel art. |
| **Sea of Stars** (Sabotage) | Pixel art 2D | Isométrica fija [S] | Estricta | Iluminación dinámica completa con un pipeline de render propio [S] | Estética de finales de los 90 con diseño moderno [S] | La luz dinámica es hoy lo esperable en pixel art premium. **No basta como diferenciador.** |
| **Golf Story / Sports Story** (Sidebar Games) | Sprites 2D en un mundo 2D | Cenital 3/4 [S] | Estricta, estilo 16 bits [S] | Plana | Estilo simple, pensado para distinguir elementos y planificar el golpe [S] | **Legibilidad por encima del detalle.** En Sports Story la crítica señaló mecánicas poco profundas [S2]. |
| **Mario Tennis** (cámara) | 3D | Por defecto elevada tras la línea de fondo; en Open existe una vista dinámica detrás del hombro [S2] | — | — | — | Hay dos arquetipos de cámara (broadcast elevada y detrás del jugador). Los evaluamos en C. |
| **Windjammers 2** (Dotemu) | Sprites 2D HD dibujados a mano [S] | Cenital [S] | Alta resolución (no píxel) | — | Críticas: personajes demasiado pequeños en pantalla por la cámara cenital [S2] | **Aviso directo: una cámara cenital empequeñece a los personajes.** Refuerza la tercera persona. |
| **Lethal League Blaze** (Team Reptile) | Modelos 3D cel-shaded en un plano 2D [S] | Lateral, con cámaras especiales en golpes potentes [S2] | — | Cel-shading | **Animación limitada (poses escogidas en lugar de 60 fps) para ganar legibilidad** [S] | Pocas poses fuertes > muchas intermedias. Reduce el coste de frames (ver E). |
| **t3ssel8r** (Unity, pieza técnica) | Todo 3D renderizado como pixel art | **Ortográfica**, ajustada a la rejilla de texel [S] | Snap de la cámara + compensación subpíxel del upscale [S2] | Cel shading para reducir el rango de color; contornos procedurales con realce solo en bordes convexos [S2] | — | **Es la referencia de estabilidad de píxel, pero solo funciona en ortográfica** [S]. |
| **Enter the Gungeon** (Dodge Roll, Unity) | Sprites 2D en un mundo 3D; **usa el depth buffer para ordenar** [S2] | Cenital 3/4 | Estricta | — | — | Ordenar con el depth buffer en lugar de sorting layers de 2D. |
| **A Short Hike** (Adam Robinson-Yu, Unity) | 3D low-poly | Tercera persona | **Render a baja resolución y ampliado; sin anti-aliasing; sombreado plano**. Opción "Pixel Size" ajustable por el jugador [S] | Plana y cohesiva [S] | — | **La prueba de que el 3D a baja resolución funciona en tercera persona con perspectiva.** Ofrecer "tamaño de píxel" como opción de accesibilidad. |
| **Dead Cells** (Motion Twin) | Modelos 3D (3ds Max) → herramienta propia que renderiza **cada frame a tamaño muy pequeño sin anti-aliasing**, con PNG + normal map y un toon shader [S] | Lateral | Estricta | Normal maps + luz 3D = toque contemporáneo [S] | — | **Es la pipeline candidata para producir personajes con muchas direcciones a bajo coste** (ver E). |
| **Rain World** (Videocult) | Animación procedural: puntos conectados más un "paper doll" de piezas [S] | Lateral fija | — | — | — | La animación procedural por piezas es otra forma de abaratar las direcciones. Es arriesgada para un juego deportivo. |
| **Hades** (Supergiant) | 3D pre-renderizado / 2D | Isométrica | No es píxel | — | Estilo audaz y de alto contraste que se lee a cualquier resolución; cada dios tiene un color identificable [S2] | **Codificar con color quién es quién** (equipo, jugador, bola). |
| **Mario Strikers: Battle League / Rematch** | 3D | Rematch: tercera persona **centrada detrás del jugador**, un pilar de diseño para la inmersión [S] | — | — | Rematch: pilares de arte "Embodiment / Connection / Tension"; impresionismo, luz vibrante [S]. Strikers: mucho squash & stretch [S2] | Una cámara detrás del jugador comunica "estoy en la pista". Definir **pilares de arte** antes que la estética. |

### A.2 Notas por juego

**Octopath Traveler / HD-2D.** Square Enix acuñó y registró "HD-2D" para describir pixel art y billboards dentro de entornos 3D, con luz dinámica, profundidad de campo y tilt-shift que dan aspecto de diorama [S] (Wikipedia HD-2D; spotlight de Unreal). Un equipo pequeño (seis programadores en el pico) se apoyó mucho en UE4 [S]. En Octopath II la proporción cabeza/cuerpo es menor y el pixel art es de "resolución alta que casi parece pixel art", con luz dinámica de día y noche [S] (entrevista de Unreal a Octopath II). Un blog de terceros afirma que la cámara usa un "tilt-shift con FOV amplio que aplana la vista" **[?]**: es contraintuitivo, porque un FOV amplio aumenta la perspectiva. No lo usamos como dato. Para las sombras de sprites en Unity, la comunidad señala que rotar el sprite en X cambia el ángulo de su sombra [S2].
*Riesgo de identidad:* cualquier combinación de tilt-shift, bloom y sprites en diorama se leerá como "imitación de HD-2D". **Lo evitamos como pilar.**

**Triangle Strategy.** Con la cámara rotable, los mapas tenían que verse bien desde los 360°. Eso consumió muchos recursos y obligó a resolver los bordes del mapa [S] (Nintendo Life, Nintendo Everything). **Lección:** una cámara libre multiplica el coste del escenario y de las direcciones de los personajes.

**Paper Mario.** Personajes 2D de papel en entornos de papel. Cada entrega busca una nueva idea basada en el papel [S] (VGC). **Lección:** un lenguaje de material coherente (papel, tinta, cerámica, etc.) crea identidad sin depender de la resolución.

**Don't Starve.** Personajes 2D en un mundo 3D mediante billboarding, con una cámara apenas inclinada [S2] (Wikipedia, foro LÖVE, foro Klei). **Lección:** la inclinación sutil más billboards es una base barata.

**Cult of the Lamb.** Todo el arte se dibuja en 2D y se coloca en 3D. Los objetos 2D escriben profundidad y usan shaders 3D para proyectar sombras, y Spine distorsiona mallas para simular volumen [S] (Unity Blog; hilo oficial de Steam). Se eligió el 2.5D al pasar a Unity y el editor sirvió para decidir ángulos de cámara y personajes [S]. Los problemas: con la cámara relativamente fija, el combate y la colocación de objetos quedaron limitados [S] (Game Rant). Según una fuente secundaria, la cámara es ortográfica a unos 45° y los sprites se inclinan igual que la cámara; además hay textura de papel, DOF, bloom y grading [S2] (itch.io devlog de recreación).

**Eastward.** El artista crea los assets en Aseprite, los divide por estructura (tejado, muro…), los reconstruye en 3D y pinta a mano los bump maps para que la luz se comporte bien [S] (80.lv). Usa niebla por capas, rayos de sol, SSAO simulado, filtro CRT y LUT [S] (Chucklefish, 80.lv). Motor propio [S].
**Lección:** la luz 3D sobre pixel art funciona **si los normales se pintan con intención**, no si se generan automáticamente.

**Sea of Stars.** Vista isométrica fija con pixel art 2D, iluminación dinámica completa y un render pipeline propio [S] (press kit de Sabotage; Wikipedia).
**Lección:** la luz dinámica en pixel art ya es un estándar del género. **Por sí sola no diferencia.**

**Golf Story / Sports Story.** Estudio de dos personas. La elección de pixel art se basó en el estilo del artista y en la simplicidad para distinguir elementos y planificar el golpe [S] (Nintendo Everything). La inspiración fue Mario Golf de GBC [S]. Sports Story (2022) es la secuela, con una rama de tenis [S]; la crítica cuestionó la profundidad de sus mecánicas deportivas [S2] (Nintendo Life).
**Lección:** en un juego deportivo, la legibilidad y la *profundidad de la mecánica* pesan más que el acabado.

**Mario Tennis (cámara).** Mario Tennis Open ofrece una vista dinámica detrás del hombro del personaje como alternativa a la vista elevada [S2] (Common Sense Media, Nintendojo). En emisión y análisis de tenis, la cámara detrás de la línea de fondo, elevada, muestra a ambos jugadores y la trayectoria completa [S2]. **No existe análisis primario de Nintendo sobre su cámara (laguna).**

**Windjammers 2.** Gráficos HD dibujados a mano [S] (GamingBolt: "all-new, high-res hand-drawn graphics"). Una reseña critica que la vista cenital hace que los personajes se vean demasiado pequeños [S2] (WayTooManyGames). **Aviso útil para nuestra cámara.**

**Lethal League Blaze.** El juego se ve en 2D, pero usa modelos 3D y fondos cel-shaded [S]. Durante el desarrollo pasaron a **animación limitada** (poses escogidas en vez de animación completa a 60 fps) para ganar legibilidad y estilo [S] (reseña de Nintendo Life; artículo de Game Developer). La bola acelera con cada golpe [S].
**Lección:** en un deporte de bola rápida, la pose clave y el *hit-stop* comunican más que la interpolación.

**t3ssel8r.** Técnica de pixel art 3D en Unity: cámara ortográfica ajustada a la rejilla de texel [S] (hilo de X de Dylan Ebert; paper Texel Splatting). Los contornos procedurales son pixel-perfect, con realce solo en bordes convexos, y el cel shading reduce el rango de color [S2] (David Holland; itch.io Brunich). Hay implementaciones públicas similares: UPixelator [V] y ProPixelizer [S].

**Enter the Gungeon.** Un vídeo divulgativo explica que el juego es "secretamente 3D": sprites en un mundo 3D ordenados con el depth buffer [S2] (YouTube). **No encontré una fuente primaria de Dodge Roll (laguna).**

**A Short Hike.** El objetivo era "un mundo 3D bonito con el menor número posible de píxeles". El juego se renderiza a baja resolución y se amplía, lo que da un sombreado plano y cohesivo sin anti-aliasing. El jugador puede ajustar el tamaño de píxel o quitarlo [S] (PlayStation Blog; FAQ de Steam).

**Dead Cells.** Thomas Vasseur hizo un model sheet en píxel, modeló y rigueó en 3ds Max y usó un programa propio que renderiza la malla muy pequeña sin anti-aliasing. Exportaba cada frame como PNG con su normal map y lo iluminaba con un toon shader [S] (Game Developer Deep Dive; Game Anim). Así se evita redibujar cada retoque [S].

**Rain World.** Animación procedural: puntos conectados con restricciones de distancia más un "paper doll" de piezas [S] (GDC Vault Animation Bootcamp 2016; Game Developer).

**Hades.** Estilo audaz y de alto contraste; cada personaje se identifica por su color [S2] (Point'n Think). Supergiant se define como "design-led" [S] (MCV/Develop). **No encontré una charla GDC primaria de arte sobre legibilidad (laguna).**

**Rematch / Mario Strikers.** En Rematch, la tercera persona centrada detrás del jugador fue una decisión central de inmersión, y los pilares de arte (Embodiment, Connection, Tension; futurista, feel-good, optimista, impresionismo) guiaron la dirección [S] (entrevista de Unreal; ArtStation Magazine). En Mario Strikers: Battle League, las reseñas destacan el squash & stretch [S2].

---

## B. Técnicas en Unity 6 URP: pros y contras

### B.1 Resolución interna baja + escalado nearest-neighbour entero

**Opción B.1a: Render Scale del URP Asset + Upscaling Filter "Nearest-Neighbor".**
- El enum `UpscalingFilterSelection` de URP 17 (Unity 6.0) contiene `Auto`, `Linear` (Bilinear), `Point` ("Nearest-Neighbor"), `FSR` y `STP` [V] (código fuente de `UniversalRenderPipelineAsset.cs`, rama 6000.0/staging).
- Render Scale está limitado a **0.1–2.0** [V] (`UniversalRenderPipeline.minRenderScale = 0.1f`, `maxRenderScale = 2.0f`).
- En modo "Automatic", la documentación indica que se usa nearest-neighbour si el escalado entero es posible y bilinear si no [V] (docs de URP en GitHub, rama 2022.3).
- **Contras [I]:** con una resolución de salida de 2160p, 180p exigiría una escala de 0.083, por debajo del mínimo de 0.1, así que **no se puede**. Además, la resolución resultante es un porcentaje, no un tamaño fijo, y cambia con la ventana: el tamaño del píxel "artístico" no es constante entre dispositivos.
- **Pros:** coste cero de implementación. La UI en *Screen Space - Overlay* no se ve afectada por Render Scale y queda nítida a resolución nativa [S2] (Unity Trouble Atlas; discusiones de Unity). Si la UI va en un overlay camera apilado, sí se degrada [S2].

**Opción B.1b: RenderTexture de tamaño fijo (p. ej. 480×270, filtro Point) + blit o RawImage a pantalla, con escala entera y letterbox.**
- Es la técnica clásica: renderizar a un RT de la resolución deseada con filtro point y mostrarlo con una segunda cámara o un RawImage que llena la pantalla [S2] (discusiones de Unity).
- **Pros [I]:** tamaño de píxel artístico idéntico en todas las pantallas; escala entera garantizada; control total del letterbox o recorte.
- **Contras:** hay que gestionar la entrada y el raycast a través del RT, el post-procesado a baja resolución y la relación de aspecto (ultrapanorámicos).

**Opción B.1c: ScriptableRendererFeature propia (render graph).**
- En URP 17 (Unity 6.0), los render passes personalizados deben reescribirse con la API de render graph [S] (guía de actualización a URP 17).
- **Pros:** permite pixelar solo ciertas capas (mundo y personajes) y dejar a resolución nativa otras (UI, algún VFX). Es lo que hacen activos como ProPixelizer, que permite mezclar objetos pixelados y no pixelados con intersección y profundidad [S] (web de ProPixelizer).
- **Contras:** más ingeniería y más mantenimiento frente a cambios de URP.

**Recomendación [I]:** prototipar con B.1b (control total) y pasar a B.1c si hace falta mezclar capas. Usar B.1a solo para una prueba de look de un día.

### B.2 Snap de cámara a la rejilla de texel (enfoque t3ssel8r) y por qué falla en perspectiva

- **Cómo funciona:** la cámara (ortográfica) se ajusta a una rejilla del tamaño de un píxel en el espacio del mundo, de modo que los objetos estáticos producen los mismos colores al moverse. Para que el movimiento no parezca a saltos, la imagen ampliada se desplaza por la diferencia entre la posición real y la ajustada [V] (documentación de UPixelator en GitHub). UPixelator indica que el beneficio de "pixel creep reduction" es **exclusivo de la ortográfica**. En perspectiva pixeliza, pero sin estabilización [V].
- Los objetos móviles pueden ajustarse a la misma rejilla, pero eso introduce un zigzag que se nota menos cuanto más rápido es el movimiento [V] (UPixelator).
- **Por qué falla en perspectiva:** en ortográfica, todos los píxeles se desplazan igual al mover la cámara y un único snap los corrige. En perspectiva, los píxeles a distinta profundidad se desplazan a distinto ritmo, así que ningún snap corrige todas las profundidades [S] (paper *Texel Splatting: Perspective-Stable 3D Pixel Art*, Dylan Ebert, arXiv 2603.14587, 2026). Ese paper propone renderizar la escena a un cubemap desde un punto fijo y "splatear" cada texel como un quad en el mundo. El origen se ajusta a una rejilla del mundo. La demo WebGPU da 240 fps en una RTX 4090 y 40 fps en un iPhone 15 [S]. **Es una técnica de investigación: no la recomendamos para producción en un equipo pequeño [I].**
- **Implicación para el pádel [I]:**
  1. La pista es pequeña (20×10 m, FIP [S]). **La cámara puede ser casi fija**. Si la cámara no se mueve, la geometría estática no "repta" (no hay pixel crawl), y el problema se reduce a los objetos móviles, que se mueven igualmente.
  2. Si la cámara acompaña lateralmente, el desplazamiento puede ajustarse a la rejilla de texel **del plano focal** (la profundidad media de la pista). Es una aproximación: los planos cercanos y lejanos seguirán reptando algo. Un **FOV estrecho (teleobjetivo, 20–30°)** reduce la diferencia de desplazamiento entre profundidades y, con ella, la reptación.
  3. Alternativa que conviene probar: cámara **ortográfica oblicua** (como Cult of the Lamb [S2]). Ofrece estabilidad total de píxel a cambio de perder el cambio de tamaño con la distancia.

### B.3 Pixel Perfect Camera de URP: limitaciones en 3D y perspectiva

- Propiedades [V] (documentación de URP en GitHub): Assets PPU, Reference Resolution, Grid Snapping (None / Pixel Snapping / Upscale Render Texture), Crop Frame y Stretch Fill. Pixel Snapping ajusta los **Sprite Renderers** a una rejilla en XY del mundo; Upscale Render Texture renderiza al tamaño de referencia y amplía.
- El componente **fija `camera.orthographicSize`** cada frame (salvo en modo de compatibilidad con Cinemachine) [V] (`PixelPerfectCamera.cs`, rama 2022.3). **Su modelo matemático es ortográfico.**
- **La ruta de RT offscreen y upscale solo está implementada en el 2D Renderer** [V]: `Renderer2D.cs` y `Renderer2DRendergraph.cs` (6000.0) consultan el `PixelPerfectCamera`, mientras que `UniversalRenderer.cs` no tiene ninguna referencia en las ramas 2022.3, 6000.0, 6000.2 y master. En los foros aparece el error "pixel perfect camera requires a camera using a 2D renderer" [S2].
- El 2D Renderer no sirve para nuestro caso: su material `Sprite-Lit-Default` responde a luces 2D, no a luces 3D [S] (documentación y discusiones de Unity).
- **Conclusión [I]:** el Pixel Perfect Camera **no sirve** para un juego 3D con Universal Renderer, luces 3D, sombras y cámara en perspectiva. Hay que hacer un sistema propio (B.1b/B.1c + B.2).

### B.4 Modos de billboard

| Modo | Descripción | Pros | Contras |
|---|---|---|---|
| **Esférico (full camera-facing)** | El quad se orienta en los tres ejes hacia la cámara o su plano [S] (Lighthouse3D) | El sprite nunca se escorza: se ve exactamente como se dibujó | Con la cámara inclinada hacia abajo, el quad se tumba hacia atrás: los pies atraviesan el suelo y la cabeza atraviesa las paredes de cristal detrás. La sombra proyectada cambia de forma con la cámara |
| **Cilíndrico (restringido a Y)** | Solo rota alrededor del eje vertical [S] (Lighthouse3D; gist de unitycoder) | Planta los pies en el suelo, se ordena bien en profundidad y proyecta sombras estables | Con una cámara inclinada θ, el sprite se ve escorzado ≈cos θ [I]: a 30° mide un 13 % menos, a 45° un 29 % menos. Los píxeles se "aplastan" verticalmente de forma no entera |
| **Cilíndrico + inclinación hacia la cámara con pivote en los pies** | Rota en Y y se inclina en X el mismo ángulo que el pitch de la cámara, con pivote en la base. Cult of the Lamb inclina los sprites como la cámara [S2] | Sin escorzo y con los pies fijos | La parte alta del quad avanza hacia la cámara y puede intersectar la red o el cristal cercano. Se mitiga con un depth offset en el shader [I] |

**Recomendación [I]:** probar el modo cilíndrico con inclinación y pivote en los pies, más un *depth bias* configurable. Con una cámara semifija, el pitch es casi constante y la inclinación puede ser fija por escena.

### B.5 Selección de dirección del sprite (4/8/16) según el ángulo relativo a la cámara

- **Técnica:** calcular con `atan2` el ángulo entre la orientación del personaje y el vector cámara→personaje, proyectados en el plano XZ, y cuantizarlo en N sectores [S2] (foros de GameDev.net).
- **Doom:** 8 rotaciones con solo 5 imágenes únicas, porque 3 se obtienen por espejo (p. ej. `TROOA2A8`) [S] (ZDoom Wiki; DoomWiki).
- **[I] Histéresis:** añadir un margen de ±5–8° al cambiar de sector para que el sprite no parpadee entre dos direcciones cuando el ángulo está en la frontera. En los golpes, **bloquear la dirección durante la animación** para que un swing no cambie de vista a mitad.

### B.6 Ordenación y profundidad: alpha clip frente a alpha blend

| | Alpha clip (opaco con recorte) | Alpha blend (transparente) |
|---|---|---|
| Qué hace | Descarta los píxeles bajo un umbral; el resto es opaco [V] (docs del Lit de URP) | Mezcla con el fondo en una pasada separada tras los opacos [V] |
| Depth | Escribe en depth y se ordena solo | Normalmente no escribe en depth: problemas de orden entre sprites, cristal y partículas [I] |
| Sombras | Puede proyectar con ShadowCaster respetando el recorte [S2] (discusiones de Unity) | Sombras problemáticas [S2] |
| Encaje con pixel art | **Natural**: el pixel art tiene bordes duros | Solo para VFX y cristal |

- Cult of the Lamb hace que sus objetos 2D escriban profundidad [S]. Según un vídeo divulgativo, Enter the Gungeon ordena con el depth buffer [S2].
- **Intersección con el suelo [I]:** con un billboard cilíndrico y el pivote en los pies no hay problema. Si se inclina, usar depth offset o, alternativamente, escribir la profundidad "de los pies" en todo el quad (depth plano por sprite) para que todo el personaje ordene como un bloque.
- **El cristal del pádel [I]:** las paredes son transparentes, y un sprite detrás de un cristal con blend es el caso patológico de ordenación. Opciones: cristal como opaco + reflejo pintado; cristal transparente que no escribe en depth, dibujado después de los personajes; o cristal "falso" (marco + reflejos + suciedad, sin transparencia real).

### B.7 Sombras de sprites billboard

- **ShadowCaster + alpha clip:** crear un Shader Graph Lit con *Cast Shadows* y *Alpha Clipping* activados y el umbral conectado [S2] (Unity Discussions; Unity Trouble Atlas). En el SpriteRenderer hay que activar Cast Shadows desde el Inspector en modo Debug [V] (repositorio Victor-Go/Unity-Sprite-Shader-for-URP). Hay que nombrar la textura principal `_MainTex` para que el SpriteRenderer y las animaciones la sustituyan [S2] (Medium, Bruno Lorenz).
- **Problema [I]:** la sombra se proyecta desde el quad, que mira a la cámara, no a la luz. Con una luz lateral rasante (hora dorada), el quad queda casi de canto a la luz y la sombra se vuelve una línea. Además, la sombra cambia al moverse la cámara.
- **Soluciones [I]:**
  1. **Billboard de sombra orientado a la luz:** en la pasada ShadowCaster se orienta el quad hacia la dirección de la luz principal, no hacia la cámara, con el mismo sprite. Es la idea que sugiere la observación comunitaria de que la rotación en X cambia la sombra en juegos tipo Octopath [S2]. **Sin fuente primaria: validar en el prototipo.**
  2. **Blob shadow** (elipse con Decal Projector de URP o quad en el suelo): barata, estable y **esencial para la bola**. La sombra de la bola en el suelo es la señal de altura más legible en 3D [I].
  3. **Híbrido (recomendado) [I]:** blob siempre (lectura de juego) + sombra real proyectada para ambiente, solo con la luz principal.

### B.8 Iluminación de sprites

| Enfoque | Descripción | Pros | Contras | Referencia |
|---|---|---|---|---|
| **Lit + normal map** | Shader Lit propio con normal map por frame | Luz 3D real, integración con la escena | Doble coste de arte por frame (color + normal); los normales automáticos quedan "plásticos" | Dead Cells exporta un normal map por frame desde 3D [S]; Eastward pinta los bump maps a mano [S] |
| **Unlit + rampa de paleta** | Se calcula N·L (o solo la luz ambiente o de zona) y se indexa una rampa de 3–5 colores de la paleta | La paleta queda intacta y hay control artístico total | Menos "realismo", hace falta un sistema de rampas | [I] (técnica estándar de toon/paleta) |
| **Lit con rim** | Luz plana + borde de luz de contraluz | Separa la silueta del fondo, **mejora la legibilidad** | Puede parecer "efecto de motor" si se abusa | [I] |

**Nota:** `Sprite-Lit-Default` y las Secondary Textures (`_NormalMap`, `_MaskTex`) son del ecosistema 2D Renderer y luces 2D [S] (manual de Unity), no de luces 3D.

**Recomendación [I]:** unlit + rampa de paleta como base (identidad y control), con un término de rim y la sombra recibida (shadow attenuation) cuantizada a 2 niveles.

### B.9 Contornos: horneados en el sprite frente a shader

- **Horneado en el sprite (1 px, selout de tono oscuro):** control total. Si el sprite se escala de forma no entera, el grosor varía (1 px se convierte en 0 o 2) [I].
- **Por shader a baja resolución:** contornos por depth y normal en el mundo 3D (t3ssel8r: realce solo en bordes convexos [S2]; ProPixelizer: contornos por silueta o por normal y por objeto [S]).
- **Recomendación [I]:** personajes con contorno horneado; mundo 3D con contorno por shader solo en siluetas relevantes (red, líneas y marcos de pared); **bola con contorno garantizado de 1 px** (ver C).

### B.10 Densidad de texel entre mundo 3D y sprites

- El pixel art en 3D solo funciona si todos los objetos resuelven la misma cantidad de texels por unidad del mundo [S2] (Beyond Extent y otros).
- **[I] Regla propuesta:** fijar una densidad objetivo en px/m **a la distancia media de la pista** y crear texturas de mundo y sprites con esa densidad. Ver C.3 para los valores.
- **[I] Advertencia de perspectiva:** en perspectiva, los sprites cercanos se amplían (>1:1) y los lejanos se reducen (<1:1). La reducción de pixel art sin mipmaps elimina píxeles de forma inestable (shimmer). Las mitigaciones están en C.4.

### B.11 Interacción con el anti-aliasing

- El MSAA suaviza los bordes de la geometría por multisampling. La documentación de URP recomienda desactivarlo en 2D o cuando el rendimiento es crítico [V].
- FXAA puede desenfocar las texturas [S2].
- A Short Hike prescinde del anti-aliasing como parte del look [S].
- **Recomendación [I]:** **MSAA, FXAA y TAA desactivados** en la pasada del mundo a baja resolución, porque cualquier AA genera píxeles intermedios y rompe la paleta. Si hace falta suavizar el movimiento de rotación, se estudia más adelante un upscale "pixel-art aware" (t3ssel8r tiene un vídeo sobre shaders de upscaling de pixel art [S2]), no un AA clásico. **STP y FSR quedan descartados** para el upscale final.

### B.12 UI: resolución nativa frente a interna

- *Screen Space - Overlay* se dibuja fuera del loop de cámara y a resolución nativa. Con cámaras overlay apiladas, la UI hereda la resolución reducida [S2].
- **Recomendación [I]:** separar dos capas.
  1. **HUD de juego** (marcador, indicadores de golpe): arte pixel dibujado con el *mismo tamaño de píxel* que el mundo (escala entera de la resolución interna) para mantener la coherencia.
  2. **Texto largo y menús**: se permite una tipografía a resolución nativa por legibilidad y accesibilidad (localización, tamaños de texto). A Short Hike ya ofrece ajustar el pixelado como opción [S].

---

## C. Plan de prueba de resoluciones internas

### C.1 Supuestos
- Pista FIP de 20×10 m; paredes de fondo de 4 m (3 m de vidrio + 1 m de malla); red de 88 cm en el centro [S] (padel.how, Book & Go, BNL Italy Major).
- Jugador de 1,80 m; bola de unos 6,5 cm de diámetro (dato aproximado).
- Jugador "cerca" en z = −8 m (cerca de su línea de fondo); jugador "lejos" en z = +8 m.
- Cálculo [I] con proyección en perspectiva real (script propio: posición de cámara, punto de mira, FOV vertical, altura proyectada en px). Error estimado de ±10 % por la posición fuera del eje.

Cámaras evaluadas:
- **Cámara A (cercana, detrás del jugador):** (0; 5; −14), FOV vertical 40°, mira a (0; 0,5; 0).
- **Cámara B (broadcast):** (0; 9; −21), FOV 30°, mira a (0; 0; 1).
- **Cámara C (tele, aplanada):** (0; 13; −32), FOV 20°, mira a (0; 0; 1).

### C.2 Resultados: altura proyectada (px) jugador / bola

| Resolución | Cam A cerca | Cam A lejos | Cam B cerca | Cam B lejos | Cam C cerca | Cam C lejos |
|---|---|---|---|---|---|---|
| 320×180 | 55 / 2,0 | 20 / 0,7 | 34 / 1,3 | 20 / 0,7 | 31 / 1,1 | 21 / 0,8 |
| 384×216 | 66 / 2,4 | 24 / 0,9 | 41 / 1,5 | 24 / 0,9 | 37 / 1,3 | 25 / 0,9 |
| 480×270 | 83 / 3,0 | 30 / 1,1 | 52 / 1,9 | 29 / 1,1 | 46 / 1,7 | 32 / 1,2 |
| 640×360 | 110 / 4,0 | 40 / 1,4 | 69 / 2,5 | 39 / 1,4 | 62 / 2,2 | 42 / 1,5 |

Ratio de tamaño cerca/lejos: A ≈ 2,75×; B ≈ 1,75×; C ≈ 1,45×.

### C.3 Lectura de los resultados [I]
1. **La bola a tamaño físico desaparece** (0,7–1,5 px en el campo contrario) en todas las resoluciones. **Hay que hacer "trampa":** bola billboard con un **tamaño mínimo en píxeles** (3–4 px a 270p) con contorno, más una **sombra blob** siempre visible y, opcionalmente, un rastro. Es una decisión de legibilidad, no de realismo, coherente con la lección de legibilidad de Golf Story [S] y con la aceleración visible de Lethal League [S].
2. **El jugador lejano necesita ≥28–32 px** para leer bandeja, víbora o remate. A 320×180 y 384×216 se queda en 20–25 px. **Es insuficiente para leer golpes (hipótesis a validar).**
3. **La Cámara A** da una diferencia de 2,75× entre jugadores: los sprites del jugador cercano se ampliarían y los del lejano se reducirían mucho, con fuerte inestabilidad de píxel. **Las cámaras B/C** (más tele) **se acercan más a un tamaño de sprite uniforme**, lo que favorece el pixel art.
4. **Densidad de texel orientativa** a media pista: ~21 px/m con 480×270 y cámara B/C (≈38 px por 1,8 m); ~17 px/m con 384×216; ~28 px/m con 640×360.

### C.4 Tres estrategias de escala de sprite (a prototipar) [I]
- **P1 (escala de mundo):** el billboard se escala con la perspectiva. Es natural, pero hay remuestreo no entero. Con la cámara C, el rango de escala es 0,8–1,15× y el daño queda acotado.
- **P2 (píxel constante):** el sprite se dibuja siempre a escala 1:1 de píxel. La profundidad se comunica por posición, sombra y oclusión, no por tamaño. Es la máxima pureza de píxel, pero con una perspectiva "rara" en la pista.
- **P3 (ortográfica oblicua):** tamaño constante por construcción y estabilidad total de píxel (B.2). Se pierde la sensación de distancia.

### C.5 Integridad del escalado entero por pantalla

| Interna | 1280×720 | 1280×800 (Deck) | 1920×1080 | 2560×1440 | 3840×2160 |
|---|---|---|---|---|---|
| 320×180 | ×4 | ×4 (+letterbox) | ×6 | ×8 | ×12 |
| 384×216 | ×3,33 ✗ | ×3 (+letterbox) | ×5 | ×6,67 ✗ | ×10 |
| 480×270 | ×2,67 ✗ | ×2 (+letterbox grande) | ×4 | ×5,33 ✗ | ×8 |
| 640×360 | ×2 | ×2 (+letterbox) | ×3 | ×4 | ×6 |

✗ = sin entero exacto: hay que usar letterbox al entero inferior o aceptar el escalado no entero. [I] (aritmética).

Además, **Render Scale no llega a 180p a 4K** (mínimo 0,1) [V]. Otro motivo para usar B.1b.

### C.6 Plan de prueba recomendado
1. **Candidata principal: 480×270** con cámara C (jugadores de 32–46 px). Es la mejor relación entre legibilidad y "píxel visible".
2. **Candidata de detalle: 640×360** con cámara B/C. Es entera en todas las resoluciones habituales y lee mejor los golpes, a cambio de más coste por sprite y de que el píxel "se note" menos.
3. **Control de estilo extremo: 320×180**, solo para ver hasta dónde aguanta la identidad. **Previsiblemente fallará en la legibilidad del jugador lejano.**
4. **384×216** solo si 480×270 resulta "demasiado limpio".

Protocolo [I]: la misma escena grey-box en las cuatro resoluciones × 3 cámaras × P1/P2/P3. Tests ciegos con 5–8 personas en los que deben identificar el tipo de golpe del jugador lejano y la altura de la bola. Medir el porcentaje de aciertos y el tiempo de reacción.

---

## D. Direcciones visuales originales

> Todas reservan un **color exclusivo para la bola** (amarillo óptico) que no aparece en ningún otro elemento del juego. Cada dirección tiene un **pilar material** distinto, para no depender de "píxel + bloom". Los valores hex son propuestas de partida, no una paleta final.

### D1: "Sobremesa"
- **Mood:** las 18:30 de un día de julio en un club de urbanización. El calor empieza a ceder y la luz rasante es dorada, con sombras larguísimas y chicharras. Nostalgia cálida, sin postal turística.
- **Paleta:**
  - Cal y tapia: `#F1E6D2` (base cálida) y `#D8C7A8` (tapia en sombra).
  - Terracota: `#C4553B` (tejas) y `#8E3B2E` (terracota en sombra).
  - Pista y agua: `#2F6F8F` (césped azul de pista) y `#1F4A63` (sombra de pista).
  - Sombras: `#4B3A6B` (sombra violeta de cielo rasante).
  - Vegetación: `#7A9A3A` (seto y adelfa) y `#E7A04A` (luz rasante).
  - Bola reservada: `#E4F53A`.
- **Escenario:** pista rodeada de tapias encaladas, pinos y adelfas, un toldo de lona y una piscina fuera de campo que refleja la luz. **Clichés que se evitan:** toros, flamenco y banderas. La identidad sale de la *luz* y del *urbanismo de club*.
- **Personajes:** siluetas deportivas con ropa de club (polos, gorras y viseras) y una paleta propia por equipo. Rim cálido lateral.
- **UI:** marcador como un **cartel de chapa pintado a mano** (tipografía pixel de rotulista), con tiza para las estadísticas.
- **VFX:** polvo dorado al frenar, destello seco en el impacto, polillas al anochecer.
- **Iluminación:** luz principal rasante y cálida (15–20° de elevación) con una ambiente violeta. Las sombras largas son el **sello visual**.
- **Pixel art:** 480×270, contorno horneado en selout (tono oscuro de cada color, no negro), 3–4 tonos por material con rampa.
- **Ventajas:** identidad cálida y mediterránea sin tópicos. La luz de sobremesa es poco habitual en juegos deportivos [I]. Puede evolucionar en ciclo horario hacia D2 con los mismos assets.
- **Problemas:** las sombras largas de billboards son el caso técnico más difícil (B.7). La luz rasante puede reducir el contraste de la bola a contraluz.
- **Dificultad de producción:** 3/5. **Diferenciación:** 4/5.

### D2: "Luz de Mástil"
- **Mood:** partido nocturno en un club de barrio rioplatense o levantino. Focos en mástiles, halos en la humedad, insectos alrededor de la luz y el resto del mundo a oscuras. Íntimo y tenso.
- **Paleta:**
  - Noche: `#0E1430` (fondo) y `#1C2750` (noche con relleno).
  - Pista: `#1E4F8A` (azul de pista iluminada).
  - Luz: `#9FE8FF` (LED blanco-cian) y `#FFB45A` (sodio antiguo, el farol de la calle).
  - Material: `#E9EEF2` (líneas y cristal iluminado) y `#3B3F4A` (metal de estructura).
  - Bola reservada: `#E4F53A`.
- **Escenario:** la pista es una "isla de luz" y el exterior se resuelve con siluetas y pocas luces (ventanas, farolas). El contraste entre focos LED nuevos y farolas de sodio viejas cuenta el barrio sin texto.
- **Personajes:** iluminación cenital dura de los focos, sombras múltiples (una por mástil) y rim frío.
- **UI:** marcador LED de **matriz de puntos** (coherente con el pixel art de forma diegética).
- **VFX:** polillas, halos, reflejos de focos en el cristal y vaho en invierno.
- **Iluminación:** 4 luces puntuales o focales como protagonistas. La ambiente casi negra ayuda a leer la bola.
- **Pixel art:** 480×270 o 640×360, contorno interior por rim en lugar de contorno negro, 2 tonos de sombra.
- **Ventajas:** la bola destaca de forma natural. Los exteriores oscuros abaratan el escenario. Mood fuerte.
- **Problemas:** varias luces con sombras en URP tienen coste. Hay riesgo de parecer "neón synthwave" si se satura; **evitar el magenta**. Es monótono como única ambientación.
- **Dificultad de producción:** 3/5. **Diferenciación:** 3/5 (los nocturnos son comunes; el contraste LED/sodio es la clave).

### D3: "Polígono"
- **Mood:** pádel indoor en una nave de polígono industrial reconvertida. Chapa, fluorescentes, suelo de resina, grafiti de club y una máquina de vending. Urbano, obrero, auténtico.
- **Paleta:**
  - Estructura: `#9AA3A8` (chapa galvanizada) y `#5E676D` (chapa en sombra).
  - Acentos industriales: `#F2C230` (amarillo de seguridad) y `#D94A2B` (rojo extintor).
  - Luz y suelo: `#E8F4EC` (tubo fluorescente) y `#2B3A55` (resina de suelo).
  - Pista: `#3C7A5A` (césped verde de pista indoor).
  - Bola reservada: `#E4F53A`.
- **Escenario:** interior acotado (sin cielo ni horizonte), con varias pistas en fila detrás que juegan otros partidos como fondo animado de bajo coste. Pancartas de patrocinadores locales **inventados**.
- **Personajes:** luz de techo difusa y fría, sombras suaves y cortas. Blob shadow casi siempre suficiente.
- **UI:** estética de **cinta americana y rotulador** sobre la pizarra de reservas del club.
- **VFX:** parpadeo de fluorescente, eco visual (ondas) en el golpe contra la chapa, chispa de goma en el suelo.
- **Iluminación:** controlada y uniforme, **la más fácil de prototipar**. La identidad viene del material, no de la luz.
- **Pixel art:** 480×270, contorno por shader en la estructura y horneado en los personajes, dithering ordenado en los degradados de chapa.
- **Ventajas:** muy barata (interior, pocas luces). Refleja una realidad del pádel español actual [I] sin tópicos. Estable técnicamente.
- **Problemas:** puede resultar "gris" y poco aspiracional. Hay que empujar el color con la señalética. Necesita personajes con mucho carácter.
- **Dificultad de producción:** 2/5. **Diferenciación:** 4/5.

### D4: "Tinta Riso"
- **Mood:** el juego parece un **cartel de torneo amateur impreso en risografía**: tintas limitadas, desregistro, tramas de medio tono y papel. Gráfico, de fanzine y de diseño.
- **Paleta** (4 tintas + papel; aproximaciones a tintas riso, **no valores oficiales**):
  - Papel: `#F4EFE3`.
  - Tintas: `#FF4FA3` (rosa fluorescente), `#0078BF` (azul), `#00838A` (verde azulado), `#1F1F24` (negro tinta).
  - Bola: `#FFE800` (amarillo riso, reservado a la bola).
- **Escenario:** geometría 3D sombreada **solo con tramas** (dithering ordenado o medio tono en espacio de pantalla a baja resolución), sin degradados. Las sombras son una tinta superpuesta con un pequeño desregistro.
- **Personajes:** sprites de 2–3 tintas, con desregistro de 1 px en los golpes fuertes como VFX.
- **UI:** tipografía de cartel y el marcador como sello de tinta.
- **VFX:** impactos como "manchas" de tinta; la trayectoria de la bola como línea de trama.
- **Iluminación:** N·L cuantizado en 3 niveles, que se mapean a 3 densidades de trama por tinta.
- **Pixel art:** 384×216 o 480×270; sin contorno (la silueta la da la tinta); trama estable en espacio de objeto o del mundo para evitar el "shower-door" [I].
- **Ventajas:** **diferenciación máxima**; paleta mínima (arte barato por asset); muy "posteable" en redes; lenguaje de material como en Paper Mario [S].
- **Problemas:** las tramas en pantalla con una cámara que se mueve reptan (ver B.2). Hay que resolver bien si la trama va anclada al mundo o a la pantalla. Legibilidad con 4 tintas y 4 jugadores más la bola: poco margen de color para equipos. Estilo más "de nicho".
- **Dificultad de producción:** 4/5 (shader). **Diferenciación:** 5/5.

### D5: "Vitrina"
- **Mood:** la pista como una **vitrina de museo o maqueta de cristal** flotando en un vacío claro. El cristal, rasgo único del pádel, es el protagonista: reflejos, huellas y marcas de bola. Limpio, contemporáneo y de diseño.
- **Paleta:**
  - Vacío y cristal: `#F6F7F8` (vacío blanco) y `#BFDCE3` (cristal).
  - Estructura: `#8FA7B0` (canto de vidrio) y `#1A1B20` (tinta y estructura).
  - Pista: `#2D5DA8` (azul de pista).
  - Acentos de equipo: `#FF5A36` (naranja equipo A) y `#6B3FD1` (violeta equipo B).
  - Bola reservada: `#E4F53A`.
- **Escenario:** mínimo, sin grada. El suelo del vacío tiene una rejilla sutil. Las paredes de cristal registran **marcas de impacto persistentes** (decals pixelados) que narran el punto.
- **Personajes:** muy saturados sobre el fondo neutro, con contorno negro fino horneado.
- **UI:** diseño suizo en pixel, con etiquetas "de museo".
- **VFX:** refracción y reflejo en el cristal al rebotar (flash de 2–3 frames) y huellas de bola.
- **Iluminación:** luz de estudio suave. La sombra de contacto es clave y el cielo es blanco.
- **Pixel art:** 640×360 (el vacío tolera más resolución), contorno negro de 1 px y 2 tonos.
- **Ventajas:** convierte la mecánica diferencial del pádel (las paredes) en identidad. Mínimo coste de escenario.
- **Problemas:** el cristal transparente es el caso más difícil de ordenación (B.6). El fondo blanco hace que el amarillo de la bola pierda contraste (necesita contorno oscuro). Riesgo de frialdad y poca "cultura".
- **Dificultad de producción:** 3/5 (cristal). **Diferenciación:** 4/5.

### D.6 Comparativa

| Dirección | Pilar | Dificultad (1–5) | Diferenciación (1–5) | Riesgo principal |
|---|---|---|---|---|
| D1 Sobremesa | Luz rasante mediterránea | 3 | 4 | Sombras de billboard |
| D2 Luz de Mástil | Focos nocturnos de barrio | 3 | 3 | Parecer synthwave |
| D3 Polígono | Material industrial indoor | 2 | 4 | Grisura |
| D4 Tinta Riso | Material de impresión | 4 | 5 | Trama y legibilidad |
| D5 Vitrina | Cristal como protagonista | 3 | 4 | Transparencia y ordenación |

### D.7 Dirección recomendada [I]

**D1 "Sobremesa" como identidad principal, con D2 como su continuación horaria.** Es la misma pista y el mismo club a otra hora, así que se reutilizan los assets. De D5 se toma solo **un** rasgo: las marcas persistentes en el cristal.

Razones:
1. **Identidad cultural sin tópicos:** el pádel es cultura de club y de urbanización en España y Argentina. La hora de la sobremesa y los focos de la noche lo evocan sin recurrir a folclore.
2. **Una identidad hecha de luz, no de cantidad de assets:** es escalable para un estudio pequeño. Es la lección de Eastward [S] y de A Short Hike [S].
3. **Variedad sistémica:** un ciclo tarde → anochecer → noche da escenarios "nuevos" con el mismo kit.
4. **Legibilidad:** el violeta y el azul de las sombras contrastan con el amarillo reservado de la bola.
5. **Evita el look HD-2D:** no se basa en tilt-shift ni en DOF de diorama.

**Control barato en paralelo:** D3 "Polígono" como grey-box de referencia (luz uniforme) para aislar los problemas de píxel y cámara de los problemas de luz. D4 queda como **"wildcard"** para una prueba de un día de shader.

---

## E. Direcciones del personaje: 4 frente a 8 frente a 16

### E.1 Fórmula de coste
`frames únicos = direcciones únicas × Σ(frames por animación)`

**Lista de animaciones mínima para el pádel [I]** (con animación limitada al estilo de Lethal League [S], pocas poses fuertes):

| Animación | Frames |
|---|---|
| idle | 6 |
| carrera | 8 |
| desplazamiento lateral | 8 |
| split-step | 4 |
| drive | 8 |
| revés | 8 |
| volea de derecha | 6 |
| volea de revés | 6 |
| bandeja | 8 |
| víbora | 8 |
| remate | 10 |
| globo | 8 |
| saque | 10 |
| salida de pared / rescate | 8 |
| celebración | 8 |
| reacción | 6 |
| **Total por dirección** | **≈120** |

| Direcciones | Únicas sin espejo | Únicas con espejo (flipX) | Frames sin espejo | Frames con espejo | Ahorro |
|---|---|---|---|---|---|
| 4 (N, E, S, O) | 4 | 3 (E = O espejo) | 480 | 360 | 25 % |
| 8 | 8 | 5 (como Doom [S]) | 960 | 600 | 37,5 % |
| 16 | 16 | 9 | 1920 | 1080 | 44 % |

Por personaje y por variante de equipación. Con 8 personajes, 8 direcciones y espejo, salen ≈4 800 frames. **Solo es viable a mano si los personajes comparten base** (swap de paleta y cabeza) **o con una pipeline 3D → píxel.**

### E.2 El problema del espejo en el pádel [I]
- **Al espejar a un diestro se obtiene un zurdo**: el drive pasa a verse como un revés y la pala cambia de mano. En el pádel, la lateralidad importa tácticamente (posición drive o revés).
- **Opciones:**
  - (a) Espejar solo animaciones simétricas (carrera, idle, lateral) y dibujar los golpes en todas sus direcciones. El ahorro real baja a ~15–20 % en 8 direcciones.
  - (b) **Separar la pala como capa o sprite propio** que no se espeja, sino que se re-ancla a la mano: más complejo, pero conserva el ahorro.
  - (c) Aceptar la inversión: **no recomendado**, porque confunde la lectura táctica.
- Doom espeja 3 de sus 8 rotaciones [S], pero sus monstruos no tienen una lateralidad táctica relevante.

### E.3 Legibilidad según la cámara [I]
- Con una **cámara semifija detrás de la línea de fondo** (B/C), el equipo cercano se ve casi siempre de **espaldas o a 3/4 de espaldas** y el lejano de **frente o a 3/4 de frente**. Los perfiles aparecen en desplazamientos laterales y en golpes.
- Con **4 direcciones**, el salto de "espalda" a "perfil" al girar 45° se nota mucho en un jugador de 40–80 px, sobre todo en carreras diagonales, que en el pádel son constantes (subir a la red en diagonal).
- **8 direcciones** cubren las diagonales, que son el caso dominante.
- **16 direcciones** solo aportan con una cámara libre o con giros lentos y visibles. Con una cámara semifija y ≤50 px de altura, la diferencia entre 22,5° y 45° **se percibe poco (hipótesis a validar en la prueba ciega)**.

### E.4 Juegos de referencia por número de direcciones
- **8:** Doom (8 con 5 únicas por espejo) [S]; Ragnarok Online (sprites 2D en un mundo 3D con cámara rotable; el sprite cambia según el ángulo de vista) [S2].
- **8/16 (y hasta 32):** Diablo II usa animaciones de 1, 2, 4, 8, 16 o 32 direcciones según el tipo [S2] (tutorial de DCC de d2mods).
- **Esqueletal en lugar de frames por dirección:** Cult of the Lamb con Spine [S].
- Don't Starve parece usar vistas frontal, lateral y trasera, pero **[?] no verificado** (no encontré documentación de Klei).

### E.5 Recomendación para el prototipo
1. **Prototipar 8 direcciones** (5 únicas) **solo para locomoción** (idle, carrera, lateral) con espejo.
2. **Golpes en 5 direcciones útiles según la cámara** (espalda, 3/4 espalda izquierda y derecha, perfil izquierdo y derecho para el equipo cercano; lo mismo de frente para el lejano). **La pala va en capa separada** para permitir el espejo sin cambiar la lateralidad.
3. **Evaluar en paralelo la pipeline 3D → píxel al estilo Dead Cells** [S]: modelo 3D low-poly, render ortográfico por dirección a la resolución final sin AA, más normal o lightmask, y limpieza a mano de los frames clave. **Con esa pipeline, pasar de 8 a 16 direcciones cuesta tiempo de render, no de dibujo**, y la decisión 8 frente a 16 deja de ser de producción y pasa a ser de legibilidad.
4. **Prueba ciega** con la misma escena: 4 frente a 8 frente a 16 direcciones en un jugador de 32–46 px (480×270, cámara C). Medir si se detecta la diferencia y si mejora la anticipación del golpe.

---

## Lagunas de evidencia

1. **Acceso a fuentes:** WebFetch estaba bloqueado por la política de salida para casi todos los dominios, y el presupuesto de búsquedas se agotó. Las afirmaciones [S] se basan en extractos del buscador, no en la lectura completa. **Prioridad de relectura:**
   - Deep dive de Dead Cells (Game Developer).
   - Spotlight de Unreal sobre Octopath.
   - Blog de Unity sobre Cult of the Lamb.
   - PlayStation Blog sobre A Short Hike.
   - Paper Texel Splatting (arXiv).
   - Artículo de David Holland.
2. **Octopath / HD-2D:** no hay datos primarios de la altura de los sprites en píxeles, el FOV, el método de sombras ni el número de direcciones. La afirmación sobre el "FOV amplio" es dudosa.
3. **Enter the Gungeon:** solo hay fuente divulgativa (YouTube), no de Dodge Roll.
4. **t3ssel8r:** la técnica se conoce por sus vídeos y por recreaciones de terceros. **No hay un texto técnico del autor**, así que los detalles concretos (compensación subpíxel, contornos) se apoyan en fuentes secundarias y en la implementación de UPixelator.
5. **Mario Tennis, Mario Strikers, Hades:** no se encontraron charlas GDC ni textos primarios sobre la cámara o la legibilidad. Las notas proceden de reseñas o de terceros.
6. **Documentación de Unity 6 sobre Pixel Perfect y el Universal Renderer:** la conclusión "solo en el 2D Renderer" se basa en la **lectura del código fuente** (2022.3, 6000.0, 6000.2, master). Conviene confirmarla contra el manual 6000.x cuando haya acceso.
7. **Sombra de billboard orientada a la luz (B.7.1):** es una propuesta de diseño sin fuente primaria. **Validar en el prototipo.**
8. **Las cifras de C** son cálculos propios con cámaras hipotéticas. **Hay que recalcularlas con la cámara real del prototipo.**
9. **Tintas riso (D4):** los hex son aproximaciones, no valores oficiales del fabricante.
10. **No se verificó la ausencia de parecido** con juegos de pádel existentes. Queda pendiente una búsqueda de competencia (store scan) antes de fijar la dirección.

---

## Fuentes

**Etiqueta de acceso:** [V] leída o verificada; [S] extracto del buscador; [S2] secundaria.

### Unity / URP (técnica)
- [V] URP `UniversalRenderPipelineAsset.cs` (enum UpscalingFilterSelection), rama 6000.0: https://raw.githubusercontent.com/Unity-Technologies/Graphics/6000.0/staging/Packages/com.unity.render-pipelines.universal/Runtime/Data/UniversalRenderPipelineAsset.cs
- [V] URP `UniversalRenderPipeline.cs` (min/max render scale), 6000.0: https://raw.githubusercontent.com/Unity-Technologies/Graphics/6000.0/staging/Packages/com.unity.render-pipelines.universal/Runtime/UniversalRenderPipeline.cs
- [V] `PixelPerfectCamera.cs` (2022.3): https://raw.githubusercontent.com/Unity-Technologies/Graphics/2022.3/staging/Packages/com.unity.render-pipelines.universal/Runtime/2D/PixelPerfectCamera.cs
- [V] `Renderer2D.cs` (2022.3): https://raw.githubusercontent.com/Unity-Technologies/Graphics/2022.3/staging/Packages/com.unity.render-pipelines.universal/Runtime/2D/Renderer2D.cs
- [V] `Renderer2DRendergraph.cs` (6000.0): https://raw.githubusercontent.com/Unity-Technologies/Graphics/6000.0/staging/Packages/com.unity.render-pipelines.universal/Runtime/2D/Rendergraph/Renderer2DRendergraph.cs
- [V] `UniversalRenderer.cs` (6000.0; sin referencias a PixelPerfect): https://raw.githubusercontent.com/Unity-Technologies/Graphics/6000.0/staging/Packages/com.unity.render-pipelines.universal/Runtime/UniversalRenderer.cs
- [V] Documentación de URP: Pixel Perfect (2022.3): https://github.com/Unity-Technologies/Graphics/blob/2022.3/staging/Packages/com.unity.render-pipelines.universal/Documentation~/2d-pixelperfect.md
- [V] Documentación de URP: URP Asset (render scale, upscaling, MSAA): https://github.com/Unity-Technologies/Graphics/blob/2022.3/staging/Packages/com.unity.render-pipelines.universal/Documentation~/universalrp-asset.md
- [V] Documentación de URP: Lit shader (alpha clipping, surface type): https://github.com/Unity-Technologies/Graphics/blob/2022.3/staging/Packages/com.unity.render-pipelines.universal/Documentation~/lit-shader.md
- [V] Espejo de la documentación de Pixel Perfect (URP 7.7.1): https://github.com/Raphael2048/URP_ForwardPlus/blob/master/Packages/com.unity.render-pipelines.universal@7.7.1/Documentation~/2d-pixelperfect.md
- [S] Manual de Unity 6: Pixel Perfect Camera reference: https://docs.unity3d.com/6000.4/Documentation/Manual/urp/2d-pixelperfect-ref.html
- [S] Manual de Unity 6: Introduction to Pixel Perfect Camera: https://docs.unity3d.com/6000.1/Documentation/Manual/urp/2d-pixelperfect-intro.html
- [S] Manual de Unity 6: Upgrade to URP 17 (render graph): https://docs.unity3d.com/6000.0/Documentation/Manual/urp/upgrade-guide-unity-6.html
- [S] Manual de Unity 6: Secondary textures for sprites: https://docs.unity3d.com/6000.0/Documentation/Manual/urp/SecondaryTextures.html
- [S] Issue Tracker: Nearest-Neighbor upscaling filter: https://issuetracker.unity.com/issues/9504
- [S2] Discusión: "Pixel perfect camera requires a camera using a 2d renderer": https://discussions.unity.com/t/help-error-pixel-perfect-camera-requires-a-camera-using-a-2d-renderer/894691
- [S2] Discusión: 3D lights on 2D sprites in URP: https://discussions.unity.com/t/how-to-make-2d-sprites-be-affected-by-3d-lighting-in-urp/836389
- [S2] Discusión: sombra de sprites en URP: https://discussions.unity.com/t/make-sprite-cast-and-receive-shadows-in-urp/897404
- [S2] Discusión: ángulo de sombras al estilo Octopath: https://discussions.unity.com/t/how-do-games-like-octopath-traveler-handle-the-angle-of-shadows-and-is-this-even-possible-in-unity-urp/246459
- [S2] Discusión: Render Scale Point Filtering: https://discussions.unity.com/t/render-scale-point-filtering/765897
- [S2] Discusión: UI a resolución nativa con Screen Space Camera: https://discussions.unity.com/t/always-render-ui-at-native-resolution-when-using-screen-space-camera/890293
- [S2] Unity Trouble Atlas: UI borrosa con Render Scale: https://unity-trouble-atlas.7colorsgame.com/en/article/unity-urp-render-scale-blurry-ui/
- [S2] Unity Trouble Atlas: sombras de materiales transparentes en Shader Graph: https://unity-trouble-atlas.7colorsgame.com/en/article/unity-shader-graph-transparent-shadows/
- [V] Victor-Go / Unity Sprite Shader for URP (sombras de sprites): https://github.com/Victor-Go/Unity-Sprite-Shader-for-URP
- [S2] Bruno Lorenz, Sprite Shadows in Unity 3D (URP): https://medium.com/@brunolorenz98/sprite-shadows-in-unity-3d-with-sprite-animation-color-support-urp-1b7d9d118e21

### Pixel art 3D / snapping
- [V] Documentación de UPixelator (snap y compensación subpíxel, solo ortográfica): https://github.com/Radivarig/UPixelator_Documentation
- [S] Texel Splatting (Dylan Ebert, 2026): https://arxiv.org/html/2603.14587v1 · https://dylanebert.com/texel-splatting/ · https://x.com/dylan_ebert_/status/2033315367766884602
- [S2] David Holland, 3D Pixel Art Rendering: https://www.davidhol.land/articles/3d-pixel-art-rendering/
- [S2] Recreating t3ssel8r's 3D pixel art (Unity Discussions): https://discussions.unity.com/t/recreating-t3ssel8rs-3d-pixel-art/928878
- [S2] Canal de t3ssel8r: https://www.youtube.com/@t3ssel8r
- [S2] "Crafting a Better Shader for Pixel Art Upscaling": https://www.youtube.com/watch?v=d6tp43wZqps
- [S2] Kit de Brunich al estilo t3ssel8r (Godot): https://brunich.itch.io/3d-pixel-art-shadergame-kit-t3ssel8r-style-renderer
- [S] ProPixelizer (web y guía): https://sites.google.com/view/propixelizer/home · https://sites.google.com/view/propixelizer/user-guide-v1-7
- [S2] KYRIOTA, Unity Pixelated Art Style in URP: https://kyriota.com/2022/08/02/UnityPixelatedArtStyleInURP/
- [S] Lighthouse3D, Billboarding tutorial: https://www.lighthouse3d.com/opengl/billboarding/billboardingtut.pdf
- [S2] Cylindrical Billboard Shader (gist de unitycoder): https://gist.github.com/unitycoder/1452d25348fb1f2daa52115ca7867d0f
- [S2] Selección de dirección con atan2 (GameDev.net): https://gamedev.net/forums/topic/679371-3d-8-directional-sprite-rotation-based-on-facing-direction-relative-to-camera-direction/5296655/
- [S] ZDoom Wiki, Sprite (rotaciones y espejo): https://zdoom.org/wiki/Sprite · https://doomwiki.org/wiki/Sprite
- [S2] Densidad de texel: https://www.beyondextent.com/deep-dives/deepdive-texeldensity

### Referencias de juegos
- [S] HD-2D (Wikipedia): https://en.wikipedia.org/wiki/HD-2D
- [S] Unreal Engine: Octopath Traveler HD-2D: https://www.unrealengine.com/en-US/spotlights/octopath-traveler-s-hd-2d-art-style-and-story-make-for-a-jrpg-dream-come-true
- [S] Unreal Engine: Octopath Traveler II: https://www.unrealengine.com/en-US/developer-interviews/octopath-traveler-ii-builds-a-bigger-bolder-world-in-its-stunning-hd-2d-style
- [S2] Samppy (blog; afirmación del FOV dudosa): https://samppy.com/octopath-travelers-hd-2d/
- [S2] Goomba Stomp, cámara de Octopath: https://goombastomp.com/project-octopath-traveler-carries-legacy-3d-functionality/
- [S2] Digital Foundry vía Cheerful Ghost: https://cheerfulghost.com/jdodson/posts/3814/digital-foundry-looks-at-the-tech-behind-octopath-traveler
- [S] Triangle Strategy: https://www.nintendolife.com/news/2022/05/triangle-strategy-producers-talk-hd-2d-and-why-other-devs-havent-used-it · https://nintendoeverything.com/triangle-strategy-devs-on-how-the-game-uses-accurate-hd-2d/
- [S] Paper Mario (VGC): https://www.videogameschronicle.com/features/interviews/paper-mario-origami-king/ · https://shmuplations.com/papermario/
- [S2] Don't Starve: https://en.wikipedia.org/wiki/Don%27t_Starve · https://kleiforums.com/forums/topic/56775-is-there-a-name-for-the-games-2d-as-3d-animation-style/ · https://love2d.org/forums/viewtopic.php?t=77692
- [S] Cult of the Lamb (Unity Blog): https://unity.com/blog/games/recipe-behind-smash-hit-cult-of-the-lamb
- [S] Cult of the Lamb (Game Rant): https://gamerant.com/cult-of-the-lamb-interview-massive-monster-inspirations-pandemic-design/
- [S] Cult of the Lamb (Steam): https://steamcommunity.com/app/1313140/discussions/0/3448087385671383167/?l=english
- [S2] Recreación de la cámara de Cult of the Lamb: https://itch.io/blog/536578/making-mechanics-of-cult-of-the-lamb-in-unity-movement-camera
- [S] Eastward: https://80.lv/articles/eastward-charming-chinese-pixel-art-adventure · https://chucklefish.org/blog/introducing-eastward-by-pixpil/ · https://www.gamedeveloper.com/art/eastward-s-creators-share-insights-on-making-pixel-art-adventures · https://pixpilgames.tumblr.com/post/132985856947/garage-dynamic-lighting-test
- [S] Sea of Stars: https://sabotagestudio.com/presskits/sea-of-stars/ · https://en.wikipedia.org/wiki/Sea_of_Stars · https://lrmonline.com/news/how-the-retro-inspired-sea-of-star-captures-the-feeling-of-90s-rpgs-exclusive-interview/
- [S] Golf Story: https://nintendoeverything.com/interview-golf-story-dev-on-wii-u-origins-cut-ideas-and-more/ · https://en.wikipedia.org/wiki/Golf_Story
- [S] Sports Story: https://sidebargames.com/presskit.html · https://en.wikipedia.org/wiki/Sports_Story · https://www.nintendolife.com/reviews/switch-eshop/sports-story
- [S2] Mario Tennis Open (vista dinámica): https://www.commonsensemedia.org/game-reviews/mario-tennis-open · https://www.nintendojo.com/reviews/mario-tennis-open-review
- [S2] Cámara de análisis de tenis: https://www.tennismethod.com/best-camera-angles-for-tennis-video-analysis/
- [S] Windjammers 2: https://gamingbolt.com/windjammer-2-interview-characters-courts-and-more · https://en.wikipedia.org/wiki/Windjammers_2 · https://waytoomany.games/2022/01/20/review-windjammers-2/
- [S] Lethal League Blaze: https://www.gamedeveloper.com/design/developing-the-stylish-indie-hit-fighting-game-i-lethal-league-blaze-i- · https://www.nintendolife.com/reviews/switch-eshop/lethal_league_blaze · https://team-reptile.com/press/sheet.php?p=Lethal_League_Blaze
- [S2] Enter the Gungeon (vídeo): https://www.youtube.com/watch?v=DrYti1nI7aA
- [S] A Short Hike: https://blog.playstation.com/2021/08/05/crafting-a-tiny-open-world-a-look-behind-the-scenes-at-the-creation-of-a-short-hike/ · https://steamcommunity.com/app/1055540/discussions/0/1639792569850442961/ · https://www.youtube.com/watch?v=ZW8gWgpptI8
- [S] Dead Cells: https://www.gamedeveloper.com/production/art-design-deep-dive-using-a-3d-pipeline-for-2d-animation-in-i-dead-cells-i- · https://www.gameanim.com/2018/01/31/dead-cells-3d-pipeline-2d-animation/
- [S] Rain World: https://www.gdcvault.com/play/1023475/Animation-Bootcamp-Rainworld-Animation · https://www.gamedeveloper.com/art/video-animating-i-rain-world-i-and-its-many-squishy-stretchy-creatures
- [S2] Hades: https://www.pointnthink.fr/en/the-art-of-hades-en/ · https://mcvuk.com/business-news/behind-the-art-of-hades-we-value-artistic-integrity-and-excellence-in-artistic-craft-at-supergiant-however-were-first-and-foremost-a-game-design-lead-team/
- [S] Rematch: https://www.unrealengine.com/developer-interviews/rematch-reinvents-football-using-unreal-engine-5 · https://magazine.artstation.com/2025/10/sloclap-rematch-art-blast/
- [S2] Mario Strikers: Battle League: https://nextlevelgames.com/mario-strikers-battle-league/ · https://www.t3.com/reviews/mario-strikers-battle-league-review
- [S2] Ragnarok Online: https://en.wikipedia.org/wiki/Ragnarok_Online · https://gamedev.net/forums/topic/680181-question-on-camera-rotation-on-2d-billboard-sprite-3d-level/
- [S2] Diablo II DCC (direcciones): https://d2mods.info/resources/infinitum/tut_files/dcc_tutorial/chapter2.html

### Pádel (reglamento)
- [S] Reglas FIP (resumen): https://padel.how/rules/official-fip-padel-rules/
- [S] Dimensiones: https://www.bookandgo.app/en/learn/padel-court-dimensions · https://www.bnlitalymajorpremierpadel.com/en/News/Padel-QA/The-padel-court-measurements-and-materials
