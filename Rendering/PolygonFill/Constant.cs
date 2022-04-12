using System.Windows.Media;

namespace Rendering.PolygonFill;

public class Constant : PolygonFiller
{
    public int ColorInt { get; }
    public override int GetColor(Barycentric bar, int x, int y) => ColorInt;

    public Constant(Triangle triangle, CubesImage cubesImage, Color color) : base(triangle, cubesImage, color) =>
        ColorInt = CalculateColor(Triangle.A).ToInt();
}