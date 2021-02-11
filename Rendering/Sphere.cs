using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Media;

namespace Rendering
{
    class Sphere : Figure
    {
        public Color Color { get; }
        public IEnumerable<Triangle> Triangles { get; }
        public Sphere(CubesImage canvas) : base(canvas)
        {
            Color = Colors.BlueViolet;

            var points = new[]
            {
                new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector3(0, 1, 1),
                new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 0), new Vector3(1, 1, 1)
            };
            points = points.Select(p => p - new Vector3(0.5f)).Select(Vector3.Normalize).ToArray();

            var triangles = new[]
            {
                new[] {points[0], points[3], points[1]},
                new[] {points[0], points[2], points[3]},

                new[] {points[4], points[5], points[6]},
                new[] {points[5], points[7], points[6]},

                new[] {points[1], points[3], points[7]},
                new[] {points[1], points[7], points[5]},

                new[] {points[0], points[4], points[2]},
                new[] {points[2], points[4], points[6]},

                new[] {points[2], points[6], points[3]},
                new[] {points[3], points[6], points[7]},

                new[] {points[0], points[1], points[5]},
                new[] {points[0], points[5], points[4]}
            };

            Triangles = triangles.SelectMany(t => new Triangle(t[0], t[1], t[2]).Subdivide())
                .SelectMany(t => t.Subdivide());
        }


        public override void Draw()
        {
            var triangles = Triangles.Select(ToProjectionSpace);
            foreach (var triangle in triangles)
            {
                Fill(triangle.A, triangle.B, triangle.C, Color, Vector3.One);
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
            return new (
                ToProjectionSpace(original.A),
                ToProjectionSpace(original.B),
                ToProjectionSpace(original.C)
            );
        }
    }
}