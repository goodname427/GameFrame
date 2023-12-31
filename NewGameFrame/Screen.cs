﻿namespace NewGameFrame
{
    public class Screen
    {
        public static Screen? Instance { get; private set; }

        private char[,] _console = new char[0, 0];

        /// <summary>
        /// 光标停靠位置
        /// </summary>
        public Vector CursorHoldPosition { get; set; } = new Vector(0, Console.WindowHeight - 1);

        public Screen()
        {
            Console.CursorVisible = false;
            Update(new char[0, 0]);
            Instance = this;
        }

        /// <summary>
        /// 设置图像
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="image"></param>
        private void SetImage(int i, int j, char image)
        {
            Console.SetCursorPosition(i * 2, j);
            Console.Write(image is '\0' ? " " : image);
            _console[i, j] = image;
        }
        /// <summary>
        /// 更新屏幕
        /// </summary>
        /// <param name="map"></param>
        public void Update(char[,] map)
        {
            if (Map.IsOutSide(_console, new(Console.WindowWidth / 2, Console.WindowHeight / 2)))
                _console = Map.GetNewSizeMap(_console, Console.WindowWidth / 2, Console.WindowHeight / 2);

            var (width, height) = (_console.GetLength(0), _console.GetLength(1));
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!Map.IsOutSide(map, new(i, j)) && _console[i, j] != map[i, j])
                        SetImage(i, j, map[i, j]);
                }
            }
            Console.SetCursorPosition(CursorHoldPosition.X, CursorHoldPosition.Y);
        }
    }
}
