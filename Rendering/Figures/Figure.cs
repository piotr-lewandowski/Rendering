using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Media;
using Rendering.PolygonFill;

namespace Rendering.Figures;

public abstract class Figure
{
    protected Figure(CubesImage canvas, Color color)
    {
        Canvas = canvas;
        Color = color;
    }

    public Vector3 TranslationVector { get; set; } = Vector3.Zero;
    public float Scale { get; set; } = 1;
    public Quaternion RotationQuaternion { get; set; } = Quaternion.Identity;
    public Matrix4x4 Translation => Matrix4x4.Transpose(Matrix4x4.CreateTranslation(TranslationVector));
    public Matrix4x4 Scaling => Matrix4x4.CreateScale(Scale);
    public Matrix4x4 Rotation => Matrix4x4.CreateFromQuaternion(RotationQuaternion);
    public Matrix4x4 Model => Translation * Scaling * Rotation;
    public CubesImage Canvas { get; }
    public IEnumerable<Triangle> Triangles { get; set; }
    public Color Color { get; }

    public void Draw()
    {
        var triangles = Triangles.Select(ToProjectionSpace);
        foreach (var triangle in triangles)
        {
            Fill(triangle, Color);
        }
    }

    public Vector3 NormalToProjectionSpace(Vector3 normal)
    {
        var matrix = Rotation;
        var vector = new Vector4(normal, 1);

        var resultVector = Utility.Multiply(matrix, vector);
        resultVector /= resultVector.W;

        return new Vector3(
            resultVector.X,
            resultVector.Y,
            resultVector.Z);
    }

    public Vector3 ToProjectionSpace(Vector3 original)
    {
        var matrix = Canvas.Projection * Canvas.View * Model;
        var vector = new Vector4(original, 1);

        var resultVector = Utility.Multiply(matrix, vector);
        resultVector /= resultVector.W;

        return new Vector3(
            (float)(resultVector.X * Canvas.ActualWidth + Canvas.ActualWidth) / 2,
            (float)(resultVector.Y * Canvas.ActualHeight + Canvas.ActualHeight) / 2,
            resultVector.Z);
    }

    public Vertex ToProjectionSpace(Vertex original) =>
        new(
            ToProjectionSpace(original.AsVector3),
            NormalToProjectionSpace(original.NormalVector)
        );

    public Triangle ToProjectionSpace(Triangle original) =>
        new(
            ToProjectionSpace(original.A),
            ToProjectionSpace(original.B),
            ToProjectionSpace(original.C)
        );

    public void Fill(Triangle triangle, Color color)
    {
        var viewVector = Canvas.CurrentCamera.Position - Canvas.CurrentCamera.Target;

        if (Canvas.BackFaceCulling && Vector3.Dot(viewVector, triangle.A.NormalVector) >= 0)
        {
            return;
        }

        Polygon polygon = Canvas.ShadingMode switch
        {
            ShadingMode.Phong => new Phong(triangle, Canvas, color),
            ShadingMode.Gouraud => new Gouraud(triangle, Canvas, color),
            ShadingMode.Constant => new Constant(triangle, Canvas, color),
            _ => throw new ArgumentOutOfRangeException($"Invalid shading mode value: {Canvas.ShadingMode}")
        };
        polygon.Fill();
    }
}