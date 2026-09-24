using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Padel.Core;
using Padel.Rules;
using Padel.Simulation.Match;
using Padel.Simulation.Players;

namespace Padel.Simulation.Tests
{
    public class MatchSimulationTests
    {
        private static PlayerCommand[] Idle(int n) => new PlayerCommand[n];

        private static List<MatchEvent> RunUntil(MatchSimulation sim, MatchState s, System.Func<MatchState, bool> done, int maxTicks,
            System.Func<MatchState, PlayerCommand[]> commands = null)
        {
            var all = new List<MatchEvent>();
            for (int i = 0; i < maxTicks && !done(s); i++)
            {
                sim.Step(s, commands != null ? commands(s) : Idle(s.Players.Length), all);
            }
            return all;
        }

        private static PlayerCommand[] Press(MatchState s, int player, ShotIntent intent = ShotIntent.Control)
        {
            var c = Idle(s.Players.Length);
            c[player].Pressed = intent;
            return c;
        }

        [TestCase(1)]
        [TestCase(2)]
        public void InitialStateIsReadyToServeWithFormation(int playersPerTeam)
        {
            var sim = new MatchSimulation(MatchConfig.Default(playersPerTeam));
            MatchState s = sim.CreateInitialState();
            Assert.That(s.Phase, Is.EqualTo(PointPhase.PreServe));
            Assert.That(s.BallLive, Is.False);
            int server = sim.ServerIndex(s);
            Assert.That(s.TeamOf(server), Is.EqualTo(TeamId.A));
            // Team A starts on the -Z half; serving from the right means standing at +X.
            Assert.That(s.Players[server].Position.Y, Is.LessThan(-8f));
            Assert.That(s.Players[server].Position.X, Is.GreaterThan(0f));
        }

        [Test]
        public void UnreturnedServeWinsThePointForTheServer()
        {
            var sim = new MatchSimulation(MatchConfig.Default(1, seed: 7UL));
            MatchState s = sim.CreateInitialState();
            var events = new List<MatchEvent>();
            sim.Step(s, Press(s, sim.ServerIndex(s)), events);
            Assert.That(s.Phase, Is.EqualTo(PointPhase.ServeToss));

            events.AddRange(RunUntil(sim, s, st => st.Phase == PointPhase.PointOver, 1200));
            Assert.That(events.Any(e => e.Kind == MatchEventKind.Stroke && e.Shot == Padel.Simulation.Shots.ShotType.Serve), Is.True);
            MatchEvent call = events.Last(e => e.Kind == MatchEventKind.Call);
            TestContext.WriteLine(string.Join("\n", events.Where(e => e.Kind != MatchEventKind.Ball || e.BallEvent.Kind != Padel.Simulation.Ball.BallEventKind.Contact || true)));
            Assert.That(call.Call.Kind, Is.EqualTo(CallKind.Point));
            Assert.That(call.Call.Winner, Is.EqualTo(TeamId.A));
            Assert.That(call.Call.Reason, Is.EqualTo(PointReason.DoubleBounce));
            Assert.That(s.Score.PointsInGame(TeamId.A), Is.EqualTo(1));
        }

        [Test]
        public void AfterThePointPauseTheNextPointStartsFromTheLeft()
        {
            var sim = new MatchSimulation(MatchConfig.Default(1, seed: 7UL));
            MatchState s = sim.CreateInitialState();
            sim.Step(s, Press(s, sim.ServerIndex(s)), null);
            RunUntil(sim, s, st => st.Phase == PointPhase.PointOver, 1200);
            RunUntil(sim, s, st => st.Phase == PointPhase.PreServe, 400);
            Assert.That(sim.CurrentServeSide(s), Is.EqualTo(ServeSide.Left));
            Assert.That(s.Players[sim.ServerIndex(s)].Position.X, Is.LessThan(0f));
        }

        [Test]
        public void ReceiverVolleyingTheServeLosesThePoint()
        {
            var sim = new MatchSimulation(MatchConfig.Default(1, seed: 3UL));
            MatchState s = sim.CreateInitialState();
            int server = sim.ServerIndex(s);
            int receiver = 1 - server;
            sim.Step(s, Press(s, server), null);
            // Put the receiver right under the serve's flight and have them swing before the bounce.
            RunUntil(sim, s, st => st.LastHitter == server, 600);
            List<MatchEvent> events = RunUntil(sim, s, st => st.Phase == PointPhase.PointOver, 600, st =>
            {
                var c = Idle(st.Players.Length);
                Vec3 b = st.Ball.Position;
                st.Players[receiver].Position = new Vec2(b.X, b.Z + 0.3f); // test harness teleport
                c[receiver].Pressed = st.Ball.Position.Z > 1f ? ShotIntent.Attack : ShotIntent.None;
                return c;
            });
            MatchEvent call = events.LastOrDefault(e => e.Kind == MatchEventKind.Call);
            Assert.That(call.Call.Reason, Is.EqualTo(PointReason.ReceiverVolleyedServe));
        }

        [Test]
        public void SameCommandsProduceTheSameStateHash()
        {
            ulong Run()
            {
                var sim = new MatchSimulation(MatchConfig.Default(2, seed: 42UL));
                MatchState s = sim.CreateInitialState();
                for (int i = 0; i < 3000; i++)
                {
                    var c = new PlayerCommand[4];
                    c[0].Move = new Vec2((i / 60) % 2 == 0 ? 1f : -1f, 0f);
                    c[3].Move = new Vec2(0f, (i / 90) % 2 == 0 ? 1f : -1f);
                    if (s.Phase == PointPhase.PreServe) c[sim.ServerIndex(s)].Pressed = ShotIntent.Control;
                    sim.Step(s, c, null);
                }
                return s.ComputeHash();
            }
            Assert.That(Run(), Is.EqualTo(Run()));
        }

        [Test]
        public void CloneProducesAnIndependentIdenticalCopy()
        {
            var sim = new MatchSimulation(MatchConfig.Default(2, seed: 9UL));
            MatchState s = sim.CreateInitialState();
            sim.Step(s, Press(s, sim.ServerIndex(s)), null);
            MatchState copy = s.Clone();
            Assert.That(copy.ComputeHash(), Is.EqualTo(s.ComputeHash()));
            for (int i = 0; i < 200; i++) sim.Step(copy, Idle(4), null);
            Assert.That(copy.ComputeHash(), Is.Not.EqualTo(s.ComputeHash()));
            for (int i = 0; i < 200; i++) sim.Step(s, Idle(4), null);
            Assert.That(copy.ComputeHash(), Is.EqualTo(s.ComputeHash()));
        }
    }
}
