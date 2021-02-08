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
        protected Vertex[] _vertices;
        protected List<Edge> _activeEdges;
        private CubesImage cubesImage;
        private int colorInt;
        public Polygon(Vector3[] points)
        {
            var n = points.Length;
            _vertices = points.Select(p => new Vertex(p)).ToArray();

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
                    cubesImage.ColorsArray[x, currentY] = colorInt;
                }
            }

            foreach (var edge in aet)
            {
                edge.CurrentX += edge.SlopeX;
            }
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
