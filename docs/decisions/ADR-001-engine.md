# ADR-001 — Motor

- **Estado:** Aceptado (provisional, ver "Validación pendiente")
- **Fecha de investigación:** 2026-09-24
- **Decisores:** director técnico (Claude Code), con el informe del arquitecto técnico
- **Depende de / afecta a:** todos los ADR. En particular ADR-002 (rendering), ADR-003 (personajes) y ADR-012 (MCP)

## Pregunta

¿Qué motor usamos para un juego de pádel 2.5D con mundo y pista 3D, pelota física 3D, personajes pixel art 2D, cámara en tercera persona, 1v1/2v2 e IA, desarrollado por un equipo muy pequeño asistido por Claude Code? La hipótesis inicial del brief era Unity 6.3 LTS y había que verificarla, no asumirla.

## Fuentes consultadas

Detalle y URLs en [`TECHNOLOGY_RESEARCH.md`](../research/TECHNOLOGY_RESEARCH.md) y [`SOURCE_VERIFICATION.md`](../research/SOURCE_VERIFICATION.md). Las más importantes:

- **[V]** Tags de [Unity-Technologies/UnityCsReference](https://github.com/Unity-Technologies/UnityCsReference): último parche 6.3 `6000.3.24f1`, 6.6 `6000.6.2f1` y 6.7 todavía en beta (`6000.7.0b1`). El director técnico lo comprobó directamente con `git ls-remote` el 2026-09-24.
- **[V]** Código fuente de URP 17.3 (rama `6000.3/staging` de [Unity-Technologies/Graphics](https://github.com/Unity-Technologies/Graphics)): límites de los sprites en 3D y de Pixel Perfect Camera.
- **[V]** Repositorio oficial [Unity-Technologies/skills](https://github.com/Unity-Technologies/skills) (commit `a851b67`): Unity CLI (`unity`, beta) con servidor MCP integrado.
- **[S]** Unity: soporte de 6.3 LTS hasta diciembre de 2027; Unity Personal gratis por debajo de 200 000 USD de ingresos o financiación; Runtime Fee cancelada; 6.7 anunciada como próxima LTS para el cuarto trimestre de 2026.
- **[S]/[S2]** Unreal Engine 5.8 (2026-06-17): royalty del 5 % a partir de 1 M USD por producto. Paper2D sigue documentado en 5.8, pero no hay declaración sobre su mantenimiento. PaperZD solo tiene soporte confirmado hasta UE 5.7.
- **[V]** Godot 4.7.2-stable (tag del 2026-08-16), que se usa solo como referencia de control.

## Alternativas evaluadas

| | Unity 6.3 LTS (URP) | Unreal Engine 5.8 | Godot 4.7 (referencia) |
|---|---|---|---|
| Sprites en 3D | SpriteRenderer/quad + Shader Graph; límites verificados en código [V] | Paper2D (mantenimiento sin confirmar) + PaperZD (5.8 sin confirmar) | Sprite3D nativo [NV] |
| Pixel art en 3D | Hay que construirlo: RT de baja resolución + point filter (igual que en los otros dos) | Igual | Igual |
| Física de pelota | Propia (ADR-004); en C# es trivial de testear | Propia en C++ | Propia en GDScript/C# |
| Cámaras | Cinemachine 3.1.7 [V] | Spring Arm / cámara [NV] | Básico [NV] |
| MCP / Claude Code | CLI oficial `unity mcp` (beta) [V] + CoplayDev/unity-mcp MIT [V] | "Integraciones MCP" en 5.8 [S2] | Proyectos comunitarios [V existe] |
| Serialización / Git | Force Text + Smart Merge [V en código del editor] | `.uasset` binario [NV] | `.tscn` en texto [NV] |
| Lenguaje / tests | C# + Unity Test Framework, en batchmode y CI Linux (GameCI) [V] | C++/Blueprints [NV] | GDScript/C# |
| Soporte | LTS hasta diciembre de 2027 [S] | Sin LTS formal; UE6 en camino [S2] | Sin LTS [NV] |
| Coste | 0 € por debajo de 200 k USD [S] | 0 € hasta 1 M USD por producto y después 5 % [S] | MIT [NV] |
| **Puntuación ponderada** (máx. 185; juicio técnico, no medida) | **158** | **121** | **135** |

La matriz completa por criterio y peso está en `TECHNOLOGY_RESEARCH.md` §3.2.

## Ventajas y desventajas

**Unity 6.3 LTS**
- Ventajas:
  - Es la LTS vigente verificada.
  - Los límites de sprites en 3D están verificados en código, así que el riesgo es conocido y acotado.
  - C# se testea fácilmente en Edit Mode.
  - Hay CLI, MCP y skills oficiales para agentes.
  - Serializa en texto y es compatible con Git.
  - Cinemachine es maduro.
  - Coste cero en nuestro rango.
- Desventajas:
  - Los shaders de sprite por defecto no reciben luz 3D ni proyectan sombras con el Universal Renderer, así que hace falta un shader propio (ADR-003).
  - Pixel Perfect Camera no sirve (ADR-002).
  - El CLI oficial y el MCP del paquete Pipeline están en beta o experimentales.
  - Historial de cambios de licencia (Runtime Fee 2023, ya cancelada).

**Unreal 5.8**
- Ventajas: renderer y Material Editor muy potentes; Behavior Trees y StateTree maduros.
- Desventajas:
  - El elemento visual diferencial (personajes 2D en 3D) dependería de Paper2D, cuyo estado de mantenimiento se desconoce, y de un plugin de terceros sin soporte confirmado en 5.8.
  - Los assets binarios dificultan el trabajo con Git y agentes.
  - Iterar en C++ es más lento [NV].
  - Su potencia fotorrealista no aporta nada a este estilo.

**Godot 4.7**
- Ventajas: Sprite3D nativo, escenas en texto, MIT.
- Desventajas:
  - Menos ecosistema.
  - Sin LTS.
  - El brief pide explícitamente Unity o Unreal. Queda como alternativa de control si cambiaran las condiciones de Unity.

## Compatibilidad

- Editor: Unity **6000.3.x**, fijado en `6000.3.24f1` o el último parche 6.3 disponible al crear el proyecto. El Linux Editor soporta Ubuntu 22.04 y 24.04 [S].
- Paquetes objetivo, verificados en repositorios oficiales (la versión exacta la fija el Package Manager al crear el proyecto y se congela en `Packages/manifest.json`):

| Paquete | Versión |
|---|---|
| URP / Shader Graph | 17.3.x |
| Cinemachine | 3.1.x (3.1.7 publicado) |
| Input System | 1.20.x |
| Test Framework | 1.6.x |

## Coste

- 0 € con Unity Personal mientras no se superen 200 000 USD de ingresos o financiación [S]. Unity Pro solo si se superan.
- Riesgo de cambio de licencia: bajo, pero existe. Hay que revisarlo en cada hito.

## Riesgos

| Riesgo | Prob. | Impacto | Mitigación |
|---|---|---|---|
| **El contenedor cloud no puede ejecutar Unity**: sin instalación, hosts de Unity bloqueados, sin GPU, y la licencia Personal requiere cuenta | Cierta | Alto | Unity se instala en la máquina del propietario. En el contenedor solo se desarrolla y prueba el núcleo en C# puro (.NET). Ver ENVIRONMENT_AUDIT y KNOWN_ISSUES |
| Personajes pixel art iluminados y con sombra en 3D sin solución estándar | Cierta (verificado) | Medio | Spike en la semana 1 (ADR-003) |
| Unity 6.7 LTS sale en Q4 2026 y NGO 3.x lo exige | Alta | Bajo (no hay online en el MVP) | Reevaluar la migración a 6.7 cuando tenga 2–3 parches; nunca en mitad de un milestone |
| CLI y MCP oficiales en beta | Media | Medio | Alternativa documentada: CoplayDev/unity-mcp fijado a un tag (ADR-012) |
| Activación de Personal en CI sin documentación oficial | Media | Medio | CI opcional; los tests del núcleo corren en .NET sin licencia Unity |
| Evidencia de Unreal débil ([S]/[NV]) | — | Bajo | La decisión no depende de detalles finos de Unreal: el factor decisivo (Paper2D/PaperZD) es un riesgo de dependencia en cualquier caso |

## Decisión

**Unity 6.3 LTS (`6000.3.x`, fijado al último parche disponible al crear el proyecto) con URP y Universal Renderer (no el 2D Renderer).** Scripting en C#.

## Motivo

1. Es la LTS vigente verificada en fuente primaria y ofrece unos 15 meses de soporte.
2. El reto visual diferencial (pixel art 2D dentro de un mundo 3D iluminado) es resoluble con piezas estándar (Shader Graph/HLSL, RenderTexture), y sus límites están verificados en código. En Unreal, ese mismo reto depende de piezas con mantenimiento incierto.
3. C# + Test Framework + ensamblados sin `UnityEngine` permiten que el núcleo (reglas, física, IA) se desarrolle y teste también fuera del editor. Esto es crítico dado que el entorno de Claude Code no puede ejecutar Unity.
4. Existen herramientas oficiales para agentes (CLI, MCP, skills) y serialización en texto: encaja con un estudio pequeño asistido por IA.

## Consecuencias

- El proyecto Unity vive en la **raíz del repositorio**: `Assets/`, `Packages/`, `ProjectSettings/`. Las fuentes de arte van en `ArtSource/`, fuera de `Assets/`.
- Project Settings obligatorios, que se fijan al crear el proyecto y se verifican en el primer commit Unity:
  - Asset Serialization = Force Text.
  - Version Control = Visible Meta Files.
  - Active Input Handling = Input System Package.
  - Color Space = Linear. Queda pendiente de ADR-002, ya que el tratamiento de paleta podría pedir cambios.
- La creación del proyecto la hace el propietario en su máquina (procedimiento en `docs/production/ROADMAP.md`, Fase 0-B), o Claude Code si se ejecuta localmente con Unity instalado.
- Se reevalúa Unity 6.7 LTS al cerrar la Fase 3 (Vertical Slice 1v1), mediante un ADR nuevo.

## Validación pendiente

1. Crear el proyecto con `6000.3.x` + plantilla URP y compilar una escena vacía: evidencia en forma de log y captura.
2. Spike de personaje con sombra (ADR-003).
3. Validación del MCP con prueba real (ADR-012).

Si 1 o 2 fallan de forma no resoluble, se reabre este ADR.

## Cómo revertirla

La simulación (reglas, pelota, IA, input abstracto) es C# puro sin `UnityEngine` (ARCHITECTURE), así que es portable a otro motor con C# (Godot C#) con poco coste. La presentación (shaders, escenas, cámara) habría que rehacerla. Cuanto antes se revierta, menor es el coste: el punto de no retorno práctico es el COURT_ART_PASS (Fase 6).

## Historial

- 2026-09-24: creado. Se confirma la hipótesis Unity 6.3 LTS con evidencia.
