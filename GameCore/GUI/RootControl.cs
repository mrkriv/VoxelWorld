using System;
using GameCore.Entity;
using GameCore.Render;
using GameCore.Render.Materials;
using GameCore.Services;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameCore.GUI
{
    public class RootControl : Control
    {
        private uint _vaoHandle;

        public new readonly MaterialManager MaterialManager;
        public new readonly TextureManager TextureManager;
        public new readonly InputManager InputManager;
        public new readonly FontManager FontManager;
        public new World World { get; private set; }
        public readonly Config Config;

        public RootControl(
            TextureManager textureManager,
            MaterialManager materialManager,
            Config config,
            InputManager inputManager,
            FontManager fontManager)
        {
            MaterialManager = materialManager;
            TextureManager = textureManager;
            InputManager = inputManager;
            FontManager = fontManager;
            Config = config;

            RootControl = this;
            Parrent = this;
        }

        public void OnAttach(World world)
        {
            Material = MaterialManager.Load<UserInterface>();
            World = world;

            var vertexs = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            var texcoods = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            var indices = new[]
            {
                0, 1, 2, 1, 2, 3
            };

            // VBO
            GL.GenBuffers(1, out uint vertexHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexHandle);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(vertexs.Length * Vector2.SizeInBytes),
                vertexs, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out uint texcoodHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texcoodHandle);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(texcoods.Length * Vector2.SizeInBytes),
                texcoods, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out uint indexHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                new IntPtr(sizeof(int) * indices.Length),
                indices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // VAO
            GL.GenVertexArrays(1, out _vaoHandle);
            GL.BindVertexArray(_vaoHandle);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexHandle);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, true, Vector2.SizeInBytes, 0);
            Material.BindInVertexPosition();

            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texcoodHandle);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, true, Vector2.SizeInBytes, 0);
            Material.BindInVertexTexcood();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexHandle);

            GL.BindVertexArray(0);
        }

        public override void OnAttach()
        {
            throw new InvalidOperationException("Use OnAttach(World) owerload");
        }

        public void OnRender()
        {
            OnRender(new Vector2());
        }
        
        public override void OnRender(Vector2 parrentPosition)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            
            GL.BindVertexArray(_vaoHandle);
            GL.ActiveTexture(TextureUnit.Texture0);
            
            foreach (var child in Childs)
            {
                if (child.IsVisiable)
                    child.OnRender(new Vector2());
            }

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}