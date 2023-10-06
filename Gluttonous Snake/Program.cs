using System;
using GameFrame;
namespace Gluttonous_Snake
{
    public partial class Program : ISystem
    {
        public static ISystem System = new Program();
        Process ISystem.Process { get; set; } = new Process();
        int ISystem.Step { get; set; } = 0;
        Map ISystem.Map { get; set; }
        Screen ISystem.Screen { get; set; }
        Camera ISystem.Camera { get; set; }
        public static Map AIMap { get; set; }//AI所在地图


        public static Timer Timer1 = new Timer(500);//计时器玩家移动
        public static Timer Timer2 = new Timer(5000/*,system.Process*/);//玩家定时器生成食物
        public static Timer Timer3 = new Timer(5000);//Ai食物
        public static Timer Timer4 = new Timer(10000);//倒计时
        public static Timer Timer5 = new Timer(500);//控制AI移动
        public static Timer Timer6 = new Timer(180000);//默认10s在增加一个道具


        public static int Hp = 1, Speed = 1, AddLength = 1, MapWidth = 21, MapHeight = 21;//游戏属性
        public static char IHead = 'O', IFood = 'F', IBody = '#', IBarrier = '〇';//游戏皮肤
        public static int OpenAI = 0;//是否开启AI，1为是，0为否
        public static Snake Player;//玩家
        public static int CountdownTime = 180000;//倒计时
        static Snake AIPlayer;//AI

        static OptionGroup Op3 = new OptionGroup("Setting Skin", "Head", "Body");
        static OptionGroup Op4 = new OptionGroup("Setting Map Size", "Width", "Height");
        static OptionGroup Op5 = new OptionGroup("AI", "On", "Off");
        static OptionGroup Op6 = new OptionGroup("Countdown time", "60S", "180S", "300S");

        public static void SumProp(Map map)//生成道具食物
        {
            Random r = new Random();
            (int, int) FP = (r.Next(map.Width), r.Next(map.Height));
            while (map[FP.Item1, FP.Item2] != 0) FP = (r.Next(map.Width), r.Next(map.Height));//避免生成在蛇内部

            Food f = Foods[r.Next(Foods.Count - 1) + 1];//从食物数组中随机取出一种
            map[FP.Item1, FP.Item2] = f.Image;//放置

            if (map == AIMap) Timer3.Enabled = false;
            else Timer2.Enabled = false;
        }
        public static void SumFood(Map map, Food food)//生成指定食物
        {
            Random r = new Random();
            (int, int) FP = (r.Next(map.Width), r.Next(map.Height));
            while (map[FP.Item1, FP.Item2] != 0) FP = (r.Next(map.Width), r.Next(map.Height));//避免生成在蛇内部
            map[FP.Item1, FP.Item2] = food.Image;
        }
        public static void SumFood(Map map)//生成食物
        {
            Random r = new Random();
            (int, int) FP = (r.Next(map.Width), r.Next(map.Height));
            while (map[FP.Item1, FP.Item2] != 0) FP = (r.Next(map.Width), r.Next(map.Height));//避免生成在蛇内部           
            map[FP.Item1, FP.Item2] = IFood;
        }

        void ISystem.GlobalInit()
        {
            AIMap = new Map();
            System.Process.Add(Timer1, Timer2, Timer3, Timer4, Timer5, Timer6);//计时器添加           
            Move<Snake.Body>.TurnDireP += (ConsoleKey? button, Snake.Body gb) =>
              {
                  if (button == ConsoleKey.R)//刷新地图
                  {
                      bool flag = true;
                      foreach (var i in System.Map) { if (i == IFood) flag = false; }//若地图上无食物则生成食物
                      if (flag) SumFood(System.Map);
                  }
                  if (button == ConsoleKey.Escape) System.Step = 0;//直接结束游戏
              };
            GameManager.GameOver += () =>
            {
                if (System.Step == 101)
                {
                    Console.SetCursorPosition(0, 24);
                    if (Player.Bodys.Count < AIPlayer.Bodys.Count)//倒计时结束判断赢家
                    {
                        Console.WriteLine("AI Win");
                    }
                    else
                    {
                        Console.WriteLine("You Win");
                    }
                    Console.ReadLine();
                    System.Step = 0;
                }
                else
                {
                    Console.SetCursorPosition(0, 24);
                    if (OpenAI == 0)
                    {
                        Console.WriteLine("You Died!");
                    }
                    else
                    {
                        if (Player.Hp > 0)//判断谁是赢家
                            Console.WriteLine("You Win");
                        else
                            Console.WriteLine("AI Win");
                    }
                    Console.ReadLine();
                }
            };
            GameManager.SetGameTitle("Welcome To Gluttonous Snake");//游戏标题
            GameManager.SetHelp("Press \"W\" \"A\" \"S\" \"D\" to move\nPress \"Esc\" to exit\nPress \"R\" to refresh\nFood Type:\n'F':Ordinary food. Nothing to tell\n'H':Increase your Hp\n'M':Food carnival\n'C':Nothing special, but it changes your color\n'f':Small snacks that do not refresh after being eaten\nSpecial food limited to two player mode:\n'B':adds a barrier to the opponent's map\n'L':Reduce opponent's length\n'U':Speed up your move speed\n'S':Slowdown opponent's move speed\n2 player mode:\n Play against AI, within the specified time, who can win the longer length");
            #region 选项类二
            GameManager.AddSettingOp("Game Speed", "Hp", "Add Length", "Snake Skin", "Map Size", "AI", "Countdown");
            GameManager.SettingUI[0].OptionEvent += () =>
              {
                  System.SetData(ref Speed, 1, 10);//设置速度
                  Timer1.Interval = (11 - Speed) * 50;
                  Timer5.Interval = (11 - Speed) * 50;
              };
            GameManager.SettingUI[1].OptionEvent += () =>
                {
                    System.SetData(ref Hp, 1, 100);//设置血量
                };
            GameManager.SettingUI[2].OptionEvent += () =>
            {
                System.SetData(ref AddLength, 1, 3);//设置食物增加的长度
            };
            GameManager.SettingUI[3].OptionEvent += () =>
               {
                   Op3.Show(false);//皮肤设置
               };
            GameManager.SettingUI[4].OptionEvent += () =>
               {
                   if (OpenAI == 1) { Console.WriteLine("Can't set the map size after open the AI!"); Console.ReadKey(); }
                   else Op4.Show(false);//地图设置
               };
            GameManager.SettingUI[5].OptionEvent += () =>
               {
                   Op5.Show(false);//AI开启
               };
            GameManager.SettingUI[6].OptionEvent += () =>
               {
                   Op6.Show(false);//倒计时设置
               };
            #endregion //设置
            #region 选项类三
            Op3[0].OptionEvent += () => { System.SetData(ref IHead); };//头
            Op3[1].OptionEvent += () => { System.SetData(ref IBody); };//身体
            #endregion  //设置游戏皮肤
            #region 选项类四
            Op4[0].OptionEvent += () =>
              {
                  System.SetData(ref MapWidth, 10, 50);//宽度
              };
            Op4[1].OptionEvent += () =>
              {
                  System.SetData(ref MapHeight, 10, 50);//高度
              };
            #endregion //设置地图尺寸
            #region 选项类五
            Op5[0].OptionEvent += () => OpenAI = 1;//开关AI
            Op5[1].OptionEvent += () => OpenAI = 0;
            #endregion
            #region 选项类六
            Op6[0].OptionEvent += () =>//设置倒计时
            {
                CountdownTime = 60000; Timer4 = new Timer(CountdownTime);
            };
            Op6[1].OptionEvent += () =>
            {
                CountdownTime = 180000; Timer4 = new Timer(CountdownTime);
            };
            Op6[2].OptionEvent += () =>
            {
                CountdownTime = 300000; Timer4 = new Timer(CountdownTime);
            };
            #endregion
        }//初始化整局游戏数据
        void ISystem.LocalInit()//每局游戏初始化
        {
            System.Process.Reset();//重置进程           
            Timer1.Update += () =>//计时器一 玩家移动
            {
                Player.Move();
            };
            Timer2.Update += () =>//计时器道具生成
              {
                  SumProp(System.Map);
              };
            Timer6.Update += () =>//定时增加道具
              {
                  SumProp(System.Map);
              };
            System.Process.Update += () =>//进程
            {
                Move<Snake.Body>.TurnDirection(Player.Head);//转向
                System.Map.Update($"Speed: {11 - Timer1.Interval / 50} Hp:{Player.Hp} Length:{Player.Bodys.Count}   ");//更新地图
            };
            FoodSingle();
            if (OpenAI == 1)//ai开启
            {
                System.Process.Update += () =>//进程
                {
                    Move<Snake.Body>.TurnDirection(AIPlayer.Head, AI.AIMove(AIPlayer));//转向
                    AIMap.Update($"Countdown:{(CountdownTime - System.Process.Elapsed) / 1000} S Speed: {11 - Timer5.Interval / 50} Hp:{AIPlayer.Hp} Length:{AIPlayer.Bodys.Count}    ");
                };
                Timer5.Update += () =>//计时器一 添加AI移动
                {
                    AIPlayer.Move();
                };
                Timer3.Update += () =>//计时器道具生成
                {
                    SumProp(AIMap);
                };
                Timer4.Update += () =>
                  {
                      System.Step = 101;
                  };
                Timer6.Update += () =>
                {
                    SumProp(AIMap);
                };
                FoodDouble();
            }

            Player = new Snake(System.Map);//重置玩家

            if (OpenAI == 1)
            {
                try { Console.WindowWidth = 100; } catch { }
                AIPlayer = new Snake(AIMap);
                AIMap.Origin = (System.Map.OX + 25, System.Map.OY);//将AI地图设在玩家地图右侧
                System.Map.Init(21, 21,true,true,true);//如果开启AI，地图尺寸强制不变
                AIMap.Init(21, 21,true,true,true);
                SumFood(AIMap);
                SumProp(AIMap);
            }
            else
            {
                System.Map.Init(MapWidth, MapHeight, true, true, true);
            }//重置地图
            SumFood(System.Map);
            SumProp(System.Map);//生成食物
        }

        public static void Main()
        {
            GameManager.GameStart(System);//游戏开始
        }
    }
}
