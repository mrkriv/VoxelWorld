using System;
using GameCore.EMath;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GameCore.GUI
{
    public class TextControl : Control
    {
        public float FontSize { get; set; }
        public bool AutoSize { get; set; }
        public string Text { get; set; }
        public Font Font { get; set; }

        public TextControl()
        {
            Color = Color4.White;
            AutoSize = true;
            FontSize = 14;
            Text = "";
        }

        public override void OnAttach()
        {
            Font = Font ?? FontManager.Load("Arial");
            base.OnAttach();
        }

        public override void OnRender(Vector2 parrentPosition)
        {
            var basePosition = parrentPosition + Position.ToVector2(this);
            
            Material.Color = Color;
            Material.Texture1 = Font.TextureAtlas;
            Material.UseTexture = true;

            var totalSizeX = 0f;
            var totalSizeY = 0f;

            foreach (var c in Font.MapString(Text))
            {
                var sizeX = c.SizeW / Font.BaseFontSize * FontSize / InputManager.ScreenSize.X;
                var sizeY = c.SizeH / Font.BaseFontSize * FontSize / InputManager.ScreenSize.Y;

                Material.Transform = new Matrix3(
                    new Vector3(sizeX, 0, 0),
                    new Vector3(0, sizeY, 0),
                    new Vector3(basePosition.X + totalSizeX, basePosition.Y, 1));

                Material.TexcoodTransform = new Matrix3(
                    new Vector3(c.AtlasW, 0, 0),
                    new Vector3(0, c.AtlasH, 0),
                    new Vector3(c.AtlasX, c.AtlasY, 1));

                totalSizeX += sizeX - 4 / InputManager.ScreenSize.X;
                totalSizeY = Math.Max(totalSizeY, sizeY);

                Material.Use();

                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }

            if (AutoSize)
                Size = new GuiVector(GuiVectorType.Screen, totalSizeX, totalSizeY);
        }
    }
}