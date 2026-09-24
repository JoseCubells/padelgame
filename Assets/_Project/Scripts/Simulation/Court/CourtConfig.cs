using System;

namespace Padel.Simulation.Court
{
    /// <summary>
    /// Immutable court dimensions in metres. Defaults are the FIP 2026 values
    /// (docs/research/PADEL_RULES_RESEARCH.md §9). Origin at net centre, X across, Y up, Z along the court.
    /// </summary>
    public sealed class CourtConfig
    {
        public float Length { get; }
        public float Width { get; }
        public float BackWallSolidHeight { get; }
        /// <summary>Total height of walls including the top mesh (back walls and outer side sections).</summary>
        public float WallTotalHeight { get; }
        /// <summary>Height of the side mesh along the central part of the side walls. FIP: 3 m (partially NV).</summary>
        public float SideMeshHeight { get; }
        public SideWallVariant SideWalls { get; }
        public float NetHeightCenter { get; }
        public float NetHeightPosts { get; }
        public float ServiceLineFromNet { get; }
        public float LineWidth { get; }
        /// <summary>Depth from the net where the Net zone ends [P] (GAMEPLAY_SPEC §4).</summary>
        public float NetZoneDepth { get; }
        /// <summary>Depth from the net where the Transition zone ends [P].</summary>
        public float TransitionZoneDepth { get; }

        public float HalfLength => Length * 0.5f;
        public float HalfWidth => Width * 0.5f;

        public CourtConfig(
            float length = 20f,
            float width = 10f,
            float backWallSolidHeight = 3f,
            float wallTotalHeight = 4f,
            float sideMeshHeight = 3f,
            SideWallVariant sideWalls = SideWallVariant.FullGlass,
            float netHeightCenter = 0.88f,
            float netHeightPosts = 0.92f,
            float serviceLineFromNet = 6.95f,
            float lineWidth = 0.05f,
            float netZoneDepth = 4f,
            float transitionZoneDepth = 7f)
        {
            if (length <= 0f) throw new ArgumentOutOfRangeException(nameof(length));
            if (width <= 0f) throw new ArgumentOutOfRangeException(nameof(width));
            if (backWallSolidHeight <= 0f || backWallSolidHeight > wallTotalHeight) throw new ArgumentOutOfRangeException(nameof(backWallSolidHeight));
            if (sideMeshHeight <= 0f || sideMeshHeight > wallTotalHeight) throw new ArgumentOutOfRangeException(nameof(sideMeshHeight));
            if (netHeightCenter <= 0f || netHeightPosts < netHeightCenter) throw new ArgumentOutOfRangeException(nameof(netHeightPosts));
            if (serviceLineFromNet <= 0f || serviceLineFromNet >= length * 0.5f) throw new ArgumentOutOfRangeException(nameof(serviceLineFromNet));
            if (netZoneDepth <= 0f || transitionZoneDepth <= netZoneDepth) throw new ArgumentOutOfRangeException(nameof(transitionZoneDepth));

            Length = length;
            Width = width;
            BackWallSolidHeight = backWallSolidHeight;
            WallTotalHeight = wallTotalHeight;
            SideMeshHeight = sideMeshHeight;
            SideWalls = sideWalls;
            NetHeightCenter = netHeightCenter;
            NetHeightPosts = netHeightPosts;
            ServiceLineFromNet = serviceLineFromNet;
            LineWidth = lineWidth;
            NetZoneDepth = netZoneDepth;
            TransitionZoneDepth = transitionZoneDepth;
        }

        /// <summary>The official FIP 2026 court with full-glass side walls.</summary>
        public static CourtConfig Fip2026() => new CourtConfig();
    }
}
