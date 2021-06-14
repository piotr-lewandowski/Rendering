namespace Rendering.PolygonFill
{
    public class Edge
    {
        public Edge(Vertex start, Vertex end)
        {
            Start = start;
            End = end;
            CurrentX = start.iX;
            MaxY = end.iY;
            SlopeX = end.iY - start.iY != 0
                ? (double) (end.iX - start.iX) / (end.iY - start.iY)
                : 0;
        }
        public readonly Vertex Start;
        public readonly Vertex End;
        public readonly double SlopeX;
        public readonly double MaxY;
        public double CurrentX;

        public override string ToString()
        {
            return $"{CurrentX}";
        }
    }
}
