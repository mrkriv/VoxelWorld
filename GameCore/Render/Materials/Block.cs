using GameCore.Entity;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameCore.Render.Materials
{
    public class Block : MaterialBase
    {
        public new static string DefaultShaderName => "block";

        [MateriaParam] public Matrix4 Model { get; set; }
        [MateriaParam] public Matrix4 View { get; set; }
        [MateriaParam] public Matrix4 Projection { get; set; }
        [MateriaParam] public Texture DiffTexture { get; set; }

        public Block(string fsPath, string vsPath) : base(fsPath, vsPath)
        {
            Model = Matrix4.Identity;
        }

        public override void AppyGlobal()
        {
            Projection = Camera.ActiveCamera.ProjectionMatrix;
            View = Camera.ActiveCamera.ViewMatrix;
        }

        public override void BindInVertexPosition()
        {
            GL.BindAttribLocation(Handle, 0, "in_position");
        }

        public override void BindInVertexNormal()
        {
            GL.BindAttribLocation(Handle, 0, "in_normal");
        }

        public override void BindInVertexColor()
        {
            throw new System.NotSupportedException();
        }

        public override void BindInVertexTexcood()
        {
            GL.BindAttribLocation(Handle, 0, "in_texcood");
        }
    }
}