using System.Numerics;
using System.Windows.Media;

namespace Rendering.LightSource
{
    public record DirectionalLight : LightSource
    {
        public DirectionalLight(Color color, Vector3 position) : base(color, position)
        {
            IntensityVector = new Vector3(color.R, color.G, color.B) / 255;
        }

        public Vector3 IntensityVector { get; }

        public override Vector3 LightVector(Vector3 point) => Position;

        public override Vector3 Intensity(Vector3 point) => IntensityVector;
    }
}