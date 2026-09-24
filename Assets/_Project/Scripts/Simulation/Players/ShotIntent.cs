namespace Padel.Simulation.Players
{
    /// <summary>Shot family chosen by the player (ADR-007). The context decides the concrete technique.</summary>
    public enum ShotIntent
    {
        None = 0,
        /// <summary>Flat/topspin; overhead: víbora or smash.</summary>
        Attack = 1,
        /// <summary>Slice; overhead: bandeja.</summary>
        Control = 2,
        Lob = 3,
    }
}
