using System;
using System.Collections.Generic;

namespace Padel.Simulation.Court
{
    /// <summary>Checks a court configuration against FIP 2026 dimensions and tolerances (PADEL_RULES_RESEARCH §2, §9).</summary>
    public static class CourtValidator
    {
        private const float Epsilon = 0.0005f;

        public static IReadOnlyList<string> Validate(CourtConfig c)
        {
            var issues = new List<string>();
            Check(issues, "length", c.Length, 20f, 20f * 0.005f);
            Check(issues, "width", c.Width, 10f, 10f * 0.005f);
            Check(issues, "net height at centre", c.NetHeightCenter, 0.88f, 0.005f);
            if (c.NetHeightPosts > 0.92f + 0.005f + Epsilon)
                issues.Add($"net height at posts {c.NetHeightPosts:0.###} m exceeds 0.92 m (+0.005)");
            Check(issues, "service line distance", c.ServiceLineFromNet, 6.95f, 0f);
            Check(issues, "back wall solid height", c.BackWallSolidHeight, 3f, 0f);
            Check(issues, "wall total height", c.WallTotalHeight, 4f, 0f);
            Check(issues, "line width", c.LineWidth, 0.05f, 0f);
            return issues;
        }

        public static bool IsFipCompliant(CourtConfig c) => Validate(c).Count == 0;

        private static void Check(List<string> issues, string name, float value, float expected, float tolerance)
        {
            if (Math.Abs(value - expected) > tolerance + Epsilon)
                issues.Add($"{name} {value:0.###} m outside FIP {expected:0.###} ± {tolerance:0.###} m");
        }
    }
}
