using System.Linq;
using System.Numerics;
using System.Windows.Media;
using Rendering.PolygonFill;

namespace Rendering.Figures;

public class Square : Figure
{
    public Square(CubesImage canvas, Color color) : base(canvas, color)
    {
        var points = new[]
        {
            new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector3(0, 1, 1),
            new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 0), new Vector3(1, 1, 1)
        };
        points = points.Select(p => p - new Vector3(0.5f)).ToArray();

        var triangles = new[]
        {
            new[] { points[0], points[1], points[5] },
            new[] { points[0], points[5], points[4] }
        };

        Triangles = triangles.SelectMany(t => ConstructTriangle(t[0], t[1], t[2]).Subdivide());
    }

    private static Triangle ConstructTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        var vertexA = new Vertex(a, Vector3.UnitZ);
        var vertexB = new Vertex(b, Vector3.UnitZ);
        var vertexC = new Vertex(c, Vector3.UnitZ);

        return new Triangle(vertexA, vertexB, vertexC);
    }
}