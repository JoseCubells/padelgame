using System;
using Padel.Core;
using Padel.Rules;
using Padel.Simulation.Ball;
using Padel.Simulation.Players;

namespace Padel.Simulation.Match
{
    /// <summary>
    /// Complete, serializable state of a match: <c>MatchState(t+1) = MatchSimulation.Step(MatchState(t), commands)</c>
    /// (ARCHITECTURE §1, ADR-008). Small enough to snapshot for replays and a future rollback/online mode.
    /// Player index = team * PlayersPerTeam + index in team.
    /// </summary>
    [Serializable]
    public sealed class MatchState
    {
        public long Tick;
        public Pcg32 Rng;
        public BallState Ball;
        /// <summary>The ball is simulated (false while the server holds it).</summary>
        public bool BallLive;
        public PlayerState[] Players;
        public StrokeState[] Strokes;
        public ScoreState Score;
        public PointReferee Referee;
        public PointPhase Phase;
        public float PhaseTime;
        public RefereeCall LastCall;
        public int LastHitter = -1;
        /// <summary>Technique of the last stroke and the tick it was struck (-1 before the first stroke of a point).</summary>
        public Padel.Simulation.Shots.ShotType LastShot;
        public long LastStrokeTick = -1;
        /// <summary>The ball bounced on the floor of the team that must hit next, since the last stroke.</summary>
        public bool BouncedSinceHit;
        /// <summary>After that bounce the ball came off a wall of the same half.</summary>
        public bool OffWallSinceBounce;
        public bool TossBounced;

        public int PlayersPerTeam => Score.PlayersPerTeam;
        public TeamId TeamOf(int player) => (TeamId)(player / PlayersPerTeam);
        public int IndexOf(PlayerSlot slot) => (int)slot.Team * PlayersPerTeam + slot.Index;

        /// <summary>-1 if the team currently plays on the negative-Z half, +1 otherwise.</summary>
        public int HalfSign(TeamId team)
        {
            int a = Score.EndsSwapped ? 1 : -1;
            return team == TeamId.A ? a : -a;
        }

        /// <summary>Team owning the half that contains world z.</summary>
        public TeamId TeamAtHalf(float z)
        {
            int sign = z < 0f ? -1 : 1;
            return sign == HalfSign(TeamId.A) ? TeamId.A : TeamId.B;
        }

        public MatchState Clone()
        {
            var copy = (MatchState)MemberwiseClone();
            copy.Players = (PlayerState[])Players.Clone();
            copy.Strokes = (StrokeState[])Strokes.Clone();
            copy.Score = Score.Clone();
            copy.Referee = Referee.Clone();
            return copy;
        }

        /// <summary>FNV-1a hash of the simulation-relevant state, for determinism tests and replay checks.</summary>
        public ulong ComputeHash()
        {
            ulong h = 14695981039346656037UL;
            void Mix(long v) { unchecked { h ^= (ulong)v; h *= 1099511628211UL; } }
            void MixF(float f) => Mix(BitConverter.SingleToInt32Bits(f));
            void MixV(Vec3 v) { MixF(v.X); MixF(v.Y); MixF(v.Z); }

            Mix(Tick);
            Mix((long)Rng.State);
            MixV(Ball.Position); MixV(Ball.Velocity); MixV(Ball.Spin);
            Mix(BallLive ? 1 : 0);
            Mix((long)Phase);
            MixF(PhaseTime);
            foreach (PlayerState p in Players)
            {
                MixF(p.Position.X); MixF(p.Position.Y); MixF(p.Velocity.X); MixF(p.Velocity.Y);
                Mix((long)p.Phase);
            }
            Mix(Score.SetsWon(TeamId.A)); Mix(Score.SetsWon(TeamId.B));
            Mix(Score.GamesInSet(TeamId.A)); Mix(Score.GamesInSet(TeamId.B));
            Mix(Score.PointsInGame(TeamId.A)); Mix(Score.PointsInGame(TeamId.B));
            return h;
        }
    }
}
