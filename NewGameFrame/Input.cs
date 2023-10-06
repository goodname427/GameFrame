using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGameFrame
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
        public static bool AnyKey => GetInput() is not null;

        /// <summary>
        /// 获取输入
        /// </summary>
        /// <returns></returns>
        public static ConsoleKey? GetInput()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey().Key;
                return key;
            }
            return null;
        }
        /// <summary>
        /// 判断是否输入指定键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetKey(ConsoleKey key)
        {
            return GetInput() == key;
        }
        /// <summary>
        /// 获取横轴向
        /// </summary>
        /// <returns></returns>
        public static int GetHorizontal()
        {
            return GetInput() switch
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
            return GetInput() switch
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
            return GetInput() switch
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
