# Investigación: MCP, Unity CLI y Skills para Claude Code

- **Fecha:** 2026-09-24
- **Rol:** Tooling / Infraestructura
- **Contexto:** juego de pádel en Unity (hipótesis: Unity 6.3 LTS). Claude Code corre en un contenedor Linux efímero (Ubuntu 24.04, sin GPU, sin Unity). Unity corre en la máquina Windows/macOS del usuario.
- **Estado del documento:** investigación. No se ha instalado ni configurado nada en el repositorio.

---

## 0. Cómo se ha investigado (y límites importantes)

- **Dominios bloqueados** por el proxy de salida de esta sesión: `docs.unity3d.com`, `docs.unity.com`, `unity.com`, `www.blender.org`, `projects.blender.org` y `gamedevllm.com`. Aplicamos la política: no se ha intentado sortear el bloqueo (ni cachés ni archivos web).
  - Consecuencia: de la documentación oficial de Unity y Blender solo tenemos **títulos y extractos de los resultados del buscador**, no el texto completo de las páginas. Donde el dato depende solo de esos extractos se marca **"Verificado parcialmente (extracto de buscador)"**.
- **Fuentes primarias que sí se leyeron completas:**
  - Repositorios oficiales de Unity en GitHub (`Unity-Technologies/skills`, `Unity-Technologies/unity-agent-plugin`), clonados y leídos. Incluyen la referencia de comandos del Unity CLI escrita por Unity (skill `unity-cli`, alineada con el CLI `1.0.0-beta.10`, 2026-09-14).
  - Repositorios de las alternativas (CoplayDev, IvanMurzak, ahujasid, anthropics/skills y repos de skills de la comunidad), clonados y leídos.
  - Documentación oficial de Claude Code (`code.claude.com/docs/en/mcp.md`, `skills.md`, `discover-plugins.md`), descargada en markdown sin procesar.
  - Metadatos de GitHub (estrellas, licencia, `pushed_at`) consultados mediante la API de búsqueda de GitHub el 2026-09-24.
- Nos quedamos sin cupo de búsquedas web durante la sesión. Algunos puntos quedan en la sección "Lagunas de evidencia".

---

## 1. MCP oficial de Unity

### Hallazgos

Hay **dos caminos oficiales** de Unity Technologies, y conviene no confundirlos:

**A) Servidor MCP del Unity CLI (`unity mcp`) + paquete `com.unity.pipeline`**
- `unity mcp` arranca un servidor MCP que viene dentro del binario `unity` y expone como tools MCP los comandos de un Unity Editor conectado. Transporte **stdio**. Existe desde el CLI `0.1.0-beta.8`. [fuente: Unity-Technologies/skills, `skills/unity-cli/references/integration-advanced.md`](https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/references/integration-advanced.md) (commit `a851b6725718`, 2026-09-22)
- Si no hay ningún Editor abierto, el servidor arranca igual e informa de que no está conectado. Anuncia `tools/list_changed`, así que las tools del Editor aparecen sin reiniciar. [misma fuente]
- Para dirigirlo a un proyecto concreto: `unity mcp --project-path /path/to/MyProject`. Ya no acepta `--instance host:port`. [misma fuente]
- Registro en clientes: `unity mcp configure claude-code`. También admite `claude` y otros 14 clientes. Opciones: `--list`, `--project-path`, `--yes` y `--dry-run`. `--local` solo funciona con cursor, vscode, vscode-insiders, kiro y codex, **no con claude-code**. [misma fuente]
- Requiere que el proyecto tenga el paquete `com.unity.pipeline` (**Unity 6.0 o superior**). Se instala con `unity pipeline install --project-path <ruta>`. Se resuelve desde el registro UPM de Unity. [fuente: `skills/unity-cli/SKILL.md`](https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/SKILL.md)
- Comandos "de producción" que expone el paquete (según Unity; hay que confirmar el catálogo real con `unity command --format json`): `create_gameobject`, `find_gameobjects`, `get_scene_hierarchy`, `set_transform`, `add_component`, `rename_gameobject`, `delete_gameobject`, `save_scene`, `save_all`, `create_script`, `recompile` y `attach_script`. También aparecen `editor_play`, `editor_status`, `log_editor`, `screenshot`, `recompile_status`, `test_status` y `run_tests`. `eval` y `eval_file` (C# arbitrario) existen solo según el proyecto o versión del paquete. [fuente: integration-advanced.md]
- Capturas de pantalla: las tools `capture_game_view` y `capture_scene_view`. Si el hilo principal del Editor no responde, recurren a una **captura de todo el escritorio a nivel de sistema operativo**. [fuente: integration-advanced.md; CHANGELOG `1.0.0-beta.10`, 2026-09-14](https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/CHANGELOG.md)
- Cuando el proyecto tiene errores de compilación, el Editor entra en **Safe Mode**. Entonces el paquete Pipeline no carga y ni `unity status` ni el MCP pueden conectar. Para diagnosticarlo: `unity pipeline list`. [fuente: SKILL.md]
- Madurez: el CLI está en **beta** (`1.0.0-beta.10`). El ejemplo de versión del paquete Pipeline, `0.3.0-exp.1`, indica que el paquete es experimental. [fuente: SKILL.md, integration-advanced.md]

**B) "Unity MCP" dentro del paquete Unity AI Assistant (`com.unity.ai.assistant`)**
- Existen páginas oficiales "Get started with Unity MCP" / "Get started with MCP server" para las versiones del paquete **2.0.0-pre.1, 2.1, 2.6, 2.7.0-pre.3, 2.10, 2.16 y 2.18.0-pre.2**. Todas llevan sufijo `pre`, es decir, son **pre-release**. [fuente: títulos de resultados de búsqueda, docs.unity3d.com/Packages/com.unity.ai.assistant@2.18/manual/integration/unity-mcp-get-started.html](https://docs.unity3d.com/Packages/com.unity.ai.assistant@2.18/manual/integration/unity-mcp-get-started.html) — **verificado parcialmente (extracto de buscador)**
- Según los extractos:
  - Requisitos: Unity 6 (6000.0) o superior, `com.unity.ai.assistant` y un cliente MCP (Claude Code, Cursor, Windsurf, Claude Desktop).
  - Arquitectura: Unity actúa como servidor MCP. Los clientes externos se conectan a través de un **binario relay** (instalado en `~/.unity/relay/` y ejecutado con el flag `--mcp`) que habla con un "MCP bridge" dentro del Editor.
  - Las conexiones de clientes externos requieren **aprobación manual** en el Editor. Las que llegan por "AI Gateway" se aprueban automáticamente.
  - [fuentes: overview 2.18](https://docs.unity3d.com/Packages/com.unity.ai.assistant@2.18/manual/integration/unity-mcp-overview.html), [blog de Unity "Unity MCP Server: Connect Claude Code, Cursor…"](https://unity.com/blog/unity-ai-mcp-how-to-get-started) — **verificado parcialmente (extracto)**
- "AI Gateway": según los extractos, permite ejecutar Claude Code dentro del Unity Editor usando tu propia suscripción o API key, sin gastar Unity AI Credits. [fuente: extractos de búsqueda sobre docs de com.unity.ai.assistant y unity.com/resources/what-is-unity-ai](https://unity.com/resources/what-is-unity-ai) — **verificado parcialmente**
- Coste y suscripción: un extracto dice que **"Unity's MCP server will not consume Unity credits"** (sin URL exacta identificada). Una fuente de terceros afirma que el Gateway exige una suscripción activa a Unity AI y solo acepta API keys, no suscripciones de consumidor. Otra dice que los planes Pro/Enterprise/Industry incluyen créditos y acceso al MCP. **Contradictorio / no verificado** con fuente oficial.
- Documentación del plugin oficial para Claude Code: página "Unity's plugin for Claude Code • Unity AI" en [docs.unity.com/en-us/ai/unity-plugin/claude-code](https://docs.unity.com/en-us/ai/unity-plugin/claude-code). Solo tenemos el título; está bloqueada.

### Verificado / No verificado

| Afirmación | Estado |
|---|---|
| Unity publica un servidor MCP oficial (`unity mcp`, stdio) dentro del Unity CLI | **Verificado** (repositorio oficial de Unity) |
| Requiere `com.unity.pipeline` y Unity 6.0+ | **Verificado** (repositorio oficial) |
| `unity mcp configure claude-code` registra el servidor en Claude Code | **Verificado** como documentado; no probado |
| `com.unity.ai.assistant` incluye un MCP con relay en `~/.unity/relay/` | **Verificado parcialmente** (extractos de docs oficiales) |
| Versión exacta de `com.unity.ai.assistant` compatible con Unity 6.3 | **No verificado** |
| El MCP del AI Assistant requiere login en Unity Cloud o suscripción a Unity AI | **No verificado** (fuentes contradictorias) |
| Lista exacta de tools del MCP del AI Assistant | **No verificado** |
| Licencia del MCP del AI Assistant | **No verificado** (docs bloqueadas) |

### Evaluación
- La ruta A (`unity mcp` del CLI) es oficial, está documentada con detalle en un repo público de Unity, usa stdio y no requiere créditos. Encaja con Unity 6.x. Riesgo: todo está en beta o es experimental.
- La ruta B (AI Assistant) es pre-release. Además, desde aquí no podemos leer sus requisitos de cuenta o suscripción. Puede resultar útil si el estudio ya usa Unity AI.

### Recomendación
1. **MCP principal a validar:** `unity mcp` (Unity CLI + `com.unity.pipeline`), en la máquina del usuario.
2. **No** usar la ruta B hasta que alguien con acceso a docs.unity3d.com confirme sus requisitos de cuenta, créditos y licencia.
3. **No** registrar ningún MCP de Unity en un `.mcp.json` versionado (ver §7).

---

## 2. Unity CLI oficial (`unity`)

### Hallazgos
- Existe un **Unity CLI oficial nuevo**, distinto del Hub CLI. Unity lo presentó en su blog "Meet the Unity CLI: manage Unity from your terminal". [fuente: título de resultado de búsqueda](https://unity.com/blog/meet-the-unity-cli) — verificado parcialmente
  - Documentación oficial (bloqueada; solo títulos): [Introducción](https://docs.unity.com/en-us/unity-cli/unity-cli), [Uso](https://docs.unity.com/en-us/unity-cli/use-unity-cli), [Referencia](https://docs.unity.com/en-us/unity-cli/unity-cli-reference), [Release notes](https://docs.unity.com/en-us/unity-cli/release-notes)
  - Anuncios de versiones en Unity Discussions (títulos): [1.0.0-beta.9](https://discussions.unity.com/t/unity-cli-1-0-0-beta-9-is-rolling-out/1736105), [1.0.0-beta.10](https://discussions.unity.com/t/unity-cli-1-0-0-beta-10-is-rolling-out/1736729)
- **Versión actual:** `1.0.0-beta.10`, del 2026-09-14. Sigue en **beta**. [fuente: CHANGELOG del skill unity-cli](https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/CHANGELOG.md)
- **Instalación** (verificada en la skill oficial de Unity):
  - macOS/Linux: `curl -fsSL https://public-cdn.cloud.unity3d.com/hub/prod/cli/install.sh | UNITY_CLI_CHANNEL=beta bash`
  - Windows (PowerShell): `$env:UNITY_CLI_CHANNEL='beta'; irm https://public-cdn.cloud.unity3d.com/hub/prod/cli/install.ps1 | iex`
  - Comprobar la instalación: `which unity && unity --version`, o `unity version --format json`.
  - En Linux instala un binario autocontenido en `~/.local/bin`. El script verifica el SHA-256 contra un manifiesto del mismo CDN. También existen paquetes `.deb` y `.rpm`.
  - [fuentes: SKILL.md](https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/SKILL.md), [SECURITY.md](https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/SECURITY.md)
- **Comandos verificados** (todos tomados de la referencia oficial en `Unity-Technologies/skills`, commit `a851b6725718`):

| Área | Comandos |
|---|---|
| Autenticación | `unity auth status --format json`, `unity auth login`, `unity auth login --client-id <id> --secret-from-stdin` (service account, CI), `unity auth logout`, `unity auth list`, `unity auth switch <email>` |
| Licencias | `unity license status`, `unity license activate` (suscripción), `--serial`, `--personal --accept-eula`, `--floating`, `--file <.ulf>`, `unity license return --yes` |
| Editores | `unity releases --stream lts --limit 5 --format json`, `unity install lts --module android --yes --accept-eula`, `unity install 6000.0.47f1 --yes --accept-eula`, `unity editors --installed --format json`, `unity install-modules` |
| Proyectos | `unity templates list --editor <6000.x.y> --type core --format json`, `unity projects create "MyGame" --path <dir> --editor-version lts --template com.unity.template.urp-blank`, `unity projects info <path> --format json`, `unity open <path>` |
| Build / test | `unity build <path> --editor-version <v> --target StandaloneLinux64 --execute-method Builder.PerformBuild --allow-install`, `unity test <path> --mode EditMode --report-format junit --output ./test-results.xml --timeout 600`, `unity run <path> -- -executeMethod X` |
| Editor en vivo | `unity status`, `unity command`, `unity command <name>`, `unity list`, `unity pipeline install`, `unity pipeline list` |
| MCP / skills | `unity mcp`, `unity mcp configure claude-code`, `unity skill install claude-code`, `unity skill install claude-code --local`, `unity skill refresh` |
| Diagnóstico | `unity doctor --format json`, `unity logs --follow --level info` |

- Códigos de salida documentados: `0` éxito, `3` fallo de autenticación, `4` falta una precondición (p. ej. sin licencia), `6` fallo del comando y `8` **tests fallidos** (solo en `unity test`). [fuente: SKILL.md]
- Telemetría: el CLI envía informes anónimos de fallos vía Sentry (se desactivan con `UNITY_NO_CRASH_REPORT`). Además envía **un ping anónimo de uso en cada ejecución, sea cual sea el consentimiento**. [fuente: SKILL.md]
- Plantillas: las plantillas Built-in (`com.unity.template.3d` y `.2d`) están **deprecadas desde Unity 6.5 y se eliminan en 6.7**. Por defecto, la skill recomienda `com.unity.template.urp-blank`. [fuente: SKILL.md, commit "Default project creation to URP templates…" 2026-09-21]

**Diferencias con otras herramientas:**
- **Unity Hub CLI:** forma parte de la aplicación de escritorio Hub y exige tenerla instalada. Se invoca con `-- --headless`, por ejemplo `"Unity Hub" -- --headless install --version 6000.3.7f1 --changeset 9b001d489a54`, `install-modules -m`, `install-path -s` o `help`. [fuente: extracto de docs.unity.com/en-us/hub/hub-cli](https://docs.unity.com/en-us/hub/hub-cli) — verificado parcialmente. El Unity CLI nuevo **no necesita el Hub**.
- **Argumentos de línea de comandos del Editor:** son flags del ejecutable del Editor, no un CLI independiente. Los que aparecen en la documentación de Unity son `-batchmode`, `-projectPath`, `-logFile`, `-quit`, `-executeMethod`, `-runTests`, `-testPlatform`, `-testResults`, `-testFilter` y `-nographics`. `unity test` y `unity run` los envuelven, y rechazan que se pasen a mano los reservados (`-batchmode`, `-runTests`…). [fuente: `references/build-run-test.md`](https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/references/build-run-test.md). No pudimos leer la página del Manual de Unity sobre argumentos del Editor (bloqueada).

### Verificado / No verificado
- **Verificado:** existe el CLI, cómo se instala, los comandos de la tabla, que está en beta y que su versión actual es 1.0.0-beta.10.
- **No verificado:** la fecha de disponibilidad general (GA), y si el binario funciona en nuestro contenedor. No se ha instalado, porque ejecutar el instalador queda fuera del alcance de esta investigación.

### Evaluación y recomendación
- `unity --version` es un comando real del Unity CLI nuevo (no del Hub).
- **Recomendación:** que el usuario instale el Unity CLI en su máquina local y lo use para instalar el Editor, crear el proyecto (URP), ejecutar tests (`unity test`, que da un código de salida 8 claro) y servir de MCP.
- En el contenedor no hace falta instalarlo. Si en el futuro queremos CI headless en Linux, habría que resolver antes la licencia (`--serial`, `--floating` o `--file`, porque los service accounts no activan licencias) y el tamaño del Editor. Esto queda fuera del alcance actual.

---

## 3. Skills oficiales de Unity para agentes

### Hallazgos
- **`Unity-Technologies/skills`**: 964 estrellas y 57 forks. Creado el 2026-05-05; último push el 2026-09-23 (último commit leído: 2026-09-22, "fix: quote four skill descriptions…"). Licencia: **Unity Companion License** ("for Unity-dependent projects"). Las incidencias se canalizan por Unity Discussions, no por GitHub Issues. [fuente](https://github.com/Unity-Technologies/skills)
  - Instalación documentada: `npx skills add Unity-Technologies/skills`. [fuente: README](https://github.com/Unity-Technologies/skills/blob/main/README.md). El paquete npm `skills` (v1.7.0, MIT, repo `vercel-labs/skills`) es el CLI de "the open agent skills ecosystem". [fuente: registry.npmjs.org/skills](https://registry.npmjs.org/skills)
  - Contiene 31 skills: `2d-pixel-perfect`, `audio-setup-mixers`, `build-live-game`, `generate-editor-search-query`, `implement-in-app-purchases`, `initialize-ai-navigation`, `levelplay-unity-integration`, `localization`, `manage-sprite-atlas`, `migrate-birp-to-urp`, `new-unity-project`, `optimize-audio`, `optimize-text-mesh-pro`, `optimize-web`, `physics-3d-collision`, `setup-multiplayer-services`, `setup-vivox-voice-chat`, `shader-graph-create-custom-node`, `sprite-editor`, `sprite-segment-3x3grid`, `tilemap-palette-create`, `tilemap-ruletile-createempty`, `tilemap-ruletile-createfromsegment`, `ui`, `ui-imgui`, `ui-ugui`, `ui-uitk`, `unity-cli`, `unity-package-management`, `urp-postprocessing` y `validate-urp-render-graph-renderer-feature`.
- **`Unity-Technologies/unity-agent-plugin`**: 343 estrellas. Creado el 2026-08-06; último push el 2026-09-22. Es un plugin de Claude Code llamado `unity`, versión `0.1.6-beta`, con licencia `LicenseRef-Unity-Companion-License`. Requiere Unity 6+. Su carpeta `skills/` es **idéntica** a la de `Unity-Technologies/skills` (lo comprobamos con `diff -rq`; el último commit es "sync: align skills catalog with Unity-Technologies/skills"). No trae `.mcp.json`. [fuente](https://github.com/Unity-Technologies/unity-agent-plugin)
  - Instalación en Claude Code: `/plugin marketplace add Unity-Technologies/unity-agent-plugin` y después `/plugin install unity@unity-agent-plugin`. Desde la terminal: `claude plugin marketplace add …` y `claude plugin install unity@unity-agent-plugin`. Las skills quedan con el prefijo `/unity:`. [fuente: README](https://github.com/Unity-Technologies/unity-agent-plugin/blob/main/README.md)
- Tercera vía: `unity skill install claude-code [--local]` instala la skill `unity-cli` embebida en el binario del CLI. Con `--local`, además copia la skill `unity-pipeline` que trae el paquete `com.unity.pipeline` a `.claude/skills/unity-pipeline/`. [fuente: integration-advanced.md]

### Evaluación
- Es la fuente con más autoridad: el propio vendor, mantenimiento activo semanal, CI que valida el frontmatter y un `SECURITY.md` que declara los riesgos aceptados.
- Instalar las 31 skills sería excesivo para un juego de pádel 3D: sobran monetización, Vivox, tilemaps 2D, sprites, etc.
- La licencia Unity Companion es adecuada porque nuestro proyecto depende de Unity.

### Recomendación
- **Copiar solo 3 skills** a `.claude/skills/` del repo, fijando el commit `a851b6725718`: `unity-cli`, `physics-3d-collision` (la física de la pelota y las colisiones con paredes y cristal del pádel) y `unity-package-management`. Opcionalmente, `new-unity-project` solo durante el bootstrap.
- Motivo técnico para copiarlas en lugar de instalar el plugin: según la documentación de Claude Code, **las sesiones cloud no leen `~/.claude/skills/`** de la máquina del usuario. Además, desde la v2.1.195 los plugins de origen externo que solo habilita el `.claude/settings.json` del proyecto no se instalan automáticamente. Por tanto, lo único que ve de forma fiable tanto el contenedor como la máquina local es `.claude/skills/` versionado. [fuente: code.claude.com/docs/en/skills](https://code.claude.com/docs/en/skills), [discover-plugins](https://code.claude.com/docs/en/discover-plugins)
- Alternativa aceptable en la máquina local del usuario: `/plugin install unity@unity-agent-plugin`. Carga las 31 skills bajo demanda, con coste de listado de descripciones.

---

## 4. Alternativas open source de Unity MCP

Datos de GitHub consultados el 2026-09-24.

| Candidato | Estrellas | Última release | Licencia | Unity | Transporte | Mantenimiento |
|---|---|---|---|---|---|---|
| [CoplayDev/unity-mcp](https://github.com/CoplayDev/unity-mcp) ("MCP for Unity"; antes justinpbarnett/unity-mcp según el enunciado, **no verificado** el redirect) | 14 445 | tag `v10.2.0` (commit 2026-09-01); último push 2026-09-22 | MIT | 2021.3 LTS → 6.x | HTTP local (`http://localhost:8080/mcp`) o stdio; servidor Python ≥3.10 vía `uv` | Muy activo; patrocinado por Aura/Coplay; 95 issues abiertas |
| [IvanMurzak/Unity-MCP](https://github.com/IvanMurzak/Unity-MCP) ("AI Game Developer") | 4 329 | `0.93.0` (2026-09-23) | Apache-2.0 | `"unity": "2022.3"` mínimo (package.json) | stdio o http; **por defecto, endpoint cloud `https://ai-game.dev/mcp/p/<pin>`** | Muy activo; un único mantenedor principal |
| [CoderGamester/mcp-unity](https://github.com/CoderGamester/mcp-unity) | 1 910 | no comprobado | MIT | no comprobado | no comprobado | push 2026-09-03 |
| [AnkleBreaker-Studio/unity-mcp-server](https://github.com/AnkleBreaker-Studio/unity-mcp-server) | 478 | no comprobado | "Other" (NOASSERTION) | no comprobado | no comprobado | push 2026-07-27 |
| [isuzu-shiranui/UnityMCP](https://github.com/isuzu-shiranui/UnityMCP) | 325 | no comprobado | MIT | no comprobado | HTTP servido por el Editor | push 2026-09-17 |
| [jackwrichards/UnityMCP](https://github.com/jackwrichards/UnityMCP) | 526 | — | Other | — | — | **abandonado** (último push 2025-03-18) |

**Detalle de los dos principales**

- **CoplayDev/unity-mcp** [fuente: README y SECURITY.md, rama `beta`, commit `63202654d5f9`](https://github.com/CoplayDev/unity-mcp)
  - Instalación: en el Package Manager, añadir la URL git `https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main` (o `openupm add com.coplaydev.unity-mcp`). Después, `Window → MCP for Unity → Configure All Detected Clients`.
  - Tiene 47 tools repartidas en grupos, y **solo `core` está activo por defecto**. `testing`, `profiling`, `docs`, etc. se activan con `manage_tools(action="activate", group="…")`.
  - Tools verificadas en el código: `manage_scene` (acciones `get_hierarchy`, `get_active`, `save`…), `find_gameobjects`, `manage_gameobject` (`create`, `modify`), `manage_components`, `read_console`, `manage_camera` (acción `screenshot`), `run_tests` / `get_test_job` (grupo `testing`), `execute_code`, `execute_menu_item`, `manage_script` y `manage_editor`.
  - Seguridad:
    - `execute_code` ejecuta **C# arbitrario** en el proceso del Editor. Su propio código advierte: "Safety checks block known dangerous patterns but are NOT a security sandbox".
    - El HTTP local escucha solo en loopback por defecto. El remoto exige https y API key.
    - Envía telemetría anónima; se desactiva con `DISABLE_TELEMETRY=true`.
    - Incluye tools de generación de assets (`generate_image`, `generate_model`, `generate_audio`) que dependen de servicios externos (no investigado).
- **IvanMurzak/Unity-MCP** [fuente: README](https://github.com/IvanMurzak/Unity-MCP)
  - Tiene más de 70 tools, entre ellas `screenshot-game-view`, `screenshot-scene-view`, `console-get-logs`, `tests-run` y `script-execute` (compila y ejecuta C# con Roslyn).
  - Su CLI `unity-mcp-cli` (npm, v0.93.0, Apache-2.0) hace `login` vía OAuth en ai-game.dev y guarda credenciales en `~/.ai-game-dev/credentials.json`.
  - Por defecto, `setup-mcp` configura el agente contra un **endpoint en la nube de un tercero**. Existe `--no-pin`, pero sigue apuntando a `https://ai-game.dev/mcp`. El modo stdio local existe ("Works locally (stdio) and remotely (http)").
  - Riesgo: el tráfico de la escena y del código pasa por un servicio de terceros si no se configura explícitamente en local.

### Evaluación y recomendación
- Todos estos MCP ejecutan código en el Editor, con los privilegios del usuario. Tratarlos como **"ejecución remota de código autorizada"**: usar solo en proyectos con control de versiones, con commits antes de cada sesión y con los prompts de aprobación de Claude Code activos.
- **Fallback recomendado:** CoplayDev/unity-mcp, fijado a un tag (p. ej. `#v10.2.0`) y **no** a `#main`. Solo si falla la validación de `unity mcp` oficial.
- **Descartado como opción por defecto:** IvanMurzak/Unity-MCP, porque usa un endpoint cloud de terceros por defecto y duplica funciones.
- **Descartados:** los demás, por duplicación, menor adopción o licencia poco clara.
- **Nunca tener dos MCP de Unity activos a la vez.** Duplican tools y confunden el enrutado del modelo.

---

## 5. Blender MCP

### Hallazgos
- **Oficial (Blender Lab):** `lab/blender_mcp` en projects.blender.org, con documentación en [blender.org/lab/mcp-server](https://www.blender.org/lab/mcp-server/). Ambos dominios están bloqueados; todo lo que sigue es **verificado parcialmente (extracto de buscador)**:
  - Es el MCP first-party de la Blender Foundation, construido en Blender Lab.
  - Arquitectura: un add-on dentro de Blender más un servidor MCP separado, comunicados por socket TCP.
  - Ofrece la API Python, inventario de datablocks, informes de archivos que faltan, capturas del viewport o thumbnails, y búsqueda en la referencia de la API y en el manual.
  - Add-on con licencia **GPL-3.0-or-later**.
  - Instalación vía `uvx --from git+https://projects.blender.org/lab/blender_mcp.git@v1.0.0#subdirectory=mcp blender-mcp`.
  - Advertencia oficial: "will execute LLM generated code in Blender without any guards… it is recommended to use a virtual machine, or a system without access to sensitive information".
  - Versiones: un extracto habla de v1.0.3 (2026-09-11) y v1.0.2 (2026-09-08); otro dice "still on v1.0.0" a 1 de septiembre. Versión actual **no confirmada**.
  - [fuentes: projects.blender.org/lab/blender_mcp](https://projects.blender.org/lab/blender_mcp), [releases](https://projects.blender.org/lab/blender_mcp/releases), [Blender Lab Q1 2026](https://www.blender.org/development/blender-lab-activity-report-q1-2026/)
- **Comunidad: `ahujasid/blender-mcp`**, ahora renombrado **`ahujasid/mcp-for-blender`**. El paquete PyPI pasa a llamarse `mcp-for-blender`; `uvx blender-mcp` sigue funcionando. [fuente: README](https://github.com/ahujasid/mcp-for-blender)
  - 29 263 estrellas, licencia MIT, último push 2026-09-21, `pyproject` versión `2.0.3`. Incluye el aviso "This is a third-party integration and not made by Blender". Requiere Blender 3.0 o superior.
  - Claude Code: `claude mcp add blender uvx mcp-for-blender`. Add-on: `uvx mcp-for-blender install-addon`, y en Blender pulsar N → pestaña "MCP for Blender" → "Start MCP Server".
  - Seguridad:
    - `execute_blender_code` ejecuta **Python arbitrario** ("ALWAYS save your work before using it").
    - Existe un modo `BLENDER_MCP_SAFE_MODE=1` que valida los scripts y bloquea el acceso a ficheros, red y subprocesos.
    - Integra servicios externos (Poly Haven, Sketchfab, Hyper3D Rodin, Hunyuan3D).
    - Telemetría: por defecto envía un **registro anónimo mínimo**; el contenido solo se envía con opt-in. Se desactiva por completo con `DISABLE_TELEMETRY=true`.

### Recomendación
- **No instalar ahora.** El pipeline de arte aún no está definido.
- Cuando haga falta, empezar por el MCP **oficial de Blender Lab** (first-party y "deliberately small"). Si falta funcionalidad, usar `mcp-for-blender` con `BLENDER_MCP_SAFE_MODE=1` y `DISABLE_TELEMETRY=true`, las integraciones externas desactivadas y ficheros de trabajo bajo control de versiones o en una VM.
- Ambos se ejecutan **solo en la máquina local**: Blender no está disponible en el contenedor.

---

## 6. Ecosistema de skills de Claude Code

### 6.1 Cómo funcionan las skills (documentación oficial)
Fuente: [code.claude.com/docs/en/skills](https://code.claude.com/docs/en/skills), leída el 2026-09-24.
- Una skill es una carpeta con `SKILL.md`. El archivo empieza con frontmatter YAML entre `---` (el primer `---` debe estar en la primera línea), seguido de las instrucciones en markdown. Puede incluir ficheros de apoyo (`reference.md`, `scripts/`…) enlazados desde `SKILL.md`. Sigue el estándar abierto [Agent Skills](https://agentskills.io).
- **Ubicaciones y precedencia:**

| Nivel | Ruta | Alcance |
|---|---|---|
| Enterprise | `.claude/skills/<skill>/SKILL.md` en el directorio de managed settings | toda la organización |
| Personal | `~/.claude/skills/<skill>/SKILL.md` | todos tus proyectos en esa máquina; **no** en sesiones cloud ni Cowork |
| Proyecto | `.claude/skills/<skill>/SKILL.md` | este repositorio; se versiona |
| Anidado | `<subdir>/.claude/skills/<skill>/SKILL.md` | sesiones en ese subdirectorio |
| Plugin | `<plugin>/skills/<skill>/SKILL.md` | invocación `/plugin:skill` |

  Si dos skills comparten nombre, gana Enterprise, luego Personal y luego Proyecto. El nombre de carpeta `synced` está reservado.
- **Campos de frontmatter** (todos opcionales; `description` es el recomendado):
  - `name` y `description`. La descripción, sumada a `when_to_use`, se trunca a **1 536 caracteres** en el listado.
  - `when_to_use`, `argument-hint`, `arguments`.
  - `disable-model-invocation: true`: solo la invoca el usuario con `/nombre`.
  - `user-invocable: false`: solo la invoca Claude.
  - `allowed-tools`: herramientas preaprobadas durante el turno.
  - `disallowed-tools`, `model`, `effort`.
  - `context: fork` junto con `agent`: la skill se ejecuta en un subagente.
  - `paths`: globs que limitan cuándo se activa.
  - `shell`, `hooks`, `metadata`, `license`, `compatibility`.
- **Inyección dinámica:** `` !`comando` `` ejecuta un comando de shell antes de cargar la skill. Se puede desactivar con `disableSkillShellExecution: true`. Esto es relevante para la seguridad: al revisar skills de terceros hay que leer estas líneas.
- Claude Code vigila `.claude/skills/` y aplica los cambios en la sesión en curso.

### 6.2 Candidatos evaluados

| Candidato | Mantenimiento | Actividad | Licencia | Reputación | Seguridad | Relevancia | Duplicación | Decisión |
|---|---|---|---|---|---|---|---|---|
| [Unity-Technologies/skills](https://github.com/Unity-Technologies/skills) → `unity-cli`, `physics-3d-collision`, `unity-package-management` | Vendor oficial | Commits semanales (último 2026-09-22) | Unity Companion | Oficial, 964★ | `unity-cli` permite C# eval y piping a shell (riesgos declarados en SECURITY.md) | Alta | Ninguna | **ADOPTAR** (copiar las 3 al proyecto, fijando el commit) |
| Unity-Technologies/skills → `new-unity-project` | Oficial | Ídem | Unity Companion | Oficial | Bajo | Alta, solo en el bootstrap | — | **ADOPTAR temporalmente** (bootstrap) |
| Unity-Technologies/skills → `ui-uitk`, `urp-postprocessing`, `optimize-audio`, `localization` | Oficial | Ídem | Unity Companion | Oficial | Bajo | Media, para fases posteriores | — | **POSPONER** |
| Unity-Technologies/skills → IAP, LevelPlay, Vivox, tilemap, sprites 2D | Oficial | Ídem | Unity Companion | Oficial | Bajo | Baja | — | **DESCARTAR** |
| [Unity-Technologies/unity-agent-plugin](https://github.com/Unity-Technologies/unity-agent-plugin) | Oficial | push 2026-09-22 | Unity Companion | Oficial, 343★ | Igual que arriba | Media | **Duplica al 100 %** Unity-Technologies/skills | **ALTERNATIVA** solo para la máquina local |
| [anthropics/skills](https://github.com/anthropics/skills) → `skill-creator` | Anthropic | push 2026-09-10 | Apache-2.0 | Oficial | Bajo | Alta (para crear nuestras skills de GDD, art bible y QA) | Ya disponible en esta sesión como `anthropic-skills:skill-creator` | **USAR la ya disponible**; no instalar |
| anthropics/skills → `doc-coauthoring` | Anthropic | Ídem | (sin LICENSE propio en la carpeta; el README dice que "muchas" son Apache-2.0) | Oficial | Bajo | Media (GDD) | Parcial con skills propias | **OPCIONAL** |
| anthropics/skills → `docx`, `xlsx`, `pptx`, `pdf` | Anthropic | Ídem | **Source-available, no open source** | Oficial | Bajo | Baja | Ya disponibles en la sesión | **NO instalar** |
| [Donchitos/Claude-Code-Game-Studios](https://github.com/Donchitos/Claude-Code-Game-Studios) | Comunidad, 1 autor | Muy activo (push 2026-09-24) | MIT | 25 390★ | Framework completo: 49 agentes, 72 skills y 13 hooks (SessionStart, PreToolUse…). Las skills ejecutan `yaml-helper.sh` vía `` !` ` `` y fijan `model: sonnet` | Alta en temática (`art-bible`, `qa-plan`, `quick-design`, `playtest-report`…) | Solapa con nuestros docs de diseño y producción | **NO ADOPTAR entero**. Usar como **referencia** (MIT) para redactar nuestras propias skills. Sus skills no son independientes (dependen de `.claude/hooks/yaml-helper.sh`) |
| [gamedev-skills/awesome-gamedev-agent-skills](https://github.com/gamedev-skills/awesome-gamedev-agent-skills) | Comunidad | push 2026-09-10; creado 2026-06-24 | Apache-2.0 | 1 124★, joven | Por revisar (tiene router en Python) | Media (`game-feel`, `physics-tuning`, `camera-systems`) | Solapa con las skills oficiales de Unity | **DESCARTAR por ahora**; quizá como lectura |
| [nowsprinting/unity-coding-skills](https://github.com/nowsprinting/unity-coding-skills) | Comunidad, 1 autor | push 2026-09-23 | Unlicense (dominio público) | 20★ | Bajo | Media (diseño de tests, TDD Unity) | Depende del MCP de JetBrains Rider (`run_unity_tests`) | **DESCARTAR** (dependencia de Rider MCP); reconsiderar si se usa Rider |
| [arjun988/blender-skills](https://github.com/arjun988/blender-skills), [kevinbadi/blender-skills](https://github.com/kevinbadi/blender-skills), [viettranx/3dviz-pro-max](https://github.com/viettranx/3dviz-pro-max) | Comunidad | Activos | No comprobada | 120–529★, muy jóvenes | Sin revisar | Baja ahora | — | **DESCARTAR** (sin revisión, pipeline de arte no definido) |
| [MRCalderon3D/everything-game-dev-code](https://github.com/MRCalderon3D/everything-game-dev-code) | Comunidad | push 2026-09-20 | No comprobada | 86★ | Sin revisar | Media | Solapa con Donchitos | **DESCARTAR** |

### 6.3 Conjunto mínimo recomendado
1. `.claude/skills/unity-cli/` (oficial Unity, commit `a851b6725718`)
2. `.claude/skills/physics-3d-collision/` (oficial Unity)
3. `.claude/skills/unity-package-management/` (oficial Unity)
4. *(Temporal, bootstrap)* `.claude/skills/new-unity-project/` (oficial Unity). Retirarla cuando el proyecto exista.
5. **Skills propias, a escribir con `skill-creator`:** `gdd` (documento de diseño del pádel), `art-bible` y `qa-checklist`. Tomar ideas de Donchitos (MIT, con atribución) pero sin hooks ni dependencias.

Total: 3–4 skills de terceros oficiales y 3 propias. No se recomienda ninguna skill comunitaria de terceros sin revisión línea a línea.

**Instalación (pendiente de aprobación; no ejecutada):** copiar las carpetas desde un clon de `https://github.com/Unity-Technologies/skills` en el commit fijado. Añadir en cada carpeta un `SOURCE.txt` con la URL, el commit y la licencia. `npx skills add Unity-Technologies/skills` instalaría el repositorio entero; no se verificó si permite seleccionar skills sueltas, así que hay que comprobarlo con `npx skills add --help` antes de usarlo.

---

## 7. Configuración MCP en Claude Code (documentación oficial)

Fuente: [code.claude.com/docs/en/mcp](https://code.claude.com/docs/en/mcp), leída el 2026-09-24.
- **Transportes:**
  - `http` (recomendado para servidores remotos; `streamable-http` es alias en JSON).
  - `sse` (**deprecado**).
  - `stdio` (procesos locales).
  - WebSocket (`"type":"ws"`, solo vía `.mcp.json` o `claude mcp add-json`).
- **Comandos:**
  - `claude mcp add --transport http <name> <url>`
  - `claude mcp add [--env K=V] [--scope …] --transport stdio <name> -- <command> [args…]` (el `--` separa las opciones de Claude del comando del servidor)
  - `claude mcp add-json <name> '<json>'`
  - `claude mcp list`, `claude mcp get <name>`, `claude mcp remove <name>`, `claude mcp reset-project-choices`
  - Dentro de la sesión: `/mcp`
- **Scopes:**

| Scope | Dónde se guarda | Compartido |
|---|---|---|
| `local` (por defecto) | `~/.claude.json`, bajo la ruta del proyecto | No |
| `project` | `.mcp.json` en la raíz del repo | Sí (git) |
| `user` | `~/.claude.json` (nivel superior) | No, pero aplica a todos tus proyectos |

- **`.mcp.json`:** `{"mcpServers": {"<name>": {"type": "stdio", "command": "...", "args": [...], "env": {...}}}}`. Admite expansión `${VAR}` y `${VAR:-default}` en `command`, `args`, `env`, `url` y `headers`. También admite `"timeout"` por servidor.
- **Aprobación:** en sesiones interactivas, Claude Code pide aprobación antes de usar servidores de `.mcp.json`. **Importante para nosotros: en `claude -p`, Agent SDK y sesiones cloud los carga sin preguntar.**

### Recomendación para este proyecto
- **Ahora, en el repositorio:** **no crear `.mcp.json`** con Unity ni Blender. En el contenedor cloud no existe el comando `unity`, así que el servidor fallaría en cada sesión. Además, la ruta del proyecto (`--project-path`) es distinta en cada máquina. Tampoco conviene que un fichero versionado dispare ejecución de código sin prompt en sesiones cloud.
- **En la máquina del usuario:** registrar el MCP en **scope local**. Hay dos opciones:
  - a) `unity mcp configure claude-code --project-path <ruta-proyecto>` (verificado como documentado; no se verificó en qué fichero y scope escribe);
  - b) componer los comandos verificados a mano: `claude mcp add --transport stdio unity -- unity mcp --project-path <ruta-proyecto>`. Esta combinación se deduce de dos fuentes verificadas, pero **no se ha probado**.
- Opcional: documentar en `docs/` un ejemplo `.mcp.json.example` (no activo) si el equipo crece.

---

## 8. Procedimiento de validación MCP (checklist en la máquina del usuario)

Usa solo comandos y tools verificados en las fuentes. Las tools del MCP oficial son los comandos que expone el Editor; los nombres pueden variar según la versión del paquete Pipeline, por eso el paso 3 los lista antes de usarlos.

**Preparación**
1. [ ] El proyecto Unity está en git y el árbol de trabajo está limpio (`git status`). Hacer commit antes de empezar.
2. [ ] Instalar el Unity CLI (§2) y comprobarlo: `unity --version` y `unity doctor --format json`.
3. [ ] `unity auth status --format json` y `unity license status --format json`. Si hace falta: `unity auth login` y `unity license activate`.
4. [ ] `unity pipeline install --project-path <proyecto>`, con el Editor **cerrado**.
5. [ ] `unity open <proyecto>`, y después `unity status --format json` hasta que la instancia aparezca en estado `ready`.
6. [ ] Si no conecta: `unity pipeline list` para descartar Safe Mode por errores de compilación.

**Registro en Claude Code**

7. [ ] `unity mcp configure claude-code --project-path <proyecto> --dry-run`. Revisar la entrada y, si es correcta, repetir sin `--dry-run`.
8. [ ] `claude mcp list` y `claude mcp get <nombre>`: el estado debe ser `✔ Connected`. Dentro de la sesión, `/mcp`.

**Pruebas funcionales** (pedírselas a Claude Code y anotar el resultado)

9. [ ] **Ver el proyecto:** listar las tools disponibles. Deben coincidir con `unity command --format json` y `unity list --format json`.
10. [ ] **Inspeccionar la escena:** `get_scene_hierarchy` devuelve la jerarquía de la escena activa, y `find_gameobjects` encuentra un objeto conocido (p. ej. `Main Camera`).
11. [ ] **Leer la consola:** no se ha verificado ningún comando de lectura de consola en el catálogo del paquete Pipeline. Hacer lo siguiente:
    - a) buscarlo con `unity command --query console` o `unity command --query log`;
    - b) como alternativa verificada, leer `<proyecto>/Logs/Editor.log` (Unity 6), o el log global (Windows `%USERPROFILE%\AppData\Local\Unity\Editor\Editor.log`, macOS `~/Library/Logs/Unity/Editor.log`);
    - c) prueba de ida y vuelta: `log_editor "MCP validation ping"` y comprobar que el mensaje aparece en la consola o en el log.
12. [ ] **Modificación pequeña:** `create_gameobject` crea `MCP_Validation_Cube`, `set_transform` lo coloca en (0, 1, 0) y `add_component` le añade un `Rigidbody`.
13. [ ] **Validar:** repetir `find_gameobjects` / `get_scene_hierarchy` y confirmar que existe el objeto con el componente. Guardar con `save_scene`. Después, `git diff` debe mostrar cambios solo en el `.unity` esperado.
14. [ ] **Captura:** usar `capture_game_view` y `capture_scene_view` (tools MCP), o `unity command screenshot --output ./shot.png --width 1920 --height 1080`. Comprobar que la imagen corresponde a la vista, y no a la captura de escritorio completo que se usa como fallback (el resultado lo indica en una nota).
15. [ ] **Tests (opcional):** `unity test <proyecto> --mode EditMode --report-format junit --output ./test-results.xml --timeout 600`. Esperar código de salida 0, o 8 si hay tests que fallan.
16. [ ] **Limpieza:** `delete_gameobject MCP_Validation_Cube` o `git checkout -- <escena>`.

**Criterio de éxito:** los pasos 8–14 pasan sin editar YAML a mano.
**Si fallan:** repetir los pasos 9–14 con CoplayDev/unity-mcp (fijado a tag) usando sus tools verificadas: `manage_scene(action="get_hierarchy")`, `find_gameobjects`, `read_console`, `manage_gameobject(action="create" | "modify")`, `manage_components`, `manage_scene(action="save")`, `manage_camera(action="screenshot")` y, tras activar el grupo `testing`, `run_tests` / `get_test_job`. **Desinstalar un MCP antes de instalar el otro.**

---

## 9. Riesgos de seguridad (resumen)

| Riesgo | Dónde aparece | Mitigación |
|---|---|---|
| Ejecución arbitraria de C# en el Editor | `unity command eval` (Pipeline), `execute_code` (CoplayDev), `script-execute` (IvanMurzak) | Usar git y commits previos; mantener los prompts de aprobación; no activar modos de bypass; no pasar a estas tools contenido no confiable (inyección de prompts) |
| Ejecución arbitraria de Python en Blender | Blender Lab MCP, `execute_blender_code` | VM o máquina sin datos sensibles (recomendación oficial de Blender Lab); `BLENDER_MCP_SAFE_MODE=1` |
| Carga de `.mcp.json` sin prompt en sesiones cloud o `-p` | Claude Code | No versionar `.mcp.json` que ejecute código; usar scope local |
| Tráfico hacia la nube de terceros | IvanMurzak (ai-game.dev), generadores de assets de CoplayDev, integraciones Poly Haven/Sketchfab/Hyper3D en Blender MCP | Configuración local y stdio; desactivar integraciones |
| Telemetría | Unity CLI (ping de uso siempre), CoplayDev, mcp-for-blender | `UNITY_NO_CRASH_REPORT` (el ping de uso no se desactiva según la doc), `DISABLE_TELEMETRY=true` |
| Instalación mediante `curl \| bash` | Unity CLI | Viene del CDN oficial con verificación SHA-256; alternativa: paquetes `.deb`/`.rpm` firmados |
| Skills con `` !`cmd` `` y `allowed-tools` amplios | Skills de terceros (p. ej. Donchitos) | Revisar cada `SKILL.md`; preferir copiar con commit fijado; valorar `disableSkillShellExecution` |
| Cadena de suministro por paquetes UPM desde `#main` | CoplayDev | Fijar tag |

---

## 10. Qué se puede hacer ahora en el repo y qué requiere la máquina local

| Acción | Dónde |
|---|---|
| Copiar 3–4 skills oficiales de Unity a `.claude/skills/` con commit fijado | **Repo (ahora)**, previa aprobación |
| Escribir skills propias (`gdd`, `art-bible`, `qa-checklist`) | **Repo (ahora)** |
| Documentar el procedimiento MCP (este documento) | **Repo (hecho)** |
| `.mcp.json` para Unity o Blender | **No**; usar scope local en la máquina del usuario |
| Instalar el Unity CLI, el Editor 6.3 LTS y la licencia | **Máquina local** |
| `unity pipeline install`, `unity mcp configure claude-code` y la validación de §8 | **Máquina local** (Claude Code ejecutándose allí) |
| Blender MCP | **Máquina local**, más adelante |

---

## 11. Lagunas de evidencia

1. **Docs de `com.unity.ai.assistant` (Unity MCP del AI Assistant) bloqueadas:** no se verificó la versión compatible con Unity 6.3, los comandos exactos del relay para Claude Code, la lista de tools, el requisito de login o suscripción a Unity AI, ni la licencia. Las fuentes secundarias se contradicen sobre créditos y suscripción.
2. **Docs de docs.unity.com (Unity CLI, Hub CLI) bloqueadas:** los comandos del CLI se verificaron contra la referencia oficial que Unity publica en `Unity-Technologies/skills`, no contra docs.unity.com. No se verificó la fecha de GA.
3. **No hay una tool verificada de lectura de consola** en el catálogo del paquete `com.unity.pipeline`. Hay que descubrirla en tiempo de ejecución (`unity command --query …`).
4. **Qué fichero y scope escribe `unity mcp configure claude-code`:** no documentado en lo que leímos. Hay que comprobarlo con `--dry-run` o `--list`.
5. **Licencia y términos del paquete `com.unity.pipeline`:** no verificados.
6. **Blender Lab MCP:** versión actual (v1.0.0 frente a v1.0.3), versiones de Blender soportadas y lista de tools. Solo tenemos extractos del buscador.
7. **El paso de `justinpbarnett/unity-mcp` a `CoplayDev/unity-mcp`** no se verificó en esta sesión.
8. **`npx skills add`:** no se verificó si permite instalar skills sueltas ni dónde las instala para Claude Code.
9. **Existencia y versión exacta de "Unity 6.3 LTS":** fuera del alcance de este documento. La skill oficial usa en un ejemplo la ruta `…/Editor/6000.3.11f1/…`, lo que sugiere que la rama 6000.3 existe, pero no confirma que sea LTS.
10. Nada de lo anterior se ha **probado** en una máquina con Unity. Toda la sección 8 está pendiente de ejecución real.

---

## 12. Fuentes

Todas consultadas el 2026-09-24. Entre paréntesis, la fecha de la fuente cuando se conoce.

**Unity (oficial)**
- https://github.com/Unity-Technologies/skills (commit a851b6725718, 2026-09-22)
- https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/SKILL.md
- https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/references/integration-advanced.md
- https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/references/build-run-test.md
- https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/references/auth-license-cloud.md
- https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/CHANGELOG.md (CLI 1.0.0-beta.10, 2026-09-14)
- https://github.com/Unity-Technologies/skills/blob/main/skills/unity-cli/SECURITY.md
- https://github.com/Unity-Technologies/skills/blob/main/CONTRIBUTING.md
- https://github.com/Unity-Technologies/unity-agent-plugin (commit ba1be1802afd, 2026-09-22; plugin 0.1.6-beta)
- https://docs.unity3d.com/Packages/com.unity.ai.assistant@2.18/manual/integration/unity-mcp-get-started.html (bloqueada; extracto)
- https://docs.unity3d.com/Packages/com.unity.ai.assistant@2.18/manual/integration/unity-mcp-overview.html (bloqueada; extracto)
- https://docs.unity3d.com/Packages/com.unity.ai.assistant@2.7/manual/integration/unity-mcp-get-started.html (bloqueada; título)
- https://unity.com/blog/unity-ai-mcp-how-to-get-started (bloqueada; extracto)
- https://unity.com/blog/meet-the-unity-cli (bloqueada; título y extracto)
- https://unity.com/resources/what-is-unity-ai (bloqueada; extracto)
- https://docs.unity.com/en-us/unity-cli/unity-cli-reference (bloqueada; título)
- https://docs.unity.com/en-us/unity-cli/release-notes (bloqueada; extracto)
- https://docs.unity.com/en-us/hub/hub-cli (bloqueada; extracto)
- https://docs.unity.com/en-us/ai/unity-plugin/claude-code (bloqueada; título)
- https://docs.unity.com/en-us/ai/credits/credits-about (bloqueada; título)
- https://discussions.unity.com/t/unity-cli-1-0-0-beta-10-is-rolling-out/1736729 (título)

**Anthropic / Claude Code (oficial)**
- https://code.claude.com/docs/en/mcp (markdown leído)
- https://code.claude.com/docs/en/skills (markdown leído)
- https://code.claude.com/docs/en/discover-plugins (markdown leído)
- https://github.com/anthropics/skills (commit 34040c9c5685, 2026-09-10)

**MCP comunitarios**
- https://github.com/CoplayDev/unity-mcp (rama beta, commit 63202654d5f9, 2026-09-20; tag v10.2.0 2026-09-01)
- https://github.com/CoplayDev/unity-mcp/blob/beta/SECURITY.md
- https://github.com/IvanMurzak/Unity-MCP (0.93.0, 2026-09-23)
- https://registry.npmjs.org/unity-mcp-cli (0.93.0, Apache-2.0)
- https://github.com/CoderGamester/mcp-unity
- https://github.com/AnkleBreaker-Studio/unity-mcp-server
- https://github.com/isuzu-shiranui/UnityMCP
- https://github.com/jackwrichards/UnityMCP

**Blender**
- https://github.com/ahujasid/mcp-for-blender (antes ahujasid/blender-mcp; último commit 2026-09-21)
- https://projects.blender.org/lab/blender_mcp (bloqueada; extracto)
- https://projects.blender.org/lab/blender_mcp/releases (bloqueada; extracto)
- https://www.blender.org/lab/mcp-server/ (bloqueada; extracto)
- https://www.blender.org/development/blender-lab-activity-report-q1-2026/ (bloqueada; título)

**Skills comunitarias**
- https://github.com/Donchitos/Claude-Code-Game-Studios (último commit 2026-09-24)
- https://github.com/gamedev-skills/awesome-gamedev-agent-skills (último commit 2026-09-10)
- https://github.com/nowsprinting/unity-coding-skills (último commit 2026-09-23)
- https://github.com/arjun988/blender-skills
- https://github.com/kevinbadi/blender-skills
- https://github.com/viettranx/3dviz-pro-max
- https://github.com/MRCalderon3D/everything-game-dev-code
- https://registry.npmjs.org/skills (skills CLI 1.7.0, vercel-labs, MIT)
