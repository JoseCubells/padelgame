using Padel.Core;
using Padel.Rules;
using Padel.Simulation.Ball;
using Padel.Simulation.Shots;

namespace Padel.Simulation.Match
{
    public enum MatchEventKind
    {
        Ball = 0,
        Stroke = 1,
        Whiff = 2,
        SplitStep = 3,
        Call = 4,
        Score = 5,
        ServeTossed = 6,
        PointStarted = 7,
    }

    /// <summary>
    /// Per-tick output of <see cref="MatchSimulation.Step"/> for presentation (audio, VFX, HUD, animation) and tests.
    /// It is a list returned per step, not a global event bus (ARCHITECTURE §4).
    /// </summary>
    public struct MatchEvent
    {
        public MatchEventKind Kind;
        public long Tick;
        public int Player;
        public BallEvent BallEvent;
        public ShotType Shot;
        public ShotQuality Quality;
        public RefereeCall Call;
        public ScoreChange Score;
        public Vec3 Position;

        public override string ToString()
        {
            switch (Kind)
            {
                case MatchEventKind.Ball: return $"[{Tick}] ball {BallEvent}";
                case MatchEventKind.Stroke: return $"[{Tick}] P{Player} {Shot} {Quality}";
                case MatchEventKind.Call: return $"[{Tick}] call {Call}";
                default: return $"[{Tick}] {Kind} P{Player}";
            }
        }
    }
}
