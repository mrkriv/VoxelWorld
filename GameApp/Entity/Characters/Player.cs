using System;
using GameApp.EMath;
using GameApp.Entity.Global;
using GameCore.EMath;
using GameCore.Entity;
using GameCore.GUI;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GameApp.Entity.Characters
{
    public class Player : Actor
    {
        private RayTraceResult _forwardRayTrace;
        private TextControl _logButtom;
        private TextControl _logTop;

        public Camera PlayerCamera { get; set; }
        public float Speed { get; set; }

        public Player()
        {
            PlayerCamera = new Camera();
            PlayerCamera.AttachTo(this);
            Camera.ActiveCamera = PlayerCamera;

            Position = new Vector3(0, -160, 400);
            Name = "Player";
            Speed = 200;
        }

        public override void OnBeginPlay()
        {
            base.OnBeginPlay();

            World.InputManager.Keyboard.KeyDown += (_, e) => OnKeyDown(e.Key);
            World.InputManager.Mouse.ButtonDown += (_, e) => OnMouseDown(e.Button);

            _logTop = new TextControl {Name = "Player.TestControl"};
            _logButtom = new TextControl {Name = "Player.TestControl2"};
            _logButtom.Position = new GuiVector(GuiVectorType.Screen, 0.01f, 0.95f);

            World.RootControl.AttachControl(_logTop);
            World.RootControl.AttachControl(_logButtom);
        }

        private void OnMouseDown(MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                if (_forwardRayTrace.Chunk != null && _forwardRayTrace.Chunk.IsVisiable)
                {
                    _forwardRayTrace.Chunk.SetBlock(_forwardRayTrace.BlockChunkPosition, Block.FindByName("void"));
                }
            }
        }

        private void OnKeyDown(Key key)
        {
            if (key == Key.BackSpace)
            {
                _logTop.Text = _logTop.Text.Substring(0, _logTop.Text.Length - 1);
            }
            else if (key.ToString().Length == 1)
            {
                _logTop.Text += key.ToString();
            }
        }

        private Vector2 _lastMousePos;

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            var ray = new Ray(Position, new Vector3(
                MathF.Sin(Rotation.X) * MathF.Cos(Rotation.Y),
                MathF.Cos(Rotation.X) * MathF.Cos(Rotation.Y),
                MathF.Sin(Rotation.Y)).Normalized());

            _forwardRayTrace = ((VoxelWorld)World).ChunkManager.RayTrace(ray, 10);
            _logButtom.Text = _forwardRayTrace.Block.StaticData.Name;

            var delta = _lastMousePos - new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            delta *= 0.25f * dt;

            PlayerCamera.AddRotation(delta.X, delta.Y);

            _lastMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            var keyboard = World.InputManager.Keyboard;

            if (keyboard[Key.W])
            {
                Position += Forward * Speed * dt;
            }

            if (keyboard[Key.S])
            {
                Position -= Forward * Speed * dt;
            }

            if (keyboard[Key.A])
            {
                Position += Right * Speed * dt;
            }

            if (keyboard[Key.D])
            {
                Position -= Right * Speed * dt;
            }

            if (keyboard[Key.E])
            {
                Position += Up * Speed * dt;
            }

            if (keyboard[Key.Q])
            {
                Position -= Up * Speed * dt;
            }

            PlayerCamera.Position = Position;
            Rotation = PlayerCamera.Rotation;
        }

        public override void OnRender()
        {
            base.OnRender();

            if (_forwardRayTrace.Block.Id > 0)
                DrawBoxDebug();
        }

        private void DrawBoxDebug()
        {
            GL.UseProgram(0);
            var modelview = Camera.ActiveCamera.ViewMatrix;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            var size = (float) World.Config.Chunk.ChunkScale;

            GL.PushMatrix();
            GL.Translate(_forwardRayTrace.BlockWorldPosition * size);
            GL.Translate(new Vector3(size / 2));

            GL.Color3(Color.White);
            GL.Begin(PrimitiveType.Lines);

            size = size * .52f;

            //buttom-plane
            GL.Vertex3(size, size, size);
            GL.Vertex3(-size, size, size);

            GL.Vertex3(-size, size, size);
            GL.Vertex3(-size, -size, size);

            GL.Vertex3(-size, -size, size);
            GL.Vertex3(size, -size, size);

            GL.Vertex3(size, -size, size);
            GL.Vertex3(size, size, size);

            //top-plane
            GL.Vertex3(size, size, -size);
            GL.Vertex3(-size, size, -size);

            GL.Vertex3(-size, size, -size);
            GL.Vertex3(-size, -size, -size);

            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(size, -size, -size);

            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, size, -size);

            //vertical
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, size, size);

            GL.Vertex3(-size, size, -size);
            GL.Vertex3(-size, size, size);

            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, -size, size);

            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, -size, size);


            GL.End();

            GL.PopMatrix();
        }
    }
}