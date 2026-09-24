---
name: padel-physics
description: Custom deterministic padel ball simulation (ADR-004) - BallSimulator integrator, drag and Magnus forces, swept collisions against court planes/net, friction-impulse bounce with spin, surface materials (floor, glass, mesh), FIP bounce calibration, TrajectoryPredictor and ShotSolver inverse ballistics. Use when writing, tuning or testing anything in Padel.Simulation.Ball or Shots. Not PhysX.
---

# Física de la pelota (ADR-004)

**La pelota no usa PhysX.** Integrador propio, C# puro y determinista, en `Padel.Simulation.Ball`. El detalle y las fórmulas con fuentes están en `docs/research/GAMEPLAY_RESEARCH.md` §2 y §3.4.

## Contrato

- `BallState { Vec3 position, velocity, spin }`: struct serializable.
- `BallSimulator.Step(ref BallState, in BallConfig, in CourtConfig, float dt, List<SimEvent> events)`: función pura sin estado oculto, con `dt = 1/120`.
- `TrajectoryPredictor` **reutiliza `Step`**. Una predicción distinta de la simulación es un bug.
- El golpe de pala **no es una colisión**: `ShotSolver` fija `velocity` y `spin` (shooting method sobre `Step`, 3–6 iteraciones).

## Fuerzas

- Gravedad: `g = 9.81`.
- Arrastre: `F_d = -½·ρ·C_D·A·|v|·v`, con ρ = 1.2 kg/m³ y C_D = 0.55 por defecto (rango de la literatura: 0.507–0.62).
- Magnus: `F_m = ½·ρ·C_L(S)·A·|v|²·(ω̂ × v̂)`, con `S = r·|ω|/|v|` y C_L(S) ≈ 0.057 + 0.364·S [P, revisar].
- Decaimiento de spin: exponencial con una τ configurable [P].

## Colisiones y rebote

- Colisiones **barridas** contra planos analíticos: suelo, fondos, laterales (según la variante), malla, red (altura h(x): 0,88 m en el centro y 0,92 m en los postes), postes y límite exterior. Hay un test de no-tunneling a 40 m/s.
- Impulso normal: `v_n' = -e·v_n`.
- Impulso tangencial con límite de Coulomb: `J_t = min(μ·J_n, m·|u|·α/(1+α))` y `Δω = J_t/(α·m·r)·(n × û)`, con α ≈ 2/3 (cáscara fina) [P].
- `SurfaceMaterial` por superficie: `e`, `μ` y aleatoriedad sembrada (solo para la malla). Todo son datos.

## Calibración (test obligatorio)

Soltada desde 2,54 m sobre el suelo, la pelota debe rebotar entre **1,35 y 1,45 m** (FIP 2026), lo que da e ≈ 0,73–0,76. El cristal y la malla se calibran con vídeo de referencia a 240 fps (KI-005).

## Prohibido

- `UnityEngine.Physics`, `Rigidbody` o `Time` en este ensamblado.
- `UnityEngine.Random`: usar el `Rng` del estado.
- Constantes mágicas fuera de `BallConfig`.

## Tests

TEST_PLAN §2, filas "Rebotes", "ShotSolver", "Determinismo" y "Ball state".
