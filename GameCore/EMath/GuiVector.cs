using System;
using GameCore.GUI;
using OpenTK;

namespace GameCore.EMath
{
    public enum GuiVectorType
    {
        Screen,
        Relative,
        Absolute,
    }

    public struct GuiVector
    {
        public GuiVectorType Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public GuiVector(GuiVectorType type, float x, float y)
        {
            Type = type;
            X = x;
            Y = y;
        }

        public Vector2 ToVector2(Control owner)
        {
            switch (Type)
            {
                case GuiVectorType.Screen:
                    return new Vector2(X, Y);

                case GuiVectorType.Relative:
                    var parrenSize = owner.Parrent.Size.ToVector2(owner.Parrent);
                    return new Vector2(X * parrenSize.X, Y * parrenSize.Y);

                case GuiVectorType.Absolute:
                    return new Vector2(X / owner.InputManager.ScreenSize.X, Y / owner.InputManager.ScreenSize.Y);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return $"{Type} {X} {Y}";
        }
    }
}