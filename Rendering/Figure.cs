using System.Numerics;
using System.Windows.Media;
using Rendering.PolygonFill;

namespace Rendering
{
    public abstract class Figure
    {
        protected Figure(CubesImage canvas)
        {
            Canvas = canvas;
        }

        public Vector3 TranslationVector { get; set; } = Vector3.Zero;
        public float Scale { get; set; } = 1;
        public Quaternion RotationQuaternion { get; set; } = Quaternion.Identity;
        public Matrix4x4 Translation => Matrix4x4.CreateTranslation(TranslationVector);
        public Matrix4x4 Scaling => Matrix4x4.CreateScale(Scale);
        public Matrix4x4 Rotation => Matrix4x4.CreateFromQuaternion(RotationQuaternion);
        public Matrix4x4 Model => Translation * Scaling * Rotation;
        public CubesImage Canvas { get; }

        public abstract void Draw();

        public void Fill(Vector3 point1, Vector3 point2, Vector3 point3, Color color, Vector3 normalVector)
        {
            var viewVector = new Vector3(3, 0.5f, 0.5f);

            var matrix = Canvas.Projection * Canvas.View * Model;
            var vector = new Vector4(viewVector, 1);

            var resultVector = SupportMatrices.Multiply(matrix, vector);
            resultVector /= resultVector.W;

            viewVector = new Vector3(
                (float) (resultVector.X * Canvas.ActualWidth + Canvas.ActualWidth) / 2,
                (float) (resultVector.Y * Canvas.ActualHeight + Canvas.ActualHeight) / 2,
                resultVector.Z);

            var n = Vector3.Cross(point1 - point2, point1 - point3);

            if (Vector3.Dot(point1 - viewVector, n) > 0)
                return;

            var polygon = new Polygon(new[] { point1, point2, point3 }, normalVector);
            polygon.Fill(Canvas, color);
        }
    }
}