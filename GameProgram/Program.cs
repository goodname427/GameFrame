using System;
using GameFrame;

namespace GameProgram//总游戏界面
{
    class Program : ISystem
    {
        static ISystem system = new Program();
        int ISystem.Step { get; set; } = 0;
        Process ISystem.Process { get; set; }
        Map ISystem.Map { get; set; }
        Screen ISystem.Screen { get; set; }
        Camera ISystem.Camera { get; set; }

        void ISystem.GlobalInit()
        {

        }
        void ISystem.LocalInit()
        {

        }

        static void Main(string[] args)
        {
            OptionGroup op = new OptionGroup("Welcome To The Console Game\nPlease Select Game:", "Gluttonous Snake", "Aircraft Battle", "Mine Clearance", "Exit");

            op[0].OptionEvent += () => Gluttonous_Snake.Program.Main();
            op[1].OptionEvent += () => Aircraft_Battle.Program.Main();
            op[2].OptionEvent += () => Mine_Clearance.Program.Main();
            op[op.Count - 1].OptionEvent += () => system.Step = -1;//退出

            while (system.Step == 0) op.Show();
        }
    }
}
