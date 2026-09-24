namespace Padel.Simulation.Match
{
    /// <summary>Flow of one point (GAMEPLAY_SPEC §6).</summary>
    public enum PointPhase
    {
        /// <summary>Server holds the ball; waits for the serve input.</summary>
        PreServe = 0,
        /// <summary>Ball tossed, bounce and serve contact pending.</summary>
        ServeToss = 1,
        Rally = 2,
        /// <summary>Short pause after a fault or let, then back to PreServe.</summary>
        ServePause = 3,
        /// <summary>Point decided; players are reset after a pause.</summary>
        PointOver = 4,
        MatchOver = 5,
    }
}
