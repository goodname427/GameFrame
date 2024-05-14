using NewGameFrame.MathCore;
using System.Collections.ObjectModel;

namespace NewGameFrame.Core
{
    public static class Input
    {
        /// <summary>
        /// 键盘映射
        /// </summary>
        public static ReadOnlyDictionary<string, ConsoleKey> KeyMapping { get; private set; } = new(new Dictionary<string, ConsoleKey>
        {
            {"Up",ConsoleKey.W },
            {"Left",ConsoleKey.A },
            {"Down",ConsoleKey.S },
            {"Right",ConsoleKey.D },
        });

        /// <summary>
        /// 检测是否有任何键按下
        /// </summary>
        public static bool AnyKey => CurrentInput is not null;

        public static ConsoleKey? CurrentInput { get; private set; }

        /// <summary>
        /// 获取输入
        /// </summary>
        /// <returns></returns>
        public static ConsoleKey? GetInput()
        {
            CurrentInput = null;
            if (Console.KeyAvailable)
            {
                CurrentInput = Console.ReadKey().Key;
            }
            return CurrentInput;
        }
        /// <summary>
        /// 判断是否输入指定键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetKey(ConsoleKey key)
        {
            return CurrentInput == key;
        }
        /// <summary>
        /// 获取横轴向
        /// </summary>
        /// <returns></returns>
        public static int GetHorizontal()
        {
            return CurrentInput switch
            {
                ConsoleKey.D => 1,
                ConsoleKey.A => -1,
                _ => 0
            };
        }
        /// <summary>
        /// 获取纵轴向
        /// </summary>
        /// <returns></returns>
        public static int GetVertical()
        {
            return CurrentInput switch
            {
                ConsoleKey.W => 1,
                ConsoleKey.S => -1,
                _ => 0
            };
        }
        /// <summary>
        /// 获取方向
        /// </summary>
        /// <returns></returns>
        public static Vector GetDirection()
        {
            return CurrentInput switch
            {
                ConsoleKey.W => Vector.Up,
                ConsoleKey.S => Vector.Down,
                ConsoleKey.A => Vector.Left,
                ConsoleKey.D => Vector.Right,
                _ => Vector.Zero
            };
        }
    }
}
