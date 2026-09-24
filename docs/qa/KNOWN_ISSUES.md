# KNOWN ISSUES

Registro de problemas, bloqueos y placeholders abiertos. Cada entrada tiene un ID estable. Cuando se resuelve se marca, pero **no se borra**.

| ID | Fecha | Tipo | Descripción | Impacto | Acción / responsable | Estado |
|---|---|---|---|---|---|---|
| KI-001 | 2026-09-24 | Bloqueo de entorno | El contenedor de Claude Code no tiene Unity. Además, la red bloquea `download.unity3d.com`, `unity.com`, `packages.unity.com` y `docs.unity3d.com`, no hay GPU y la licencia Personal requiere la cuenta del propietario | No se puede crear el proyecto, ejecutar Play Mode, capturar la Game View ni validar el MCP desde el contenedor | Fase 0-B en la máquina del propietario (ROADMAP). Opcional: ampliar el acceso de red del entorno cloud | Abierto |
| KI-002 | 2026-09-24 | Bloqueo de entorno | La red bloquea `padelfip.com`, `blender.org`, `pixelorama.org` y otras webs oficiales. La cuota de búsquedas web se agotó durante la investigación | Parte de la evidencia es de nivel [S] o [NV] | Cerrar las lagunas de RESEARCH_REPORT §5 cuando haya acceso | Abierto |
| KI-003 | 2026-09-24 | Evidencia | PDF de reglas FIP 2026 no leído completo (artículos literales, posición del pie en el saque, primer sacador tras un tie-break, cambio de lado al terminar un set) | Medio (reglas) | Leer el PDF en la máquina del propietario y citar los artículos en PADEL_RULES_RESEARCH | Abierto |
| KI-004 | 2026-09-24 | Diseño | No hay un formato oficial de 1v1 verificado | Medio | El vertical slice se hace en 20×10; playtest con una variante estrecha (GDD §5) | Abierto |
| KI-005 | 2026-09-24 | Física | Faltan coeficientes medidos de la pelota de pádel contra cristal, malla y césped | Medio | Vídeo a 240 fps y calibración (ADR-004) | Abierto |
| KI-006 | 2026-09-24 | Técnico | No se sabe si un Shader Graph Lit 3D funciona sobre SpriteRenderer | Medio | Spike de ADR-003 | Abierto |
| KI-007 | 2026-09-24 | Pipeline | No se sabe si Pixelorama → `.aseprite` → Unity importer funciona, ni si el CLI de Pixelorama corre sin ventana | Medio | Spike de ADR-010 | Abierto |
| KI-008 | 2026-09-24 | Herramientas | La lectura de consola del MCP oficial no está verificada | Medio | Procedimiento de ADR-012 | Abierto |
| KI-009 | 2026-09-24 | VCS | El push de objetos LFS a través del proxy del contenedor no está probado (aún no hay binarios) | Bajo | Validarlo con el primer asset binario | Abierto |
| KI-010 | 2026-09-24 | Licencia | Precio del exceso de LFS contradictorio entre fuentes; términos de uso de Unity Personal en CI sin verificar | Bajo | El propietario revisa su configuración de billing y los términos de Unity | Abierto |
| KI-011 | 2026-09-24 | Arte | Dirección visual sin elegir | Bloquea el arte definitivo | Decisión del propietario | Abierto |

## Placeholders activos (REPLACE BEFORE ART LOCK)

| Asset | Tipo | Dónde | Reemplazo previsto |
|---|---|---|---|
| — | — | Aún no hay assets | — |
