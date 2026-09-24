using NUnit.Framework;
using Padel.Core;

namespace Padel.Rules.Tests
{
    public class PointRefereeTests
    {
        private static readonly TeamId S = TeamId.A; // server
        private static readonly TeamId R = TeamId.B; // receiver

        private static RefereeCall Feed(PointReferee referee, params RallyEvent[] events)
        {
            RefereeCall last = RefereeCall.Continue;
            foreach (RallyEvent e in events)
            {
                last = referee.Process(e);
                if (last.Kind != CallKind.None) return last;
            }
            return last;
        }

        private static PointReferee Serving()
        {
            var referee = new PointReferee();
            referee.StartPoint(S);
            return referee;
        }

        // A good serve: toss bounce, hit, crosses, bounces in the box.
        private static readonly RallyEvent[] GoodServe =
        {
            RallyEvent.Bounce(S), RallyEvent.Hit(S), RallyEvent.Crossed(R), RallyEvent.Bounce(R, inServiceBox: true),
        };

        [Test]
        public void ServeOutOfTheBoxIsAFaultThenDoubleFaultLosesThePoint()
        {
            PointReferee referee = Serving();
            RefereeCall call = Feed(referee, RallyEvent.Hit(S), RallyEvent.Crossed(R), RallyEvent.Bounce(R, inServiceBox: false));
            Assert.That(call.Kind, Is.EqualTo(CallKind.Fault));
            Assert.That(call.Reason, Is.EqualTo(PointReason.ServeOutOfBox));

            referee.StartServe();
            call = Feed(referee, RallyEvent.Hit(S), RallyEvent.Crossed(R), RallyEvent.Bounce(R, inServiceBox: false));
            Assert.That(call.Kind, Is.EqualTo(CallKind.Point));
            Assert.That(call.Winner, Is.EqualTo(R));
            Assert.That(call.Reason, Is.EqualTo(PointReason.DoubleFault));
        }

        [Test]
        public void ServeIntoTheNetIsAFault()
        {
            RefereeCall call = Feed(Serving(), RallyEvent.Hit(S), RallyEvent.Net(), RallyEvent.Bounce(S));
            Assert.That(call.Kind, Is.EqualTo(CallKind.Fault));
        }

        [Test]
        public void ServeTouchingOwnGlassIsAFault()
        {
            RefereeCall call = Feed(Serving(), RallyEvent.Hit(S), RallyEvent.Glass(S));
            Assert.That(call.Kind, Is.EqualTo(CallKind.Fault));
            Assert.That(call.Reason, Is.EqualTo(PointReason.ServeHitOwnWall));
        }

        [Test]
        public void ServeBouncingInTheBoxThenMeshIsAFault()
        {
            PointReferee referee = Serving();
            Feed(referee, GoodServe);
            RefereeCall call = referee.Process(RallyEvent.Mesh(R));
            Assert.That(call.Kind, Is.EqualTo(CallKind.Fault));
            Assert.That(call.Reason, Is.EqualTo(PointReason.ServeHitMeshAfterBounce));
        }

        [Test]
        public void ServeBouncingInTheBoxThenGlassIsGood()
        {
            PointReferee referee = Serving();
            Feed(referee, GoodServe);
            Assert.That(referee.Process(RallyEvent.Glass(R)).Kind, Is.EqualTo(CallKind.None));
            Assert.That(referee.Process(RallyEvent.Hit(R)).Kind, Is.EqualTo(CallKind.None));
        }

        [Test]
        public void NetCordServeLandingInTheBoxIsALetAndKeepsFaultCount()
        {
            PointReferee referee = Serving();
            Feed(referee, RallyEvent.Hit(S), RallyEvent.Out()); // first serve fault
            referee.StartServe();
            RefereeCall call = Feed(referee, RallyEvent.Hit(S), RallyEvent.Net(), RallyEvent.Crossed(R),
                RallyEvent.Bounce(R, inServiceBox: true), RallyEvent.Hit(R));
            Assert.That(call.Kind, Is.EqualTo(CallKind.Let));
            Assert.That(referee.Faults, Is.EqualTo(1));
        }

        [Test]
        public void NetCordServeThenMeshIsAFaultNotALet()
        {
            PointReferee referee = Serving();
            RefereeCall call = Feed(referee, RallyEvent.Hit(S), RallyEvent.Net(), RallyEvent.Crossed(R),
                RallyEvent.Bounce(R, inServiceBox: true), RallyEvent.Mesh(R));
            Assert.That(call.Kind, Is.EqualTo(CallKind.Fault));
        }

        [Test]
        public void ReceiverMayNotVolleyTheServe()
        {
            RefereeCall call = Feed(Serving(), RallyEvent.Hit(S), RallyEvent.Crossed(R), RallyEvent.Hit(R));
            Assert.That(call.Winner, Is.EqualTo(S));
            Assert.That(call.Reason, Is.EqualTo(PointReason.ReceiverVolleyedServe));
        }

        [Test]
        public void UnreturnedServeBouncingTwiceWinsThePoint()
        {
            PointReferee referee = Serving();
            Feed(referee, GoodServe);
            RefereeCall call = referee.Process(RallyEvent.Bounce(R));
            Assert.That(call.Winner, Is.EqualTo(S));
            Assert.That(call.Reason, Is.EqualTo(PointReason.DoubleBounce));
        }

        private static PointReferee InRallyAfterReturn()
        {
            PointReferee referee = Serving();
            Feed(referee, GoodServe);
            Assert.That(referee.Process(RallyEvent.Hit(R)).Kind, Is.EqualTo(CallKind.None)); // return
            return referee;
        }

        [Test]
        public void ReturnIntoOpponentGlassWithoutBouncingLoses()
        {
            PointReferee referee = InRallyAfterReturn();
            RefereeCall call = Feed(referee, RallyEvent.Crossed(S), RallyEvent.Glass(S));
            Assert.That(call.Winner, Is.EqualTo(S));
            Assert.That(call.Reason, Is.EqualTo(PointReason.HitOpponentWallBeforeBounce));
        }

        [Test]
        public void BallMayBounceThenPlayOffTheGlassAndBeReturned()
        {
            PointReferee referee = InRallyAfterReturn();
            RefereeCall call = Feed(referee, RallyEvent.Crossed(S), RallyEvent.Bounce(S), RallyEvent.Glass(S),
                RallyEvent.Mesh(S), RallyEvent.Hit(S), RallyEvent.Crossed(R), RallyEvent.Bounce(R), RallyEvent.Bounce(R));
            Assert.That(call.Winner, Is.EqualTo(S));
            Assert.That(call.Reason, Is.EqualTo(PointReason.DoubleBounce));
        }

        [Test]
        public void PlayerMayPlayOffOwnGlassButNotOwnMesh()
        {
            PointReferee referee = InRallyAfterReturn();
            Feed(referee, RallyEvent.Crossed(S), RallyEvent.Bounce(S));
            Assert.That(Feed(referee, RallyEvent.Hit(S), RallyEvent.Glass(S), RallyEvent.Crossed(R)).Kind, Is.EqualTo(CallKind.None));

            referee = InRallyAfterReturn();
            Feed(referee, RallyEvent.Crossed(S), RallyEvent.Bounce(S));
            RefereeCall call = Feed(referee, RallyEvent.Hit(S), RallyEvent.Mesh(S));
            Assert.That(call.Winner, Is.EqualTo(R));
            Assert.That(call.Reason, Is.EqualTo(PointReason.HitOwnMesh));
        }

        [Test]
        public void ShotBouncingOnOwnSideLoses()
        {
            PointReferee referee = InRallyAfterReturn();
            RefereeCall call = referee.Process(RallyEvent.Bounce(R));
            Assert.That(call.Winner, Is.EqualTo(S));
            Assert.That(call.Reason, Is.EqualTo(PointReason.DidNotCrossNet));
        }

        [Test]
        public void VolleyIsAllowedDuringTheRally()
        {
            PointReferee referee = InRallyAfterReturn();
            RefereeCall call = Feed(referee, RallyEvent.Crossed(S), RallyEvent.Hit(S), RallyEvent.Crossed(R), RallyEvent.Bounce(R));
            Assert.That(call.Kind, Is.EqualTo(CallKind.None));
        }

        [Test]
        public void BallLeavingTheCourtAfterBouncingWinsForTheHitter()
        {
            PointReferee referee = InRallyAfterReturn();
            RefereeCall call = Feed(referee, RallyEvent.Crossed(S), RallyEvent.Bounce(S), RallyEvent.Out());
            Assert.That(call.Winner, Is.EqualTo(R));
            Assert.That(call.Reason, Is.EqualTo(PointReason.OutAfterBounce));
        }

        [Test]
        public void BallLeavingTheCourtBeforeBouncingLosesForTheHitter()
        {
            PointReferee referee = InRallyAfterReturn();
            RefereeCall call = Feed(referee, RallyEvent.Crossed(S), RallyEvent.Out());
            Assert.That(call.Winner, Is.EqualTo(S));
            Assert.That(call.Reason, Is.EqualTo(PointReason.OutBeforeBounce));
        }

        [Test]
        public void SameTeamTouchingTwiceLoses()
        {
            PointReferee referee = InRallyAfterReturn();
            RefereeCall call = referee.Process(RallyEvent.Hit(R));
            Assert.That(call.Winner, Is.EqualTo(S));
            Assert.That(call.Reason, Is.EqualTo(PointReason.DoubleHit));
        }

        [Test]
        public void EventsAfterThePointEndsAreIgnored()
        {
            PointReferee referee = InRallyAfterReturn();
            referee.Process(RallyEvent.Hit(R));
            Assert.That(referee.IsPointOver, Is.True);
            Assert.That(referee.Process(RallyEvent.Bounce(S)).Kind, Is.EqualTo(CallKind.None));
        }
    }
}
