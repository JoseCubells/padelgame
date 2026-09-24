# ASSET PROVENANCE

Todo asset que no se haya creado desde cero para este juego **debe** registrarse aquí antes de hacer commit (skill `asset-provenance`). También se registran los assets propios hechos a partir de una referencia externa, como un modelo 3D usado de guía para dibujar.

## Reglas de licencia (ADR-010, TOOLS_RESEARCH §6)

| Licencia | ¿Se permite? | Condiciones |
|---|---|---|
| Creado para el juego | ✅ | Registrar autor y fecha si es relevante |
| CC0 / dominio público | ✅ | Registrar el origen igualmente |
| CC-BY | ✅ | Crédito en el juego y en `CREDITS` |
| CC-BY-SA | ⚠️ Solo con aprobación explícita del propietario | Share-alike puede contaminar los derivados |
| **CC-BY-NC / cualquier NC** | ❌ **Prohibido** | No comercial |
| **ND (NoDerivatives)** | ❌ Evitar | No se puede adaptar |
| SIL OFL (fuentes) | ✅ | Incluir la licencia y el copyright; no vender la fuente suelta; renombrar si se modifica una fuente con Reserved Font Name |
| GPL / LGPL (código o assets) | ❌ En el build, salvo decisión legal | Las herramientas GPL sí se pueden usar: lo que se crea con ellas es nuestro |
| MIT / Apache-2.0 (código) | ✅ | Aviso en `THIRD_PARTY_NOTICES` |
| Unity Asset Store (Standard EULA) | ⚠️ Solo con justificación en un ADR | No redistribuir el asset suelto ni subirlo a un repo público [NV] |
| Unity Companion License (skills de Unity) | ✅ | Proyectos que dependen de Unity. Licencia en `.claude/skills/UNITY_SKILLS_LICENSE.md` |
| "Free asset" sin licencia explícita | ❌ | Free ≠ legal para uso comercial |

Prohibido también: copiar personajes, logos, marcas o assets de otros juegos, y usar marcas o jugadores reales.

## Registro

| ID | Asset / ruta | Tipo | Origen (URL) | Autor | Licencia | Fecha de obtención | Restricciones | ¿Atribución? | Notas |
|---|---|---|---|---|---|---|---|---|---|
| AP-001 | `.gitignore` (sección 1) | Config | https://github.com/github/gitignore/blob/main/Unity.gitignore | GitHub y colaboradores | CC0-1.0 [V: LICENSE del repo, 2026-09-24] | 2026-09-24 | — | No | Copia literal |
| AP-002 | `.gitattributes` (sección 1) | Config | https://github.com/gitattributes/gitattributes/blob/master/Unity.gitattributes | Alexander Karatarakis y colaboradores | MIT [V: LICENSE del repo, 2026-09-24] | 2026-09-24 | Conservar aviso de copyright | Sí (aviso MIT en la cabecera del fichero) | Copia literal |
| AP-003 | `.claude/skills/{unity-cli, physics-3d-collision, unity-package-management, validate-urp-render-graph-renderer-feature}` | Skills (docs + 1 script C# de depuración) | https://github.com/Unity-Technologies/skills @ `a851b67` | Unity Technologies | Unity Companion License [V] | 2026-09-24 | Proyectos que dependen de Unity | No | Ver `VENDORED_SKILLS.md` |

**Assets de juego (arte, audio, fuentes):** ninguno todavía.
