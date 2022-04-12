namespace Rendering.PolygonFill;

public record struct ActiveEdge(Vertex Start, Vertex End)
{
    public double SlopeX { get; } = End.iY - Start.iY != 0
        ? (double)(End.iX - Start.iX) / (End.iY - Start.iY)
        : 0;

    public double MaxY { get; } = End.iY;
    public double CurrentX { get; init; } = Start.iX;

    public override string ToString() => $"{CurrentX}";
}