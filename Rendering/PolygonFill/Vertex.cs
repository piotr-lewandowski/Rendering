using System.Numerics;

namespace Rendering.PolygonFill
{
    internal class Vertex
    {
        public Vertex(Vector3 p, Vector3 normalVector)
        {
            NormalVector = normalVector;
            fX = p.X;
            fY = p.Y;
            fZ = p.Z;
            iX = (int) fX;
            iY = (int) fY;
            iZ = (int) fZ;
        }

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