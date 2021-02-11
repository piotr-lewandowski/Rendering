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
        private Vertex[] _vertices;
        private List<Edge> _activeEdges;
        private CubesImage _cubesImage;
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
            _cubesImage = image;
            _color = color;

            _vertices = _vertices.OrderBy(v => v.iY).ToArray();
            _activeEdges = new List<Edge>();
            int n = _vertices.Length;
            int minY = _vertices[0].iY;
            int maxY = _vertices.Last().iY;
            int j = 0;

            for (int currentY = minY; currentY <= maxY; ++currentY)
            {
                while (j < n && _vertices[j].iY <= currentY)
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
                    
                    if (x >= _cubesImage.Bitmap.PixelWidth || currentY >= _cubesImage.Bitmap.PixelHeight || x < 0 || currentY < 0 || !(_cubesImage.ZIndex[x, currentY] > z))
                        continue;

                    _cubesImage.ZIndex[x, currentY] = z;
                    _cubesImage.ColorsArray[x, currentY] = GetColor(_vertices[0], _color);
                }
            }

            foreach (var edge in aet)
            {
                edge.CurrentX += edge.SlopeX;
            }
        }


        private double Ka { get; } = 0.3d;
        private double Kd { get; } = 0.5d;
        private double Ks { get; } = 0.3d;
        private double M { get; } = 10;

        private int GetColor(Vertex vertex, Color color)
        {
            var lightColor = Colors.White;
            var il = new Vector3(lightColor.R, lightColor.G, lightColor.B);
            il /= 255;
            var toLightSource = vertex.AsVector3 - new Vector3(30, 0, 0);
            var surfaceNormal = vertex.NormalVector;
            var toCamera = new Vector3(3, 0.5f, 0.5f) - vertex.AsVector3;

            toLightSource = -Vector3.Normalize(toLightSource);
            surfaceNormal = Vector3.Normalize(surfaceNormal);
            toCamera = Vector3.Normalize(toCamera);

            var reflected = Vector3.Reflect(toLightSource, surfaceNormal);
            reflected = Vector3.Normalize(reflected);

            var dot = Vector3.Dot(surfaceNormal, toLightSource);
            dot = Math.Clamp(dot, 0, 1);

            var dot1 = Vector3.Dot(toCamera, reflected);
            dot1 = Math.Clamp(dot1, 0, 1);
            var pow = dot > 0 ? Math.Pow(dot1, M) : 0;

            var calculatedR = Ka + Kd * il.X * dot + Ks * il.X * pow;
            var calculatedG = Ka + Kd * il.Y * dot + Ks * il.Y * pow;
            var calculatedB = Ka + Kd * il.Z * dot + Ks * il.Z * pow;

            calculatedR = Math.Abs(calculatedR * color.R);
            calculatedG = Math.Abs(calculatedG * color.G);
            calculatedB = Math.Abs(calculatedB * color.B);

            var resR = (byte) calculatedR;
            var resG = (byte) calculatedG;
            var resB = (byte) calculatedB;

            var tmpColor = resR << 16; // R
            tmpColor |= resG << 8;   // G
            tmpColor |= resB << 0;   // B

            return tmpColor;
        }

        private float InterpolateZ(int x, int y)
        {
            var p = new Vector2(x, y);
            var a = _vertices[0].AsVector2;
            var b = _vertices[1].AsVector2;
            var c = _vertices[2].AsVector2;
            var bar = new Barycentric(a, b, c, p);

            return bar.Interpolate(_vertices[0].AsVector3, _vertices[1].AsVector3, _vertices[2].AsVector3).Z;
        }

    }
}
