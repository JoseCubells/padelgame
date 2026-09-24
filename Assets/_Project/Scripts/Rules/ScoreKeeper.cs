using System;
using Padel.Core;

namespace Padel.Rules
{
    /// <summary>
    /// Pure scoring logic for padel (FIP 2026, docs/research/PADEL_RULES_RESEARCH.md §5.2, §5.4, §6, §10).
    /// Knows nothing about the ball: it receives "team X won the point" and updates games, sets, tie-breaks,
    /// serve rotation, serve side and ends. Decisive-point behaviour comes from <see cref="ScoringConfig.MaxAdvantages"/>.
    /// </summary>
    public static class ScoreKeeper
    {
        private const int FortyPoints = 3;

        public static ScoreState NewMatch(ScoringConfig config, MatchSetup setup)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (setup == null) throw new ArgumentNullException(nameof(setup));

            var state = new ScoreState { PlayersPerTeam = setup.PlayersPerTeam };
            TeamId first = setup.FirstServingTeam;
            TeamId second = first.Other();

            // Serve order kept for the whole match: first team, other team, first team's partner, other partner (FIP O2).
            state.ServeOrder = setup.PlayersPerTeam == 1
                ? new[] { new PlayerSlot(first, 0), new PlayerSlot(second, 0) }
                : new[]
                {
                    new PlayerSlot(first, setup.FirstServer(first)),
                    new PlayerSlot(second, setup.FirstServer(second)),
                    new PlayerSlot(first, 1 - setup.FirstServer(first)),
                    new PlayerSlot(second, 1 - setup.FirstServer(second)),
                };
            state.RightReceiver[(int)TeamId.A] = setup.RightReceiverA;
            state.RightReceiver[(int)TeamId.B] = setup.RightReceiverB;
            StartSet(state, config);
            return state;
        }

        /// <summary>Deuce number of the current game: 0 when not at deuce, 1 at 40-40, 2 after the first lost advantage...</summary>
        public static int DeuceNumber(ScoreState state)
        {
            if (state.InTiebreak) return 0;
            int a = state.Points[0];
            int b = state.Points[1];
            return a == b && a >= FortyPoints ? a - FortyPoints + 1 : 0;
        }

        /// <summary>
        /// True when the next point decides the game: golden point (MaxAdvantages = 0) at the first deuce,
        /// Star Point (MaxAdvantages = 2) at the third deuce. Never in classic scoring or in tie-breaks.
        /// </summary>
        public static bool IsDecisivePoint(ScoreState state, ScoringConfig config)
        {
            if (state.IsOver || config.MaxAdvantages == ScoringConfig.UnlimitedAdvantages) return false;
            int deuce = DeuceNumber(state);
            return deuce > 0 && deuce > config.MaxAdvantages;
        }

        public static PlayerSlot CurrentServer(ScoreState state)
        {
            int n = state.ServeOrder.Length;
            if (!state.InTiebreak) return state.ServeOrder[state.ServerOrderIndex % n];

            // Tie-break (FIP P7): first point by the due server, then blocks of two points per server in order.
            int k = state.PointsPlayedInGame;
            int offset = k == 0 ? 0 : 1 + (k - 1) / 2;
            return state.ServeOrder[(state.ServerOrderIndex + offset) % n];
        }

        public static ServeSide CurrentServeSide(ScoreState state, ScoringConfig config)
        {
            if (IsDecisivePoint(state, config)) return state.DecisivePointSide;
            // Games start from the right and alternate (FIP S4). In tie-breaks the same parity rule yields
            // "first point from the right, each new server starts from the left" (FIP P7).
            return state.PointsPlayedInGame % 2 == 0 ? ServeSide.Right : ServeSide.Left;
        }

        public static PlayerSlot CurrentReceiver(ScoreState state, ScoringConfig config)
        {
            TeamId receivingTeam = CurrentServer(state).Team.Other();
            if (state.PlayersPerTeam == 1) return new PlayerSlot(receivingTeam, 0);

            // Each receiver keeps its side for the set (FIP O3); on a decisive point the pair picks the side
            // but cannot swap positions (FIP O4), so the player on the chosen side receives.
            int rightReceiver = state.RightReceiver[(int)receivingTeam];
            int index = CurrentServeSide(state, config) == ServeSide.Right ? rightReceiver : 1 - rightReceiver;
            return new PlayerSlot(receivingTeam, index);
        }

        /// <summary>The receiving team chooses the side for the decisive point (FIP O4).</summary>
        public static void ChooseDecisivePointSide(ScoreState state, ScoringConfig config, ServeSide side)
        {
            if (!IsDecisivePoint(state, config))
                throw new InvalidOperationException("The receiving side can only be chosen on a decisive point.");
            state.DecisivePointSide = side;
        }

        public static ScoreChange AwardPoint(ScoreState state, ScoringConfig config, TeamId winner)
        {
            if (state.IsOver) throw new InvalidOperationException("The match is already over.");

            var change = new ScoreChange { PointWinner = winner };
            int w = (int)winner;
            int l = 1 - w;

            if (state.InTiebreak)
            {
                state.Points[w]++;
                int target = state.IsSuperTiebreak ? config.SuperTiebreakPoints : config.TiebreakPoints;
                if (state.Points[w] >= target && state.Points[w] - state.Points[l] >= 2)
                {
                    CompleteGame(state, config, winner, ref change);
                }
                else if (state.PointsPlayedInGame % config.TiebreakChangeEndsEvery == 0)
                {
                    SwapEnds(state, ref change);
                }
                return change;
            }

            bool decisive = IsDecisivePoint(state, config);
            state.Points[w]++;
            if (decisive || (state.Points[w] > FortyPoints && state.Points[w] - state.Points[l] >= 2))
            {
                CompleteGame(state, config, winner, ref change);
            }
            else
            {
                change.DecisivePointNext = IsDecisivePoint(state, config);
            }
            return change;
        }

        private static void CompleteGame(ScoreState state, ScoringConfig config, TeamId winner, ref ScoreChange change)
        {
            int w = (int)winner;
            int l = 1 - w;
            bool wasTiebreak = state.InTiebreak;
            var tiebreakPoints = (a: state.Points[0], b: state.Points[1]);

            state.Games[w]++;
            state.GamesPlayedInSet++;
            change.GameWinner = winner;

            // A tie-break counts as one game served by its first server; the rotation simply continues, so the
            // next set is opened by the team that received first in the tie-break. [P] convention, FIP P10 is NV.
            state.ServerOrderIndex = (state.ServerOrderIndex + 1) % state.ServeOrder.Length;
            state.Points[0] = 0;
            state.Points[1] = 0;
            state.DecisivePointSide = ServeSide.Right;
            state.InTiebreak = false;

            // Change ends after every odd game of the set (FIP P9); a tie-break is game 13, so ends change after it.
            if (state.GamesPlayedInSet % 2 == 1) SwapEnds(state, ref change);

            bool finalSet = IsFinalSet(state, config);
            int gw = state.Games[w];
            int gl = state.Games[l];
            bool setWon = wasTiebreak || (gw >= config.GamesPerSet && gw - gl >= 2);

            if (setWon)
            {
                CompleteSet(state, config, winner, wasTiebreak ? tiebreakPoints : default, ref change);
                return;
            }

            bool tiebreakAllowed = !(finalSet && config.FinalSet == FinalSetMode.AdvantageSet);
            if (tiebreakAllowed && gw == config.GamesPerSet && gl == config.GamesPerSet)
            {
                state.InTiebreak = true;
                change.TiebreakStarted = true;
            }
        }

        private static void CompleteSet(ScoreState state, ScoringConfig config, TeamId winner, (int a, int b) tiebreakPoints, ref ScoreChange change)
        {
            state.CompletedSetList.Add(new SetResult
            {
                GamesA = state.Games[0],
                GamesB = state.Games[1],
                TiebreakPointsA = tiebreakPoints.a,
                TiebreakPointsB = tiebreakPoints.b,
                WasSuperTiebreak = state.IsSuperTiebreak,
            });
            state.Sets[(int)winner]++;
            change.SetWinner = winner;

            if (state.Sets[(int)winner] >= config.SetsToWin)
            {
                state.Winner = winner;
                change.MatchWinner = winner;
                return;
            }

            StartSet(state, config);
            if (state.InTiebreak) change.TiebreakStarted = true;
        }

        private static void StartSet(ScoreState state, ScoringConfig config)
        {
            state.Games[0] = 0;
            state.Games[1] = 0;
            state.Points[0] = 0;
            state.Points[1] = 0;
            state.GamesPlayedInSet = 0;
            state.DecisivePointSide = ServeSide.Right;
            state.IsSuperTiebreak = IsFinalSet(state, config) && config.FinalSet == FinalSetMode.SuperTiebreak;
            state.InTiebreak = state.IsSuperTiebreak;
        }

        private static bool IsFinalSet(ScoreState state, ScoringConfig config) =>
            state.Sets[0] == config.SetsToWin - 1 && state.Sets[1] == config.SetsToWin - 1;

        private static void SwapEnds(ScoreState state, ref ScoreChange change)
        {
            state.EndsSwapped = !state.EndsSwapped;
            change.ChangeEnds = true;
        }
    }
}
