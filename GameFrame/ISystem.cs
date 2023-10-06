using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace GameFrame
{
    public interface ISystem
    {
        int Step { get; set; }//游戏进程步骤
        Process Process { get; set; }//游戏进程
        Map Map { get; set; }//游戏地图
        Screen Screen { get; set; }//游戏屏幕 
        Camera Camera { get; set; }//游戏相机

        void GlobalInit();//全局初始化,整个游戏初始化
        void LocalInit();//局部初始化,每一局游戏初始化

        public static void Alert(string errorInformation = "Error!")//报错
        {
            Console.WriteLine(errorInformation);
            Console.ReadKey();
        }

        public static ConsoleKey? KeyPress()//按键检测
        {
            if (Console.KeyAvailable)//若有按键按下则检测该按键
            {
                ConsoleKey button = Console.ReadKey().Key;
                Console.Write("\b");//删除该字符
                return button;
            }
            return null;
        }
        public static ConsoleKey? KeyPress(out char? @char)//按键检测
        {
            if (Console.KeyAvailable)//若有按键按下则检测该按键
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                ConsoleKey button = consoleKeyInfo.Key;
                @char = consoleKeyInfo.KeyChar;
                Console.Write("\b");//删除该字符
                return button;
            }
            @char = null;
            return null;
        }
        public static ConsoleKey? KeyPress(out ConsoleKeyInfo? consoleKeyInfo)//按键检测
        {
            if (Console.KeyAvailable)//若有按键按下则检测该按键
            {
                consoleKeyInfo = Console.ReadKey();
                ConsoleKey button = consoleKeyInfo.Value.Key;
                Console.Write("\b");//删除该字符
                return button;
            }
            else { consoleKeyInfo = null; return null; }
        }

        public void SetData(ref int Data, int Min = 1, int Max = 10/*数据范围*/) //设置整数类型
        {
            Console.WriteLine($"Please input the value you want to set({Min}-{Max}):");
            if (!int.TryParse(Console.ReadLine(), out Data) || Data < Min || Data > Max) { Alert("Please input a correct number"); }
            else
            {
                Console.WriteLine($"The data has set to {Data}");
                Console.ReadKey();
            }
        }
        public void SetData(ref char Data) //设置字符类型
        {
            Console.WriteLine("Please input the value you want to set:");
            if (!char.TryParse(Console.ReadLine(), out Data)) { Alert("Please input a correct char"); }
            else
            {
                Console.WriteLine($"The data has set to {Data}");
                Console.ReadKey();
            }
        }

        public void CameraBind(GameObject GameObj, FllowMode fllow, uint sort, bool isRestrict, bool isShowFrame, int width = 21, int height = 21)//将某物体绑定在相机上，并且将相机与屏幕和地图绑定,然后初始化屏幕
        {
            Camera = new Camera(GameObj, fllow, isRestrict, Map, Screen);
            Screen.Init(width, height, isShowFrame: isShowFrame);
            Screen.AddReRendObj(GameObj, sort);
        }
    }
}
