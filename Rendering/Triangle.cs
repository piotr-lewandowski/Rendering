using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Rendering.PolygonFill;

namespace Rendering
{
    public readonly struct Triangle
    {
        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Vector3 A { get; }
        public Vector3 B { get; }
        public Vector3 C { get; }

        public IEnumerable<Triangle> SubdivideAndNormalize()
        {
            var ab = (A + B) / 2;
            var bc = (B + C) / 2;
            var ca = (C + A) / 2;
            
            ab = Vector3.Normalize(ab);
            bc = Vector3.Normalize(bc);
            ca = Vector3.Normalize(ca);

            var t1 = new Triangle(A, ab, ca);
            var t2 = new Triangle(ab, B, bc);
            var t3 = new Triangle(ab, bc, ca);
            var t4 = new Triangle(ca, bc, C);

            return new[] { t1, t2, t3, t4 };
        }
        public IEnumerable<Triangle> Subdivide()
        {
            var ab = (A + B) / 2;
            var bc = (B + C) / 2;
            var ca = (C + A) / 2;

            var t1 = new Triangle(A, ab, ca);
            var t2 = new Triangle(ab, B, bc);
            var t3 = new Triangle(ab, bc, ca);
            var t4 = new Triangle(ca, bc, C);

            return new[] { t1, t2, t3, t4 };
        }

    }
}
