using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using GameCore.Additional.JsonConverters;
using GameCore.Entity;
using GameCore.GUI;
using GameCore.Services;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GameCore.Render
{
    public class AppWindow : GameWindow
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly MaterialManager _materialManager;
        private readonly TextureManager _textureManager;
        private readonly InputManager _inputManager;
        private readonly FontManager _fontManager;
        private readonly RootControl _rootControl;
        private readonly World _world;

        public AppWindow(
            World world,
            FontManager fontManager,
            RootControl rootControl,
            InputManager inputManager,
            TextureManager textureManager,
            MaterialManager materialManager)
            : base(720, 480, GraphicsMode.Default, "Voxel World")
        {
            VSync = VSyncMode.On;

            _rootControl = rootControl;
            _materialManager = materialManager;
            _fontManager = fontManager;
            _inputManager = inputManager;
            _world = world;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Converters = new List<JsonConverter>    // todo: придумать что то более удобное
                {
                    new TextureConverter(textureManager),
                    new FontConverter(_fontManager),
                    new GuiVectorConverter(),
                    new ColorConverter(),
                    new RangeConverter()
                }
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Fog);
            GL.Fog(FogParameter.FogColor, new[] {0.1f, 0.2f, 0.5f, 1.0f});
            GL.Fog(FogParameter.FogMode, (int) FogMode.Linear);
            GL.Fog(FogParameter.FogEnd, 500);
            
            _inputManager.ScreenDeviceSize = new Vector2(DisplayDevice.Default.Width, DisplayDevice.Default.Height);
            _inputManager.Keyboard = Keyboard;
            _inputManager.Mouse = Mouse;
            
            Cursor = MouseCursor.Empty;
            CursorVisible = false;

            _world.OnLoad();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);
            _inputManager.ScreenSize = new Vector2(Width, Height);
            
            var matrix = Matrix4.CreatePerspectiveFieldOfView(1.0f, (Width / (float) Height), 1.0f, 10000.0f);
            Camera.ActiveCamera.ProjectionMatrix = matrix;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref matrix);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            var dt = _stopwatch.ElapsedMilliseconds / 1000.0f;
            _stopwatch.Restart();
            
            _world.OnTick(dt);
            _rootControl.OnTick(dt);
            
            if (Keyboard[Key.Escape])
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _materialManager.AppyGlobal();
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (Camera.ActiveCamera == null)
                return;

            _world.OnRender();
            _rootControl.OnRender();
            
            SwapBuffers();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _world.OnDestroy();
        }
    }
}