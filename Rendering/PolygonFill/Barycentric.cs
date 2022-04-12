using System.Numerics;
using System.Windows.Media;

namespace Rendering.PolygonFill;

// source:
// http://wiki.unity3d.com/index.php?title=Barycentric
public readonly struct Barycentric
{
    public float U { get; }
    public float V { get; }
    public float W { get; }

    public Barycentric(Vector2 aV1, Vector2 aV2, Vector2 aV3, Vector2 aP)
    {
        Vector2 a = aV2 - aV3, b = aV1 - aV3, c = aP - aV3;
        var aLen = a.X * a.X + a.Y * a.Y;
        var bLen = b.X * b.X + b.Y * b.Y;
        var ab = a.X * b.X + a.Y * b.Y;
        var ac = a.X * c.X + a.Y * c.Y;
        var bc = b.X * c.X + b.Y * c.Y;
        var d = aLen * bLen - ab * ab;
        U = (aLen * bc - ab * ac) / d;
        V = (bLen * ac - ab * bc) / d;
        W = 1.0f - U - V;
    }

    public Vector3 Interpolate(Vector3 v1, Vector3 v2, Vector3 v3) => v1 * U + v2 * V + v3 * W;

    public Color Interpolate(Color v1, Color v2, Color v3) => v1 * U + v2 * V + v3 * W;
}