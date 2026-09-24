namespace Padel.Core
{
    /// <summary>One of the two sides of a match. Team A starts on the negative-Z half of the court.</summary>
    public enum TeamId
    {
        A = 0,
        B = 1,
    }

    public static class TeamIdExtensions
    {
        public static TeamId Other(this TeamId team) => team == TeamId.A ? TeamId.B : TeamId.A;
    }
}
