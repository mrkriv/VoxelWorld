using GameCore.EMath;
using OpenTK.Graphics;

namespace GameCore.GUI
{
    public class CursorControl : Control
    {
        public CursorControl()
        {
            Size = new GuiVector(GuiVectorType.Absolute, 64, 64);
            Color = Color4.White;
            Name = "Cursor";
        }

        public override void OnAttach()
        {
            base.OnAttach();
            Texture = TextureManager.Load("Cursor");
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            Position = new GuiVector(GuiVectorType.Absolute, InputManager.Mouse.X - 32, InputManager.Mouse.Y - 32);
        }
    }
}