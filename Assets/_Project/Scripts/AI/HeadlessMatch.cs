using System;
using System.Collections.Generic;
using Padel.Core;
using Padel.Rules;
using Padel.Simulation.Match;
using Padel.Simulation.Players;

namespace Padel.AI
{
    /// <summary>Aggregated outcome of a headless run, used for AI tuning and regression tests (ADR-006).</summary>
    public sealed class MatchStats
    {
        public int Points;
        public int PointsWonA;
        public int PointsWonB;
        public int Strokes;
        public int Faults;
        public int Lets;
        public int TimeoutLets;
        public int Whiffs;
        public long Ticks;
        public TeamId? Winner;
        public readonly Dictionary<PointReason, int> Reasons = new Dictionary<PointReason, int>();
        public readonly List<int> StrokesPerPoint = new List<int>();

        public double MeanStrokesPerPoint => Points == 0 ? 0 : (double)Strokes / Points;

        public int MedianStrokesPerPoint
        {
            get
            {
                if (StrokesPerPoint.Count == 0) return 0;
                var sorted = new List<int>(StrokesPerPoint);
                sorted.Sort();
                return sorted[sorted.Count / 2];
            }
        }

        public override string ToString()
        {
            var parts = new List<string>();
            foreach (var kv in Reasons) parts.Add($"{kv.Key}:{kv.Value}");
            return $"points {Points} (A {PointsWonA} / B {PointsWonB}), strokes/point mean {MeanStrokesPerPoint:0.00} median {MedianStrokesPerPoint}, " +
                   $"faults {Faults}, lets {Lets} (timeouts {TimeoutLets}), whiffs {Whiffs}, winner {Winner?.ToString() ?? "-"}, reasons [{string.Join(", ", parts)}]";
        }
    }

    /// <summary>Runs a match without Unity: every player driven by an <see cref="ICommandSource"/>.</summary>
    public static class HeadlessMatch
    {
        public static MatchStats Run(MatchSimulation sim, MatchState state, ICommandSource[] controllers, int maxPoints, long maxTicks)
        {
            var stats = new MatchStats();
            var commands = new PlayerCommand[state.Players.Length];
            var events = new List<MatchEvent>(32);
            int strokesThisPoint = 0;

            for (long t = 0; t < maxTicks && stats.Points < maxPoints && state.Phase != PointPhase.MatchOver; t++)
            {
                for (int p = 0; p < commands.Length; p++) commands[p] = controllers[p].GetCommand(state, p);
                events.Clear();
                sim.Step(state, commands, events);
                stats.Ticks++;

                foreach (MatchEvent e in events)
                {
                    switch (e.Kind)
                    {
                        case MatchEventKind.Stroke:
                            stats.Strokes++;
                            strokesThisPoint++;
                            break;
                        case MatchEventKind.Whiff:
                            stats.Whiffs++;
                            break;
                        case MatchEventKind.Call when e.Call.Kind == CallKind.Fault:
                            stats.Faults++;
                            break;
                        case MatchEventKind.Call when e.Call.Kind == CallKind.Let:
                            stats.Lets++;
                            if (e.Call.Reason == PointReason.RallyTimeout) stats.TimeoutLets++;
                            break;
                        case MatchEventKind.Score:
                            stats.Points++;
                            if (e.Call.Winner == TeamId.A) stats.PointsWonA++; else stats.PointsWonB++;
                            stats.Reasons.TryGetValue(e.Call.Reason, out int n);
                            stats.Reasons[e.Call.Reason] = n + 1;
                            stats.StrokesPerPoint.Add(strokesThisPoint);
                            strokesThisPoint = 0;
                            if (e.Score.MatchWinner.HasValue) stats.Winner = e.Score.MatchWinner;
                            break;
                    }
                }
            }
            return stats;
        }

        /// <summary>Convenience: all players AI with the given profiles per team.</summary>
        public static ICommandSource[] Bots(MatchSimulation sim, AIProfile teamA, AIProfile teamB, ulong seed)
        {
            int n = sim.Config.PlayerCount;
            int perTeam = sim.Config.Setup.PlayersPerTeam;
            var brains = new ICommandSource[n];
            var a = new TeamBrain(sim, TeamId.A);
            var b = new TeamBrain(sim, TeamId.B);
            for (int p = 0; p < n; p++)
            {
                bool isA = p < perTeam;
                brains[p] = new PlayerBrain(sim, isA ? a : b, isA ? teamA : teamB, seed * 31UL + (ulong)p);
            }
            return brains;
        }
    }
}
