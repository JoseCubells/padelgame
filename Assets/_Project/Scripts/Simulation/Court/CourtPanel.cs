using Padel.Core;

namespace Padel.Simulation.Court
{
    /// <summary>Horizontal axis a wall panel is perpendicular to.</summary>
    public enum PanelAxis
    {
        /// <summary>Side walls: plane x = constant.</summary>
        X = 0,
        /// <summary>Back walls: plane z = constant.</summary>
        Z = 1,
    }

    /// <summary>
    /// Axis-aligned rectangular wall panel (glass or mesh). <see cref="Min"/>/<see cref="Max"/> span the other
    /// horizontal axis (z for side walls, x for back walls).
    /// </summary>
    public readonly struct CourtPanel
    {
        public readonly PanelAxis Axis;
        public readonly float PlaneCoord;
        public readonly float Min;
        public readonly float Max;
        public readonly float MinY;
        public readonly float MaxY;
        public readonly SurfaceKind Surface;

        public CourtPanel(PanelAxis axis, float planeCoord, float min, float max, float minY, float maxY, SurfaceKind surface)
        {
            Axis = axis;
            PlaneCoord = planeCoord;
            Min = min;
            Max = max;
            MinY = minY;
            MaxY = maxY;
            Surface = surface;
        }

        /// <summary>Unit normal pointing into the court.</summary>
        public Vec3 InwardNormal => Axis == PanelAxis.X
            ? new Vec3(PlaneCoord > 0f ? -1f : 1f, 0f, 0f)
            : new Vec3(0f, 0f, PlaneCoord > 0f ? -1f : 1f);

        /// <summary>True if the point lies within the panel rectangle (ignoring the distance to the plane).</summary>
        public bool ContainsProjected(Vec3 p)
        {
            float along = Axis == PanelAxis.X ? p.Z : p.X;
            return along >= Min && along <= Max && p.Y >= MinY && p.Y <= MaxY;
        }

        public override string ToString() => $"{Surface} {Axis}={PlaneCoord} [{Min},{Max}] y[{MinY},{MaxY}]";
    }
}
