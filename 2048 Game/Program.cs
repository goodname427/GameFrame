using GameFrame;
using System;

namespace _2048_Game//总游戏界面
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
            system.Process.Update += () =>
              {
                  system.Map.Update();
                  var key = ISystem.KeyPress();
                  if(key!=null&&key==ConsoleKey.J)
                  {
                      Number.SummonNumber(system.Map,1,9);
                  }
              };
        }
        void ISystem.LocalInit()
        {
            system.Map.Init(4, 4);
            system.CameraBind(null, FllowMode.NoFllow, 1, false, true, 4, 4);
        }

        static void Main(string[] args)
        {
            GameManager.GameStart(system);
        }
    }
}
