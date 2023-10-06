using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;

namespace Mine_Clearance
{
    class Cursor : GameObject
    {
        public Cursor() : base(1, 1, (char)0, Program.cursorMap)
        {

        }
        public void ShowAroundBlank(int dx, int dy)//显示周围空白区域
        {
            Map map = Program.system.Map;
            for (int i = dx - 1; i <= dx + 1; i++)
            {
                if (i > -1 && i < Program.size)
                {
                    for (int j = dy - 1; j <= dy + 1; j++)
                        if (j>-1&&j<Program.size)
                        {
                            if(Program.map[(i,j)]!=Program.iMines&&map[(i, j)] == Program.iBlank)
                            {
                                map[(i, j)] = Program.map[(i, j)] == '0' ? (char)0:Program.map[(i,j)];
                                if (Program.map[(i, j)] == '0') ShowAroundBlank(i,j);
                            }
                        }
                }
            }
        }
        public void HighLighted()//高亮显示
        {
            Console.SetCursorPosition((Program.map.OX + Position.X + 1) * 2, Program.map.OY + Position.Y);//移动光标
            Console.Write("\b\b");
            Console.BackgroundColor = ConsoleColor.Magenta;
            if (Program.system.Map[Position] != 0) Console.Write($"{Program.system.Map[Position]}");//重新输入
            else Console.Write(" ");
            Console.SetCursorPosition(Program.system.Map.OX, Program.system.Map.OY + Program.system.Map.Width + 1);
            Console.ResetColor();//重置
            Console.ForegroundColor = ConsoleColor.Green;
            Program.system.Map.Update();
            Console.ResetColor();
        }
        public void CancelHighLighted()//取消显示
        {
            //取消高亮显示光标选中的方格
            Console.SetCursorPosition((Program.map.OX + Position.X + 1) * 2, Program.map.OY + Position.Y);
            Console.Write("\b\b");
            if (Program.system.Map[Position] != 0)
            {
                if (Program.system.Map[Position] != Program.iBlank)
                    Console.ForegroundColor = ConsoleColor.Green;
                if (Program.system.Map[Position] == Program.iFlag)
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{Program.system.Map[Position]}");
                Console.ResetColor();
            }
            else Console.Write("  ");//恢复地图信息
            Console.SetCursorPosition(Program.system.Map.OX, Program.system.Map.OY + Program.system.Map.Width + 1);

        }
    }
}
