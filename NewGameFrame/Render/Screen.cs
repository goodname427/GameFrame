using NewGameFrame.Core;
using NewGameFrame.MathCore;
using System.Runtime.InteropServices;

namespace NewGameFrame.Render
{
    public class Screen
    {
        public static Screen? Instance { get; private set; }

        private char[,] _console = new char[0, 0];

        /// <summary>
        /// 光标停靠位置
        /// </summary>
        public Vector CursorHoldPosition { get; set; } = new Vector(0, Console.WindowHeight - 1);
        /// <summary>
        /// 提示信息
        /// </summary>
        public string HUD { get; set; } = "";

        public Screen()
        {
            Console.CursorVisible = false;
            Update(new Image());
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
        /// <param name="renderCache"></param>
        public void Update(Image renderCache)
        {
            if (Map.IsOutSide(_console, new(Console.WindowWidth / 2, Console.WindowHeight / 2)))
                _console = Map.GetNewSizeMap(_console, Console.WindowWidth / 2, Console.WindowHeight / 2);

            var (width, height) = (_console.GetLength(0), _console.GetLength(1));
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!Map.IsOutSide(renderCache.Data, new(i, j)) && _console[i, j] != renderCache[i, j])
                        SetImage(i, j, renderCache[i, j]);
                }
            }
            Console.SetCursorPosition(CursorHoldPosition.X, CursorHoldPosition.Y);
            Console.Write(HUD);
        }
    }
}
