using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameCore.Entity
{
    public class Grid : Entity
    {
        public override void OnRender()
        {
            base.OnRender();
            
            GL.UseProgram(0);
            
            var cellSize = 16;
            var gridSize = 1600;
            var ratio = gridSize / cellSize;

            var modelview = Camera.ActiveCamera.ViewMatrix;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            
            GL.PushMatrix();
            GL.Translate(gridSize * -.5f, gridSize * -.5f, 0);

            GL.Color3(Color.Gray);
            GL.Begin(PrimitiveType.Lines);

            for (var i = 0; i < ratio + 1; i++)
            {
                var current = i * cellSize;

                if (Math.Abs(current - gridSize * .5f) < float.Epsilon)
                    continue;

                GL.Vertex3(current, 0, 0);
                GL.Vertex3(current, gridSize, 0);

                GL.Vertex3(0, current, 0);
                GL.Vertex3(gridSize, current, 0);
            }

            GL.Color3(Color.Red);
            GL.Vertex3(gridSize * .5f, gridSize * .5f, 0);
            GL.Vertex3(gridSize, gridSize * .5f, 0);

            GL.Color3(Color.Green);
            GL.Vertex3(gridSize * .5f, gridSize * .5f, 0);
            GL.Vertex3(gridSize * .5f, gridSize, 0);

            GL.Color3(Color.Blue);
            GL.Vertex3(gridSize * .5f, gridSize * .5f, gridSize * .5f);
            GL.Vertex3(gridSize * .5f, gridSize * .5f, gridSize * .5f);
            GL.End();

            GL.PopMatrix();
        }
    }
}