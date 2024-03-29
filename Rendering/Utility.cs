﻿using System;
using System.Numerics;
using System.Windows.Media;

namespace Rendering;

public static class Utility
{
    public static Matrix4x4 ProjectionMatrix(float fov, float a, float n = 1, float f = 100)
    {
        fov = ToRadians(fov);
        return Matrix4x4.Transpose(Matrix4x4.CreatePerspectiveFieldOfView(fov, a, n, f));
    }

    public static float ToRadians(this float degrees) => degrees * MathF.PI / 180;

    public static int ToInt(this Color color)
    {
        var tmpColor = color.R << 16;
        tmpColor |= color.G << 8;
        tmpColor |= color.B;

        return tmpColor;
    }

    public static Vector4 Multiply(Matrix4x4 matrix, Vector4 vector)
    {
        var x = matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z + matrix.M14 * vector.W;
        var y = matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z + matrix.M24 * vector.W;
        var z = matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z + matrix.M34 * vector.W;
        var m = matrix.M41 * vector.X + matrix.M42 * vector.Y + matrix.M43 * vector.Z + matrix.M44 * vector.W;

        return new Vector4(x, y, z, m);
    }
}