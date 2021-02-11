using System.Numerics;
using System.Windows.Media;

namespace Rendering
{
    public class LightSource
    {
        public LightSource(Color color, Vector3 position)
        {
            Color = color;
            Position = position;
        }

        public Color Color { get; set; }
        public Vector3 Position { get; set; }
    }
}