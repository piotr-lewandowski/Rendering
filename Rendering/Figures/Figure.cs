using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Media;
using Rendering.PolygonFill;

namespace Rendering.Figures
{
    public abstract class Figure
    {
        protected Figure(CubesImage canvas, Color color)
        {
            Canvas = canvas;
            Color = color;
        }

        public Vector3 TranslationVector { get; set; } = Vector3.Zero;
        public float Scale { get; set; } = 1;
        public Quaternion RotationQuaternion { get; set; } = Quaternion.Identity;
        public Matrix4x4 Translation => Matrix4x4.Transpose( Matrix4x4.CreateTranslation(TranslationVector) );
        public Matrix4x4 Scaling => Matrix4x4.CreateScale(Scale);
        public Matrix4x4 Rotation => Matrix4x4.CreateFromQuaternion(RotationQuaternion);
        public Matrix4x4 Model => Translation * Scaling * Rotation;
        public CubesImage Canvas { get; }
        public IEnumerable<Triangle> Triangles { get; set; }
        public Color Color { get; }

        public void Draw()
        {
            var triangles = Triangles.Select(triangle => (ToProjectionSpace(triangle), Vector3.Cross(triangle.B - triangle.A, triangle.C - triangle.A)));
            foreach (var (triangle, normal) in triangles)
            {
                Fill(triangle.A, triangle.B, triangle.C, Color, Vector3.Normalize( Vector3.Cross(triangle.A - triangle.B, triangle.A - triangle.C)));
            }
        }

        public Vector3 ToProjectionSpace(Vector3 original)
        {
            var matrix = Canvas.Projection * Canvas.View * Model;
            var vector = new Vector4(original, 1);

            var resultVector = SupportMatrices.Multiply(matrix, vector);
            resultVector /= resultVector.W;

            return new Vector3(
                (float) (resultVector.X * Canvas.ActualWidth + Canvas.ActualWidth) / 2,
                (float) (resultVector.Y * Canvas.ActualHeight + Canvas.ActualHeight) / 2,
                resultVector.Z);
        }

        public Triangle ToProjectionSpace(Triangle original)
        {
            return new(
                ToProjectionSpace(original.A),
                ToProjectionSpace(original.B),
                ToProjectionSpace(original.C)
            );
        }

        public void Fill(Vector3 point1, Vector3 point2, Vector3 point3, Color color, Vector3 normalVector)
        {
            var viewVector = Canvas.CurrentCamera.Target;

            if (Vector3.Dot(viewVector, normalVector) >= 0)
                return;

            var polygon = new Polygon(new[] { point1, point2, point3 }, normalVector);
            polygon.Fill(Canvas, color);
        }
    }
}