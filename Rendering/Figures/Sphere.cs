using System.Linq;
using System.Numerics;
using System.Windows.Media;
using Rendering.PolygonFill;

namespace Rendering.Figures
{
    public class Sphere : Figure
    {
        public Sphere(CubesImage canvas, Color color) : base(canvas, color)
        {
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

            Triangles = triangles.Select(t => ConstructTriangle(t[0], t[1], t[2]))
                .SelectMany(t => t.SubdivideAndNormalize())
                .SelectMany(t => t.SubdivideAndNormalize())
                .SelectMany(t => t.SubdivideAndNormalize());
        }

        public static Triangle ConstructTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            var vertexA = new Vertex(a, Vector3.Normalize(a));
            var vertexB = new Vertex(b, Vector3.Normalize(b));
            var vertexC = new Vertex(c, Vector3.Normalize(c));

            return new Triangle(vertexA, vertexB, vertexC);
        }
    }
}