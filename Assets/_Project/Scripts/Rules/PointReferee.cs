using System;
using Padel.Core;

namespace Padel.Rules
{
    /// <summary>
    /// Decides serve faults, lets and point outcomes from <see cref="RallyEvent"/>s
    /// (FIP 2026, docs/research/PADEL_RULES_RESEARCH.md §5.1 S4-S10 and §5.3 J1-J12).
    ///
    /// Not modelled in the MVP (documented in KNOWN_ISSUES): ball touching a player (J7/J8), player touching the net
    /// (J5), outside play (J13), ball stuck in the mesh (J11), serve toss bounce enforcement (S3).
    /// </summary>
    [Serializable]
    public sealed class PointReferee
    {
        private enum Phase
        {
            Idle,
            AwaitingServeHit,
            ServeInFlight,
            ServeBounced,
            Rally,
            Over,
        }

        private Phase _phase = Phase.Idle;
        private TeamId _server;
        private TeamId _lastHitter;
        private bool _crossed;
        private int _bouncesAfterCross;
        private bool _serveTouchedNet;

        /// <summary>Faults already committed on the current point (0 or 1).</summary>
        public int Faults { get; private set; }
        public bool IsPointOver => _phase == Phase.Over;
        public bool IsServing => _phase == Phase.AwaitingServeHit || _phase == Phase.ServeInFlight || _phase == Phase.ServeBounced;

        /// <summary>Deep copy (all fields are values) for snapshots, prediction and replays.</summary>
        public PointReferee Clone() => (PointReferee)MemberwiseClone();

        /// <summary>Starts a new point (first serve).</summary>
        public void StartPoint(TeamId server)
        {
            _server = server;
            Faults = 0;
            StartServe();
        }

        /// <summary>Re-arms the serve after a fault or a let, keeping the fault count.</summary>
        public void StartServe()
        {
            _phase = Phase.AwaitingServeHit;
            _crossed = false;
            _bouncesAfterCross = 0;
            _serveTouchedNet = false;
        }

        public RefereeCall Process(RallyEvent e)
        {
            switch (_phase)
            {
                case Phase.AwaitingServeHit: return OnAwaitingServe(e);
                case Phase.ServeInFlight: return OnServeInFlight(e);
                case Phase.ServeBounced: return OnServeBounced(e);
                case Phase.Rally: return OnRally(e);
                default: return RefereeCall.Continue;
            }
        }

        private RefereeCall OnAwaitingServe(RallyEvent e)
        {
            // The toss bounce and anything before the serve contact is ignored.
            if (e.Kind == RallyEventKind.Hit && e.Team == _server)
            {
                _lastHitter = _server;
                _phase = Phase.ServeInFlight;
            }
            return RefereeCall.Continue;
        }

        private RefereeCall OnServeInFlight(RallyEvent e)
        {
            TeamId receiver = _server.Other();
            switch (e.Kind)
            {
                case RallyEventKind.NetContact:
                    _serveTouchedNet = true;
                    return RefereeCall.Continue;
                case RallyEventKind.CrossedNet:
                    _crossed = e.Team == receiver;
                    return RefereeCall.Continue;
                case RallyEventKind.WallContact:
                    // S6: before bouncing in the box, any wall/mesh contact is a fault (own side, or receiver's walls).
                    return Fault(PointReason.ServeHitOwnWall);
                case RallyEventKind.LeftCourt:
                    return Fault(PointReason.ServeLeftCourt);
                case RallyEventKind.FloorBounce:
                    if (e.Team != receiver || !e.InServiceBox) return Fault(PointReason.ServeOutOfBox);
                    _phase = Phase.ServeBounced;
                    _bouncesAfterCross = 1;
                    return RefereeCall.Continue;
                case RallyEventKind.Hit:
                    // S10 [P]: the receiver must let the serve bounce.
                    if (e.Team == receiver) return End(_server, PointReason.ReceiverVolleyedServe);
                    return RefereeCall.Continue;
                default:
                    return RefereeCall.Continue;
            }
        }

        private RefereeCall OnServeBounced(RallyEvent e)
        {
            TeamId receiver = _server.Other();
            switch (e.Kind)
            {
                case RallyEventKind.WallContact when e.IsMesh:
                    return Fault(PointReason.ServeHitMeshAfterBounce); // S6
                case RallyEventKind.WallContact:
                    return RefereeCall.Continue; // S7: glass after the bounce keeps the serve good
                case RallyEventKind.Hit when e.Team == receiver:
                    if (_serveTouchedNet) return Let();
                    BeginRallyAfterHit(receiver);
                    return RefereeCall.Continue;
                case RallyEventKind.FloorBounce:
                    if (_serveTouchedNet) return Let();
                    return End(_server, PointReason.DoubleBounce);
                case RallyEventKind.LeftCourt:
                    if (_serveTouchedNet) return Let();
                    return End(_server, PointReason.OutAfterBounce);
                default:
                    return RefereeCall.Continue;
            }
        }

        private RefereeCall OnRally(RallyEvent e)
        {
            TeamId hitter = _lastHitter;
            TeamId opponent = hitter.Other();

            if (e.Kind == RallyEventKind.Hit)
            {
                if (e.Team == hitter) return End(opponent, PointReason.DoubleHit); // J6, or a second touch on the same side
                BeginRallyAfterHit(opponent); // volley or ground stroke by the opponent
                return RefereeCall.Continue;
            }

            if (!_crossed)
            {
                switch (e.Kind)
                {
                    case RallyEventKind.CrossedNet:
                        if (e.Team == opponent) _crossed = true;
                        return RefereeCall.Continue;
                    case RallyEventKind.WallContact when e.IsMesh:
                        return End(opponent, PointReason.HitOwnMesh); // J9
                    case RallyEventKind.WallContact:
                        return RefereeCall.Continue; // J2: may play off own glass
                    case RallyEventKind.FloorBounce:
                        return End(opponent, PointReason.DidNotCrossNet); // J9
                    case RallyEventKind.LeftCourt:
                        return End(opponent, PointReason.OutBeforeBounce);
                    default:
                        return RefereeCall.Continue;
                }
            }

            switch (e.Kind)
            {
                case RallyEventKind.WallContact:
                    // J3: walls of the opponent before the ball bounces on their floor. After the bounce: in play (J1).
                    if (_bouncesAfterCross == 0) return End(opponent, PointReason.HitOpponentWallBeforeBounce);
                    return RefereeCall.Continue;
                case RallyEventKind.FloorBounce:
                    _bouncesAfterCross++;
                    if (_bouncesAfterCross >= 2) return End(hitter, PointReason.DoubleBounce); // J4
                    return RefereeCall.Continue;
                case RallyEventKind.LeftCourt:
                    return _bouncesAfterCross == 0
                        ? End(opponent, PointReason.OutBeforeBounce)
                        : End(hitter, PointReason.OutAfterBounce); // J12
                default:
                    return RefereeCall.Continue;
            }
        }

        private void BeginRallyAfterHit(TeamId hitter)
        {
            _phase = Phase.Rally;
            _lastHitter = hitter;
            _crossed = false;
            _bouncesAfterCross = 0;
        }

        private RefereeCall Fault(PointReason reason)
        {
            Faults++;
            if (Faults >= 2) return End(_server.Other(), PointReason.DoubleFault);
            _phase = Phase.Idle; // the match flow re-arms the serve with StartServe()
            return new RefereeCall { Kind = CallKind.Fault, Reason = reason };
        }

        private RefereeCall Let()
        {
            _phase = Phase.Idle;
            return new RefereeCall { Kind = CallKind.Let };
        }

        private RefereeCall End(TeamId winner, PointReason reason)
        {
            _phase = Phase.Over;
            return RefereeCall.PointTo(winner, reason);
        }
    }
}
