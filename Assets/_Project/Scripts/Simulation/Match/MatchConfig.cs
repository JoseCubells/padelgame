using System;
using Padel.Rules;
using Padel.Simulation.Ball;
using Padel.Simulation.Court;
using Padel.Simulation.Players;
using Padel.Simulation.Shots;

namespace Padel.Simulation.Match
{
    /// <summary>
    /// Immutable configuration of a match. One model for 1v1 and 2v2: <see cref="Setup"/> decides players per team
    /// (ARCHITECTURE §3). In Unity it is assembled from ScriptableObject definitions.
    /// </summary>
    public sealed class MatchConfig
    {
        public CourtGeometry Court { get; }
        public BallConfig Ball { get; }
        public ShotCatalog Shots { get; }
        public ScoringConfig Scoring { get; }
        public MatchSetup Setup { get; }
        public PlayerStats[] Players { get; }
        public ulong Seed { get; }
        public float TimeStep { get; }

        /// <summary>
        /// A press this much earlier than needed still swings (held until the ball arrives, graded early);
        /// earlier presses are ignored [P] (ADR-007: presses up to 400 ms early act as charge).
        /// </summary>
        public float MaxEarlyPress { get; } = 0.4f;
        /// <summary>Holding the button this long gives full charge [P].</summary>
        public float MaxChargeTime { get; } = 0.5f;
        /// <summary>Pause after a fault or let before the serve is re-armed [P].</summary>
        public float FaultPause { get; } = 0.8f;
        /// <summary>Pause after a point before players are reset (presentation shows the reason) [P].</summary>
        public float PointPause { get; } = 1.5f;
        /// <summary>A rally longer than this is replayed as a let (safety net for stuck balls) [P].</summary>
        public float RallyTimeout { get; } = 90f;
        /// <summary>Height of the serve toss release above the floor [P].</summary>
        public float ServeTossHeight { get; } = 1.0f;

        public MatchConfig(
            CourtGeometry court, BallConfig ball, ShotCatalog shots, ScoringConfig scoring, MatchSetup setup,
            PlayerStats[] players, ulong seed, float timeStep = BallSimulator.DefaultTimeStep)
        {
            Court = court ?? throw new ArgumentNullException(nameof(court));
            Ball = ball ?? throw new ArgumentNullException(nameof(ball));
            Shots = shots ?? throw new ArgumentNullException(nameof(shots));
            Scoring = scoring ?? throw new ArgumentNullException(nameof(scoring));
            Setup = setup ?? throw new ArgumentNullException(nameof(setup));
            Players = players ?? throw new ArgumentNullException(nameof(players));
            if (players.Length != setup.PlayersPerTeam * 2) throw new ArgumentException("One PlayerStats per player is required.", nameof(players));
            Seed = seed;
            TimeStep = timeStep;
        }

        public int PlayerCount => Setup.PlayersPerTeam * 2;

        /// <summary>FIP 2026 defaults with the given team size.</summary>
        public static MatchConfig Default(int playersPerTeam, ulong seed = 1UL, ScoringConfig scoring = null)
        {
            var stats = new PlayerStats[playersPerTeam * 2];
            for (int i = 0; i < stats.Length; i++) stats[i] = new PlayerStats();
            return new MatchConfig(
                new CourtGeometry(CourtConfig.Fip2026()), BallConfig.Fip2026Default(), ShotCatalog.Default(),
                scoring ?? ScoringConfig.StarPoint(), new MatchSetup(playersPerTeam), stats, seed);
        }
    }
}
