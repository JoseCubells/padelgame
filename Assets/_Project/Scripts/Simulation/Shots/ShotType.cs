namespace Padel.Simulation.Shots
{
    /// <summary>Concrete padel techniques (GAMEPLAY_RESEARCH §3.2 taxonomy).</summary>
    public enum ShotType
    {
        Serve = 0,
        Drive = 1,
        Slice = 2,
        Lob = 3,
        Chiquita = 4,
        Volley = 5,
        DropVolley = 6,
        Bandeja = 7,
        Vibora = 8,
        Smash = 9,
        WallExit = 10,
    }

    /// <summary>How the solver interprets the trajectory parameters of a shot.</summary>
    public enum TrajectoryMode
    {
        /// <summary>Solve the launch for a target apex height (lobs, drives, serves, volleys, bandeja).</summary>
        Apex = 0,
        /// <summary>Solve the launch for a target speed (smash, víbora).</summary>
        Speed = 1,
    }
}
