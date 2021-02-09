using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Media;

namespace Rendering.PolygonFill
{
    class Polygon
    {
        public Vector3 NormalVector { get; }
        protected Vertex[] _vertices;
        protected List<Edge> _activeEdges;
        private CubesImage cubesImage;
        private int colorInt;
        private Color _color;
        public Polygon(Vector3[] points, Vector3 normalVector)
        {
            NormalVector = normalVector;
            var n = points.Length;
            _vertices = points.Select(p => new Vertex(p,normalVector)).ToArray();

            for (int i = 0; i < n; ++i)
            {
                _vertices[i].Next = i + 1 < n ? _vertices[i + 1] : _vertices[0];
                _vertices[i].Previous = i > 0 ? _vertices[i - 1] : _vertices[n - 1];
            }
        }
        public void Fill(CubesImage image, Color color)
        {
            cubesImage = image;

            int tmpColor = color.R << 16; // R
            tmpColor |= color.G << 8;   // G
            tmpColor |= color.B << 0;   // B
            _color = color;
            colorInt = tmpColor;

            _vertices = _vertices.OrderBy(v => v.iY).ToArray();
            _activeEdges = new List<Edge>();
            int n = _vertices.Length;
            int minY = _vertices[0].iY;
            int maxY = _vertices.Last().iY;
            int j = 0;

            for (int currentY = minY; currentY <= maxY; ++currentY)
            {
                while (j < n && Math.Abs(_vertices[j].iY - currentY) < 1e-3)
                {
                    if (_vertices[j].Next.iY >= _vertices[j].iY)
                    {
                        _activeEdges.Add(new Edge(_vertices[j], _vertices[j].Next));
                    }
                    if (_vertices[j].Previous.iY >= _vertices[j].iY)
                    {
                        _activeEdges.Add(new Edge(_vertices[j], _vertices[j].Previous));
                    }
                    ++j;
                }
                UpdateAet(_activeEdges, currentY);
            }
        }

        class EdgeXComparison : Comparer<Edge>
        {
            public override int Compare(Edge x, Edge y)
            {
               return (int) (x.CurrentX - y.CurrentX);
            }
        }

        private void UpdateAet(List<Edge> aet, int currentY)
        {
            aet.RemoveAll(edge => edge.MaxY <= currentY);
            aet.Sort(new EdgeXComparison());

            for (int i = 0; i < aet.Count - 1; i += 2)
            {
                var startX = (int) aet[i].CurrentX;
                var endX = (int) aet[i + 1].CurrentX;
                for (var x = startX; x <= endX; ++x)
                {
                    var z = InterpolateZ(x, currentY);
                    
                    if (x >= cubesImage.Bitmap.PixelWidth || currentY >= cubesImage.Bitmap.PixelHeight || x < 0 || currentY < 0 || !(cubesImage.ZIndex[x, currentY] > z))
                        continue;

                    cubesImage.ZIndex[x, currentY] = z;
                    cubesImage.ColorsArray[x, currentY] = GetColor(_vertices[0], _color);
                }
            }

            foreach (var edge in aet)
            {
                edge.CurrentX += edge.SlopeX;
            }
        }


        private double Ka { get; } = 0.2d;
        private double Kd { get; } = 0.7d;
        private double Ks { get; } = 0.5d;
        private double M { get; } = 50;

        private int GetColor(Vertex vertex, Color color)
        {
            var lightColor = Colors.White;
            var il = new Vector3(lightColor.R, lightColor.G, lightColor.B);
            il = il / 255;
            var io = color;
            var l =  -new Vector3(vertex.fX, vertex.fY+200, vertex.fZ-1000);
            var n = vertex.NormalVector;
            var v = new Vector3(0, 0, 1);

            l = Vector3.Normalize(l);
            n = Vector3.Normalize(n);

            var r = new Vector3(
                (2 * Vector3.Dot(n, l) * n.X) - l.X,
                (2 * Vector3.Dot(n, l) * n.Y) - l.Y,
                (2 * Vector3.Dot(n, l) * n.Z) - l.Z);
            r = Vector3.Normalize(r);

            var pow = Math.Pow(Vector3.Dot(v, r), M);
            var dot = Math.Abs(Vector3.Dot(n, l));

            var calculatedR = Ka * color.R + Kd * il.X * io.R * dot + Ks * il.X * io.R * pow;
            var calculatedG = Ka * color.G + Kd * il.Y * io.G * dot + Ks * il.Y * io.G * pow;
            var calculatedB = Ka * color.B + Kd * il.Z * io.B * dot + Ks * il.Z * io.B * pow;

            calculatedR = Math.Abs(calculatedR);
            calculatedG = Math.Abs(calculatedG);
            calculatedB = Math.Abs(calculatedB);

            var resR = calculatedR > 255 ? (byte) 255 : (byte) calculatedR;
            var resG = calculatedG > 255 ? (byte) 255 : (byte) calculatedG;
            var resB = calculatedB > 255 ? (byte) 255 : (byte) calculatedB;


            int tmpColor = resR << 16; // R
            tmpColor |= resG << 8;   // G
            tmpColor |= resB << 0;   // B

            return tmpColor;
        }

        private float InterpolateZ(int x, int y)
        {
            var p = new Point(x, y);
            var a = new Point(_vertices[0].fX, _vertices[0].fY);
            var b = new Point(_vertices[1].fX, _vertices[1].fY);
            var c = new Point(_vertices[2].fX, _vertices[2].fY);
            var v0 = b - a;
            var v1 = c - a;
            var v2 = p - a;
            var d00 = Dot(v0, v0);
            var d01 = Dot(v0, v1);
            var d11 = Dot(v1, v1);
            var d20 = Dot(v2, v0);
            var d21 = Dot(v2, v1);
            var denom = d00 * d11 - d01 * d01;
            var v = (d11 * d20 - d01 * d21) / denom;
            var w = (d00 * d21 - d01 * d20) / denom;
            var u = 1.0f - v - w;

            return (float) (u * _vertices[0].fZ + v * _vertices[1].fZ + w * _vertices[2].fZ);

            static double Dot(System.Windows.Vector a, System.Windows.Vector b)
            {
                return (a.X * b.X + a.Y * b.Y);
            }
        }
    }
}
