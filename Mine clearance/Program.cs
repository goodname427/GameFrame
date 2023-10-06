using System;
using GameFrame;
namespace Mine_Clearance
{
    public class Program : ISystem
    {
        public static ISystem system = new Program();
        int ISystem.Step { set; get; }
        Process ISystem.Process { get; set; }
        Map ISystem.Map { get; set; }
        Screen ISystem.Screen { get; set; }
        Camera ISystem.Camera { get; set; }


        static Timer timer1 = new Timer(1000);//记录时间

        static Cursor cursor;//光标

        public static Map map = new Map();//储存雷的位置
        public static Map cursorMap = new Map();
        public static int size = 9;
        public static char iMines = '*';
        public static char iBlank = '#';
        public static char iFlag = 'F';

        static int clickNum;//点击次数
        static int pastTime;//过去的时间
        static int flaggedMines;//标记的地雷数
        void ISystem.GlobalInit()
        {
            cursor = new Cursor();
            system.Process.Add(timer1);
            timer1.Update += () =>
              {
                  pastTime++;//记录时间
                 if(Console.CursorLeft== system.Map.OX&&Console.CursorTop== system.Map.OY + system.Map.Width + 1) Console.WriteLine($" Time:{pastTime} S Click Numbers:{clickNum}   ");//显示信息
              };
            system.Process.Update += () =>
                  {
                      Move<Cursor>.ManualMove(cursor, null, false, false);//移动玩家光标
                      Console.SetCursorPosition(system.Map.OX, system.Map.OY + system.Map.Width + 1);//移动控制台光标到最下方
                  };

            Move<Cursor>.ManualMoveP += (ConsoleKey? button, Cursor cursor) =>
              {

                  if (button == ConsoleKey.J)
                  {
                      clickNum++;//增加点击数
                      system.Map[cursor.Position] = map[cursor.Position] == '0' ? (char)0 : map[cursor.Position];
                      //显示真地图
                      if (map[cursor.Position] == iMines)//踩中地雷
                      {
                          ISystem.Alert("Game Over!                                    \nPress Enter to Exit");//游戏结束
                          Console.ReadLine();
                          system.Step = 0;
                      }
                      else if (map[cursor.Position] == '0')//踩中空点是显示周围空点
                      {
                          cursor.ShowAroundBlank(cursor.Position.X, cursor.Position.Y);
                      }
                      else
                      {

                      }
                  }
                  if (button == ConsoleKey.K)//标记地雷
                  {
                      clickNum++;
                      if (map[cursor.Position] == iMines && system.Map[cursor.Position] != iFlag)
                      {
                          flaggedMines++;//如果标记正确则增加被标记的地雷数量
                      }
                      system.Map[cursor.Position] = iFlag;
                  }
                  if (flaggedMines == (size * size) / 8)
                  {
                      ISystem.Alert("You Win!                                         \nPress Enter to Exit");//所有地雷被标记后游戏获胜
                      Console.ReadLine();
                      system.Step = 0;
                  }
                  //高亮显示光标所选中的方格
                  cursor.HighLighted();
              };
            Move<Cursor>.ForeManualMoveP += (ConsoleKey? button, Cursor cursor) =>
              {
                  cursor.CancelHighLighted();
              };

            GameManager.SetGameTitle("Mine Clearance");
            GameManager.SetHelp("Press W A S D to Move Cursor\nPress J to Ensure\nPress K to Flag Mines\nYou Win When All of Mines Are Flagged");
            GameManager.AddSettingOp("MapSize");
            GameManager.SettingUI[0].OptionEvent += () => { system.SetData(ref size, 9, 40); };

        }
        void ISystem.LocalInit()
        {

            system.Map.Init(size, size,true,true,true);//初始化地图
            map.Init(size, size, false,false,false);
            cursorMap.Init(size, size, false,false,false);
            cursor = new Cursor();
            clickNum = 0;
            pastTime = 0;
            flaggedMines = 0;
            for (int i = 0; i < size; i++)//使答案不可见
            {
                for (int j = 0; j < size; j++)
                {
                    system.Map[(i, j)] = iBlank;//''
                    map[(i, j)] = '0';//真地图均设为数字0
                }
            }
            for (int i = 0; i < (size * size) / 8; i++)//随机生成地雷
            {
                Random r = new Random();
                int x = r.Next(size), y = r.Next(size);
                while (map[(x, y)] == iMines) { x = r.Next(size); y = r.Next(size); }
                map[(x, y)] = iMines;
                for (int dx = x - 1; dx <= x + 1; dx++)//生成雷后改变周围数字
                {
                    if (dx >= 0 && dx < size)//确保在地图范围内
                    {
                        for (int dy = y - 1; dy <= y + 1; dy++)
                        {
                            if (dy >= 0 && dy < size && map[(dx, dy)] != iMines)//确保在地图范围内
                            {
                                map[(dx, dy)]++;
                            }
                        }
                    }
                }
            }
            system.Map.Update();//更新地图
            Console.SetCursorPosition(system.Map.OX, system.Map.OY + system.Map.Width + 1);
        }

        public static void Main()
        {
            GameManager.GameStart(system);
        }
    }
}
