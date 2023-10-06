using System;
using GameFrame;
namespace MapEditor
{
    class Program : ISystem
    {
        static ISystem System = new Program();
        int ISystem.Step { set; get; }
        Process ISystem.Process { get; set; }
        Map ISystem.Map { get; set; }
        Screen ISystem.Screen { get; set; }
        Camera ISystem.Camera { get; set; }
        void ISystem.GlobalInit() 
        {
            System.Process.Update += () =>
              {
                  System.Map.Update();
              };
        }
        void ISystem.LocalInit() 
        {
            SetMapSize();
        }
        void SetMapSize()//设置地图大小
        {
            Console.WriteLine("设置地图大小(以空格隔开,宽在前,高在后):");
            string answer = Console.ReadLine();
            char[] split = { ' ' };
            string[] answers = answer.Split(split);
            while(answers.Length!=2||answer.Length<2||!int.TryParse(answers[0],out _)||!int.TryParse(answers[1],out _))
            {
                Console.WriteLine("请重新输入:");
                answer = Console.ReadLine();
                answers = answer.Split(split);
            }
            System.Map.Init(int.Parse(answers[0]),int.Parse(answers[1]),false);
        }
         
        static void Main(string[] args)
        {
            GameManager.GameStart(System);
        }
    }
}
