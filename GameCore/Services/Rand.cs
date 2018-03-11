using System.Collections.Generic;

namespace GameCore.Services
{
    public class Rand
    {
        readonly byte[] _permutationTable;

        public Rand(int seed = 0)
        {
            var rand = new System.Random(seed);
            _permutationTable = new byte[1024];
            rand.NextBytes(_permutationTable);
        }

        private float[] GetPseudoRandomGradientVector(int x, int y)
        {
            var v = (int) (((x * 1836311903) ^ (y * 2971215073) + 4807526976) & 1023);
            v = _permutationTable[v] & 3;

            switch (v)
            {
                case 0: return new float[] {1, 0};
                case 1: return new float[] {-1, 0};
                case 2: return new float[] {0, 1};
                default: return new float[] {0, -1};
            }
        }

        static float QunticCurve(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        static float Dot(IReadOnlyList<float> a, IReadOnlyList<float> b)
        {
            return a[0] * b[0] + a[1] * b[1];
        }

        public float Noise(float fx, float fy)
        {
            var left = (int) System.Math.Floor(fx);
            var top = (int) System.Math.Floor(fy);
            var pointInQuadX = fx - left;
            var pointInQuadY = fy - top;

            var topLeftGradient = GetPseudoRandomGradientVector(left, top);
            var topRightGradient = GetPseudoRandomGradientVector(left + 1, top);
            var bottomLeftGradient = GetPseudoRandomGradientVector(left, top + 1);
            var bottomRightGradient = GetPseudoRandomGradientVector(left + 1, top + 1);

            var distanceToTopLeft = new[] {pointInQuadX, pointInQuadY};
            var distanceToTopRight = new[] {pointInQuadX - 1, pointInQuadY};
            var distanceToBottomLeft = new[] {pointInQuadX, pointInQuadY - 1};
            var distanceToBottomRight = new[] {pointInQuadX - 1, pointInQuadY - 1};

            var tx1 = Dot(distanceToTopLeft, topLeftGradient);
            var tx2 = Dot(distanceToTopRight, topRightGradient);
            var bx1 = Dot(distanceToBottomLeft, bottomLeftGradient);
            var bx2 = Dot(distanceToBottomRight, bottomRightGradient);

            pointInQuadX = QunticCurve(pointInQuadX);
            pointInQuadY = QunticCurve(pointInQuadY);

            var tx = Lerp(tx1, tx2, pointInQuadX);
            var bx = Lerp(bx1, bx2, pointInQuadX);
            var tb = Lerp(tx, bx, pointInQuadY);

            return tb;
        }

        public float Noise(float fx, float fy, int octaves, float persistence = 0.5f)
        {
            float amplitude = 1;
            float max = 0;
            float result = 0;

            while (octaves-- > 0)
            {
                max += amplitude;
                result += Noise(fx, fy) * amplitude;
                amplitude *= persistence;
                fx *= 2;
                fy *= 2;
            }

            return result / max;
        }
    }
}