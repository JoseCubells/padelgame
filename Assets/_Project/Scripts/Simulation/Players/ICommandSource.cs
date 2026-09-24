using Padel.Simulation.Match;

namespace Padel.Simulation.Players
{
    /// <summary>
    /// Anything that controls a player: human input adapter, AI brain, scripted test or replay, future network source
    /// (ADR-007). The simulation only ever sees the returned <see cref="PlayerCommand"/>.
    /// </summary>
    public interface ICommandSource
    {
        PlayerCommand GetCommand(MatchState state, int player);
    }
}
