# Investigación tecnológica — Motor y stack para el juego de pádel

- **Fecha de investigación:** 2026-09-24
- **Rol:** Research Agent / Technical Architect
- **Proyecto:** juego de pádel — mundo y pista 3D, pelota física 3D, personajes 2D pixel-art (billboards) en espacio 3D, cámara en tercera persona, 1v1 y 2v2, primero single-player + IA.
- **Hipótesis de partida:** Unity 6.3 LTS (a verificar, no a asumir).

---

## 0. Método y limitaciones (leer primero)

### 0.1 Fuentes consultadas y cómo

| Canal | Estado en esta sesión | Uso |
|---|---|---|
| `WebSearch` | Disponible al inicio; **se agotó el presupuesto de la sesión (200/200)** a mitad de la investigación | Descubrimiento; los resúmenes del buscador sobre páginas oficiales se usan como evidencia de nivel **B** |
| `WebFetch` / `curl` a `unity.com`, `docs.unity3d.com`, `discussions.unity.com`, `game.ci`, `dev.epicgames.com`, `unrealengine.com`, `godotengine.org`, `endoflife.date` | **Bloqueado por el proxy de salida** (`EGRESS_BLOCKED`) | No se pudo leer ninguna página de estos dominios directamente |
| `git clone` / `git ls-remote` de repos **públicos** de GitHub | Disponible | Lectura directa de código fuente, changelogs y tags oficiales (Unity-Technologies, game-ci, github/gitignore, godotengine) |
| `raw.githubusercontent.com` | Disponible | Lectura directa de ficheros concretos |
| API pública de Docker Hub (`hub.docker.com/v2/...`) | Disponible | Listado de imágenes `unityci/editor` (sirve también como inventario de versiones de Unity publicadas) |

### 0.2 Niveles de verificación usados en este documento

- **[A] Verificado directamente**: leído en esta sesión en una fuente primaria (código fuente oficial, changelog oficial, tag git oficial, registro Docker, fichero oficial).
- **[B] Verificado vía buscador**: la afirmación aparece en el resumen del buscador atribuida a una URL oficial, pero **no se pudo abrir la página** (bloqueo de red). Fiabilidad media-alta, pero sin lectura literal.
- **[C] Fuente secundaria**: blog/medio de terceros (80.lv, CG Channel, etc.), solo vía buscador.
- **[NV] No verificado en esta sesión**: conocimiento previo o inferencia; se marca explícitamente y requiere comprobación antes de decidir sobre ello.

> Las fechas de "commit/tag" de GitHub son la fecha del commit que Unity publica en su repo espejo; se aproximan a la fecha de release pero **no son** la fecha oficial del anuncio.

---

## 1. Unity: versiones, soporte, licencias, Linux y activación headless

### 1.1 Hallazgos — versiones de Unity 6 existentes a 2026-09-24

Inventario obtenido de los **tags oficiales** del repo `Unity-Technologies/UnityCsReference` (espejo del código C# del editor, un tag por release) **[A]**, contrastado con las imágenes `unityci/editor` de Docker Hub **[A]**:

| Línea | Nombre comercial | Primer tag | Último tag (a 2026-09-24) | Fecha commit último tag | Tipo |
|---|---|---|---|---|---|
| 6000.0 | Unity 6.0 LTS | 6000.0.0b11 | **6000.0.84f1** | 2026-09-16 | LTS |
| 6000.1 | Unity 6.1 | 6000.1.0a2 | 6000.1.17f1 | — | Supported Update (cerrada) |
| 6000.2 | Unity 6.2 | 6000.2.0a1 | 6000.2.15f1 | — | Supported Update (cerrada) |
| 6000.3 | **Unity 6.3 LTS** | 6000.3.0a2 | **6000.3.24f1** | 2026-09-10 | **LTS** (6000.3.0f1: 2025-12-03) |
| 6000.4 | Unity 6.4 | 6000.4.0a2 | 6000.4.12f1 | 2026-06-17 | Supported Update (6000.4.0f1: 2026-03-18) |
| 6000.5 | Unity 6.5 | 6000.5.0a3 | 6000.5.11f1 | 2026-09-02 | Supported Update (6000.5.0f1: 2026-06-15) |
| 6000.6 | Unity 6.6 | 6000.6.0a2 | **6000.6.2f1** | 2026-09-18 | Supported Update actual (6000.6.0f1: 2026-08-31) |
| 6000.7 | Unity 6.7 (anunciada como LTS) | 6000.7.0a1 (2026-06-25) | **6000.7.0b1** | 2026-09-17 | **Beta** |

Fuentes: [UnityCsReference tags](https://github.com/Unity-Technologies/UnityCsReference/tags) (leído con `git ls-remote` y `git log` sobre cada tag, 2026-09-24) **[A]**; [Docker Hub unityci/editor](https://hub.docker.com/r/unityci/editor/tags) (API, 2026-09-24: existen imágenes `ubuntu-6000.3.24f1-*` de 2026-09-10, `ubuntu-6000.6.2f1-*` de 2026-09-18, `ubuntu-6000.0.84f1-*` de 2026-09-16; ninguna `6000.7`) **[A]**.

Otros hallazgos:

- Unity 6.3 LTS: soporte LTS de dos años **hasta diciembre de 2027**, +1 año adicional para Enterprise/Industry — [Unity 6 Releases & Support](https://unity.com/releases/unity-6/support) y [Unity 6.3 LTS is now available (blog)](https://unity.com/blog/unity-6-3-lts-is-now-available) **[B]** (fecha del blog no visible; release 6000.3.0f1 = 2025-12-03 según tag **[A]**).
- Unity 6.0 LTS: soportada **hasta octubre de 2026** — [Unity 6 Releases & Support](https://unity.com/releases/unity-6/support) **[B]**. Es decir, está a punto de salir de soporte.
- Cadencia: Unity declara un LTS anual y varias "Supported Update" por año; recomienda LTS para juegos live / producción bloqueada y Update releases para producciones nuevas o a mitad de ciclo — [Unity 6 Releases & Support](https://unity.com/releases/unity-6/support) **[B]**.
- Unity 6.4 publicada el 2026-03-20 — [CG Channel, 2026-03](https://www.cgchannel.com/2026/03/unity-releases-unity-6-4-and-unity-studio/) **[C]** (tag 6000.4.0f1 del 2026-03-18 **[A]**).
- Unity 6.5 publicada a mediados de junio de 2026; es "Supported release" — [Unity Discussions: Unity 6.5 is now available](https://discussions.unity.com/t/unity-6-5-is-now-available/1723176) **[B]**, [CG Channel, 2026-06](https://www.cgchannel.com/2026/06/unity-releases-unity-6-5-discover-5-key-features-for-cg-artists/) **[C]**.
- Unity 6.6 es "Supported release" (no LTS), "última build no-LTS antes de **Unity 6.7 LTS, prevista para Q4 [2026]**"; introduce componentes de CoreCLR, WebGPU production-ready para Web, serialización nativa de `Dictionary`, "Content Directories", Build Analysis window y Fast Enter Play Mode por defecto en proyectos nuevos — [Unity Discussions: Unity 6.6 is now available](https://discussions.unity.com/t/unity-6-6-is-now-available/1735357) **[B]**, [AlternativeTo, 2026-09](https://alternativeto.net/news/2026/9/unity-6-6-adds-webgpu-build-analysis-and-coreclr-prep/) **[C]**.
- **Contradicción detectada sobre CoreCLR / Unity 7:** una fuente de terceros afirma que Unity 7 entra en beta en diciembre (2026) con release en Q1 del año siguiente ([Inven Global](https://www.invenglobal.com/articles/24003/unity-engine-7-changing-the-development-paradigm-and-the-roadmap-ahead) **[C]**), mientras que otro resumen afirma que el runtime de scripting se sustituye por CoreCLR en **Unity 6.8** ([Unity Discussions: Path to CoreCLR, 2026](https://discussions.unity.com/t/path-to-coreclr-2026-upgrade-guide/1714279) **[B]**). No se pudo leer ninguna de las dos páginas; **el roadmap post-6.7 no está verificado**.

### 1.2 Hallazgos — precios y licencia (2026)

- **Runtime Fee cancelada** para clientes de juegos; vuelta al modelo por asiento; Unity Personal sigue gratis y el techo de ingresos/financiación sube de 100.000 a **200.000 USD** — [Unity is Canceling the Runtime Fee (blog)](https://unity.com/blog/unity-is-canceling-the-runtime-fee) **[B]** (publicado en septiembre de 2024 según conocimiento previo **[NV]** para la fecha exacta) y [Terms update: Runtime Fee cancellation](https://unity.com/blog/terms-update-runtime-fee-cancellation) **[B]**.
- **Subida del 5 % de Unity Pro y Enterprise a partir del 12-ene-2026** (los suscriptores existentes la reciben en la renovación) — [Unity Pricing Changes](https://unity.com/products/pricing-updates) **[B]**.
- Precio de Pro tras la subida: **2.310 USD/asiento/año** (antes 2.200) — solo en fuentes de terceros ([vendr](https://www.vendr.com/marketplace/unity), [tech-insider](https://tech-insider.org/unity-vs-unreal-engine-2026/)) **[C]**. **La cifra exacta no está verificada en fuente oficial.**
- Unity Pro/Enterprise/Industry sobre 6.0 LTS dejan de incluir Havok Physics for Unity; Unity Version Control en cloud pública elimina cargos por asiento (Q1 2026); free tier de DevOps ampliado a 25 GB — [Unity Pricing Changes](https://unity.com/products/pricing-updates) **[B]**; nuevos cargos de DevOps desde 2026-03-01 — [Unity Support: New Unity DevOps charges](https://support.unity.com/hc/en-us/articles/34748492914964-Understanding-New-Unity-DevOps-charges-starting-from-Mar-1-2026) **[B]** (solo título/resumen).
- Unity Personal puede desactivar/personalizar el splash "Made with Unity" desde 6000.0.23f1 — [Release notes 6000.0.23f1](https://unity.com/releases/editor/whats-new/6000.0.23f1) **[B]**.

### 1.3 Hallazgos — Linux Editor, batchmode y activación headless

- **Linux Editor:** Unity 6.3 soporta el Editor en Linux **solo en Ubuntu, versiones 22.04 y 24.04**; soporte Wayland experimental — [System requirements for Unity 6.3](https://docs.unity3d.com/6000.3/Documentation/Manual/system-requirements.html) **[B]**. (Existen páginas equivalentes para 6.4, 6.5 y 6.6; no se pudieron leer.)
- **Batchmode headless:** las imágenes oficiales de GameCI ejecutan el editor como `xvfb-run -ae /dev/stdout "$UNITY_PATH/Editor/Unity" -batchmode "$@"` y los tests con `-batchmode -nographics -runTests -testPlatform <plataforma> -testResults <fichero>` — código de [game-ci/cli](https://github.com/game-ci/cli) (`src/command/build-image/build-image-command.ts`, `dist/platforms/ubuntu/steps/test.sh`, commit 2026-09-24) **[A]**. Base de las imágenes: `ubuntu:22.04` ([game-ci/docker images/ubuntu/base/Dockerfile](https://github.com/game-ci/docker/blob/main/images/ubuntu/base/Dockerfile)) **[A]**.
- **Activación por línea de comandos (Unity):** el procedimiento `-batchmode -serial ... -username ... -password ...` **solo aplica a Plus/Pro**, porque las licencias Personal no tienen número de serie; Personal se activa con Unity Hub — [Manage your license through the command line (6000.4)](https://docs.unity3d.com/6000.4/Documentation/Manual/ManagingYourUnityLicense.html) **[B]**.
- **Activación manual (.alf/.ulf) para Personal: eliminada.** Unity muestra "Unity no longer supports manual activation of Personal licenses" — [Unity Discussions](https://discussions.unity.com/t/unity-no-longer-supports-manual-activation-of-personal-licenses/926760) **[B]**, [Manual activation guide](https://docs.unity3d.com/Manual/ManualActivationGuide.html) **[B]**. El código actual de GameCI documenta además que `license.unity3d.com/manual` redirige y responde "Offline activation is available only for Enterprise and Industry seats" — comentario en [game-ci/cli `dist/platforms/ubuntu/steps/activate.sh`](https://github.com/game-ci/cli/blob/main/dist/platforms/ubuntu/steps/activate.sh) (commit 2026-09-24) **[A]** (es el testimonio de GameCI, no una página de Unity).
- **Activación Personal sin GUI: sí es posible hoy vía el Unity Licensing Client con credenciales de cuenta.** GameCI (CLI) implementa un método `personal` que ejecuta el licensing client con `--activate-all --include-personal --username "$UNITY_EMAIL" --password "$UNITY_PASSWORD"` (detectando si el cliente soporta `--include-personal`), y devuelve el asiento al terminar (trap `EXIT`), porque "a leaked Personal seat breaks every subsequent run on the account" — [game-ci/cli activate.sh / resolve_unity_path.sh / licensing_method.sh](https://github.com/game-ci/cli/tree/main/dist/platforms/ubuntu/steps) (2026-09-24) **[A]**. **Esto es comportamiento implementado por un tercero (GameCI) sobre una herramienta de Unity; no he encontrado (ni podido leer) documentación oficial de Unity que describa `--include-personal`.** → [NV] en fuente oficial.
- El orden de resolución del método de licencia en GameCI es: `file` (UNITY_LICENSE/UNITY_LICENSE_FILE) → `serial` (UNITY_SERIAL+EMAIL+PASSWORD) → `floating` (UNITY_LICENSING_SERVER) → `personal` (solo EMAIL+PASSWORD) — `licensing_method.sh` **[A]**.
- Un `.ulf` está ligado criptográficamente a la máquina que lo solicitó; en contenedores efímeros falla con "Machine bindings don't match" (observado por GameCI en 6000.6.0f1), y GameCI hace fallback al método `personal` — `activate.sh` **[A]**.

### 1.4 Verificado / No verificado

| Afirmación | Nivel |
|---|---|
| Versiones y últimos parches 6000.0/.3/.4/.5/.6/.7 | **[A]** |
| 6.3 es LTS con soporte hasta dic-2027 | **[B]** |
| 6.7 será LTS, Q4 2026 | **[B]** (anuncio); existencia de 6000.7.0b1 **[A]** |
| 6.0 LTS soporte hasta oct-2026 | **[B]** |
| Personal gratis < 200.000 USD; Runtime Fee cancelada | **[B]** |
| +5 % Pro desde 12-ene-2026 | **[B]**; cifra 2.310 USD **[C]** |
| Linux Editor: Ubuntu 22.04 y 24.04 | **[B]** |
| Personal sin activación manual (.ulf) | **[B]** + testimonio GameCI **[A]** |
| Activación Personal headless vía licensing client `--include-personal` | Implementación GameCI **[A]**; soporte/documentación oficial de Unity **[NV]** |
| Roadmap CoreCLR (6.8 vs Unity 7) | **Contradictorio / [NV]** |

### 1.5 Implicaciones para el proyecto

1. **La hipótesis "Unity 6.3 LTS" se confirma como la LTS vigente** a 2026-09-24. Último parche: **6000.3.24f1**. No existe todavía una LTS más nueva (6.7 está en beta 1).
2. Hay una decisión temporal real: **6.7 LTS llegará en Q4 2026** (según anuncio). Recomendación: arrancar en **6000.3.24f1** (o el último 6000.3.x en el momento de crear el proyecto) y **planificar una evaluación de migración a 6.7 LTS** cuando salga su `f1` y tenga 2–3 parches, sin bloquear el arranque. 6.6 (Supported Update) no aporta nada crítico para el alcance single-player y tiene ventana de soporte corta.
3. **No usar 6.0 LTS** para un proyecto nuevo (soporte hasta oct-2026 **[B]**).
4. **Licencia:** Personal es suficiente mientras el estudio esté por debajo de 200.000 USD de ingresos/financiación en 12 meses **[B]**. Pro solo si se supera el umbral.
5. **Linux:** si el equipo o la CI usan Linux, fijar **Ubuntu 22.04 o 24.04** (únicas soportadas **[B]**). Las imágenes GameCI son Ubuntu 22.04 **[A]**.
6. **Riesgo de licencia en CI:** la activación Personal headless depende de un mecanismo que no está documentado oficialmente (según lo que pude comprobar) y que consume un asiento que hay que devolver. Mitigación: ver §6.

---

## 2. Unreal Engine 5 (última versión a 2026)

### 2.1 Hallazgos

- **Unreal Engine 5.8** publicado el **2026-06-17**; disponible en Epic Games Launcher, GitHub y página de Linux; incluye Megalights, Mesh Terrain experimental, "Lumen-Lite" e **integraciones de IA basadas en MCP** — [80.lv](https://80.lv/articles/unreal-engine-5-8-is-out-today-with-big-optimization-improvements-and-mesh-terrain) **[C]**, [Unreal Engine 5.8 is now available (Epic)](https://www.unrealengine.com/news/unreal-engine-5-8-is-now-available) **[B]**, [Epic forums: UE 5.8 Released](https://forums.unrealengine.com/t/unreal-engine-5-8-released/2729274) **[B]**.
- Hotfixes publicados: **5.8.1** y **5.8.2** — [5.8.1 Hotfix](https://forums.unrealengine.com/t/5-8-1-hotfix-released/2738864), [5.8.2 Hotfix](https://forums.unrealengine.com/t/5-8-2-hotfix-released/2746335) **[B]** (solo títulos; fechas no visibles). No verifiqué si existe 5.8.3.
- UE 5.8 sería "la última release mayor planificada de UE5" mientras Epic trabaja en UE6 — resumen del buscador sobre [80.lv](https://80.lv/articles/unreal-engine-5-8-is-out-today-with-big-optimization-improvements-and-mesh-terrain) **[C]**. **No verificado en fuente de Epic.**
- **Licencia (juegos):** gratis; **5 % de royalty sobre ingresos brutos de por vida por producto por encima de 1 millón USD** — [Unreal Engine licensing options](https://www.unrealengine.com/license) **[B]**. Programa **"Launch Everywhere with Epic"**: royalty reducido al **3,5 %** para juegos de Windows/macOS/Android lanzados en la Epic Games Store antes o a la vez que en otras tiendas, efectivo desde 2025-01-01 — [GameFromScratch](https://gamefromscratch.com/unreal-engine-launch-everywhere-with-epic/) **[C]**, [PocketGamer.biz](https://www.pocketgamer.biz/unreal-engine-royalty-fee-reducing-to-35-for-games-landing-on-epic-games-store-on-launch-day/) **[C]**, [CG Channel, 2024-10](https://www.cgchannel.com/2024/10/epic-games-to-cut-royalty-rate-on-unreal-engine-games/) **[C]**. Un resumen menciona "0 % para exclusivas EGS" — **no verificado, no usar**.
- **Licencia (no-juegos):** 1.850 USD/asiento/año solo para empresas >1 M USD/año que no hacen juegos — [Engadget](https://www.engadget.com/epic-will-charge-non-game-developers-1850-per-seat-to-use-unreal-engine-162015997.html) **[C]**, [unrealengine.com/license](https://www.unrealengine.com/license) **[B]**. No aplica a este proyecto.
- **Paper2D:** sigue presente y documentado en la documentación de **UE 5.8** ("Paper 2D is a sprite-based system for creating 2D and 2D/3D hybrid games") — [Paper 2D Overview, UE 5.8 docs](https://dev.epicgames.com/documentation/en-us/unreal-engine/paper-2d-overview-in-unreal-engine) **[B]**. **No encontré ninguna declaración oficial sobre su estado de mantenimiento** (ni deprecado ni en desarrollo activo). Existe un componente `ue_gameplay_paper2d` en el bug tracker — [issues.unrealengine.com](https://issues.unrealengine.com/issue/search?component=ue_gameplay_paper2d) **[B]**.
- **PaperZD** (plugin de Critical Failure Studio para animación 2D en UE): versión **2.2.0 "Production"**, soporta **UE 5.7**; **no hay evidencia de soporte para 5.8** en lo que pude consultar — [Unreal Directive: PaperZD](https://unrealdirective.com/resources/engine-plugins/paperzd/) **[C]**, [PaperZD docs](https://www.criticalfailure-studio.com/paperzd-documentation/) **[B]**.
- **Linux editor:** Epic distribuye UE para Linux (mención a "their Linux page" en el anuncio de 5.8) **[C]**. Distribuciones soportadas y requisitos **[NV]**.
- **Tiempos de compilación C++ / iteración / Blueprint vs C++:** no encontré (ni pude consultar) datos oficiales cuantitativos. **[NV]** — cualquier afirmación sobre tiempos de build sería opinión.

### 2.2 Verificado / No verificado

| Afirmación | Nivel |
|---|---|
| UE 5.8 (2026-06-17), hotfixes 5.8.1/5.8.2 | **[B]/[C]** |
| 5 % > 1 M USD lifetime; 3,5 % con Launch Everywhere | **[B]/[C]** |
| Paper2D presente en docs 5.8 | **[B]** |
| Paper2D mantenido activamente | **[NV] — sin evidencia en ninguna dirección** |
| PaperZD soporta 5.7; soporte 5.8 | 5.7 **[C]**; 5.8 **[NV]** |
| UE 5.8 incluye integraciones MCP | **[C]** |
| Linux editor, build times, BP vs C++ | **[NV]** |

### 2.3 Implicaciones para el proyecto

- Para personajes 2D pixel-art en 3D, UE depende de Paper2D (estado de mantenimiento desconocido) y de un plugin de terceros (PaperZD) cuyo soporte para la última versión no está confirmado. Es un **riesgo de dependencia** precisamente en el elemento visual diferencial del juego.
- La royalty del 5 % solo se paga a partir de 1 M USD por producto; para un estudio pequeño el coste de licencia **no es un factor decisivo** frente a Unity Personal (ambos 0 € al inicio).

---

## 3. Comparación Unity vs Unreal (Godot 4.x como referencia)

### 3.1 Hallazgos que sustentan la comparación

- **Unity — sprites en 3D (verificado en código fuente de URP 17.3, rama `6000.3/staging` de [Unity-Technologies/Graphics](https://github.com/Unity-Technologies/Graphics/tree/6000.3/staging), commit 2026-07-01) [A]:** ver §4 para el detalle. Resumen: los shaders de sprite por defecto no reciben luz 3D ni proyectan sombras con el Universal Renderer; hay que usar un Shader Graph Lit (3D) o shader propio. Es trabajo acotado y conocido.
- **Unity — MCP/Claude Code:** existe **MCP for Unity** (CoplayDev/unity-mcp, licencia MIT), versión **v10.0.0 (2026-06-30)**, requisitos "Unity 2021.3 LTS → 6.x", compatible con "Claude Desktop & Code", 47 herramientas MCP (escenas, GameObjects, scripts, assets, **ejecución de tests**, builds); último commit 2026-09-20 — [README de CoplayDev/unity-mcp](https://github.com/CoplayDev/unity-mcp) **[A]**. Existe también [IvanMurzak/Unity-MCP](https://github.com/IvanMurzak/Unity-MCP) (repo verificado existente **[A]**, contenido no revisado). Integración MCP oficial de Unity: **[NV]**.
- **Unreal — MCP:** UE 5.8 anuncia "integraciones de IA basadas en MCP" **[C]**.
- **Unity — IA de gameplay:** paquete Unity Behavior (behaviour graphs) — estado y versión **[NV]** (repo no público, docs bloqueadas).
- **Unity — física:** PhysX integrado para GameObjects; en 6.3 el Physics Settings permite elegir la "physics SDK integration" para la API de GameObjects y para Entities por separado (texto del inspector en `Modules/PhysicsEditor/PhysicsManagerInspector.cs`, tag 6000.3.24f1) **[A]**. Modos CCD: `Discrete`, `Continuous`, `ContinuousDynamic`, `ContinuousSpeculative` (`Modules/Physics/Managed/CollisionDetectionMode.cs`, 6000.3.24f1) **[A]**.
- **Godot:** última estable **4.7.2-stable** (commit del tag 2026-08-16) — [godotengine/godot tags](https://github.com/godotengine/godot/tags) **[A]**. Godot es MIT, sin royalties **[NV en esta sesión; conocimiento previo]**. Existe "Godot AI" (proyecto MCP de los autores de MCP for Unity) — mención en el README de unity-mcp **[A]** (contenido no revisado).

### 3.2 Matriz comparativa puntuada

Escala 1–5 (5 = mejor para **este** proyecto). Pesos según prioridades del proyecto (equipo pequeño, 2.5D pixel-art, física de pelota, single-player+IA, uso intensivo de Claude Code). **Las puntuaciones son juicio técnico basado en la evidencia anterior; no son medidas.** Donde la evidencia es [NV] se indica.

| Criterio | Peso | Unity 6.3 LTS | Unreal 5.8 | Godot 4.7 (ref.) | Evidencia / comentario |
|---|---:|---:|---:|---:|---|
| 2.5D / sprites en 3D | 3 | 4 | 3 | 4 | Unity: SpriteRenderer + Shader Graph Lit, limitaciones verificadas [A]. UE: Paper2D sin estado de mantenimiento claro [B/NV]. Godot: Sprite3D nativo [NV] |
| Pixel art (nitidez, filtrado, animación) | 3 | 4 | 3 | 4 | Unity: Pixel Perfect Camera no aplica a cámara 3D perspectiva [A]; hay que resolver con point filter / RT de baja resolución (igual en los tres motores) |
| Shaders (Shader Graph vs Material Editor) | 2 | 4 | 5 | 3 | UE Material Editor más potente [NV, conocimiento general]; Shader Graph suficiente y con targets Sprite verificados [A] |
| Física de pelota (PhysX / Chaos / Jolt) | 3 | 4 | 4 | 3 | Unity: CCD modes verificados [A]; en todos los casos el vuelo de la pelota se programará de forma determinista propia (ver §4) |
| Cámaras | 2 | 5 | 4 | 3 | Cinemachine 3.1.7 [A]; UE Spring Arm/Camera [NV] |
| Herramientas IA (Behavior, BT) | 2 | 3 | 5 | 2 | UE Behavior Trees/StateTree maduros [NV]; Unity Behavior [NV] |
| MCP / Claude Code | 3 | 5 | 3 | 3 | MCP for Unity v10, MIT, soporta Claude Code y tests [A]; UE 5.8 MCP [C] |
| Flujo de assets (texto/merge/Git) | 3 | 4 | 2 | 5 | Unity: Force Text + Smart Merge [A]; UE: `.uasset` binarios [NV]; Godot: `.tscn` texto [NV] |
| Rendimiento (para este alcance) | 1 | 4 | 5 | 4 | Alcance modesto; ninguno es cuello de botella [NV] |
| Dificultad / curva de aprendizaje (C#) | 3 | 4 | 2 | 4 | C# + tests en batchmode [A] vs C++/Blueprints [NV] |
| Ecosistema / Asset Store / documentación | 2 | 5 | 4 | 3 | [NV] |
| Coste / licencia | 2 | 4 | 4 | 5 | Unity Personal 0 € < 200 k USD [B]; UE 5 % > 1 M USD [B]; Godot MIT [NV] |
| Soporte / estabilidad LTS | 2 | 5 | 3 | 3 | 6.3 LTS hasta dic-2027 [B]; UE sin LTS formal [NV]; UE5 → UE6 en camino [C] |
| Mantenibilidad (tests, asmdef, CI) | 3 | 5 | 3 | 3 | Test Framework + GameCI en Linux [A] |
| Velocidad de iteración | 3 | 4 | 2 | 5 | Fast Enter Play Mode (default en 6.6 [B]); C++ UE requiere compilación [NV] |
| **Total ponderado** (máx. 185) | 37 | **158** | **121** | **135** | Σ(peso × puntuación) |

Cálculo: Unity = 12+12+8+12+10+6+15+12+4+12+10+8+10+15+12 = 158; Unreal = 9+9+10+12+8+10+9+6+5+6+8+8+6+9+6 = 121; Godot = 12+12+6+9+6+4+9+15+4+12+6+10+6+9+15 = 135.

### 3.3 Recomendación

**Motor recomendado: Unity 6.3 LTS (6000.3.24f1 o último parche 6000.3.x), con Universal Render Pipeline (Universal Renderer, no 2D Renderer).**

Razonamiento explícito:

1. **Es la LTS vigente verificada** (tag 6000.3.24f1 de 2026-09-10 [A]; soporte hasta dic-2027 [B]). Da ~15 meses de soporte garantizado y un camino claro a 6.7 LTS.
2. **El requisito visual clave (sprites pixel-art en mundo 3D con luz y sombras) es resoluble en Unity con componentes estándar**, y sus limitaciones están **verificadas en código fuente** (§4), por lo que el riesgo es conocido y acotado. En Unreal depende de Paper2D (estado de mantenimiento no verificado) + PaperZD (soporte 5.8 no verificado).
3. **Integración con Claude Code:** MCP for Unity (MIT, v10.0.0, 2026-06-30) soporta Claude Code y la ejecución de tests [A]; además el flujo C# + Test Framework en batchmode + GameCI en Linux está verificado a nivel de código [A]. Esto encaja con un estudio pequeño que automatiza con agentes.
4. **Serialización en texto + Smart Merge** (opciones verificadas en el código del editor [A]) favorece Git y revisiones por agentes; Unreal usa binarios `.uasset` [NV].
5. **Coste:** 0 € con Unity Personal hasta 200.000 USD [B].

**Cuándo reconsiderar:** si el proyecto pivotara a gráficos 3D realistas de alto nivel (UE ganaría) o si la licencia/runtime de Unity volviera a cambiar desfavorablemente (Godot 4.7 es la alternativa de control más cercana en flujo 2.5D y texto).

---

## 4. Paquetes de Unity y compatibilidad con Unity 6.3 LTS

### 4.1 Hallazgos — URP (Universal Renderer vs 2D Renderer)

- **Versión de URP para Unity 6.3:** `com.unity.render-pipelines.universal` **17.3.0**, `"unity": "6000.3"` en `package.json` de la rama `6000.3/staging` — [Graphics/6000.3/staging](https://github.com/Unity-Technologies/Graphics/blob/6000.3/staging/Packages/com.unity.render-pipelines.universal/package.json) **[A]**. Changelog: 17.3.0 (2025-08-27) "compatible with Unity 6000.3.0b1"; 17.4.0 ↔ 6000.4; 17.5.0 ↔ 6000.5; `master` = 17.6.0 — [URP CHANGELOG (master)](https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/CHANGELOG.md) **[A]**. URP forma parte del editor (versión fijada por la línea 6000.x).
- **La documentación de URP se ha movido al Manual de Unity** (el `Documentation~/index.md` del paquete dice "The content is moved to the Unity Manual") **[A]**; no se pudo leer el manual (bloqueado).
- **2D Renderer con mallas 3D:** el shader **URP/Lit** tiene un pase `LightMode = "Universal2D"` cuyo fragment (`Shaders/Utils/Universal2D.hlsl`) devuelve solo `_BaseMap * _BaseColor` con alpha clip, **sin iluminación** (ni luces 3D ni 2D) **[A]**. Existe el shader `Universal Render Pipeline/2D/Mesh2D-Lit-Default` para mallas iluminadas por **luces 2D** **[A]**. Conclusión (inferida del código): con el 2D Renderer, las mallas 3D **no reciben iluminación 3D ni sombras 3D en tiempo real**; solo luces 2D con shaders específicos. Esto **descarta el 2D Renderer** para una pista 3D iluminada.
- **Render Graph en URP 17.3:** el "Compatibility Mode" (Render Graph desactivado) está **deprecado**; el aviso del código dice "Compatibility Mode is deprecated. Migrate your ScriptableRenderPasses to the Render Graph API … remove the URP_COMPATIBILITY_MODE define" (`Runtime/Data/UniversalRenderPipelineAsset.cs`, 6000.3/staging) **[A]**. En `master` (17.6) esas cadenas ya no aparecen en el fichero — indicio de eliminación **(inferencia, no confirmado)**.

### 4.2 Hallazgos — Pixel Perfect Camera

- El componente URP `PixelPerfectCamera` muestra en el inspector: **"URP Pixel Perfect Camera requires a camera using a 2D Renderer. Some features, such as Upscale Render Texture, are not supported with other Renderers."** (warning si se usa otro renderer) y error si no hay SRP — `Editor/2D/PixelPerfectCameraEditor.cs`, 6000.3/staging **[A]**.
- Otros avisos del mismo editor: no funciona bien con **camera stacking**; "Crop Frame" y "Upscale Render Texture" no se soportan con Render Scale ≠ 1.0 **[A]**.
- En tiempo de ejecución, el componente **sobrescribe `camera.orthographicSize`** en cada frame (salvo en modo compatibilidad Cinemachine) y ajusta `worldToCameraMatrix` para snap de píxel — `Runtime/2D/PixelPerfectCamera.cs` **[A]**. Es decir, está diseñado para **cámaras ortográficas**.
- Existe extensión `CinemachineUniversalPixelPerfect` (`Runtime/2D/CinemachineUniversalPixelPerfect.cs`) **[A]**.
- **Conclusión:** Pixel Perfect Camera **no es aplicable** a una cámara 3D en perspectiva en tercera persona con Universal Renderer (inferencia directa del código). El "look" pixel-art deberá conseguirse con: texturas en *point filtering* sin compresión, PPU coherente, y opcionalmente renderizar a un RenderTexture de baja resolución y escalar (técnica estándar, **no un componente de Unity verificado aquí**).

### 4.3 Hallazgos — Shader Graph Sprite targets y sombras de SpriteRenderer

- **Shader Graph "Sprite Lit" (URP 17.3)** genera tres pases: `Universal2D` ("Sprite Lit"), `NormalsRendering` ("Sprite Normal") y `UniversalForward` ("Sprite Forward") — `Editor/2D/ShaderGraph/Targets/UniversalSpriteLitSubTarget.cs` **[A]**. El pase `UniversalForward` (el que usa el **Universal Renderer**) ejecuta `SpriteForwardPass.hlsl`, que devuelve el color base × color de vértice **sin cálculo de iluminación ni sombras** **[A]**. También existen subtargets "Sprite Unlit" y "Sprite Custom Lit" **[A]**.
- **Shader `Sprite-Lit-Default`** (el material por defecto de sprites en URP): pases `Universal2D`, `NormalsRendering` y `UniversalForward`; el pase `UniversalForward` usa `UnlitVertex/UnlitFragment` → **en el Universal Renderer se ve sin iluminar**, y **no tiene pase `ShadowCaster`** → **no proyecta sombras** — `Shaders/2D/Sprite-Lit-Default.shader` **[A]**.
- **SpriteRenderer y sombras:** el inspector de `SpriteRenderer` **no expone** ninguna opción de sombras en 6000.3.24f1, 6000.6.2f1 ni 6000.7.0b1 (`Editor/Mono/Inspector/SpriteRendererEditor.cs`, 0 coincidencias de "shadow") **[A]**. La API base `Renderer` sí tiene `shadowCastingMode` y `receiveShadows` (`Runtime/Export/Graphics/GraphicsRenderers.bindings.cs`, 6000.3.24f1) **[A]**, así que pueden fijarse por script; que la sombra se dibuje depende de que el material tenga pase `ShadowCaster`.
- **Conclusión e implicación:** para personajes pixel-art que **proyecten y reciban sombras** en la pista 3D con Universal Renderer, hay que usar un **material con Shader Graph target Lit/Unlit 3D de URP (con Alpha Clipping)** o un shader HLSL propio con pases `UniversalForward` + `ShadowCaster` (+ `DepthOnly`), y activar `shadowCastingMode`/`receiveShadows` por script. **Que un Shader Graph Lit 3D funcione correctamente sobre `SpriteRenderer` (color de vértice, flip, normales, sorting) no está verificado** → requiere un *spike* técnico en la semana 1. Alternativa de menor riesgo: `MeshRenderer` en quad + material Lit con alpha clip + animación por flipbook/atlas propia; o sombra falsa (blob/decal) bajo el jugador.

### 4.4 Hallazgos — Cinemachine, Input System, física, tests

- **Cinemachine:** última estable **3.1.7 (2026-06-08)**, pre-release **3.1.8-pre.2 (2026-08-04)**; `package.json` en `main`: `"unity": "2022.3"` (mínimo) — [com.unity.cinemachine CHANGELOG](https://github.com/Unity-Technologies/com.unity.cinemachine/blob/main/com.unity.cinemachine/CHANGELOG.md) **[A]**. 3.0.0 es de 2023-10-25; el componente principal es `CinemachineCamera` (referenciado en el changelog) **[A]**. 3.1.7 añade soporte de Fast Enter Play Mode **[A]**. Compatibilidad explícita con 6.3: el mínimo 2022.3 la incluye; pruebas oficiales en 6.3 **[NV]**.
- **Input System:** última **1.20.0 (2026-07-21)**; `develop` ya en 1.20.1; `package.json` `"unity": "6000.0"` — [InputSystem CHANGELOG](https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/CHANGELOG.md) **[A]**. El changelog corrige avisos "on Unity 6.3 (beta)" (ISXB-1718) y en 1.19 elimina el define que permitía compilar fuera las Project-Wide Actions **[A]**. **Que sea el sistema de input por defecto en proyectos nuevos de Unity 6: [NV]** (docs bloqueadas).
- **Física (PhysX):** Unity 6.3 usa una "physics SDK integration" seleccionable para GameObjects (PhysX por defecto según conocimiento previo) **[A para el selector; NV para la versión exacta de PhysX]**. **No pude verificar la versión de PhysX** (4.1 u otra). Modos CCD verificados [A] (§3.1). Fixed timestep por defecto 0,02 s **[NV]** (valor conocido, no re-verificado). Unity 6.5 añade un "Direct Solver" a **Unity Physics** (el paquete DOTS, no PhysX) — [CG Channel](https://www.cgchannel.com/2026/06/unity-releases-unity-6-5-discover-5-key-features-for-cg-artists/) **[C]**.
- **Test Framework:** un proyecto oficial de Unity en **6000.3.9f1** usa `com.unity.test-framework` **1.6.0**; uno en **6000.5.1f1** usa **1.7.0** — `NetcodeSamples/Packages/manifest.json` en [EntityComponentSystemSamples (master y rama NetcodeSamples/6000.5)](https://github.com/Unity-Technologies/EntityComponentSystemSamples) **[A]**. Edit Mode y Play Mode en batchmode en Linux: GameCI los ejecuta con `-runTests -testPlatform editmode|playmode -testResults ...` en contenedores Ubuntu **[A]** (§6). GameCI advierte que la instrumentación de code coverage ha causado "a PlayMode SIGSEGV on Unity 6" y permite desactivarla (`coverageEnabled: false`) — [docs test-runner](https://github.com/game-ci/documentation/blob/main/docs/03-github/03-test-runner.mdx) (2026-08-14) **[A]**.
- **Assembly Definitions (`.asmdef`):** usadas por los propios paquetes de URP 17.3 (p. ej. `Unity.RenderPipelines.Universal.2D.Runtime.asmdef`) **[A]**; documentación oficial no consultada **[NV]**.

### 4.5 Hallazgos — Netcode y servicios multijugador (futuro)

- **Netcode for GameObjects (NGO):** rama 2.x última **2.13.3 (2026-09-14)**, `develop-2.0.0` en 2.13.4, `"unity": "6000.0"` **[A]**; **NGO 3.0.0 (2026-09-14)** en `develop-3.x.x` con `"unity": "6000.7"` → **NGO 3.x requiere Unity 6.7** **[A]**; rama 1.x última 1.15.1 (2026-01-21) **[A]** — [com.unity.netcode.gameobjects](https://github.com/Unity-Technologies/com.unity.netcode.gameobjects).
- **Netcode for Entities:** `com.unity.netcode` **1.12.0** con Entities 1.4.4 en proyecto 6000.3.9f1; en 6000.5.1f1 aparece como **6.5.0** (Entities 6.5.0), es decir, se alinea con la versión del editor a partir de 6.5 **[A]** (mismos manifests que arriba).
- **Multiplayer Services:** `com.unity.services.multiplayer` 2.1.1 (6.3) / 2.1.3 (6.5) **[A]**. **Precios de Relay/Lobby/Multiplayer Services 2026: [NV]** (unity.com bloqueado; no invento cifras).
- **Unity Behavior:** versión, estado y compatibilidad con 6.3 **[NV]**.

### 4.6 Verificado / No verificado (resumen)

| Paquete | Versión para 6.3 | Nivel |
|---|---|---|
| URP / Shader Graph | 17.3.x (core, ligado al editor) | [A] |
| Render Graph | obligatorio de facto; Compatibility Mode deprecado | [A] |
| Pixel Perfect Camera | requiere 2D Renderer; orientado a ortográfica | [A] |
| Sprite-Lit-Default / Sprite Lit SG en Universal Renderer | sin luz 3D, sin ShadowCaster | [A] |
| SpriteRenderer sombras | sin UI; API `Renderer` disponible | [A] |
| Cinemachine | 3.1.7 (mín. 2022.3) | [A] |
| Input System | 1.20.0 (mín. 6000.0) | [A]; "por defecto" [NV] |
| PhysX versión / fixed timestep | — | [NV] |
| CCD modes | Discrete/Continuous/ContinuousDynamic/ContinuousSpeculative | [A] |
| Test Framework | 1.6.0 (observado en proyecto 6.3) | [A] |
| NGO | 2.13.3 (mín. 6000.0); 3.0.x requiere 6000.7 | [A] |
| Netcode for Entities | 1.12.0 (en 6.3) | [A] |
| Unity Behavior | — | [NV] |
| Relay/Multiplayer pricing | — | [NV] |

### 4.7 Implicaciones para el proyecto

1. **Renderer:** URP con **Universal Renderer** (Forward o Forward+). **No** usar el 2D Renderer (las mallas 3D quedarían sin luz 3D) [A].
2. **Personajes:** no confiar en `Sprite-Lit-Default` ni en el target Shader Graph "Sprite Lit" para luz/sombras en 3D [A]. Hacer un **spike** (≤ 2 días) comparando: (a) `SpriteRenderer` + Shader Graph Lit/Unlit 3D con alpha clip + `shadowCastingMode` por script; (b) quad `MeshRenderer` + flipbook; (c) sombra blob. Criterios: sombra correcta sobre la pista, orden de dibujado con la red/pelota, coste de animación.
3. **Pixel Perfect Camera: descartada** para la cámara de juego 3D [A]. Definir en el art bible: PPU, *point filter*, sin mipmaps en sprites, y (opcional) render a RT de baja resolución.
4. **Render passes propias** (outline, pixelado) deben escribirse con la **API de Render Graph** [A].
5. **Pelota:** Rigidbody con `ContinuousDynamic` o `ContinuousSpeculative` si se usa física, pero se recomienda simular el vuelo de la pelota con un integrador propio determinista en `FixedUpdate` (gravedad, drag, Magnus) y usar PhysX solo para detección de colisiones con paredes/red — decisión de diseño, no hecho verificado.
6. **Cámara:** Cinemachine 3.1.7 (`CinemachineCamera`).
7. **Input:** Input System 1.20.x.
8. **Tests:** Test Framework (1.6.x en 6.3) con asmdefs separadas por Runtime/Editor/Tests; Edit Mode para lógica pura (reglas de puntuación del pádel, IA), Play Mode para física.
9. **Multijugador online (fase posterior):** NGO 2.13.x funciona en 6.3; **NGO 3.x obliga a 6.7** → decidir la migración a 6.7 LTS antes de empezar el online.

---

## 5. Git para Unity: .gitignore oficial, LFS, Smart Merge, Force Text, Visible Meta Files

### 5.1 Hallazgos

- **`.gitignore` oficial:** [github/gitignore — Unity.gitignore](https://github.com/github/gitignore/blob/main/Unity.gitignore), último cambio **2026-06-08** (commit `3d81db8` "fix: folder formatting") **[A]**. Contenido leído literalmente: ignora `.utmp/`, `/[Ll]ibrary/`, `/[Tt]emp/`, `/[Oo]bj/`, `/[Bb]uild/`, `/[Bb]uilds/`, `/[Ll]ogs/`, `/[Uu]ser[Ss]ettings/`, `*.log`, `*.blend1`, `/[Mm]emoryCaptures/`, `/[Rr]ecordings/`, ficheros de Rider/VS (`.vs/`, `*.csproj`, `*.sln`, `*.slnx`, …), `.gradle/`, `*.apk`, `*.aab`, `*.unitypackage`, `*.app`, `InitTestScene*.unity*`, ficheros de Addressables, Visual Scripting generados, `/UIElementsSchema/`, `/[Aa]ssets/[Ss]ceneDependencyCache*` **[A]**. El `.gitignore` del repo ya declara haberlo copiado literalmente el 2026-09-24 **[A]**.
- **Force Text / Smart Merge (Unity 6.3):** el inspector de Editor Settings ofrece Asset Serialization Mode `Mixed`, `Force Binary`, `Force Text` (`Editor/Mono/Inspector/EditorSettingsInspector.cs`) y el de Version Control ofrece "Smart merge" con opciones `Off`, `Premerge`, `Ask` (`EditorUserSettings.semanticMergeMode`, `Editor/Mono/Inspector/VersionControlSettingsInspector.cs`) — UnityCsReference tag 6000.3.24f1 **[A]**.
- **Visible Meta Files:** opción de modo de control de versiones; valores por defecto en Unity 6 **[NV]** (la lista de modos se construye dinámicamente en el código; no pude leer el manual).
- **UnityYAMLMerge:** herramienta de Smart Merge que se distribuye con el editor y se configura como merge tool de Git — [Unity Manual: Smart merge](https://docs.unity3d.com/Manual/SmartMerge.html) **[NV — página bloqueada, no leída]**. **No transcribo la configuración de `.gitconfig`** porque no pude verificarla literalmente en esta sesión.
- **Git LFS:** el workflow de ejemplo oficial de GameCI usa `actions/checkout@v4` con `lfs: true` — [GameCI getting started](https://github.com/game-ci/documentation/blob/main/docs/03-github/01-getting-started.mdx) (2026-08-14) **[A]**. Guía de Unity sobre LFS **[NV]**.

### 5.2 Verificado / No verificado

- .gitignore oficial y fecha: **[A]**. Opciones Force Text y Smart Merge existen en 6.3: **[A]**. Defaults de Unity 6 (Force Text / Visible Meta Files): **[NV]**. Configuración de UnityYAMLMerge: **[NV]**. Recomendación de LFS por parte de Unity: **[NV]**.

### 5.3 Implicaciones para el proyecto

- Mantener el `.gitignore` oficial verbatim + sección propia.
- Fijar en `ProjectSettings`: Asset Serialization = **Force Text**; Version Control = **Visible Meta Files**; comprobar en el primer commit del proyecto Unity que `ProjectSettings/EditorSettings.asset` refleja ambos (verificación local obligatoria porque los defaults son [NV]).
- Configurar UnityYAMLMerge siguiendo la página oficial *en el momento de hacerlo* (no copiar de memoria).
- Git LFS para binarios pesados (`*.psd`, `*.aseprite` exportados, `*.wav`, `*.fbx`, `*.png` grandes), con `.gitattributes` definido en una ADR.

---

## 6. CI: GameCI (game.ci) para Unity en Linux

### 6.1 Hallazgos

- **Estado del proyecto:** activo. `game-ci/unity-builder` **v6.0.0** (tag de 2026-08-29; último commit 2026-09-16) — cambio "Thin wrapper: invoke game-ci/cli as a subprocess" **[A]**; `game-ci/unity-test-runner` **v4.4.0** (2026-08-29; último commit 2026-09-16) — también "thin wrapper" sobre `game-ci/cli` **[A]**; existe `unity-test-runner v5.0.0-beta.1` **[A]**; `game-ci/docker` tag **v3.2.2** (último commit 2026-05-02) **[A]**; `game-ci/cli` último commit **2026-09-24** **[A]**. Fuentes: `git ls-remote`/`git log` de [unity-builder](https://github.com/game-ci/unity-builder), [unity-test-runner](https://github.com/game-ci/unity-test-runner), [docker](https://github.com/game-ci/docker), [cli](https://github.com/game-ci/cli).
- **Soporte Unity 6.x:** hay imágenes `unityci/editor:ubuntu-6000.3.24f1-base-3.2.2` (y `-linux-il2cpp-`, `-windows-mono-`, `-mac-mono-`, `-webgl-`, `-android-`, `-ios-`) publicadas el 2026-09-10; también para 6000.0.84f1, 6000.4.12f1, 6000.5.11f1, 6000.6.2f1 — [Docker Hub unityci/editor](https://hub.docker.com/r/unityci/editor/tags) **[A]**. Commits recientes de test-runner: "fix: insufficient shared memory available in Unity 6.6 beta" (2026-08-15) **[A]**.
- **Tests:** `testMode` acepta `All`, `PlayMode`, `EditMode`, `Standalone` (`All` = PlayMode + EditMode) — [docs test-runner](https://github.com/game-ci/documentation/blob/main/docs/03-github/03-test-runner.mdx) (2026-08-14) **[A]**. Invocación real en el contenedor: `unity-editor -batchmode -nographics -projectPath … -runTests -testPlatform <plataforma> -testResults <xml>` — `game-ci/cli dist/platforms/ubuntu/steps/test.sh` **[A]**.
- **Activación — lo que dice la documentación de GameCI (2026-08-14):**
  - Personal: activar en Unity Hub para generar `Unity_lic.ulf` y guardar `UNITY_LICENSE` (contenido del `.ulf`), `UNITY_EMAIL`, `UNITY_PASSWORD` como secrets — [activation.mdx](https://github.com/game-ci/documentation/blob/main/docs/03-github/02-activation.mdx) **[A]**.
  - Pro/Plus: `UNITY_SERIAL`, `UNITY_EMAIL`, `UNITY_PASSWORD`; "Return License" para liberar el asiento **[A]**.
  - License server: `unityLicensingServer` (licencia flotante) **[A]**.
  - Guía GitLab: extraer el "serial" de un `.ulf` Personal (campo `DeveloperData` en base64) y usarlo como `UNITY_SERIAL` — [gitlab/activation.mdx](https://github.com/game-ci/documentation/blob/main/docs/05-gitlab/02-activation.mdx) **[A]**.
  - Troubleshooting: "Unity no longer supports manual activation of Personal licenses" con un workaround editando el HTML de license.unity3d.com — [common-issues.mdx](https://github.com/game-ci/documentation/blob/main/docs/09-troubleshooting/common-issues.mdx) **[A]**.
- **Activación — lo que hace el código actual (game-ci/cli, 2026-09-24) [A]:**
  - Si solo se proporcionan `UNITY_EMAIL` + `UNITY_PASSWORD`, usa el método **`personal`** (licensing client `--activate-all --include-personal --username --password`), sin `.ulf` ni serial, y devuelve el asiento al finalizar.
  - Indica que un `.ulf` ya **solo** puede obtenerse en asientos Enterprise/Industry y que un `.ulf` está ligado a la máquina ("Machine bindings don't match" en contenedores efímeros, observado en 6000.6.0f1), con fallback automático a `personal`.
  - Commit 2026-08-16 en test-runner: "fix: restore UNITY_LICENSE / UNITY_LICENSE_FILE activation methods" **[A]**.

### 6.2 Verificado / No verificado

- GameCI activo y con imágenes para 6000.3.24f1: **[A]**.
- Tests Edit/Play Mode en batchmode sobre Linux en CI: **[A]** (a nivel de implementación de GameCI; no ejecutado en esta sesión).
- **Uso de licencia Personal en CI en 2026: técnicamente implementado por GameCI [A]**; **la documentación de GameCI y su código se contradicen** (la doc aún pide `.ulf`; el código dice que el `.ulf` ya no es obtenible para Personal y usa email+contraseña). **Si los Términos de Unity permiten el uso de Personal en CI y si el mecanismo `--include-personal` es oficial: [NV].**
- Cuentas con 2FA/SSO: comportamiento con el método `personal` **[NV]**.

### 6.3 Implicaciones para el proyecto

1. CI recomendada: **GitHub Actions + `game-ci/unity-test-runner@v4` (EditMode+PlayMode) + `game-ci/unity-builder@v6`** con `unityVersion: auto` (lee `ProjectSettings/ProjectVersion.txt`) y caché de `Library/`.
2. Secrets: empezar con `UNITY_EMAIL` + `UNITY_PASSWORD` de una **cuenta Unity dedicada a CI** (sin 2FA interactivo si fuese necesario — a verificar) → método `personal`. Mantener la opción de `UNITY_SERIAL` si se pasa a Pro.
3. **Riesgo:** fallos intermitentes de licencia (GameCI implementa reintentos con backoff 20/40/80/160 s por errores transitorios de Unity [A]) y asientos "colgados". Mitigación: `concurrency` en el workflow (1 job de Unity a la vez por cuenta), y job de *return license* garantizado.
4. Desactivar code coverage en PlayMode si aparece el SIGSEGV documentado en Unity 6 [A].
5. **Primer hito de CI:** un workflow mínimo que active, ejecute 1 test EditMode y devuelva la licencia — antes de escribir gameplay.

---

## 7. Lagunas de evidencia

1. **Bloqueo de red a todos los dominios oficiales de Unity y Epic** (unity.com, docs.unity3d.com, discussions.unity.com, game.ci, dev.epicgames.com, unrealengine.com, godotengine.org). Todo lo marcado [B] se basa en resúmenes del buscador, no en lectura literal.
2. **Presupuesto de búsqueda agotado** (200/200) antes de cubrir: Unity Behavior, precios de Relay/Multiplayer Services, versión de PhysX, fixed timestep por defecto, Input System como default, defaults de Force Text/Visible Meta Files, configuración de UnityYAMLMerge, guía de LFS de Unity, requisitos de Linux de UE, tiempos de compilación de UE, estado de mantenimiento de Paper2D, soporte de PaperZD en 5.8.
3. **Precio exacto de Unity Pro 2026** (2.310 USD) solo en fuentes de terceros.
4. **Roadmap CoreCLR**: contradicción entre "Unity 6.8" y "Unity 7 (beta dic-2026)".
5. **Activación Personal headless**: implementada por GameCI; sin documentación oficial de Unity leída; compatibilidad con 2FA desconocida; política de Unity sobre Personal en CI no verificada.
6. **SpriteRenderer + Shader Graph Lit 3D**: comportamiento real (vertex color, flip, sorting, sombras) no probado — requiere spike.
7. **Fecha exacta de salida de Unity 6.7 LTS**: solo "Q4 2026" [B]; existe 6000.7.0b1 (2026-09-17) [A].
8. Fechas de las páginas oficiales citadas vía buscador: en su mayoría **no visibles**.
9. No se ejecutó Unity en esta sesión (no hay editor instalado); nada se ha probado empíricamente.

---

## 8. Resumen de decisiones propuestas (para ADR-001)

| Tema | Propuesta | Confianza |
|---|---|---|
| Motor | Unity 6.3 LTS, parche 6000.3.24f1 (o último 6000.3.x) | Alta [A/B] |
| Actualización | Evaluar 6.7 LTS tras su f1 + 2–3 parches (Q4 2026 / Q1 2027) | Media [B] |
| Pipeline | URP 17.3, Universal Renderer, Render Graph | Alta [A] |
| Personajes | Spike: SpriteRenderer+SG Lit 3D vs quad+flipbook vs blob shadow | Media (requiere spike) |
| Pixel art | Sin Pixel Perfect Camera; point filter + PPU fijo (+ RT low-res opcional) | Alta [A] |
| Cámara | Cinemachine 3.1.7 | Alta [A] |
| Input | Input System 1.20.x | Alta [A] |
| Física pelota | Integrador propio en FixedUpdate + colisiones PhysX con CCD | Media (diseño) |
| Tests | Test Framework 1.6.x, asmdefs, EditMode+PlayMode | Alta [A] |
| CI | GameCI unity-test-runner v4 / unity-builder v6, método `personal` | Media [A + NV política] |
| IA agentes | MCP for Unity (CoplayDev) v10.x | Media-alta [A] |
| Online (futuro) | NGO 2.13.x en 6.3, o NGO 3.x tras migrar a 6.7 | Alta [A] |

---

## 9. Lista completa de fuentes

### Fuentes primarias leídas directamente [A]
- UnityCsReference (tags y código 6000.3.24f1 / 6000.6.2f1 / 6000.7.0b1): https://github.com/Unity-Technologies/UnityCsReference
  - `Editor/Mono/Inspector/SpriteRendererEditor.cs`, `Runtime/Export/Graphics/GraphicsRenderers.bindings.cs`, `Modules/Physics/Managed/CollisionDetectionMode.cs`, `Modules/PhysicsEditor/PhysicsManagerInspector.cs`, `Editor/Mono/Inspector/EditorSettingsInspector.cs`, `Editor/Mono/Inspector/VersionControlSettingsInspector.cs`
- Unity Graphics (URP 17.3, rama 6000.3/staging, commit 2026-07-01): https://github.com/Unity-Technologies/Graphics/tree/6000.3/staging
  - `Packages/com.unity.render-pipelines.universal/package.json`, `CHANGELOG.md` (master), `Shaders/2D/Sprite-Lit-Default.shader`, `Shaders/Lit.shader`, `Shaders/Utils/Universal2D.hlsl`, `Shaders/2D/Mesh2D-Lit-Default.shader`, `Runtime/2D/PixelPerfectCamera.cs`, `Editor/2D/PixelPerfectCameraEditor.cs`, `Editor/2D/ShaderGraph/Targets/UniversalSpriteLitSubTarget.cs`, `Editor/2D/ShaderGraph/Includes/SpriteForwardPass.hlsl`, `Runtime/Data/UniversalRenderPipelineAsset.cs`
- Cinemachine CHANGELOG / package.json: https://github.com/Unity-Technologies/com.unity.cinemachine/blob/main/com.unity.cinemachine/CHANGELOG.md
- Input System CHANGELOG / package.json: https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/CHANGELOG.md
- Netcode for GameObjects (ramas develop, develop-2.0.0, develop-3.x.x): https://github.com/Unity-Technologies/com.unity.netcode.gameobjects
- EntityComponentSystemSamples (manifests NetcodeSamples, master y NetcodeSamples/6000.5): https://github.com/Unity-Technologies/EntityComponentSystemSamples
- github/gitignore Unity.gitignore (último cambio 2026-06-08): https://github.com/github/gitignore/blob/main/Unity.gitignore
- GameCI documentation (commits 2026-08-14): https://github.com/game-ci/documentation — `docs/03-github/01-getting-started.mdx`, `02-activation.mdx`, `03-test-runner.mdx`, `docs/05-gitlab/02-activation.mdx`, `docs/09-troubleshooting/common-issues.mdx`
- GameCI CLI (commit 2026-09-24): https://github.com/game-ci/cli — `dist/platforms/ubuntu/steps/activate.sh`, `licensing_method.sh`, `resolve_unity_path.sh`, `test.sh`, `src/command/build-image/build-image-command.ts`
- GameCI unity-builder (v6.0.0, 2026-08-29): https://github.com/game-ci/unity-builder
- GameCI unity-test-runner (v4.4.0, 2026-08-29): https://github.com/game-ci/unity-test-runner
- GameCI docker (v3.2.2; base ubuntu:22.04): https://github.com/game-ci/docker
- Docker Hub unityci/editor (API consultada 2026-09-24): https://hub.docker.com/r/unityci/editor/tags
- MCP for Unity (v10.0.0, 2026-06-30; commit 2026-09-20): https://github.com/CoplayDev/unity-mcp
- Unity-MCP (IvanMurzak), existencia del repo: https://github.com/IvanMurzak/Unity-MCP
- Godot tags (4.7.2-stable, 2026-08-16): https://github.com/godotengine/godot/tags

### Fuentes oficiales vistas solo vía buscador [B]
- Unity 6 Releases & Support: https://unity.com/releases/unity-6/support
- Unity 6.3 LTS is now available (blog): https://unity.com/blog/unity-6-3-lts-is-now-available
- Unity 6.3 LTS is now available (Discussions): https://discussions.unity.com/t/unity-6-3-lts-is-now-available/1697328
- Unity 6.4 is now available (Discussions): https://discussions.unity.com/t/unity-6-4-is-now-available/1713245
- Unity 6.5 is now available (Discussions): https://discussions.unity.com/t/unity-6-5-is-now-available/1723176
- Unity 6.6 is now available (Discussions): https://discussions.unity.com/t/unity-6-6-is-now-available/1735357
- Path to CoreCLR, 2026: https://discussions.unity.com/t/path-to-coreclr-2026-upgrade-guide/1714279
- Unity is Canceling the Runtime Fee: https://unity.com/blog/unity-is-canceling-the-runtime-fee
- Terms update – Runtime Fee cancellation: https://unity.com/blog/terms-update-runtime-fee-cancellation
- Unity Pricing Changes: https://unity.com/products/pricing-updates
- Unity Plans & Pricing: https://unity.com/products
- Unity DevOps charges from 2026-03-01: https://support.unity.com/hc/en-us/articles/34748492914964-Understanding-New-Unity-DevOps-charges-starting-from-Mar-1-2026
- Unity 6000.0.23f1 release notes: https://unity.com/releases/editor/whats-new/6000.0.23f1
- System requirements for Unity 6.3: https://docs.unity3d.com/6000.3/Documentation/Manual/system-requirements.html
- Manage your license through the command line (6000.4): https://docs.unity3d.com/6000.4/Documentation/Manual/ManagingYourUnityLicense.html
- Manual activation guide: https://docs.unity3d.com/Manual/ManualActivationGuide.html
- Unity no longer supports manual activation of Personal licenses: https://discussions.unity.com/t/unity-no-longer-supports-manual-activation-of-personal-licenses/926760
- Unity Smart Merge (no leída): https://docs.unity3d.com/Manual/SmartMerge.html
- Unreal Engine 5.8 is now available: https://www.unrealengine.com/news/unreal-engine-5-8-is-now-available
- UE 5.8 Released (forums): https://forums.unrealengine.com/t/unreal-engine-5-8-released/2729274
- 5.8.1 Hotfix: https://forums.unrealengine.com/t/5-8-1-hotfix-released/2738864
- 5.8.2 Hotfix: https://forums.unrealengine.com/t/5-8-2-hotfix-released/2746335
- Unreal Engine licensing: https://www.unrealengine.com/license
- Paper 2D Overview (UE 5.8 docs): https://dev.epicgames.com/documentation/en-us/unreal-engine/paper-2d-overview-in-unreal-engine
- Paper2D bug tracker component: https://issues.unrealengine.com/issue/search?component=ue_gameplay_paper2d
- PaperZD documentation: https://www.criticalfailure-studio.com/paperzd-documentation/

### Fuentes secundarias [C]
- CG Channel – Unity 6.4 (2026-03): https://www.cgchannel.com/2026/03/unity-releases-unity-6-4-and-unity-studio/
- CG Channel – Unity 6.5 (2026-06): https://www.cgchannel.com/2026/06/unity-releases-unity-6-5-discover-5-key-features-for-cg-artists/
- AlternativeTo – Unity 6.6 (2026-09): https://alternativeto.net/news/2026/9/unity-6-6-adds-webgpu-build-analysis-and-coreclr-prep/
- Inven Global – Unity 7 roadmap: https://www.invenglobal.com/articles/24003/unity-engine-7-changing-the-development-paradigm-and-the-roadmap-ahead
- Vendr – Unity pricing: https://www.vendr.com/marketplace/unity
- Tech Insider – Unity vs Unreal 2026: https://tech-insider.org/unity-vs-unreal-engine-2026/
- 80.lv – UE 5.8: https://80.lv/articles/unreal-engine-5-8-is-out-today-with-big-optimization-improvements-and-mesh-terrain
- GameFromScratch – Launch Everywhere with Epic: https://gamefromscratch.com/unreal-engine-launch-everywhere-with-epic/
- PocketGamer.biz – 3.5 % royalty: https://www.pocketgamer.biz/unreal-engine-royalty-fee-reducing-to-35-for-games-landing-on-epic-games-store-on-launch-day/
- CG Channel – Epic royalty cut (2024-10): https://www.cgchannel.com/2024/10/epic-games-to-cut-royalty-rate-on-unreal-engine-games/
- Engadget – UE seat license: https://www.engadget.com/epic-will-charge-non-game-developers-1850-per-seat-to-use-unreal-engine-162015997.html
- Unreal Directive – PaperZD: https://unrealdirective.com/resources/engine-plugins/paperzd/
