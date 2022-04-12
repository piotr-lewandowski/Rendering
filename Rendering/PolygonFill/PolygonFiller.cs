using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Media;

namespace Rendering.PolygonFill;

public abstract class PolygonFiller
{
    private static readonly Comparison<Vertex> VertexComparison = (v1, v2) => v1.iY - v2.iY;

    private static readonly Comparison<ActiveEdge> ActiveEdgeComparison = (x, y) => (int)(x.CurrentX - y.CurrentX);

    protected PolygonFiller(Triangle triangle, CubesImage cubesImage, Color color)
    {
        Color = color;
        Triangle = triangle;
        CubesImage = cubesImage;
        Vertices = triangle.AsArray();
    }

    public Triangle Triangle { get; }
    public Vertex[] Vertices { get; set; }
    public List<ActiveEdge> ActiveEdges { get; set; }
    public CubesImage CubesImage { get; }
    public Color Color { get; }
    public float Ka => 0.3f;
    public float Kd => 0.5f;
    public float Ks => 0.3f;
    public float M => 40;

    public Color CalculateColor(Vertex vertex)
    {
        var resultIntensity = new Vector3(Ka);
        var toCamera = CubesImage.CurrentCamera.Target;
        toCamera = Vector3.Normalize(toCamera);

        foreach (var lightSourceBase in CubesImage.ActiveLightSources)
        {
            var lightSource = lightSourceBase.ToViewSpace(CubesImage);
            var lightIntensity = lightSource.Intensity(vertex.Position);
            var toLightSource = lightSource.LightVector(vertex.Position);
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

        var resR = (byte)(resultIntensity.X * Color.R);
        var resG = (byte)(resultIntensity.Y * Color.G);
        var resB = (byte)(resultIntensity.Z * Color.B);

        var fog = CubesImage.Fog
            ? Colors.White * (vertex.Position.Z - CubesImage.CurrentCamera.Position.Z)
            : Colors.Black;

        return Color.FromRgb(resR, resG, resB) + fog;
    }

    public void Fill()
    {
        Array.Sort(Vertices, VertexComparison);
        var n = Vertices.Length;
        ActiveEdges = new List<ActiveEdge>(n);
        var minY = Math.Max(Vertices[0].iY, 0);
        var maxY = Math.Min(Vertices[n - 1].iY, CubesImage.Bitmap.PixelHeight - 1);
        var j = 0;

        for (var currentY = minY; currentY <= maxY; ++currentY)
        {
            while (j < n && Vertices[j].iY <= currentY)
            {
                if (Vertices[j].Next.iY >= Vertices[j].iY)
                {
                    ActiveEdges.Add(new ActiveEdge(Vertices[j], Vertices[j].Next));
                }

                if (Vertices[j].Previous.iY >= Vertices[j].iY)
                {
                    ActiveEdges.Add(new ActiveEdge(Vertices[j], Vertices[j].Previous));
                }

                ++j;
            }

            UpdateAet(ActiveEdges, currentY);
        }
    }

    private void UpdateAet(List<ActiveEdge> aet, int currentY)
    {
        for (var i = 0; i < aet.Count; i++)
        {
            if (aet[i].MaxY <= currentY)
            {
                aet.RemoveAt(i);
            }
        }

        aet.Sort(ActiveEdgeComparison);

        for (var i = 0; i < aet.Count - 1; i += 2)
        {
            var startX = (int)aet[i].CurrentX;
            var endX = (int)aet[i + 1].CurrentX;
            for (var x = startX; x <= endX; ++x)
            {
                var bar = GetBarycentric(x, currentY);
                var z = InterpolateZ(bar);

                if (CubesImage.ZIndex[x, currentY] < z)
                {
                    continue;
                }

                CubesImage.ZIndex[x, currentY] = z;
                CubesImage.ColorsArray[x, currentY] = GetColor(bar, x, currentY);
            }
        }

        for (var i = 0; i < aet.Count; i++)
        {
            aet[i] = aet[i] with { CurrentX = aet[i].CurrentX + aet[i].SlopeX };
        }
    }

    public Barycentric GetBarycentric(int x, int y)
    {
        var p = new Vector2(x, y);
        var a = Vertices[0].AsVector2;
        var b = Vertices[1].AsVector2;
        var c = Vertices[2].AsVector2;
        return new Barycentric(a, b, c, p);
    }

    public abstract int GetColor(Barycentric barycentric, int x, int y);

    public float InterpolateZ(Barycentric barycentric) =>
        barycentric.Interpolate(Vertices[0].Position, Vertices[1].Position, Vertices[2].Position).Z;
}