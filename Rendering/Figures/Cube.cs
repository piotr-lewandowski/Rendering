using System.Linq;
using System.Numerics;
using System.Windows.Media;
using Rendering.PolygonFill;

namespace Rendering.Figures;

public class Cube : Figure
{
    public Cube(CubesImage canvas, Color color) : base(canvas, color)
    {
        var points = new[]
        {
            new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector3(0, 1, 1),
            new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 0), new Vector3(1, 1, 1)
        };
        points = points.Select(p => p - new Vector3(0.5f)).ToArray();

        var triangles = new[]
        {
            new[] { points[0], points[3], points[1] },
            new[] { points[0], points[2], points[3] },

            new[] { points[4], points[5], points[6] },
            new[] { points[5], points[7], points[6] },

            new[] { points[1], points[3], points[7] },
            new[] { points[1], points[7], points[5] },

            new[] { points[0], points[4], points[2] },
            new[] { points[2], points[4], points[6] },

            new[] { points[2], points[6], points[3] },
            new[] { points[3], points[6], points[7] },

            new[] { points[0], points[1], points[5] },
            new[] { points[0], points[5], points[4] }
        };

        Triangles = triangles.SelectMany(t => ConstructTriangle(t[0], t[1], t[2]).Subdivide())
            .SelectMany(t => t.Subdivide())
            .SelectMany(t => t.Subdivide());
    }

    private static Triangle ConstructTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        var normal = Vector3.Cross(b - a, c - a);

        var vertexA = new Vertex(a, normal);
        var vertexB = new Vertex(b, normal);
        var vertexC = new Vertex(c, normal);

        return new Triangle(vertexA, vertexB, vertexC);
    }
}