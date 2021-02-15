using System;
using System.Numerics;
using System.Windows.Media;

namespace Rendering
{
    public abstract record LightSource
    {
        protected LightSource(Color color, Vector3 position)
        {
            Color = color;
            Position = position;
        }
        public Color Color { get; }
        public Vector3 Position { get; set; }
        public abstract Vector3 LightVector(Vector3 point);
        public abstract Vector3 Intensity(Vector3 point);

        public LightSource ToViewSpace(CubesImage image)
        {
            var matrix = image.Projection * image.View;
            var vector = new Vector4(Position, 1);

            var resultVector = SupportMatrices.Multiply(matrix, vector);
            resultVector /= resultVector.W;

             return this with{ Position = new Vector3(
                resultVector.X,
                resultVector.Y,
                resultVector.Z)};
        }
    }

    public record DirectionalLight : LightSource
    {
        public DirectionalLight(Color color, Vector3 position) : base(color, position)
        {
            IntensityVector = new Vector3(color.R, color.G, color.B) / 255;
        }

        public Vector3 IntensityVector { get; }

        public override Vector3 LightVector(Vector3 point)
        {
            return Position;
        }

        public override Vector3 Intensity(Vector3 point)
        {
            return IntensityVector;
        }
    }

    public record PointLight : LightSource
    {
        public PointLight(Color color, Vector3 position) : base(color, position)
        {
            IntensityVector = new Vector3(color.R, color.G, color.B) / 255;
        }
        public Vector3 IntensityVector { get; }

        public override Vector3 LightVector(Vector3 point)
        {
            return point - Position;
        }

        public override Vector3 Intensity(Vector3 point)
        {
            return IntensityVector;
        }
    }

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
        public override Vector3 LightVector(Vector3 point)
        {
            return point - Position;
        }

        public override Vector3 Intensity(Vector3 point)
        {
            var d = Vector3.Normalize(Direction);
            var l = -Vector3.Normalize(LightVector(point));
            var spotCosine = Vector3.Dot(d, -l);

                return IntensityVector * MathF.Pow(spotCosine, Power);
            if (spotCosine <= CutOffAngle)
            {
            }

            return Vector3.Zero;
        }
    }
}