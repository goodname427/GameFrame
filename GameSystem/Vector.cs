using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem
{
    public struct Vector
    {
        public static Vector Up { get; } = new Vector(0, 1);
        public static Vector Down { get; } = new Vector(0, -1);
        public static Vector Left { get; } = new Vector(-1, 0);
        public static Vector Right { get; } = new Vector(1, 0);
        public static Vector UpLeft { get; } = new Vector(-1, 1);
        public static Vector UpRight { get; } = new Vector(1, 1);
        public static Vector DownLeft { get; } = new Vector(-1, -1);
        public static Vector DownRight { get; } = new Vector(1, -1);
        public static Vector Center { get; } = new Vector(0, 0);

        public int X;
        public int Y;

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector left, Vector right)
        {
            return new Vector(left.X + right.X, left.Y + right.Y);
        }

        public static Vector operator *(Vector left, int right)
        {
            return new Vector(left.X * right, left.Y * right);
        }

        public static Vector operator *(int left, Vector right)
        {
            return right * left;
        }
    }
}
