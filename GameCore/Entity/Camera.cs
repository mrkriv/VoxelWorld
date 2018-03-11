using System;
using OpenTK;

namespace GameCore.Entity
{
    public class Camera : Entity
    {
        public static Camera ActiveCamera { get; set; }

        public Matrix4 ViewMatrix { get; set; }
        public Matrix4 ProjectionMatrix { get; set; }

        public override void OnTick(float dt)
        {
            ViewMatrix = Matrix4.LookAt(Position, Position + Forward, Vector3.UnitZ);

            base.OnTick(dt);
        }

        public void AddRotation(float x, float y)
        {
            Rotation = new Vector3(
                (Rotation.X - x) % (MathF.PI * 2.0f),
                Math.Max(Math.Min(Rotation.Y + y, MathF.PI / 2.0f - 0.1f), -MathF.PI / 2.0f + 0.1f),
                0
            );
        }
    }
}