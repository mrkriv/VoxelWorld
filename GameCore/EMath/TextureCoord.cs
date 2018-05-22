using System.Collections.Generic;
using OpenTK;

namespace GameCore.EMath
{
    public struct CubeTextureCoord
    {
        public TextureCoord Top { get; set; }
        public TextureCoord Buttom { get; set; }
        public TextureCoord Front { get; set; }
        public TextureCoord Back { get; set; }
        public TextureCoord Left { get; set; }
        public TextureCoord Right { get; set; }

        public static CubeTextureCoord FromSolo(TextureCoord solo)
        {
            return new CubeTextureCoord
            {
                Top = solo,
                Buttom = solo,
                Left = solo,
                Right = solo,
                Front = solo,
                Back = solo,
            };
        }

        public static CubeTextureCoord FromT_B_LRFB(
            TextureCoord top,
            TextureCoord buttom,
            TextureCoord leftRightFrontBack)
        {
            return new CubeTextureCoord
            {
                Top = top,
                Buttom = buttom,
                Left = leftRightFrontBack,
                Right = leftRightFrontBack,
                Front = leftRightFrontBack,
                Back = leftRightFrontBack,
            };
        }
    }

    public struct TextureCoord
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }

        public TextureCoord(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public IEnumerable<Vector2> ToArray()
        {
            return new[]
            {
                new Vector2(X + W, Y + H),
                new Vector2(X, Y + H),
                new Vector2(X + W, Y),
                new Vector2(X, Y),
            };
        }

        public void CopyTo(Vector2[] array, ref int offest)
        {
            array[offest++] = new Vector2(X + W, Y + H);
            array[offest++] = new Vector2(X, Y + H);
            array[offest++] = new Vector2(X + W, Y);
            array[offest++] = new Vector2(X, Y);
        }
    }
}