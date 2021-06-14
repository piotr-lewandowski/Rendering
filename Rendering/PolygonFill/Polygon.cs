using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Media;

namespace Rendering.PolygonFill
{
    public abstract class Polygon
    {
        public Triangle Triangle { get; }
        public Vertex[] Vertices  { get; set; }
        public List<Edge> ActiveEdges { get; set; }
        public CubesImage CubesImage { get; }
        public Color Color { get; }
        public float Ka { get; } = 0.3f;
        public float Kd { get; } = 0.5f;
        public float Ks { get; } = 0.3f;
        public float M { get; } = 40;
        protected Polygon(Triangle triangle, CubesImage cubesImage, Color color)
        {
            Color = color;
            Triangle = triangle;
            CubesImage = cubesImage;
            Vertices = triangle.AsArray();
        }

        public Color CalculateColor(Vertex vertex)
        {
            var resultIntensity = new Vector3(Ka);
            var toCamera = CubesImage.CurrentCamera.Target;
            toCamera = Vector3.Normalize(toCamera);

            foreach (var lightSource in CubesImage.ActiveLightSources.Select(s => s.ToViewSpace(CubesImage)))
            {
                var lightIntensity = lightSource.Intensity(vertex.AsVector3);
                var toLightSource = lightSource.LightVector(vertex.AsVector3);
                var surfaceNormal = vertex.NormalVector;
                toLightSource = -Vector3.Normalize(toLightSource);
                surfaceNormal = Vector3.Normalize(surfaceNormal);

                var reflected = Vector3.Reflect(toLightSource, surfaceNormal);
                reflected = Vector3.Normalize(reflected);

                var diffuseAngle = Vector3.Dot(surfaceNormal, toLightSource);
                diffuseAngle = Math.Clamp(diffuseAngle, 0, 1);

                var specularAngle = Vector3.Dot(toCamera, reflected);
                specularAngle = Math.Clamp(specularAngle, 0, 1);
                var reflection = diffuseAngle > 0 ? MathF.Pow(specularAngle, M) : 0;

                resultIntensity += Kd * lightIntensity * diffuseAngle + Ks * lightIntensity * reflection;
            }

            var resR = (byte) (resultIntensity.X * Color.R);
            var resG = (byte) (resultIntensity.Y * Color.G);
            var resB = (byte) (resultIntensity.Z * Color.B);

            var fog = CubesImage.Fog
                ? Colors.White * (vertex.AsVector3.Z - CubesImage.CurrentCamera.Position.Z)
                : Colors.Black;

            return Color.FromRgb(resR, resG, resB) + fog;
        }
        public void Fill()
        {
            Vertices = Vertices.OrderBy(v => v.iY).ToArray();
            ActiveEdges = new List<Edge>();
            int n = Vertices.Length;
            int minY = Vertices[0].iY;
            int maxY = Vertices.Last().iY;
            int j = 0;

            for (int currentY = minY; currentY <= maxY; ++currentY)
            {
                while (j < n && Vertices[j].iY <= currentY)
                {
                    if (Vertices[j].Next.iY >= Vertices[j].iY)
                    {
                        ActiveEdges.Add(new Edge(Vertices[j], Vertices[j].Next));
                    }
                    if (Vertices[j].Previous.iY >= Vertices[j].iY)
                    {
                        ActiveEdges.Add(new Edge(Vertices[j], Vertices[j].Previous));
                    }
                    ++j;
                }
                UpdateAet(ActiveEdges, currentY);
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

            for (var i = 0; i < aet.Count - 1; i += 2)
            {
                var startX = (int) aet[i].CurrentX;
                var endX = (int) aet[i + 1].CurrentX;
                for (var x = startX; x <= endX; ++x)
                {
                    var z = InterpolateZ(x, currentY);
                    
                    if (x >= CubesImage.Bitmap.PixelWidth || currentY >= CubesImage.Bitmap.PixelHeight || x < 0 || currentY < 0 || !(CubesImage.ZIndex[x, currentY] > z))
                        continue;

                    CubesImage.ZIndex[x, currentY] = z;
                    CubesImage.ColorsArray[x, currentY] = GetColor(x, currentY);
                }
            }

            foreach (var edge in aet)
            {
                edge.CurrentX += edge.SlopeX;
            }
        }

        public abstract int GetColor(int x, int y);

        public float InterpolateZ(int x, int y)
        {
            var p = new Vector2(x, y);
            var a = Vertices[0].AsVector2;
            var b = Vertices[1].AsVector2;
            var c = Vertices[2].AsVector2;
            var bar = new Barycentric(a, b, c, p);

            return bar.Interpolate(Vertices[0].AsVector3, Vertices[1].AsVector3, Vertices[2].AsVector3).Z;
        }

    }
}
