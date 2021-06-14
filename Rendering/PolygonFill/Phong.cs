using System.Numerics;
using System.Windows.Media;

namespace Rendering.PolygonFill
{
    public class Phong : Polygon
    {

        public override int GetColor(int x, int y)
        {
            var point = new Vector3(x, y, InterpolateZ(x, y));

            var p = new Vector2(x, y);
            var a = Vertices[0].AsVector2;
            var b = Vertices[1].AsVector2;
            var c = Vertices[2].AsVector2;
            var bar = new Barycentric(a, b, c, p);

            var n = bar.Interpolate(Vertices[0].NormalVector, Vertices[1].NormalVector, Vertices[2].NormalVector);

            var v = new Vertex(point, n);
            return CalculateColor(v).ToInt();
        }

        public Phong(Triangle triangle, CubesImage cubesImage, Color color) : base(triangle, cubesImage, color)
        {
        }
    }
}