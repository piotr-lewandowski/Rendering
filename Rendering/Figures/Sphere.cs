﻿using System.Linq;
using System.Numerics;
using System.Windows.Media;

namespace Rendering.Figures
{
    public class Sphere : Figure
    {
        public Sphere(CubesImage canvas, Color color) : base(canvas, color)
        {
            var points = new[]
            {
                new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector3(0, 1, 1),
                new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 0), new Vector3(1, 1, 1)
            };
            points = points.Select(p => p - new Vector3(0.5f)).Select(Vector3.Normalize).ToArray();

            var triangles = new[]
            {
                new[] {points[0], points[3], points[1]},
                new[] {points[0], points[2], points[3]},

                new[] {points[4], points[5], points[6]},
                new[] {points[5], points[7], points[6]},

                new[] {points[1], points[3], points[7]},
                new[] {points[1], points[7], points[5]},

                new[] {points[0], points[4], points[2]},
                new[] {points[2], points[4], points[6]},

                new[] {points[2], points[6], points[3]},
                new[] {points[3], points[6], points[7]},

                new[] {points[0], points[1], points[5]},
                new[] {points[0], points[5], points[4]}
            };

            Triangles = triangles.SelectMany(t => new Triangle(t[0], t[1], t[2]).SubdivideAndNormalize())
                .SelectMany(t => t.SubdivideAndNormalize())
                .SelectMany(t => t.SubdivideAndNormalize());
        }
    }
}