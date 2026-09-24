# ADR-012 — Unity MCP, Unity CLI y skills de Claude Code

- **Estado:** Aceptado. La configuración del MCP queda **pendiente de prueba real** en la máquina del propietario. Mientras la prueba no pase, el MCP **no se considera configurado**.
- **Fecha de investigación:** 2026-09-24
- **Depende de / afecta a:** ADR-001, CLAUDE.md, `.claude/skills/`

## Pregunta

¿Cómo conectamos Claude Code con Unity (escenas, GameObjects, componentes, consola, tests, capturas) y qué skills instalamos, con prioridad para lo oficial y sin duplicar?

## Fuentes consultadas

Todas están en [`MCP_SKILLS_RESEARCH.md`](../research/MCP_SKILLS_RESEARCH.md).

- **[V]** [Unity-Technologies/skills](https://github.com/Unity-Technologies/skills) en el commit `a851b67` (2026-09-22). Tiene 31 skills y la Unity Companion License. El director técnico lo clonó y revisó directamente.
- **[V]** Skill `unity-cli`, del mismo repositorio. Documenta el CLI oficial `unity` (beta, 1.0.0-beta.10, 2026-09-14) con:
  - `unity --version`, `unity doctor`
  - `unity auth`, `unity license`
  - `unity install`, `unity projects create`
  - `unity test`
  - `unity mcp` (servidor MCP stdio; requiere el paquete `com.unity.pipeline` y Unity 6.0 o superior)
  - `unity mcp configure claude-code`
- **[S]** MCP de `com.unity.ai.assistant`: solo versiones preliminares (2.x-pre). No se ha verificado si requiere suscripción o créditos.
- **[V]** [CoplayDev/unity-mcp](https://github.com/CoplayDev/unity-mcp): MIT, Unity 2021.3 a 6.x, soporta Claude Code y ejecución de tests. Hay una discrepancia sobre la última versión: v10.0.0 según un informe y v10.2.0 según otro; se fija por tag al instalar.
- **[V]** Documentación de Claude Code sobre MCP: scopes `local`, `project` y `user`. En sesiones cloud y con `-p`, el `.mcp.json` del proyecto **se carga sin preguntar**.

## Alternativas evaluadas

| | CLI oficial `unity mcp` (Pipeline) | MCP de AI Assistant | CoplayDev/unity-mcp | IvanMurzak/Unity-MCP |
|---|---|---|---|---|
| Origen | Oficial Unity | Oficial Unity | Comunidad (MIT) | Comunidad |
| Madurez | Beta / experimental | Pre-release | Estable (v10.x) | — |
| Requisitos | CLI + `com.unity.pipeline`, Unity ≥ 6.0 | Posible suscripción [NV] | Paquete UPM + servidor Python | Por defecto conecta con la nube de un tercero (ai-game.dev) |
| Capacidades verificadas | Jerarquía, crear y modificar objetos, componentes, guardar escena, `capture_game_view` y `capture_scene_view`, `run_tests`; la lectura de consola **no está verificada** | [NV] | Jerarquía, consola, objetos, componentes, capturas y tests | — |
| Decisión | **1.ª opción** | Descartado por ahora (requisitos sin verificar) | **Alternativa** si falla la oficial | **Descartado** (nube de terceros) |

**Regla:** nunca dos MCP de Unity instalados a la vez.

## Decisión

1. **MCP:** usar el **CLI oficial `unity mcp`** registrado en **scope local** en la máquina del propietario. **No se versiona `.mcp.json`**: en las sesiones cloud se ejecutaría sin preguntar, y el comando `unity` no existe en el contenedor.
2. El MCP se considera configurado **solo** cuando se completa en la máquina del propietario el procedimiento de `MCP_SKILLS_RESEARCH.md` §8:
   - ver el proyecto;
   - inspeccionar la escena;
   - leer la consola;
   - hacer una modificación pequeña;
   - validarla;
   - capturar Game View y Scene View.

   La evidencia se guarda en `docs/qa/evidence/mcp/`.
3. **Skills oficiales copiadas al repositorio** (fijadas a `a851b67`, con procedencia en `.claude/skills/VENDORED_SKILLS.md`):
   - `unity-cli`
   - `physics-3d-collision`
   - `unity-package-management`
   - `validate-urp-render-graph-renderer-feature`

   Se descartan, por ahora:
   - `2d-pixel-perfect`: es para el 2D Renderer y ortográfica; ADR-002 no la usa.
   - `new-unity-project`: se usará puntualmente al crear el proyecto y no hace falta tenerla copiada.
   - El resto: fuera de alcance (IAP, Vivox, LevelPlay, Web…).
4. **Skills propias** en `.claude/skills/`:
   - `padel-rules`, `padel-physics`, `padel-gameplay`, `padel-ai`
   - `2p5d-character-pipeline`, `pixel-art-pipeline`
   - `visual-quality-review`, `asset-provenance`, `gameplay-qa`
   - `padel-research`
5. **No se instalan skills comunitarias** (por ejemplo, Donchitos/Claude-Code-Game-Studios): dependen de hooks propios y duplican las nuestras. Se usan solo como referencia.
6. **Blender MCP:** no se instala hasta que haga falta en la Fase 6. Preferencia: el MCP oficial de Blender Lab en una VM, siguiendo su propia recomendación de seguridad.

## Motivo

Primero lo oficial, como pide el brief, y con una alternativa ya evaluada. Copiar las skills al repositorio y fijarlas a un commit garantiza que las sesiones cloud las carguen y que su contenido esté revisado. Por seguridad no se versiona `.mcp.json`: todos estos MCP ejecutan código arbitrario en el editor.

## Riesgos

- **El CLI y el Pipeline están en beta.** Mitigación: alternativa con CoplayDev fijado por tag.
- **Telemetría del CLI** (ping de uso no desactivable según su documentación). Se acepta y queda documentado.
- **Instalación con `curl | bash`.** Viene del CDN oficial con verificación SHA-256. Alternativa: paquetes firmados.

## Cómo revertirla

Hay que borrar las carpetas de skills y desregistrar el MCP con `claude mcp remove`. No afecta al código del juego.

## Historial

- 2026-09-24: creado. Skills oficiales copiadas al repositorio en el commit `c81dc9e`.
