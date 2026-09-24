# Auditoría de entorno — Fase 0

- **Fecha:** 2026-09-24
- **Responsable:** Claude Code (director técnico)
- **Método:** comandos ejecutados en el contenedor de trabajo; la salida relevante se resume abajo. Nada se infiere: lo que no se pudo comprobar aparece como "no comprobable".

## 1. Contexto de ejecución

| Elemento | Resultado | Evidencia |
|---|---|---|
| Tipo de entorno | Contenedor cloud efímero (Claude Code on the web) | `uname -a` → `Linux vm 6.18.44-fc-v37 x86_64` |
| SO | Ubuntu 24.04.4 LTS | `/etc/os-release` |
| CPU / RAM | 4 vCPU / 15 GiB, sin swap | `nproc`, `free -h` |
| Disco | 30 GB disponibles (asignación por sesión) | `df -h` |
| GPU | **No hay GPU** | `nvidia-smi` no existe, `/dev/dri` no existe |
| Pantalla | Headless (sin servidor X) | — |

## 2. Herramientas

| Herramienta | Estado | Versión / nota |
|---|---|---|
| Unity Editor | **NO instalado** | `unity`, `Unity` no están en PATH |
| Unity Hub | **NO instalado** | `unityhub` no está en PATH |
| Unity CLI | **NO instalado** | — |
| Claude Code | Instalado | `2.1.281 (Claude Code)` |
| Git | Instalado | `2.43.0` |
| Git LFS | **Instalado 3.4.1** (apt, 2026-09-24) | `git lfs version` |
| Node / npm | Instalado | Node `v22.22.2`, npm `10.9.7` |
| Python | Instalado | `3.11.15` |
| Docker | Instalado | Docker Engine `29.3.1` |
| Blender | **NO instalado** (apt ofrece `4.0.2`, versión antigua; download.blender.org bloqueado) | — |
| Pixelorama | **NO instalado** | — |
| Krita | **NO instalado** (apt ofrece `5.2.2`) | — |
| Aseprite | No instalado (no requerido) | — |
| Audacity / LMMS | No instalados | — |
| ffmpeg / ImageMagick | No en PATH (Playwright trae un ffmpeg en `/opt/pw-browsers/ffmpeg-1011`) | — |
| Chromium (Playwright) | Instalado | `/opt/pw-browsers/chromium` |
| .NET SDK | **Instalado 8.0.131** (apt, 2026-09-24). Antes: no instalado; apt ofrece `dotnet-sdk-8.0` `8.0.125`; `api.nuget.org` accesible (HTTP 200) | `apt-cache policy`, `curl` |

## 3. Red

La política de red del entorno **deniega** (HTTP 403 del proxy de salida) estos hosts, probados con `curl`:

- `download.unity3d.com`, `public-cdn.cloud.unity3d.com`, `unity.com`, `docs.unity3d.com`, `packages.unity.com`
- `download.blender.org`
- Servidores de licencias y cuentas de Unity: `license.unity3d.com`, `core.cloud.unity3d.com`, `api.unity.com` (comprobado el 2026-09-24, segunda sesión). Aunque la imagen Docker `unityci/editor` es accesible por Docker Hub, **el editor no puede activar licencia** en este entorno.

`github.com` sí es accesible. La investigación web se hace mediante las herramientas de búsqueda/lectura web de Claude Code, que no dependen de este proxy.

**Consecuencia:** en este contenedor no se puede instalar Unity ni resolver paquetes del registro de Unity. Para cambiarlo, el propietario debe ampliar el acceso de red del entorno (menú del entorno cloud → Edit → Network access) o añadir esos dominios a la lista permitida.

## 4. Claude Code: MCP y skills

| Elemento | Estado |
|---|---|
| `.mcp.json` en el proyecto | No existe |
| MCP de Unity | No configurado (no hay Unity) |
| Skills de usuario | `~/.claude/skills/session-start-hook` (skill del entorno web) |
| Skills de proyecto | Ninguna (repositorio vacío) |
| Plugins | Ninguno instalado aparte de los sincronizados del entorno |

## 5. Repositorio

| Elemento | Estado |
|---|---|
| Remoto | `https://github.com/JoseCubells/padelgame` |
| Rama de trabajo | `claude/great-pasteur-aw5ek6` |
| Commits | **Ninguno** ("No commits yet") — repositorio vacío |
| Proyecto Unity existente | **No existe** (no hay `Assets/`, `ProjectSettings/`, `Packages/`) |

## 6. Conclusiones que condicionan el plan

1. **Aquí no se puede ejecutar Unity.** No está instalado, sus hosts están bloqueados, no hay GPU y activar una licencia Personal exige una cuenta Unity del propietario. Por tanto, en este contenedor **no** se pueden hacer compilación Unity, Play Mode, capturas de Game View ni pruebas reales de MCP.
2. **Lo que sí se puede hacer aquí:** investigación, documentación, ADRs, arquitectura, código C# puro (reglas, puntuación, física determinista) y sus pruebas unitarias con .NET SDK 8 (instalable por apt; NuGet es accesible), scripts de pipeline y skills de Claude Code.
3. **Validación visual y MCP:** requieren la máquina local del propietario con Unity instalado, o un runner de CI con licencia Unity (ver ADR-001 y el informe de MCP). Se documenta como bloqueo en `docs/qa/KNOWN_ISSUES.md`.
