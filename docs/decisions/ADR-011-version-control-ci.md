# ADR-011 — Control de versiones y CI

- **Estado:** Aceptado. La CI con licencia de Unity queda como Propuesto, pendiente de validar términos y credenciales.
- **Fecha de investigación:** 2026-09-24
- **Depende de / afecta a:** ADR-001, `.gitignore`, `.gitattributes`, TEST_PLAN

## Pregunta

¿Cómo versionamos un proyecto Unity con binarios de arte y audio, y cómo automatizamos compilación y tests?

## Fuentes consultadas

- **[V]** [github/gitignore Unity.gitignore](https://github.com/github/gitignore/blob/main/Unity.gitignore), la plantilla oficial de GitHub.
- **[V]** [gitattributes/gitattributes Unity.gitattributes](https://github.com/gitattributes/gitattributes/blob/master/Unity.gitattributes), plantilla comunitaria de referencia.
- **[V]** GitHub Docs, leídos en el código fuente del repositorio `github/docs`:
  - cuota de LFS en Free y Pro: 10 GiB de almacenamiento y 10 GiB de descarga al mes;
  - tamaño máximo por fichero: 2 GiB;
  - si se agota la cuota sin método de pago, se bloquea.
- **[S]** Precio del excedente: 0,07 USD/GiB al mes y 0,0875 USD/GiB. Es contradictorio con la página de precios; ver KNOWN_ISSUES.
- **[V]** Código del editor: existen las opciones Force Text y Visible Meta Files. **[NV]** Configuración de UnityYAMLMerge.
- **[V]** GameCI:
  - `unity-builder` v6.0.0 y `unity-test-runner` v4.4.0;
  - imágenes para `6000.3.24f1`;
  - activación de Personal con `UNITY_EMAIL` y `UNITY_PASSWORD` según el código de `game-ci/cli`.
  - La documentación oficial de Unity sobre ese mecanismo es [NV].

## Decisión

1. **Git + Git LFS desde el primer commit con binarios.** El `.gitattributes` es la plantilla de referencia más añadidos del proyecto: `.pxo`, `.aseprite`, `.kra`, `.blend` y `.psd` van en LFS con `lockable`.
2. **`.gitignore`**: la plantilla oficial copiada literalmente, más los añadidos documentados en el propio fichero.
3. Force Text, Visible Meta Files y **UnityYAMLMerge** se configuran en cada máquina. Hay instrucciones en TOOLS_RESEARCH §4.1, pendientes de verificación.
4. **CI en dos niveles:**
   - **Nivel 1 (activo desde la Fase 1, sin licencia):** GitHub Actions compila y testea el núcleo `Padel.Simulation` con .NET SDK y NUnit, fuera de Unity. Así se validan reglas, física e IA en cada push.
   - **Nivel 2 (propuesto):** GameCI `unity-test-runner` para Edit Mode y Play Mode en Unity. Requiere que el propietario acepte los términos, cree los secretos y confirme que la activación de Personal en CI está permitida y funciona (incluido 2FA). **No se activa sin decisión explícita del propietario.**
5. Para ahorrar cuota de LFS, los jobs que solo compilan código usan `lfs: false`.
6. **Commits pequeños que describen la intención**, en inglés en el asunto. Prohibidos los mensajes "misc", "stuff" o "update".

## Motivo

Usar LFS desde el principio evita reescribir el historial con `git lfs migrate`. La CI de nivel 1 es gratuita, no necesita licencia y cubre la lógica crítica, que es justo la que más se rompe sin que se note.

## Coste

- 0 € mientras LFS no pase de 10 GiB de almacenamiento y 10 GiB de descarga al mes.
- Hay que vigilar el uso y fijar un límite de gasto.

## Riesgos

- **Cuota de descarga de LFS.** Mitigación: `lfs: false` en CI y caché de `.git/lfs`.
- **LFS a través del proxy del contenedor.** El push de un objeto LFS aún no se ha probado (no hay binarios todavía) y se valida con el primer asset.

## Cómo revertirla

- Salir de LFS requiere `git lfs migrate export`, que reescribe el historial. Por eso la decisión se toma ahora.
- La CI es aditiva.

## Historial

- 2026-09-24: creado. `git-lfs` 3.4.1 instalado en el contenedor con `git lfs install --local`.
