using System.Collections.Generic;

namespace Padel.Simulation.Shots
{
    /// <summary>Set of shot definitions available in a match. In Unity it is built from ShotDefinition assets.</summary>
    public sealed class ShotCatalog
    {
        private readonly Dictionary<ShotType, ShotDefinition> _shots = new Dictionary<ShotType, ShotDefinition>();

        public ShotCatalog(IEnumerable<ShotDefinition> shots)
        {
            foreach (ShotDefinition shot in shots) _shots[shot.Type] = shot;
        }

        public ShotDefinition Get(ShotType type) => _shots[type];
        public bool Contains(ShotType type) => _shots.ContainsKey(type);

        /// <summary>
        /// Starting values [P] derived from GAMEPLAY_RESEARCH §3.2 (speeds converted from km/h; apex heights from the
        /// taxonomy). Spin magnitudes are placeholders pending calibration (KI-005).
        /// </summary>
        public static ShotCatalog Default() => new ShotCatalog(new[]
        {
            //                 type              mode                 apex  minV  maxV  dMin  dMax  top   side   perf   good  aimErr depthErr windup recov
            new ShotDefinition(ShotType.Serve,    TrajectoryMode.Apex,  0.5f, 0f,   0f,   4.5f, 6.6f, -60f, 0f,    0.06f, 0.14f, 6f,  1.0f,   0.25f, 0.35f),
            new ShotDefinition(ShotType.Drive,    TrajectoryMode.Apex,  0.6f, 0f,   0f,   6.0f, 9.0f, 80f,  0f,    0.05f, 0.12f, 7f,  1.5f,   0.20f, 0.35f),
            new ShotDefinition(ShotType.Slice,    TrajectoryMode.Apex,  0.5f, 0f,   0f,   5.0f, 8.5f, -90f, 0f,    0.06f, 0.14f, 6f,  1.2f,   0.20f, 0.35f),
            new ShotDefinition(ShotType.Lob,      TrajectoryMode.Apex,  5.0f, 0f,   0f,   7.5f, 9.3f, 60f,  0f,    0.07f, 0.16f, 5f,  1.2f,   0.22f, 0.40f),
            new ShotDefinition(ShotType.Chiquita, TrajectoryMode.Apex,  0.35f,0f,   0f,   1.0f, 3.0f, -40f, 0f,    0.05f, 0.12f, 6f,  0.8f,   0.18f, 0.30f),
            new ShotDefinition(ShotType.Volley,   TrajectoryMode.Apex,  0.25f,0f,   0f,   5.0f, 8.5f, -70f, 0f,    0.04f, 0.10f, 7f,  1.5f,   0.12f, 0.25f),
            new ShotDefinition(ShotType.DropVolley,TrajectoryMode.Apex, 0.2f, 0f,   0f,   1.0f, 2.5f, -80f, 0f,    0.04f, 0.10f, 6f,  0.6f,   0.12f, 0.25f),
            new ShotDefinition(ShotType.Bandeja,  TrajectoryMode.Apex,  0.1f, 0f,   0f,   6.5f, 9.0f, -110f, 40f,  0.06f, 0.14f, 6f,  1.2f,   0.28f, 0.40f),
            new ShotDefinition(ShotType.Vibora,   TrajectoryMode.Speed, 0f,   19f,  26f,  6.5f, 9.0f, -80f, 110f,  0.05f, 0.12f, 8f,  1.5f,   0.30f, 0.45f),
            new ShotDefinition(ShotType.Smash,    TrajectoryMode.Speed, 0f,   26f,  36f,  5.5f, 8.5f, 40f,  0f,    0.05f, 0.11f, 9f,  1.8f,   0.32f, 0.50f),
            new ShotDefinition(ShotType.WallExit, TrajectoryMode.Apex,  0.8f, 0f,   0f,   6.0f, 9.0f, 70f,  0f,    0.06f, 0.14f, 7f,  1.5f,   0.20f, 0.35f),
        });
    }
}
