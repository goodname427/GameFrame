using System;
using GameFrame;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Timer = GameFrame.Timer;

namespace Rogue_Remake
{
    class Program : ISystem
    {
        public static ISystem System = new Program();
        int ISystem.Step { set; get; }
        Process ISystem.Process { get; set; }
        Map ISystem.Map { get; set; }
        Screen ISystem.Screen { get; set; }
        Camera ISystem.Camera { get; set; }

        public static Building Build { get; set; }
        public static Player player;
        public static int layer;//玩家所在的楼层数
        public static string RemindInfo = string.Empty;//提示的信息
        public static Mutex mutex = new Mutex();
        public static Timer ScreenTmr = new Timer(300);

        public static OptionGroup testOp1 = new OptionGroup("Test1\ntese\ntest", "下一页", "上一页", "退出");
        public static OptionGroup testOp2 = new OptionGroup("Test2\ntest\ntest", "下一页", "上一页", "退出");


        static void OptionInit()//选项事件初始化
        {
            testOp1[0].OptionEvent += () => { testOp2.Show(); };
            testOp1[1].OptionEvent += () => { testOp2.Show(); };
            testOp1[2].OptionEvent += () => { System.Screen.Continue(false); };
            testOp2[0].OptionEvent += () => { testOp1.Show(); };
            testOp2[1].OptionEvent += () => { testOp1.Show(); };
            testOp2[2].OptionEvent += () => { System.Screen.Continue(false); };
        }
        static void PropertyInit()//初始化各属性
        {

        }
        static void TimerInit()//计时器初始化
        {
            System.Process.Add(ScreenTmr);
            System.Process.Update += () =>
            {
                player.Move();
                System.Screen.Mapping(player.PlayerInfo + "\n" + RemindInfo);
            };
            ScreenTmr.Update += () =>
                  {
                      MonsterAct();
                  };
        }
        static void PressInit()//按键初始化
        {

        }
        public static void ChangeMap(int layer)//改变地图
        {
            System.Screen.ResetRendObj(1);//重置怪兽
            foreach (var m in Build[layer].Monsters)//添加怪兽
                System.Screen.AddReRendObj(m, 1);
            System.Map = Build[layer];
            player.Map = System.Map;
            player.Position = Build[layer].StartPos;
            System.Camera.SetMap(System.Map, System.Screen, false);
        }
        public static void MonsterAct()//控制所有怪物行为
        {
            try
            {
                var tasks = new List<Task>();
                foreach (var m in Build[layer].Monsters)
                {
                    if (m != null) tasks.Add(m.Move());
                }
                Task.WaitAll(tasks.ToArray());// 等待所有怪物
            }
            catch { }
        }
        void ISystem.GlobalInit()
        {
            OptionInit();
            Monster.MonsterInitialize();
            PropertyInit();
            TimerInit();
            PressInit();
            GameManager.SetGameTitle("Rogue 重制版");
        }
        void ISystem.LocalInit()
        {
            if (Console.WindowHeight < Console.LargestWindowHeight - 10)
                Console.SetWindowSize(Console.WindowWidth, Console.WindowHeight + 10);//将窗体设置为最大

            System.Map = new Map(Building.MapWidth, Building.MapHeight);
            player = new Player(System.Map);
            System.CameraBind(player, FllowMode.RealTime, 5, true, false, 50, 28);//相机绑定
            player.moveObj.Screen = System.Screen;//玩家绑定屏幕
            Build = new Building();
            Build.CreatStair();
            layer = 0;
            ChangeMap(layer);
        }

        static void Main(string[] args)
        {
            GameManager.GameStart(System, Language.Chinese);
        }
    }
}
