using System;
using System.Numerics;

namespace MinimapIcons
{
    public class MapIcon
    {
        const float CameraAngle = 38 * MathF.PI / 180;
        private static readonly double AngleCos = Math.Cos(CameraAngle);
        private static readonly double AngleSin = Math.Sin(CameraAngle);

        public static Vector2 DeltaInWorldToMinimapDelta(Vector2 delta, double diag, float scale, float deltaZ = 0)
        {
            var cos = (float)(diag * AngleCos / scale);
            var sin = (float)(diag * AngleSin / scale);

            // 2D rotation formulas not correct, but it's what appears to work?
            return new Vector2((delta.X - delta.Y) * cos, deltaZ - (delta.X + delta.Y) * sin);
        }
    }
}