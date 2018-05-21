using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GameCore.Render.Materials
{
    public class UserInterface : MaterialBase
    {
        public new static string DefaultShaderName => "gui";
        
        [MateriaParam] public Matrix3 Screen { get; set; }
        [MateriaParam] public Matrix3 Transform { get; set; }
        [MateriaParam] public Matrix3 TexcoodTransform { get; set; }
        [MateriaParam] public Texture Texture1 { get; set; }
        [MateriaParam] public Color4 Color { get; set; }
        [MateriaParam] public bool UseTexture { get; set; }
        
        public UserInterface(string fsPath, string vsPath) : base(fsPath, vsPath)
        {
        }

        public override void AppyGlobal()
        {
            Screen = new Matrix3(
                new Vector3(2, 0, 0),
                new Vector3(0, -2, 0),
                new Vector3(-1, 1, 1));
        }

        public override void BindInVertexPosition()
        {
            GL.BindAttribLocation(Handle, 0, "in_position");
        }

        public override void BindInVertexNormal()
        {
            throw new System.NotSupportedException();
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