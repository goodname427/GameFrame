namespace NewGameFrame.MathCore
{
    public struct Vector
    {
        public static Vector Zero => new();
        public static Vector Up => new(0, 1, 0);
        public static Vector Down => new(0, -1, 0);
        public static Vector Left => new(-1, 0, 0);
        public static Vector Right => new(1, 0, 0);
        public static Vector Forward => new(0, 0, 1);
        public static Vector Back => new(0, 0, -1);

        public static Vector operator +(Vector left, Vector right)
        {
            return new Vector(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }
        public static Vector operator -(Vector vector)
        {
            return vector.Operate(c => -c);
        }
        public static Vector operator -(Vector left, Vector right)
        {
            return left + -right;
        }
        public static Vector operator *(Vector vector, int num)
        {
            return vector.Operate(c => c * num);
        }
        public static Vector operator *(int num, Vector vector)
        {
            return vector * num;
        }
        public static Vector operator /(Vector vector, int num)
        {
            return vector.Operate(c => c / num);
        }

        public static bool operator ==(Vector left, Vector right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public static bool operator !=(Vector left, Vector right)
        {
            return !(left == right);
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public Vector Abs => Operate(Math.Abs);

        public Vector(int x = 0, int y = 0, int z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override readonly string ToString()
        {
            return $"({X},{Y})";
        }

        public Vector Operate(Func<int, int> operate)
        {
            return new(operate(X), operate(Y), operate(Z));
        }

    }
}
