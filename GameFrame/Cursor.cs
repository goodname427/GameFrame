using System;
using System.Collections.Generic;
using System.Text;

namespace GameFrame
{
    [Obsolete]
    public class Cursor : GameObject//光标类
    {
        public ConsoleColor color;//光标颜色
        public Screen Screen;//真地图,光标所在地图为屏幕地图,即只做显示作用.
        Vector RelativePos { get => GetRelativePos(Screen.RelativePos); }//光标相对与真地图位置
        public Cursor(int x, int y, Screen screen, Map realMap, ConsoleColor color = ConsoleColor.Green) : base(x, y, (char)0, realMap)
        {
            this.color = color;
            Screen = screen;
        }
        public void Move()//控制光标移动 
        {
            var info = Input();
            ConsoleKey? button = null;
            if (info.HasValue) button = info.Value.Key;
            if (button != null)
            {
                CancelHighLighted();//取消高亮
                switch (button)
                {
                    case ConsoleKey.LeftArrow: Position.X--; break;
                    case ConsoleKey.RightArrow: Position.X++; break;
                    case ConsoleKey.UpArrow: Position.Y--; break;
                    case ConsoleKey.DownArrow: Position.Y++; break;
                    default: Controler(info); break;
                }
                HighLighted();//高亮显示
            }
            AdjustPos();
        }
        void AdjustPos()//根据相机的位置调整光标的位置
        {
           
        }

        void HighLighted()//高亮显示光标
        {
            if (!IsOutMap())
            {
                Console.SetCursorPosition(Screen.OX + (RelativePos.X + 1) * 2 + 1, Screen.OY + RelativePos.Y);//移动光标
                Console.Write("\b\b");
                Console.BackgroundColor = color;
                if (Map[Position] != 0) Console.Write($"{Map[Position]}");//重新输入
                else Console.Write(" ");
                Console.SetCursorPosition(Screen.OX, Screen.OY + Screen.Width + 1);
                Console.ResetColor();//重置
            }
        }
        void CancelHighLighted()//取消高亮
        {
            if (!IsOutMap())
            {
                Console.SetCursorPosition(Screen.OX + (RelativePos.X + 1) * 2 + 1, Screen.OY + RelativePos.Y);
                Console.Write("\b\b");
                if (Map[Position] != 0) Console.Write($"{ Map[Position]}");
                else Console.Write("  ");//恢复地图信息
                Console.SetCursorPosition(Screen.OX, Screen.OY + Screen.Width + 1);
            }
        }

        ConsoleKeyInfo? Input()//光标控制以及输入
        {
            ISystem.KeyPress(out ConsoleKeyInfo? info);//获取按键信息
            char? input = null;
            if (info.HasValue)
                input = info.Value.KeyChar;
            if (input.HasValue && input.Value >= 32 && input <= 126)
            {
                Map[Position] = input.Value;//输入字符
            }
            return info;
        }
        void Controler(ConsoleKeyInfo? info)//光标快捷键操控
        {
            if (info.HasValue)
            {
                var button = info.Value.Key;
                if (info.Value.Modifiers == ConsoleModifiers.Control && button == ConsoleKey.S)//存档
                {
                    Map.Save();
                }
                if (info.Value.Modifiers == ConsoleModifiers.Control && button == ConsoleKey.Z)
                {

                }
            }
        }
    }
}
