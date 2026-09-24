using System;
using System.Collections.Generic;
using Padel.Core;
using Padel.Rules;

namespace Padel.Simulation.Court
{
    /// <summary>
    /// Collision and query geometry built from a <see cref="CourtConfig"/>. Used by the ball simulation, the rules,
    /// the AI and (in Unity) the graybox generator, so simulation and view share one source of truth.
    ///
    /// MVP simplifications, documented in docs/art/COURT_BIBLE.md: doors are treated as closed mesh (no outside play),
    /// posts are not separate colliders, the net top follows a straight line from centre to posts [P].
    /// </summary>
    public sealed class CourtGeometry
    {
        // FIP 2026 side-wall section lengths (PADEL_RULES_RESEARCH 2.3-2.4).
        private const float EndGlassLength = 4f;
        private const float StepLength = 2f;
        private const float SteppedLowGlassHeight = 2f;

        private readonly List<CourtPanel> _panels = new List<CourtPanel>();

        public CourtConfig Config { get; }
        public IReadOnlyList<CourtPanel> Panels => _panels;

        public CourtGeometry(CourtConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            BuildBackWalls();
            BuildSideWalls();
        }

        /// <summary>Height of the top of the net at lateral position x.</summary>
        public float NetHeightAt(float x)
        {
            float t = Math.Min(Math.Abs(x) / Config.HalfWidth, 1f);
            return Config.NetHeightCenter + (Config.NetHeightPosts - Config.NetHeightCenter) * t;
        }

        public bool IsInsideFootprint(float x, float z) =>
            Math.Abs(x) <= Config.HalfWidth && Math.Abs(z) <= Config.HalfLength;

        /// <summary>-1 for the negative-Z half, +1 for the positive-Z half (the net line counts as positive).</summary>
        public static int HalfSign(float z) => z < 0f ? -1 : 1;

        public CourtZone ZoneOf(float z)
        {
            float depth = Math.Abs(z);
            if (depth < Config.NetZoneDepth) return CourtZone.Net;
            if (depth < Config.TransitionZoneDepth) return CourtZone.Transition;
            return CourtZone.Back;
        }

        /// <summary>
        /// Target service box for a server on half <paramref name="serverHalfSign"/> serving from <paramref name="side"/>.
        /// A server on the -Z half faces +Z, so their right is +X; the serve goes diagonally (FIP S4).
        /// Returned as (minX, maxX, minZ, maxZ) without line tolerance.
        /// </summary>
        public (float minX, float maxX, float minZ, float maxZ) ServiceBox(int serverHalfSign, ServeSide side)
        {
            int receiverSign = -serverHalfSign;
            // Server's right is x-sign -serverHalfSign; diagonal target is the opposite x-sign.
            int targetXSign = side == ServeSide.Right ? serverHalfSign : -serverHalfSign;
            float minX = targetXSign > 0 ? 0f : -Config.HalfWidth;
            float maxX = targetXSign > 0 ? Config.HalfWidth : 0f;
            float minZ = receiverSign > 0 ? 0f : -Config.ServiceLineFromNet;
            float maxZ = receiverSign > 0 ? Config.ServiceLineFromNet : 0f;
            return (minX, maxX, minZ, maxZ);
        }

        /// <summary>
        /// True if a bounce at (x, z) is inside the target service box. Lines are good (FIP S4): the box is widened by
        /// half a line width, since whether 6.95 m is measured to the line edge or axis is NV (PADEL_RULES 2.12) [P].
        /// </summary>
        public bool IsInServiceBox(float x, float z, int serverHalfSign, ServeSide side)
        {
            var box = ServiceBox(serverHalfSign, side);
            float tol = Config.LineWidth * 0.5f;
            return x >= box.minX - tol && x <= box.maxX + tol && z >= box.minZ - tol && z <= box.maxZ + tol;
        }

        /// <summary>Server x-sign for a server on half <paramref name="serverHalfSign"/> serving from <paramref name="side"/>.</summary>
        public static int ServerXSign(int serverHalfSign, ServeSide side) =>
            side == ServeSide.Right ? -serverHalfSign : serverHalfSign;

        private void BuildBackWalls()
        {
            float hw = Config.HalfWidth;
            foreach (float z in new[] { -Config.HalfLength, Config.HalfLength })
            {
                _panels.Add(new CourtPanel(PanelAxis.Z, z, -hw, hw, 0f, Config.BackWallSolidHeight, SurfaceKind.Glass));
                _panels.Add(new CourtPanel(PanelAxis.Z, z, -hw, hw, Config.BackWallSolidHeight, Config.WallTotalHeight, SurfaceKind.Mesh));
            }
        }

        private void BuildSideWalls()
        {
            float hl = Config.HalfLength;
            float glassStart = hl - EndGlassLength; // 6 m from the net on a 20 m court
            foreach (float x in new[] { -Config.HalfWidth, Config.HalfWidth })
            {
                // Central mesh section.
                AddSide(x, -glassStart, glassStart, 0f, Config.SideMeshHeight, SurfaceKind.Mesh);

                foreach (int s in new[] { -1, 1 })
                {
                    float outerStart = hl - StepLength; // 8 m
                    (float a, float b) outer = Span(s, outerStart, hl);
                    (float a, float b) inner = Span(s, glassStart, outerStart);

                    if (Config.SideWalls == SideWallVariant.FullGlass)
                    {
                        AddSide(x, inner.a, inner.b, 0f, Config.BackWallSolidHeight, SurfaceKind.Glass);
                        AddSide(x, outer.a, outer.b, 0f, Config.BackWallSolidHeight, SurfaceKind.Glass);
                    }
                    else
                    {
                        AddSide(x, inner.a, inner.b, 0f, SteppedLowGlassHeight, SurfaceKind.Glass);
                        AddSide(x, inner.a, inner.b, SteppedLowGlassHeight, Config.SideMeshHeight, SurfaceKind.Mesh);
                        AddSide(x, outer.a, outer.b, 0f, Config.BackWallSolidHeight, SurfaceKind.Glass);
                    }
                    // Outer 2 m: mesh up to the full wall height, continuous with the back wall.
                    AddSide(x, outer.a, outer.b, Config.BackWallSolidHeight, Config.WallTotalHeight, SurfaceKind.Mesh);
                }
            }
        }

        private static (float a, float b) Span(int sign, float from, float to) =>
            sign > 0 ? (from, to) : (-to, -from);

        private void AddSide(float x, float zMin, float zMax, float yMin, float yMax, SurfaceKind surface)
        {
            if (yMax > yMin) _panels.Add(new CourtPanel(PanelAxis.X, x, zMin, zMax, yMin, yMax, surface));
        }
    }
}
