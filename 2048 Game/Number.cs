using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;

namespace _2048_Game
{
    class Number : GameObject
    {
        public int size;
        List<PerNum> nums = new List<PerNum>();
        Number(Map map, int x, int y, int size) : base(x, y, '\0', map) { this.size = size; }
        public static Number SummonNumber(Map map, int minSize, int maxsize)
        {
            Random r = new Random();
            (int x, int y) pos;
            do
            {
                pos = (r.Next(0, map.Width), r.Next(0, map.Height));
            }
            while (map[pos] != '\0');
            Number number = new Number(map, pos.x, pos.y, r.Next(minSize, maxsize + 1));
            number.Place();
            return number;
        }
        public void Move(Direction dire)
        {
            var (x, y) = Move<Number>.DireToPos(dire);
            while (Detect(Position.X + x, Position.Y + y))
                Position = (Position.X + x, Position.Y + y);
        }
        bool Detect(int x, int y)
        {
            if (Map.IsOut((x, y))) return false;
            if (Map[x, y] == size)
            {
                Synthetic();
                return true;
            }
            else return false;
        }
        void Synthetic()
        {

        }

        class PerNum : GameObject
        {
            public PerNum(int x, int y, int num, Map map) : base(x, y, (char)(num + '0'), map)
            {

            }
        }
    }
}
