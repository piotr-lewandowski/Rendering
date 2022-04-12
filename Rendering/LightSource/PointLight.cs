using System.Numerics;
using System.Windows.Media;

namespace Rendering.LightSource
{
    public record PointLight(Color Color, Vector3 Position) : LightSource(Color, Position)
    {
        public Vector3 IntensityVector { get; } = new Vector3(Color.R, Color.G, Color.B) / 255;

        public override Vector3 LightVector(Vector3 point) => Position - point;

        public override Vector3 Intensity(Vector3 point) => IntensityVector;
    }
}