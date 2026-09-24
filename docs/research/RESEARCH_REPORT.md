# RESEARCH REPORT — Fase 0

- **Fecha:** 2026-09-24
- **Autor:** director técnico (Claude Code), a partir de seis investigaciones especializadas en paralelo y verificación directa en fuentes primarias
- **Estado de la Fase 0:** investigación y decisiones **completas**. Setup del proyecto Unity **bloqueado por entorno** (ver §0)

## 0. Resumen ejecutivo

1. **Stack elegido:**
   - Motor: **Unity 6.3 LTS** (`6000.3.24f1` es el último parche verificado) con **URP y Universal Renderer**.
   - Pelota: **simulación propia determinista en C# puro**.
   - Personajes: **quad billboard con shader propio de rampa de paleta**.
   - Render: **RenderTexture de baja resolución con escalado entero**.
   - Cámara: **Cinemachine 3.1.x**.
   - Input: **Input System 1.20.x** tras una capa de comandos.
   - IA: **HFSM + Utility**.
   - Modo MVP: **un jugador contra IA**, sin online.
2. **Hallazgo técnico más importante,** verificado en el código fuente de URP 17.3:
   - con el Universal Renderer, los sprites por defecto **no reciben luz 3D, no proyectan sombras y no escriben profundidad**;
   - Pixel Perfect Camera **solo funciona en ortográfica y con el 2D Renderer**.

   El brief acertaba al pedir no aplicar Pixel Perfect automáticamente: aquí sería un error técnico.
3. **Hallazgo de reglas:** desde 2026 la FIP usa el **Star Point**. Tras dos ventajas perdidas se juega un punto decisivo. Lo aprobó la Asamblea FIP el 28-11-2025 y se aplica en Premier Padel desde febrero de 2026. El punto de oro sigue como modalidad alternativa.
4. **Hallazgo de mercado:** no hay ningún juego de pádel bien valorado en PC/consola a septiembre de 2026. Hay dos competidores indie anunciados para 2026–27. El hueco es **un modo individual profundo con compañero IA creíble y lectura clara de paredes**.
5. **Bloqueo principal:**
   - el contenedor de Claude Code **no puede ejecutar Unity**: no está instalado, los hosts de Unity están bloqueados, no hay GPU y la licencia necesita la cuenta del propietario;
   - no se puede crear el proyecto ni validar el MCP aquí.

   El núcleo en C# puro sí puede desarrollarse y testearse con .NET 8 en el contenedor.
6. **Limitación de la investigación:**
   - la política de red bloqueó casi todas las webs oficiales (unity.com, docs.unity3d.com, padelfip.com, blender.org…);
   - la cuota de búsquedas web (200) se agotó.

   Para compensarlo, las decisiones críticas se verificaron en **código fuente y repositorios oficiales en GitHub** [V]. El resto lleva su nivel de evidencia en cada documento. Las lagunas están en §5.

## 1. Documentos de investigación

| Documento | Contenido |
|---|---|
| [ENVIRONMENT_AUDIT](../production/ENVIRONMENT_AUDIT.md) | Auditoría del entorno (16 puntos del brief) |
| [TECHNOLOGY_RESEARCH](TECHNOLOGY_RESEARCH.md) | Motor (Unity/Unreal/Godot), paquetes, licencias, Linux, CI |
| [MCP_SKILLS_RESEARCH](MCP_SKILLS_RESEARCH.md) | Unity MCP, Unity CLI, skills oficiales y de la comunidad, Blender MCP, seguridad |
| [VISUAL_RESEARCH](VISUAL_RESEARCH.md) | Referencias, técnicas 2.5D en URP, plan de resoluciones, 5 direcciones visuales, 4/8/16 direcciones |
| [GAMEPLAY_RESEARCH](GAMEPLAY_RESEARCH.md) | Mercado, modelos de input, física de pelota, golpes, movimiento, IA, red, testing |
| [PADEL_RULES_RESEARCH](PADEL_RULES_RESEARCH.md) | Reglamento FIP 2026, Star Point, dimensiones, pelota, máquina de puntuación |
| [TOOLS_RESEARCH](TOOLS_RESEARCH.md) | Pixel art, Blender, audio, Git LFS, tipografías, licencias |
| [SOURCE_VERIFICATION](SOURCE_VERIFICATION.md) | Verificación directa en código fuente oficial |

## 2. Hallazgos y decisiones por área

### ENGINE → [ADR-001](../decisions/ADR-001-engine.md)
- Unity 6.3 LTS `6000.3.24f1` [V]; soporte hasta diciembre de 2027 [S]. 6.7 está en beta [V] y se anuncia como próxima LTS en el cuarto trimestre de 2026 [S].
- Unreal 5.8 [S]: el soporte de Paper2D/PaperZD no está confirmado para los personajes 2D.
- Godot 4.7.2 [V] se usa solo como control.
- **Decisión:** Unity 6.3 LTS con URP y Universal Renderer.

### RENDERING → [ADR-002](../decisions/ADR-002-rendering.md)
- Pixel Perfect Camera solo funciona en ortográfica y con el 2D Renderer [V].
- El 2D Renderer no ilumina las mallas 3D [V].
- Render Scale tiene un mínimo de 0.1 [V].
- **Decisión:** RenderTexture fija con filtro point y escalado entero, sin AA en esa pasada, y shaders de rampa propios.
- **Resolución final por prueba:** 480×270 y 640×360, con 320×180 como control.

### CAMERA → [ADR-009](../decisions/ADR-009-camera.md)
- Cinemachine 3.1.7 [V].
- Snap exacto solo en ortográfica [V].
- Cámara tele (FOV 20–30°): cociente de tamaño entre jugadores de 1,45–1,75×, frente a 2,75× de una cámara cercana [I, cálculo].
- **Decisión:** Cinemachine con extensión de snap. Se prueban tres candidatas (broadcast, tele y ortográfica inclinada).

### PHYSICS → [ADR-004](../decisions/ADR-004-physics.md)
- PhysX no modela Magnus ni el acoplamiento spin-fricción. Además tiene un `bounceThreshold` de 2 m/s y la predicción es cara [V/S].
- Tiene que calibrar con el bote FIP: soltada desde 2,54 m debe rebotar 1,35–1,45 m, lo que da e ≈ 0,73–0,76.
- **Decisión:** integrador propio determinista a 1/120 s con arrastre, Magnus y rebote con fricción. El golpe se trata como evento de gameplay con balística inversa.

### INPUT → [ADR-007](../decisions/ADR-007-input.md)
- Input System 1.20.x, compatible con Unity 6000.0 o superior [V].
- **Decisión:**
  - capa `PlayerCommand` / `ICommandSource`;
  - tres familias de golpe (Ataque, Control, Globo) más un modificador de toque, y el contexto decide la técnica;
  - buffer de 120 ms.

### CHARACTERS → [ADR-003](../decisions/ADR-003-character-pipeline.md), [ADR-005](../decisions/ADR-005-animation.md)
- Los sprites por defecto no tienen ShadowCaster y usan la cola Transparent [V].
- El inspector de SpriteRenderer no tiene opciones de sombra [V].
- **Decisión:**
  - quad con billboard cilíndrico inclinado al pitch de la cámara y pivote en los pies;
  - shader de rampa con alpha clip, ShadowCaster y DepthOnly;
  - un spike contra SpriteRenderer con Shader Graph 3D;
  - flipbook propio dirigido por la simulación;
  - 8 direcciones para locomoción y 5 para golpes, con la pala en una capa aparte;
  - una prueba ciega de 4, 8 y 16 direcciones.

### PIXEL ART → [ADR-010](../decisions/ADR-010-content-tools.md), [VISUAL_RESEARCH §D–E](VISUAL_RESEARCH.md)
- Pixelorama 1.2.3 (MIT) [V] exporta `.aseprite` desde la 1.2.2 [V].
- Krita 6.0.4 [V].
- Aseprite es opcional.
- **Decisión:** Pixelorama como herramienta principal, entrando en Unity como `.aseprite` o, si falla, como PNG con JSON (se decide con un spike).
- La resolución base del sprite y los PPU se fijan en la ART_BIBLE tras la prueba de resolución. El tamaño objetivo del jugador en pantalla es de 32–46 px a 480×270 [I].

### 3D ART → [ADR-010](../decisions/ADR-010-content-tools.md)
- Blender 5.2 LTS (5.2.2) [V]. Se exporta en FBX y el `.blend` queda fuera de `Assets/`.
- ProBuilder solo para el graybox.

### AI → [ADR-006](../decisions/ADR-006-ai.md)
- `TeamBrain` (FSM de formación, asignación de bola, "cuerda" de 3–4 m) y `PlayerBrain` (HFSM + Utility con softmax).
- Usa el mismo predictor que la física, sin trampas.
- La dificultad se configura con datos.
- Se descartan BT, GOAP y Unity Behavior.

### AUDIO → [ADR-010](../decisions/ADR-010-content-tools.md)
- **Foley propio** grabado en pista real para golpe, cristal, malla, suelo, red y pasos.
- Audacity 3.7.9/4.0.0, jsfxr (UI) y Surge XT (síntesis).
- Librerías externas solo CC0 o CC-BY, registradas en ASSET_PROVENANCE. NC prohibido.

### UI → [ADR-002](../decisions/ADR-002-rendering.md), [UI_BIBLE](../art/UI_BIBLE.md)
- HUD dibujado a la escala entera del píxel del mundo.
- Menús y textos largos a resolución nativa.
- Fuente bitmap propia hecha con FontForge; fuentes OFL solo en prototipo.

### TESTING → [ARCHITECTURE](../architecture/ARCHITECTURE.md), [TEST_PLAN](../qa/TEST_PLAN.md)
- Unity Test Framework con Edit Mode y Play Mode, que puede ejecutarse en batchmode `-runTests` [V].
- **Además**, el núcleo `Padel.Simulation` no tiene referencias al motor y se testea con .NET 8 y NUnit **fuera de Unity**, incluso en el contenedor de Claude Code y en CI sin licencia.

### VERSION CONTROL → [ADR-011](../decisions/ADR-011-version-control-ci.md)
- `.gitignore` oficial [V] y `.gitattributes` de referencia [V] con LFS desde el principio. GitHub Free incluye 10 GiB + 10 GiB al mes [V].
- UnityYAMLMerge configurado en cada máquina.

### MCP → [ADR-012](../decisions/ADR-012-mcp-cli-skills.md)
- Hay un MCP oficial dentro del CLI `unity` (beta) [V vía la skill oficial]. Como alternativa, CoplayDev/unity-mcp (MIT) [V].
- **Decisión:** usar el oficial en scope local, sin `.mcp.json` versionado.
- **Pendiente de prueba real** en la máquina del propietario.

### SKILLS → [ADR-012](../decisions/ADR-012-mcp-cli-skills.md)
- Se copiaron al repo cuatro skills oficiales de Unity, fijadas a `a851b67` [V].
- Hay diez skills propias del proyecto.
- No se usan skills comunitarias.

### NETWORKING → [ADR-008](../decisions/ADR-008-networking.md)
- NGO 2.13.x requiere Unity 6000.0 o superior, y NGO 3.x requiere 6000.7 [V].
- **Decisión:**
  - el MVP es un jugador contra IA;
  - el multijugador local llega en la Fase 4;
  - no hay online y no se instala Netcode;
  - el núcleo es determinista y está separado por capas.

### LICENSING → [ADR-010](../decisions/ADR-010-content-tools.md), [ASSET_PROVENANCE](../production/ASSET_PROVENANCE.md)
- Unity Personal cuesta 0 € por debajo de 200 k USD [S].
- Las herramientas son MIT o GPL: la GPL de una herramienta no afecta a los assets que se crean con ella.
- Las skills de Unity usan la Unity Companion License [V].
- Las licencias NC y ND están prohibidas y todo asset externo se registra.

## 3. Matriz de decisión

| Tecnología | Alternativa A | Alternativa B | Alternativa C | Coste | Riesgo | Compatibilidad (Unity 6000.3) | Decisión |
|---|---|---|---|---|---|---|---|
| Motor | **Unity 6.3 LTS** | Unreal 5.8 | Godot 4.7 | 0 € (< 200 k USD) | Medio (no ejecutable en el contenedor) | — | **A** |
| Renderer | **URP Universal** | URP 2D Renderer | HDRP | 0 € | Medio (shader propio) | URP 17.3 [V] | **A** |
| Pixelado | **RT fija + escalado entero** | Render Scale + Point | Pixel Perfect Camera | 0 € | Medio (input y aspecto) | [V] | **A** (C descartada [V]) |
| Resolución interna | 480×270 | 640×360 | 320×180 | — | — | — | **Se decide por prueba (Fase 2)** |
| Cámara | **Cinemachine 3.1 + extensión** | Script propio | — | 0 € | Bajo | 3.1.7 [V] | **A** |
| Física de pelota | PhysX | **Integrador propio** | Trayectorias sobre raíles | 0 € | Bajo–Medio (calibración) | C# puro | **B** |
| Movimiento | CharacterController | NavMesh | **Movimiento propio 2D por zonas** | 0 € | Bajo | C# puro | **C** |
| Input | Input Manager | Input System directo | **Input System + comandos** | 0 € | Bajo | 1.20.x [V] | **C** |
| Personaje | SpriteRenderer por defecto | **Quad + shader propio** | SpriteRenderer + Shader Graph 3D | 0 € | Medio | [V] límites | **B** (C en el spike) |
| Direcciones | 4 | **8 (5 únicas)** | 16 | 360 / 600 / 1080 frames por personaje | Medio (coste de arte) | — | **B provisional, prueba ciega** |
| Animación | Mecanim | **Flipbook propio dirigido por la simulación** | Esqueletal 2D | 0 € | Bajo | C# | **B** |
| IA | BT | GOAP | **HFSM + Utility** | 0 € | Bajo | C# puro | **C** |
| Pixel art | **Pixelorama** | Krita | Aseprite | 0 € / 19,99 USD | Bajo (importación por validar) | `.aseprite`/PNG | **A** (+ Krita de apoyo) |
| 3D | **Blender 5.2 LTS** | ProBuilder | — | 0 € | Bajo | FBX | **A** (B solo para graybox) |
| Audio | **Audacity + foley propio** | LMMS | Librerías CC0 | 0 € | Bajo | WAV/OGG | **A** |
| VCS | Git sin LFS | **Git + LFS** | Plastic/UVCS | 0 € (10 + 10 GiB) | Bajo | — | **B** |
| CI | Ninguna | **.NET del núcleo (N1) + GameCI (N2, opcional)** | Unity Build Automation | 0 € | Medio (licencia en CI) | GameCI 6000.3.24f1 [V] | **B** |
| MCP | **CLI `unity mcp` oficial** | CoplayDev/unity-mcp | AI Assistant MCP | 0 € | Medio (beta) | Unity ≥ 6.0 | **A** (B como alternativa) |
| Networking | **Sin online (un jugador + IA, local después)** | NGO 2.x | Netcode for Entities | 0 € | Bajo | NGO 3.x exige 6000.7 [V] | **A** |
| Tests | Solo Unity Test Framework | **UTF + NUnit .NET para el núcleo** | — | 0 € | Bajo | UTF 1.6.x | **B** |

## 4. Dirección visual

Hay **cinco direcciones** propuestas en [`VISUAL_RESEARCH.md` §D](VISUAL_RESEARCH.md):

- D1 "Sobremesa"
- D2 "Luz de Mástil"
- D3 "Polígono"
- D4 "Tinta Riso"
- D5 "Vitrina"

Recomendación del equipo de arte: **D1 como identidad principal y D2 como su versión nocturna** (mismo club, otra hora), tomando de D5 las marcas persistentes en el cristal. D3 serviría como graybox de control.

**Esta elección cambia de forma significativa la experiencia del juego, así que la decide el propietario** (brief §44.4). Hasta entonces, la ART_BIBLE queda en borrador y **no se produce arte definitivo**.

## 5. Lagunas de evidencia (consolidadas)

| # | Laguna | Impacto | Cómo cerrarla |
|---|---|---|---|
| 1 | PDFs oficiales de la FIP no leídos completos (proxy): artículos y redacción literal | Medio | Descargar `FIP_Rules-of-Padel.pdf` 2026 en la máquina del propietario y citar los artículos |
| 2 | Formato oficial de 1v1 (¿pista de 20×6?) | Medio | Buscar el anexo de homologación FIP o la normativa de la FEP. Mientras tanto, decisión de diseño (GDD) |
| 3 | Coeficientes físicos de la pelota de pádel contra cristal, malla y césped | Medio | Vídeo de referencia a 240 fps y calibración |
| 4 | ¿Funciona un Shader Graph Lit 3D sobre SpriteRenderer? | Medio | Spike de ADR-003 |
| 5 | ¿Importa el `.aseprite` de Pixelorama en Unity? ¿Funciona su CLI sin ventana? | Medio | Spike de ADR-010 |
| 6 | Capacidades exactas del MCP oficial (lectura de consola) | Medio | Procedimiento de validación de ADR-012 |
| 7 | Activación de Unity Personal en CI (términos, 2FA) | Bajo | Consulta a la documentación y los términos de Unity por el propietario |
| 8 | Precio del excedente de Git LFS (contradicción entre fuentes) | Bajo | Revisar la configuración de billing de la cuenta |
| 9 | Evidencia sobre Unreal débil ([S]/[NV]) | Bajo | No cambia la decisión (ver ADR-001) |
| 10 | Documentación de Unity (Smart Merge, Input System por defecto, paso fijo por defecto, Unity Behavior) | Bajo | Leerla en la máquina del propietario al crear el proyecto |
| 11 | Postmortems de física de juegos de tenis o golf | Bajo | La calibración empírica sustituye a esa evidencia |

## 6. Próximo paso

Ver [`docs/production/ROADMAP.md`](../production/ROADMAP.md):

- **Fase 0-B (Setup):** proyecto Unity y validación del MCP en la máquina del propietario.
- **Fase 1 (Technical Prototype):**
  - núcleo C# (reglas y puntuación con tests, BALL_MVP con tests), que puede empezar ya en el contenedor;
  - spikes de personaje y de importación de sprites en Unity.
