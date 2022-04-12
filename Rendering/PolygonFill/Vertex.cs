using System.Numerics;

namespace Rendering.PolygonFill;

public class Vertex
{
    public Vertex(Vector3 point, Vector3 normalVector)
    {
        NormalVector = normalVector;
        iX = (int)point.X;
        iY = (int)point.Y;
        Position = point;
        AsVector2 = new Vector2(point.X, point.Y);
    }

    public Vector3 Position { get; }
    public Vector2 AsVector2 { get; }
    public Vector3 NormalVector { get; }
    public Vertex Next { get; set; }
    public Vertex Previous { get; set; }
    public int iX { get; }
    public int iY { get; }

    public override string ToString() => $"({iX}, {iY})";
}