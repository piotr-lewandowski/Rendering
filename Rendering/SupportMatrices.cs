using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Rendering
{
    static class SupportMatrices
    {
        public static Matrix4x4 ViewMatrix { get; } = new (
            0, 1, 0, -0.5f,
            0, 0, 1, -0.5f,
            1, 0, 0, -3,
            0, 0, 0, 1);

        public static Matrix4x4 ModelMatrix = Matrix4x4.Identity;

        public static Matrix4x4 ModelMatrix1(float angle)
        {
            angle = ToRadians(angle);

            return new Matrix4x4(
                MathF.Cos(angle), -MathF.Sin(angle), 0, 0.2f,
                MathF.Sin(angle), MathF.Cos(angle), 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }

        public static Matrix4x4 ModelMatrix2(float angle)
        {
            angle = ToRadians(angle);
            return new Matrix4x4(
                1, 0, 0, 0,
                0, MathF.Cos(2*angle), -MathF.Sin(2*angle), 0,
                0, MathF.Sin(2*angle), MathF.Cos(2*angle),  0,
                0, 0, 0, 1);
        }

        public static Matrix4x4 ProjectionMatrix(float fov, float a, float n = 1, float f = 100)
        {
            fov = ToRadians(fov);
            return Matrix4x4.CreatePerspectiveFieldOfView(fov, a, n, f);
        }

        public static float ToRadians(float degrees)
        {
            return degrees * MathF.PI / 180;
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
}
