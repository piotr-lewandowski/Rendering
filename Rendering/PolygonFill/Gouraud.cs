using System.Windows.Media;

namespace Rendering.PolygonFill;

public class Gouraud : PolygonFiller
{
    public Color ColorA { get; set; }
    public Color ColorB { get; set; }
    public Color ColorC { get; set; }

    public override int GetColor(Barycentric barycentric, int x, int y) =>
        barycentric.Interpolate(ColorA, ColorB, ColorC).ToInt();

    public Gouraud(Triangle triangle, CubesImage cubesImage, Color color) : base(triangle, cubesImage, color)
    {
        ColorA = CalculateColor(Triangle.A);
        ColorB = CalculateColor(Triangle.B);
        ColorC = CalculateColor(Triangle.C);
    }
}