using System.Linq;
using System.Numerics;
using System.Windows.Media;

namespace Rendering.Figures
{
    public class Square : Figure
    {
        public Square(CubesImage canvas, Color color) : base(canvas, color)
        {
            var points = new[]
            {
                new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector3(0, 1, 1),
                new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 0), new Vector3(1, 1, 1)
            };
            points = points.Select(p => p - new Vector3(0.5f)).ToArray();

            var triangles = new[]
            {
                new[] {points[0], points[1], points[5]},
                new[] {points[0], points[5], points[4]},

                new[] {points[5], points[1], points[0]},
                new[] {points[4], points[5], points[0]}
            };

            Triangles = triangles.SelectMany(t => new Triangle(t[0], t[1], t[2]).Subdivide())
                .SelectMany(t => t.Subdivide())
                .SelectMany(t => t.Subdivide());
        }
    }
}