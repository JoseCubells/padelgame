namespace Padel.Simulation.Players
{
    /// <summary>Movement/stroke phase of a player (ADR-005: animation reads it, never drives it).</summary>
    public enum PlayerPhase
    {
        Free = 0,
        Windup = 1,
        Contact = 2,
        Recovery = 3,
    }
}
