---
name: gameplay-qa
description: QA workflow for padelgame - running core tests in .NET and Unity (Edit/Play Mode), mandatory suites per system, determinism hash tests, evidence recording per milestone, playtest protocol and phase gates (vertical slice "padel is fun" gate). Use when writing tests, finishing an implementation step, closing a milestone, or deciding whether a phase can advance.
---

# QA de gameplay

Plan completo: `docs/qa/TEST_PLAN.md`.

## Cada paso de implementación (brief §41)

1. Escribir o ajustar los tests **antes o junto con** el código. Las suites obligatorias están en TEST_PLAN §2.
2. **Compilar y ejecutar:**
   - Núcleo (Core, Rules, Simulation, AI): `dotnet test tools/CoreTests`, disponible en el contenedor. Requiere `apt-get install -y dotnet-sdk-8.0`.
   - Unity: `unity test <proyecto> --mode EditMode --report-format junit --output ./test-results.xml` y lo mismo con `PlayMode` (ver skill `unity-cli`). El código de salida 8 significa que hay tests fallidos.
3. Si hay cambio visual, aplicar la skill `visual-quality-review`.
4. **Dejar evidencia en el mensaje final:**
   - archivos modificados;
   - tests ejecutados, con número de pasados y fallidos;
   - capturas;
   - errores encontrados y corregidos;
   - riesgos pendientes.

   Formato: ESTADO / DECISIÓN / EVIDENCIA / PRÓXIMO PASO.
5. Hacer commit con un mensaje que describa la intención.

**Nunca** decir "funciona" sin la salida de los tests. **Nunca** desactivar, saltar ni poner en cuarentena un test para que pase en verde.

## Determinismo

Todo sistema de la simulación tiene un test de hash: misma semilla + mismos comandos → mismo hash de `MatchState` tras N ticks. Si cambia el hash de un *golden replay*, se trata como una regresión: se explica y se actualiza de forma consciente.

## Gates de fase

- **Fase 1:** se puede sacar, golpear, rebotar y puntuar en graybox; el núcleo pasa en .NET y en Unity; los spikes están decididos.
- **Fase 3 (vertical slice):** GAMEPLAY_SPEC §9 y brief §36. Se juzga con playtest de 5–8 personas. **Si falla cualquier criterio, no se pasa a la Fase 4.**

## Playtest

- Primeros 5 minutos sin ayuda.
- Métricas: duración del punto, errores no forzados, uso de la pared, comprensión del saque y del globo.
- Cuestionario de identidad: ¿es pádel?, ¿es tercera persona?, ¿2.5D?, ¿personajes pixel art?, ¿pista 3D?, ¿física creíble?
- Resultados en `docs/qa/` con fecha.

## Bugs

Todo bug que no se corrige en el momento va a `docs/qa/KNOWN_ISSUES.md` con un ID estable. Nunca se borran entradas.
