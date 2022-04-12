using System;
using System.Numerics;
using System.Windows.Media;

namespace Rendering.LightSource
{
    public record SpotLight(Color Color, Vector3 Position, Vector3 Direction, float CutOffAngle) : LightSource(Color,
        Position)
    {
        public Vector3 IntensityVector { get; } = new Vector3(Color.R, Color.G, Color.B) / 255;
        public float Power { get; } = 10;
        public override Vector3 LightVector(Vector3 point) => Position - point;

        public override Vector3 Intensity(Vector3 point)
        {
            var d = Vector3.Normalize(Direction);
            var l = -Vector3.Normalize(LightVector(point));
            var spotCosine = Vector3.Dot(d, -l);

            if (spotCosine <= MathF.Cos(CutOffAngle))
            {
                return IntensityVector * MathF.Pow(spotCosine, Power);
            }

            return Vector3.Zero;
        }
    }
}