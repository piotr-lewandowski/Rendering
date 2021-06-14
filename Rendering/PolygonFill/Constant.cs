using System.Windows.Media;

namespace Rendering.PolygonFill
{
    public class Constant : Polygon
    {
        public int ColorInt { get; }
        public override int GetColor(int x, int y) => ColorInt;

        public Constant(Triangle triangle, CubesImage cubesImage, Color color) : base(triangle, cubesImage, color)
        {
            ColorInt = CalculateColor(Triangle.A).ToInt();
        }
    }
}