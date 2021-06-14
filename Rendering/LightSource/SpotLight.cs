using System;
using System.Numerics;
using System.Windows.Media;

namespace Rendering.LightSource
{
    public record SpotLight : LightSource
    {
        public SpotLight(Color color, Vector3 position, Vector3 direction, float cutOffAngle) : base(color, position)
        {
            Direction = direction;
            CutOffAngle = cutOffAngle;
            IntensityVector = new Vector3(color.R, color.G, color.B) / 255;
        }

        public float CutOffAngle { get; }
        public Vector3 Direction { get; }
        public Vector3 IntensityVector { get; }
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