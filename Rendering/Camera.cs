using System.Numerics;

namespace Rendering;

public class Camera
{
    public Camera(Vector3 position, Vector3 target, Vector3 up)
    {
        Position = position;
        Target = target;
        Up = up;
    }

    public Vector3 Position { get; set; }
    public Vector3 Target { get; set; }
    public Vector3 Up { get; set; }
    public Matrix4x4 ViewMatrix => Matrix4x4.Transpose(Matrix4x4.CreateLookAt(Position, Target, Up));
}