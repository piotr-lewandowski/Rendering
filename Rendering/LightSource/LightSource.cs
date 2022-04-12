using System.Numerics;
using System.Windows.Media;

namespace Rendering.LightSource
{
    public abstract record LightSource(Color Color, Vector3 Position)
    {
        public Vector3 Position { get; set; } = Position;
        public abstract Vector3 LightVector(Vector3 point);
        public abstract Vector3 Intensity(Vector3 point);

        public LightSource ToViewSpace(CubesImage image)
        {
            var matrix = image.Projection * image.View;
            var vector = new Vector4(Position, 1);

            var resultVector = Utility.Multiply(matrix, vector);
            resultVector /= resultVector.W;

             return this with{ Position = new Vector3(
                 (float) (resultVector.X * image.ActualWidth + image.ActualWidth) / 2,
                 (float) (resultVector.Y * image.ActualHeight + image.ActualHeight) / 2,
                resultVector.Z)};
        }
    }
}