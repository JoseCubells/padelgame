using System.Collections.Generic;
using Padel.Simulation.Court;

namespace Padel.Simulation.Ball
{
    /// <summary>A ball event with the simulated time (seconds from the prediction start) at which it happens.</summary>
    public struct PredictedEvent
    {
        public float Time;
        public BallEvent Event;
    }

    /// <summary>
    /// Forecasts the ball by running <see cref="BallSimulator.Step"/> on a copy of the state, so predictions are exactly
    /// what the simulation will do (ADR-004). Used by the AI, movement assistance and the shot solver.
    /// Holds scratch buffers to avoid per-call allocations; not thread-safe.
    /// </summary>
    public sealed class TrajectoryPredictor
    {
        private readonly List<BallEvent> _stepEvents = new List<BallEvent>(8);

        /// <summary>
        /// Simulates up to <paramref name="horizon"/> seconds. Fills <paramref name="samples"/> with the state after
        /// every step (optional) and <paramref name="events"/> with timed events (optional). Stops early after the
        /// ball leaves the court. Returns the final state.
        /// </summary>
        public BallState Predict(
            BallState start, BallConfig config, CourtGeometry court, float dt, float horizon,
            List<BallState> samples = null, List<PredictedEvent> events = null)
        {
            samples?.Clear();
            events?.Clear();
            BallState state = start;
            int steps = (int)(horizon / dt + 0.5f);
            for (int i = 0; i < steps; i++)
            {
                _stepEvents.Clear();
                BallSimulator.Step(ref state, config, court, dt, _stepEvents);
                samples?.Add(state);
                bool left = false;
                for (int e = 0; e < _stepEvents.Count; e++)
                {
                    events?.Add(new PredictedEvent { Time = (i + 1) * dt, Event = _stepEvents[e] });
                    left |= _stepEvents[e].Kind == BallEventKind.LeftCourt;
                }
                if (left) break;
            }
            return state;
        }
    }
}
