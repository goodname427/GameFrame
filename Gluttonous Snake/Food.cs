using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;

namespace Gluttonous_Snake
{
    public partial class Program
    {
        public static List<Food> Foods;//食物序列
        public class Food//食物
        {
            internal char Image;
            internal bool CanRefresh;
            public Food(char im,bool canRefresh=true)
            {
                Image = im;
                CanRefresh = canRefresh;
            }
            public delegate void dele(Snake snake);
            public event dele Effect;
            public void Execute(Snake snake)
            {
                Effect(snake);
            }
        }
        void FoodSingle()//单人模式
        {
            Foods = new List<Food> { new Food('F'), new Food('H'), new Food('M'), new Food('C'), new Food('f') };//单人食物序列
            Foods[0].Effect += (Snake snake) =>
             {
                 for (int i = 1; i <= AddLength; i++)
                     snake.Bodys.Add(new Snake.Body(-1, -1, IBody, snake.Map));//增加一部分身体
             };
            Foods[1].Effect += (Snake snake) =>
             {
                 snake.Hp++;//增加玩家生命
             };
            Foods[2].Effect += (Snake snake) =>
              {
                  for (int i = 0; i < 4; i++)
                      SumFood(snake.Map, Foods[4]);//食物狂欢
              };
            Foods[3].Effect += (Snake snake) =>
              {
                  Random r = new Random();
                  switch (r.Next(5))
                  {
                      case 0: Console.ForegroundColor = ConsoleColor.Red;/*变色*/ break;
                      case 1: Console.ForegroundColor = ConsoleColor.Blue;/*变色*/ break;
                      case 2: Console.ForegroundColor = ConsoleColor.Yellow;/*变色*/ break;
                      case 3: Console.ForegroundColor = ConsoleColor.Green;/*变色*/ break;
                      case 4: Console.ForegroundColor = ConsoleColor.Cyan;/*变色*/ break;
                  }
              };
            Foods[4].Effect += (Snake snake) =>
              {
                  snake.Bodys.Add(new Snake.Body(-1, -1, IBody, snake.Map));//增加一格身体
              };

        }
        void FoodDouble()//双人模式
        {
            Foods = new List<Food> { new Food('F'), new Food('H'), new Food('M'), new Food('C'), new Food('f'), new Food('B'), new Food('L'), new Food('U'), new Food('S') };//双人食物序列
            Foods[0].Effect += (Snake snake) =>
            {
                for (int i = 1; i <= AddLength; i++)
                    snake.Bodys.Add(new Snake.Body(-1, -1, IBody, snake.Map));//增加一部分身体
            };
            Foods[1].Effect += (Snake snake) =>
            {
                snake.Hp++;//增加玩家生命
            };
            Foods[2].Effect += (Snake snake) =>
            {
                for (int i = 0; i < 4; i++)
                    SumFood(snake.Map, Foods[4]);//食物狂欢
            };
            Foods[3].Effect += (Snake snake) =>
            {
                Random r = new Random();
                switch (r.Next(5))
                {
                    case 0: Console.ForegroundColor = ConsoleColor.Red;/*变色*/ break;
                    case 1: Console.ForegroundColor = ConsoleColor.Blue;/*变色*/ break;
                    case 2: Console.ForegroundColor = ConsoleColor.Yellow;/*变色*/ break;
                    case 3: Console.ForegroundColor = ConsoleColor.Green;/*变色*/ break;
                    case 4: Console.ForegroundColor = ConsoleColor.Cyan;/*变色*/ break;
                }
            };
            Foods[4].Effect += (Snake snake) =>
            {
                snake.Bodys.Add(new Snake.Body(-1, -1, IBody, snake.Map));//增加一格身体
            };
            Foods[5].Effect += (Snake snake) =>
            {
                Random r = new Random();
                (int, int) FP = (r.Next(21), r.Next(21));
                if (snake.Map != AIMap)//判断该玩家在那个地图
                {
                    while (AIMap[FP.Item1, FP.Item2] != 0) FP = (r.Next(21), r.Next(21));//避免生成在蛇内部               
                    AIMap[FP.Item1, FP.Item2] = IBarrier;//为对方生成一个障碍
                }
                else
                {
                    while (System.Map[FP.Item1, FP.Item2] != 0) FP = (r.Next(21), r.Next(21));
                    System.Map[FP.Item1, FP.Item2] = IBarrier;
                }
            };
            Foods[6].Effect += (Snake snake) =>
              {
                  if (snake == Player)//减少对方长度
                  {
                      if (AIPlayer.Bodys.Count > 3)
                      {
                          AIPlayer.Bodys[AIPlayer.Bodys.Count - 1].Place(true);
                          AIPlayer.Bodys.RemoveAt(AIPlayer.Bodys.Count - 1);
                      }
                  }
                  else
                  {
                      if (Player.Bodys.Count > 3)
                      {
                          Player.Bodys[Player.Bodys.Count - 1].Place(true);
                          Player.Bodys.RemoveAt(Player.Bodys.Count - 1);
                      }
                  }
              };
            Foods[7].Effect += (Snake snake) =>
            {
                if (snake == AIPlayer)
                {
                    if (Timer5.Interval >= 100)//加速
                        Timer5.Interval -= 50;
                }
                else
                {
                    if (Timer1.Interval >= 100)//加速
                        Timer1.Interval -= 50;
                }
            };
            Foods[8].Effect += (Snake snake) =>
            {
                if (snake == AIPlayer)
                {
                    if (Timer1.Interval <= 450)//减速                      
                        Timer1.Interval += 50;
                }
                else
                {
                    if (Timer5.Interval <= 450)
                        Timer5.Interval += 50;
                }
            };
        }
    }
}
