# Verificación directa en código fuente oficial

- **Fecha:** 2026-09-24
- **Motivo:** la política de red del entorno bloquea `docs.unity3d.com`, `unity.com` y `padelfip.com`, y la cuota de búsquedas web de la sesión se agotó durante la investigación. Los repositorios oficiales de Unity Technologies en GitHub sí son accesibles. Para las decisiones críticas de rendering se verificó el **código fuente** oficial, que es la fuente primaria más fuerte disponible.
- **Método:** `git clone --depth 1 --filter=blob:none --sparse`. Se fijan commit y rama, y se cita cada archivo leído.

## 1. Versiones de paquetes URP / Shader Graph para Unity 6.3

Fuente: [Unity-Technologies/Graphics](https://github.com/Unity-Technologies/Graphics), rama `6000.3/staging`, commit `2d2e78c` (2026-07-01).

| Paquete | `version` | `unity` |
|---|---|---|
| `com.unity.render-pipelines.universal` | `17.3.0` | `6000.3` |
| `com.unity.shadergraph` | `17.3.0` | `6000.3` |

El repositorio también tiene ramas `6000.4/staging` y `6000.5/staging`, así que existen líneas de Unity 6.4 y 6.5 (su estado LTS/Update no se deduce de las ramas, ver `TECHNOLOGY_RESEARCH.md`).

## 2. Pixel Perfect Camera (URP 17.3)

Archivo: `Packages/com.unity.render-pipelines.universal/Runtime/2D/PixelPerfectCamera.cs`

- En `OnBeginCameraRendering` el componente **escribe `m_Camera.orthographicSize`** (salvo en modo de compatibilidad con Cinemachine) y fija `PixelPerfectRendering.pixelSnapSpacing`.
- Todo su cálculo (`PixelPerfectCameraInternal.cs`: "orthographic size", "Find a pixel-perfect orthographic size...") gira en torno al tamaño ortográfico.
- Vive en `Runtime/2D/`, dentro del conjunto de funciones del 2D Renderer.

**Conclusión verificada:** el componente está diseñado para cámaras **ortográficas**. Con una cámara en perspectiva su parámetro principal (`orthographicSize`) no tiene efecto sobre la proyección. **No sirve como solución** para una cámara de tercera persona en perspectiva; aplicarlo "porque el juego es pixel art" sería un error, tal como anticipaba el brief.

## 3. Shaders de sprite por defecto en URP 17.3

Archivos: `Packages/com.unity.render-pipelines.universal/Shaders/2D/Sprite-Lit-Default.shader`, `Sprite-Unlit-Default.shader`

- `Sprite-Lit-Default` tiene tres pases: `Universal2D`, `NormalsRendering` y `UniversalForward`.
- Tags del SubShader: `"Queue"="Transparent" "RenderType"="Transparent"`, `Blend SrcAlpha OneMinusSrcAlpha`, `Cull Off`, `ZWrite [_ZWrite]` con valor por defecto `0`.
- `grep -c ShadowCaster` sobre todos los `.shader` de `Shaders/2D/` devuelve **0**: ningún shader de sprite por defecto tiene pase `ShadowCaster`.

**Conclusiones verificadas:**
1. Con el Universal Renderer (3D), un `SpriteRenderer` con los materiales por defecto se dibuja en la cola **transparente**, sin escribir profundidad por defecto y **sin proyectar sombras 3D**, porque no tiene pase ShadowCaster.
2. Para que los personajes pixel art proyecten sombras reales sobre la cancha 3D, se integren con la profundidad (red, cristal) y reciban luz de forma coherente con el mundo, hace falta un **shader propio** (Shader Graph Lit/Unlit con Alpha Clipping, que genera ShadowCaster/DepthOnly, o HLSL) o una sombra alternativa (blob / proyectada). Esto alimenta ADR-002 y ADR-003.

## 4. Subtargets de Shader Graph disponibles en URP 17.3

`Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Targets/`: `UniversalLitSubTarget`, `UniversalUnlitSubTarget`, `UniversalFullscreenSubTarget`, `UniversalDecalSubTarget`, `UniversalCanvasSubTarget`, `UniversalUISubTarget`, `UniversalSixWaySubTarget`, `UniversalTerrainLitSubTarget`.

- `Fullscreen` es relevante para un pase de post-proceso propio (paleta, dithering, outline) con el Render Graph de URP.
- `Lit`/`Unlit` con Alpha Clipping es la base candidata para el material de billboard de personaje.

## 5. Versiones en ramas de desarrollo de paquetes oficiales (raw `package.json`, 2026-09-24)

| Repositorio @ rama | `version` | `unity` mínimo | Lectura |
|---|---|---|---|
| `Unity-Technologies/com.unity.cinemachine@main` | `3.1.8-pre.2` | `2022.3` | La línea activa es Cinemachine 3.1.x. La rama de desarrollo va por delante del último release publicado, que no se verificó aquí (registro bloqueado). |
| `Unity-Technologies/InputSystem@develop` | `1.20.1` | `6000.0` | Input System 1.x, compatible con Unity 6. |
| `Unity-Technologies/com.unity.netcode.gameobjects@develop-2.0.0` | `2.13.4` | `6000.0` | NGO 2.x existe para Unity 6. **No se instala** (ver ADR-008). |

Advertencia: una rama de desarrollo **no** equivale a la versión publicada en el registro. La versión exacta que resuelva el Package Manager de Unity 6.3 se fijará en `Packages/manifest.json` cuando se cree el proyecto en la máquina del propietario.

## 6. Skills oficiales de Unity

Fuente: [Unity-Technologies/skills](https://github.com/Unity-Technologies/skills), commit `a851b67` (2026-09-22). Tiene 31 skills y licencia Unity Companion License. Ver `.claude/skills/VENDORED_SKILLS.md`.

## 7. Plantilla `.gitignore`

Fuente: [github/gitignore Unity.gitignore](https://github.com/github/gitignore/blob/main/Unity.gitignore), obtenida el 2026-09-24 y copiada literalmente en la sección 1 del `.gitignore`.
