namespace Padel.Simulation.Court
{
    /// <summary>FIP 2026 side-wall layouts (docs/research/PADEL_RULES_RESEARCH.md 2.3-2.4).</summary>
    public enum SideWallVariant
    {
        /// <summary>Variant 2: 3 m x 4 m glass at each end, mesh to 4 m on the outer 2 m. Default (COURT_BIBLE).</summary>
        FullGlass = 0,
        /// <summary>Variant 1: stepped glass (3 m high x 2 m, then 2 m high x 2 m) with mesh completing the enclosure.</summary>
        Stepped = 1,
    }
}
