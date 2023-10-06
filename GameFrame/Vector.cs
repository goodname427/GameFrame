using System;
using System.Collections.Generic;
using System.Text;

namespace GameFrame
{
    public struct Vector
    {
        public static Vector Up { get; } = new Vector(0, -1);
        public static Vector Down { get; } = new Vector(0, 1);
        public static Vector Left { get; } = new Vector(-1, 0);
        public static Vector Right { get; } = new Vector(1, 0);
        public static Vector UpLeft { get; } = new Vector(-1, -1);
        public static Vector UpRight { get; } = new Vector(1, -1);
        public static Vector DownLeft { get; } = new Vector(-1, 1);
        public static Vector DownRight { get; } = new Vector(1, 1);
        public static Vector Center { get; } = new Vector(0, 0);

        public int X { get; set; }
        public int Y { get; set; }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector left, Vector right)
        {
            return new Vector(left.X + right.X, left.Y + right.Y);
        }

        public static Vector operator -(Vector left, Vector right)
        {
            return new Vector(left.X - right.X, left.Y - right.Y);
        }

        public static Vector operator *(Vector left, int right)
        {
            return new Vector(left.X * right, left.Y * right);
        }

        public static Vector operator *(int left, Vector right)
        {
            return right * left;
        }

        public static bool operator ==(Vector left, Vector right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Vector left, Vector right)
        {
            return !(left == right);
        }

        public static implicit operator (int x, int y)(Vector d)
        {
            return (d.X, d.Y);
        }

        public static implicit operator Vector((int x, int y) d)
        {
            return new Vector(d.x, d.y);
        }

        public static implicit operator Direction(Vector d)
        {
            if (d == Up) return Direction.Up;
            if (d == Down) return Direction.Down;
            if (d == Left) return Direction.Left;
            if (d == Right) return Direction.Right;
            if (d == UpLeft) return Direction.UpLeft;
            if (d == DownLeft) return Direction.DownLeft;
            if (d == UpRight) return Direction.UpRight;
            if (d == DownRight) return Direction.DownRight;
            return Direction.Center;
        }


        public static implicit operator Vector(Direction d)
        {
            return d switch
            {
                Direction.Up => Up,
                Direction.Down => Down,
                Direction.Left => Left,
                Direction.Right => Right,
                Direction.UpLeft => UpLeft,
                Direction.UpRight => UpRight,
                Direction.DownLeft => DownLeft,
                Direction.DownRight => DownRight,
                Direction.Center => Center,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public override bool Equals(object other)
        {
            return (Vector)other == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


    }
}
