using System.Collections.Generic;
using System.Numerics;

namespace Rendering.PolygonFill;

public readonly struct Triangle
{
    public Triangle(Vertex a, Vertex b, Vertex c)
    {
        A = a;
        B = b;
        C = c;
        Vertices = new[] { A, B, C };

        var n = 3;

        for (var i = 0; i < 3; ++i)
        {
            Vertices[i].Next = i + 1 < n ? Vertices[i + 1] : Vertices[0];
            Vertices[i].Previous = i > 0 ? Vertices[i - 1] : Vertices[n - 1];
        }
    }

    public Vertex A { get; }
    public Vertex B { get; }
    public Vertex C { get; }
    public Vertex[] Vertices { get; }

    public IEnumerable<Triangle> SubdivideAndNormalize()
    {
        var a = A.Position;
        var b = B.Position;
        var c = C.Position;

        var ab = (a + b) / 2;
        var bc = (b + c) / 2;
        var ca = (c + a) / 2;

        ab = Vector3.Normalize(ab);
        bc = Vector3.Normalize(bc);
        ca = Vector3.Normalize(ca);

        var AB = new Vertex(ab, (A.NormalVector + B.NormalVector) / 2);
        var BC = new Vertex(bc, (B.NormalVector + C.NormalVector) / 2);
        var CA = new Vertex(ca, (C.NormalVector + A.NormalVector) / 2);

        var t1 = new Triangle(A, AB, CA);
        var t2 = new Triangle(AB, B, BC);
        var t3 = new Triangle(AB, BC, CA);
        var t4 = new Triangle(CA, BC, C);

        return new[] { t1, t2, t3, t4 };
    }

    public IEnumerable<Triangle> Subdivide()
    {
        var a = A.Position;
        var b = B.Position;
        var c = C.Position;

        var ab = (a + b) / 2;
        var bc = (b + c) / 2;
        var ca = (c + a) / 2;

        var AB = new Vertex(ab, A.NormalVector);
        var BC = new Vertex(bc, B.NormalVector);
        var CA = new Vertex(ca, C.NormalVector);

        var t1 = new Triangle(A, AB, CA);
        var t2 = new Triangle(AB, B, BC);
        var t3 = new Triangle(AB, BC, CA);
        var t4 = new Triangle(CA, BC, C);

        return new[] { t1, t2, t3, t4 };
    }

    public Vertex[] AsArray() => Vertices;
}