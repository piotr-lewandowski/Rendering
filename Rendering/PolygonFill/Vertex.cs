using System.Numerics;

namespace Rendering.PolygonFill
{
    public class Vertex
    {
        public Vertex(Vector3 point, Vector3 normalVector)
        {
            NormalVector = normalVector;
            fX = point.X;
            fY = point.Y;
            fZ = point.Z;
            iX = (int) fX;
            iY = (int) fY;
            iZ = (int) fZ;
            AsVector3 = new Vector3(point.X, point.Y, point.Z);
            AsVector2 = new Vector2(point.X, point.Y);
        }

        public Vector3 AsVector3 { get; }
        public Vector2 AsVector2 { get; }
        public Vector3 NormalVector { get; }
        public Vertex Next { get; set; }
        public Vertex Previous { get; set; }
        public int iX { get; }
        public int iY { get; }
        public int iZ { get; }
        public float fX { get; }
        public float fY { get; }
        public float fZ { get; }

        public override string ToString()
        {
            return $"({iX}, {iY})";
        }
    }
}