using System.Numerics;
using System.Windows.Media;

namespace Rendering.PolygonFill
{
    public class Gouraud : Polygon
    {
        public Color ColorA { get; set; }
        public Color ColorB { get; set; }
        public Color ColorC { get; set; }

        public override int GetColor(int x, int y)
        {
            var p = new Vector2(x, y);
            var a = Vertices[0].AsVector2;
            var b = Vertices[1].AsVector2;
            var c = Vertices[2].AsVector2;
            var bar = new Barycentric(a, b, c, p);

            return bar.Interpolate(ColorA, ColorB, ColorC).ToInt();
        }

        public Gouraud(Triangle triangle, CubesImage cubesImage, Color color) : base(triangle, cubesImage, color)
        {
            ColorA = CalculateColor(Triangle.A);
            ColorB = CalculateColor(Triangle.B);
            ColorC = CalculateColor(Triangle.C);
        }
    }
}