using System.Numerics;
using System.Windows.Media;

namespace Rendering.PolygonFill;

public class Phong : PolygonFiller
{
    public override int GetColor(Barycentric barycentric, int x, int y)
    {
        var point = new Vector3(x, y, InterpolateZ(barycentric));

        var n = barycentric.Interpolate(Vertices[0].NormalVector, Vertices[1].NormalVector, Vertices[2].NormalVector);

        var v = new Vertex(point, n);
        return CalculateColor(v).ToInt();
    }

    public Phong(Triangle triangle, CubesImage cubesImage, Color color) : base(triangle, cubesImage, color) { }
}