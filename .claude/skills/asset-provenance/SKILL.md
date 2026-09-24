---
name: asset-provenance
description: Licensing and provenance gate for any external or reference-derived asset (art, audio, fonts, code snippets, templates) - allowed/forbidden licences (NC and ND forbidden, CC-BY-SA needs approval, GPL not in build), required fields in docs/production/ASSET_PROVENANCE.md, attribution and THIRD_PARTY_NOTICES. Use BEFORE adding or committing any asset not created from scratch for this game.
---

# Procedencia y licencias de assets

Registro: `docs/production/ASSET_PROVENANCE.md`. Avisos: `THIRD_PARTY_NOTICES.md`. Reglas completas: ADR-010 y `docs/research/TOOLS_RESEARCH.md` §6.

## Gate (antes del commit)

1. ¿Es 100 % original y creado para este juego, sin referencia externa directa? No hace falta registrarlo.
2. Si no:
   - **leer la licencia en su fuente oficial** (el fichero LICENSE o la página del autor), nunca en un agregador;
   - anotar la fecha.
3. **Decidir** con esta tabla:

| Licencia | Decisión |
|---|---|
| CC0 / dominio público | ✅ |
| CC-BY | ✅ + crédito |
| CC-BY-SA | ⚠️ Solo con aprobación del propietario |
| **NC (cualquiera)** | ❌ |
| **ND** | ❌ |
| OFL (fuentes) | ✅ + licencia incluida |
| MIT / Apache (código o plantillas) | ✅ + aviso completo en `THIRD_PARTY_NOTICES.md` |
| GPL / LGPL dentro del build | ❌ Salvo ADR con análisis legal |
| Unity Asset Store | ⚠️ Solo con ADR; nunca en repositorio público |
| Sin licencia explícita / "free" | ❌ |

4. **Registrar** una fila con:
   - ID (`AP-NNN`);
   - ruta;
   - tipo;
   - URL de origen;
   - autor;
   - licencia y nivel de verificación;
   - fecha de obtención;
   - restricciones;
   - si requiere atribución;
   - notas.
5. Si requiere atribución, añadir el crédito, el texto de la licencia y el copyright a `THIRD_PARTY_NOTICES.md`.

## Siempre prohibido

- Personajes, logos, marcas o assets de otros juegos.
- Jugadores, marcas o clubes reales.
- Texturas de stock como solución visual principal.
- Sonidos de librerías sin licencia verificada.

## Referencias

Un modelo 3D o una foto usados solo como guía para dibujar a mano también se registran, con la nota "referencia, redibujado a mano".
