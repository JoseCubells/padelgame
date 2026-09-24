using System;
using System.Collections.Generic;
using Padel.Core;
using Padel.Simulation.Court;

namespace Padel.Simulation.Ball
{
    /// <summary>
    /// Deterministic padel ball integrator (ADR-004). Pure function of (state, config, geometry, dt):
    /// no hidden state, no engine physics. <see cref="TrajectoryPredictor"/> reuses <see cref="Step"/>, so AI
    /// predictions match the simulation exactly.
    ///
    /// Per step: semi-implicit Euler velocity update (gravity + drag + Magnus), then a swept move against the court
    /// (floor, wall panels, net) resolving up to <see cref="MaxContactsPerStep"/> contacts with a friction-impulse
    /// bounce that couples spin and tangential velocity (GAMEPLAY_RESEARCH §2.4).
    /// </summary>
    public static class BallSimulator
    {
        public const float DefaultTimeStep = 1f / 120f;
        private const int MaxContactsPerStep = 4;
        private const float Epsilon = 1e-6f;

        public static void Step(ref BallState state, BallConfig config, CourtGeometry court, float dt, List<BallEvent> events)
        {
            state.Velocity += Acceleration(state, config) * dt;
            Move(ref state, config, court, dt, events);
            state.Spin *= MathF.Exp(-dt / config.SpinDecayTime);
        }

        /// <summary>Gravity + drag + Magnus acceleration (GAMEPLAY_RESEARCH §2.3).</summary>
        public static Vec3 Acceleration(in BallState state, BallConfig config)
        {
            Vec3 a = new Vec3(0f, -config.Gravity, 0f);
            float speed = state.Velocity.Magnitude;
            if (speed < Epsilon) return a;

            float k = config.AeroFactor;
            a -= state.Velocity * (k * config.DragCoefficient * speed);

            float spinRate = state.Spin.Magnitude;
            if (spinRate > Epsilon)
            {
                float s = config.Radius * spinRate / speed;
                float cl = config.LiftIntercept + config.LiftSlope * s;
                cl = cl < 0f ? 0f : (cl > config.LiftMax ? config.LiftMax : cl);
                Vec3 spinAxis = state.Spin / spinRate;
                a += Vec3.Cross(spinAxis, state.Velocity) * (k * cl * speed);
            }
            return a;
        }

        private static void Move(ref BallState state, BallConfig config, CourtGeometry court, float dt, List<BallEvent> events)
        {
            float remaining = dt;
            for (int i = 0; i < MaxContactsPerStep && remaining > Epsilon; i++)
            {
                Hit hit = FindFirstHit(state, config, court, remaining);
                Vec3 before = state.Position;

                if (!hit.Found)
                {
                    state.Position += state.Velocity * remaining;
                    EmitCrossings(before, state.Position, court, config, events);
                    return;
                }

                state.Position += state.Velocity * hit.Time;
                EmitCrossings(before, state.Position, court, config, events);
                Bounce(ref state, config, hit.Normal, config.Material(hit.Surface), out float impactSpeed);
                // A ball resting on the floor touches it every step; only report real impacts.
                bool restingContact = hit.Surface == SurfaceKind.Floor && impactSpeed < config.Gravity * dt * 1.5f;
                if (!restingContact) events?.Add(new BallEvent
                {
                    Kind = BallEventKind.Contact,
                    Surface = hit.Surface,
                    Position = state.Position,
                    Normal = hit.Normal,
                    ImpactSpeed = impactSpeed,
                });
                remaining -= hit.Time;
            }
            // Contact budget exhausted (corner): finish the step without further collision checks.
            if (remaining > Epsilon) state.Position += state.Velocity * remaining;
        }

        private struct Hit
        {
            public bool Found;
            public float Time;
            public Vec3 Normal;
            public SurfaceKind Surface;
        }

        private static Hit FindFirstHit(in BallState s, BallConfig config, CourtGeometry court, float maxTime)
        {
            var best = new Hit { Found = false, Time = maxTime };
            float r = config.Radius;
            Vec3 p = s.Position;
            Vec3 v = s.Velocity;

            // Floor (y = 0).
            if (v.Y < 0f)
            {
                float t = (p.Y - r) / -v.Y;
                if (t < 0f) t = 0f;
                if (t <= best.Time) best = new Hit { Found = true, Time = t, Normal = Vec3.Up, Surface = SurfaceKind.Floor };
            }

            // Wall panels: only while inside the enclosure and moving towards the panel.
            IReadOnlyList<CourtPanel> panels = court.Panels;
            for (int i = 0; i < panels.Count; i++)
            {
                CourtPanel panel = panels[i];
                Vec3 n = panel.InwardNormal;
                float vn = Vec3.Dot(v, n);
                if (vn >= 0f) continue;
                float dist = panel.Axis == PanelAxis.X ? (p.X - panel.PlaneCoord) * n.X : (p.Z - panel.PlaneCoord) * n.Z;
                if (dist < -Epsilon) continue; // already outside this wall
                float t = (dist - r) / -vn;
                if (t < 0f) t = 0f;
                if (t > best.Time) continue;
                Vec3 at = p + v * t;
                if (!panel.ContainsProjected(at)) continue;
                best = new Hit { Found = true, Time = t, Normal = n, Surface = panel.Surface };
            }

            // Net: thin vertical sheet at z = 0 from the floor to NetHeightAt(x).
            if (Math.Abs(v.Z) > Epsilon)
            {
                float side = p.Z >= 0f ? 1f : -1f;
                float dist = p.Z * side;
                if (v.Z * side < 0f && dist >= -Epsilon)
                {
                    float t = (dist - r) / Math.Abs(v.Z);
                    if (t < 0f) t = 0f;
                    if (t <= best.Time)
                    {
                        Vec3 at = p + v * t;
                        if (Math.Abs(at.X) <= court.Config.HalfWidth && at.Y < court.NetHeightAt(at.X))
                        {
                            best = new Hit { Found = true, Time = t, Normal = new Vec3(0f, 0f, side), Surface = SurfaceKind.Net };
                        }
                    }
                }
            }
            return best;
        }

        private static void EmitCrossings(Vec3 from, Vec3 to, CourtGeometry court, BallConfig config, List<BallEvent> events)
        {
            if (events == null) return;

            if ((from.Z < 0f) != (to.Z < 0f))
            {
                float t = from.Z / (from.Z - to.Z);
                Vec3 at = Vec3.Lerp(from, to, t);
                events.Add(new BallEvent { Kind = BallEventKind.CrossedNet, Position = at });
            }

            bool wasInside = court.IsInsideFootprint(from.X, from.Z);
            bool isInside = court.IsInsideFootprint(to.X, to.Z);
            if (wasInside && !isInside)
            {
                events.Add(new BallEvent { Kind = BallEventKind.LeftCourt, Position = to });
            }
        }

        /// <summary>
        /// Friction-impulse bounce (rigid, Coulomb): normal restitution plus a tangential impulse limited by friction
        /// that drives the contact point towards rolling ("grip"), transferring between spin and velocity.
        /// </summary>
        private static void Bounce(ref BallState s, BallConfig config, Vec3 normal, SurfaceMaterial material, out float impactSpeed)
        {
            Vec3 n = normal;
            if (material.NormalJitterDegrees > 0f)
            {
                n = JitterNormal(n, s.Position, material.NormalJitterDegrees);
            }

            float vn = Vec3.Dot(s.Velocity, n);
            impactSpeed = vn < 0f ? -vn : 0f;
            if (vn >= 0f) return;

            float m = config.Mass;
            float r = config.Radius;
            float alpha = config.InertiaFactor;
            float e = material.RestitutionAt(impactSpeed);

            Vec3 vNormal = n * vn;
            Vec3 vTangent = s.Velocity - vNormal;
            Vec3 contactVelocity = vTangent + Vec3.Cross(s.Spin, n * -r);

            float newVn = -e * vn;
            if (newVn < config.MinBounceSpeed && n.Y > 0.5f) newVn = 0f; // settle on the floor

            float jn = m * (1f + e) * impactSpeed;
            float slip = contactVelocity.Magnitude;
            if (slip > Epsilon)
            {
                Vec3 slipDir = contactVelocity / slip;
                float jGrip = m * slip * alpha / (1f + alpha);
                float jt = Math.Min(material.Friction * jn, jGrip);
                vTangent -= slipDir * (jt / m);
                s.Spin += Vec3.Cross(n, slipDir) * (jt / (alpha * m * r));
            }

            s.Velocity = vTangent + n * newVn;
        }

        /// <summary>
        /// Tilts the normal pseudo-randomly as a function of the impact point only (quantised to 1 mm), so irregular
        /// mesh bounces are deterministic and predictable by <see cref="TrajectoryPredictor"/> regardless of how many
        /// random numbers other systems consumed.
        /// </summary>
        private static Vec3 JitterNormal(Vec3 n, Vec3 at, float degrees)
        {
            ulong key = unchecked(
                (ulong)(long)MathF.Round(at.X * 1000f) * 73856093UL ^
                (ulong)(long)MathF.Round(at.Y * 1000f) * 19349663UL ^
                (ulong)(long)MathF.Round(at.Z * 1000f) * 83492791UL);
            var rng = new Pcg32(key);
            // Two tangent axes of the (axis-aligned) normal.
            Vec3 t1 = Math.Abs(n.Y) > 0.5f ? new Vec3(1f, 0f, 0f) : new Vec3(0f, 1f, 0f);
            Vec3 t2 = Vec3.Cross(n, t1);
            float maxTan = MathF.Tan(degrees * (MathF.PI / 180f));
            float a = rng.Range(-maxTan, maxTan);
            float b = rng.Range(-maxTan, maxTan);
            return (n + t1 * a + t2 * b).Normalized;
        }
    }
}
