using System;
using GameFrame;
//using Aircraft_Battle;
using System.Collections.Generic;
//using System.Collections;
//using System.IO;
//using ProjectManager;
//using System.Timers;
using System.Xml;
using System.Threading;

namespace Test
{
    class Program
    {
        static OptionGroup option = new OptionGroup("test\nthis option group is for test the pausing function\nmeanwhile,test the page reading by the way\nthe fllow content is a text:\n", "next", "last", "resume");
        static void OptionInit()
        {
            option.Title += @"你知道我们家谁是电脑迷吗？不是我妈妈，也不是我爸爸，更不是我。那到底是谁呢？哈哈！那就是我的外婆。

　　别看外婆早已满头白发、一脸皱纹，玩起电脑来可疯狂的很！

　　有一次，外婆在烧菜，忽然想起她的菜忘记收了，就以惊人的速度从厨房跑到客厅，还差点摔倒。外婆来到电脑面前，手紧紧地拿住鼠标，眼睛死死盯着电脑，嘴巴不停地唠叨：“快点，快点，再迟又 要被偷光了！”看到菜没被偷，外婆总算松了口气。不知不觉中，十多分钟过去了，外婆突然想起锅中还有菜，就大呼小叫地跑到了厨房。看到满锅烧焦的菜，真是哭笑不得。

　　这就是我的电脑迷外婆。";
            option[0].OptionEvent += () =>
            {
                option.Show();
                Console.WriteLine("it is just a test function");
            };
            option[1].OptionEvent += () =>
            {
                option.Show();
                Console.WriteLine("it is just a test function");
            };
           
        }
        static unsafe void Main(string[] args)
        {
            TestScreen();
            Console.ReadLine();
        }
        static void TestScreen()
        {
            OptionInit();
            Map realMap = Map.GetMapFrom("ppp");
            Screen screenMap = new Screen();
            option[2].OptionEvent += () =>
            {
                Console.Clear();
                screenMap.Continue();
            };
            //Cursor moveMan = new Cursor(0, 0, screenMap, realMap);
            Test test = new Test(realMap);
            Camera camera = new Camera(test, FllowMode.RealTime, false, realMap, screenMap);
            screenMap.Init(20, 20);

            Move<Test>.AddBarrier('3');
            screenMap.AddReRendObj(test, 0);
            Console.WriteLine("请按下任意键吧__");
            Console.ReadKey();
            Thread thread = new Thread(test.Move);
            thread.Start();
            while (true)
            {
                screenMap.Mapping(string.Format("{0},{1},{2}                           "
                    , test.Position, camera.Position, test.ap.TarPos));

                var button = ISystem.KeyPress();
                if (button == ConsoleKey.P)//重新搜索目标点
                {
                    Console.SetCursorPosition(0, screenMap.Height + 5);
                    var str = Console.ReadLine();
                    try
                    {
                        var vals = str.Split(new char[] { ' ' });
                        test.ap.ChangeTarget((Convert.ToInt32(vals[0]), Convert.ToInt32(vals[1])));
                        thread = new Thread(() => test.Move());
                        thread.Start();
                    }
                    catch (Exception e) { Console.WriteLine(e.Message); }
                }
                if (button == ConsoleKey.J)
                {
                    //screenMap.Pause();
                }
            }
        }
        static void TestXML()
        {
            var xmlDoc = new XmlDocument();
        }
    }

    class Com : IComparer<int>
    {
        public int Compare(int o1, int o2)
        {
            int i1 = (int)o1, i2 = (int)o2;
            if (i1 > i2) return -1;
            else if (i1 == i2) return 0;
            else return 1;
        }
    }
    class Test : GameObject
    {
        Movement<Test> moveObj = new Movement<Test>();
        public Pathfinder ap;//寻路器
        public Test(Map map) : base(0, 0, 'I', map)
        {
            ap = new Pathfinder((50, 50), (0, 0), map, PathFindDirect.EightDirect, PathFindWay.Straight, '3');
            SetMoveParam();
        }
        void SetMoveParam()
        {
            moveObj.Init(this);
            moveObj.SetMoveParam(false, false, true);
            moveObj.AddBarrier('3');
            moveObj.SetKey(4, ConsoleKey.Q);
            moveObj.SetKey(5, ConsoleKey.E);
            moveObj.SetKey(6, ConsoleKey.Z);
            moveObj.SetKey(7, ConsoleKey.C);
            ap.AfterMove += () =>//自动改变位置
            {
                Position = ap.CurPos;
                Thread.Sleep(100);
            };

        }
        public void Move()
        {
            ap.FindLoop();//循换寻找
            //moveObj.ManualMoveS();
            if (this)
            {

            }

        }
        public static bool operator true(Test test)
        {
            return test.ap.IsRun;
        }
        public static bool operator false(Test test)
        {
            return test.ap.IsRun;
        }
        public static implicit operator float(Test test)
        {
            return test.Position.X;
        }
    }
}

