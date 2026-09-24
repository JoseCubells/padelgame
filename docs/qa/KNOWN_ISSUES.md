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
| KI-011 | 2026-09-24 | Arte | Dirección visual sin elegir | Bloquea el arte definitivo | Decisión del propietario | **Cerrado 2026-09-24** (ADR-013) |
| KI-012 | 2026-09-24 | Reglas (MVP) | `PointReferee` no modela: pelota que toca a un jugador (J7/J8), jugador que toca la red (J5), juego exterior (J13), pelota enganchada en la malla (J11), ni la obligación del bote previo al saque (S3). Las puertas se tratan como malla cerrada | Medio | Añadir cuando exista colisión jugador-pelota (Fase 1-B/3) | Abierto |
| KI-013 | 2026-09-24 | IA / balance | En partidos headless IA contra IA (semillas fijas, 24 puntos): los puntos terminan casi siempre por doble bote (pocos errores no forzados o golpes ganadores). En 2v2 Medium contra Medium hay rallies muy largos (media de 27,6 golpes; 4 de 24 superan el timeout de 90 s). Whiffs en ~5 % de los golpes | Medio (sensación) | Fase 5 (AI Polish): modelo de riesgo del golpe (red/fuera según la dificultad de la bola), ganadores por colocación, ajuste de `AIProfile` con la simulación headless | Abierto |

## Placeholders activos (REPLACE BEFORE ART LOCK)

| Asset | Tipo | Dónde | Reemplazo previsto |
|---|---|---|---|
| — | — | Aún no hay assets | — |
