using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;

namespace Gluttonous_Snake
{
    class AI
    {
        static (int X, int Y) TarP;
        static (int X, int Y) CurP;
        static int XDis;
        static int YDis;
        static GameFrame.Direction Dire;
        static Direction[] AllDire = { new Direction(), new Direction(), new Direction(), new Direction() };//四个方向
        static List<Direction> Optional = new List<Direction>();//可行的方向     
        static bool HasOnefesible = true;//是否有可行的路
        static bool HasFood = false;//当前地图是否存在食物
        static Map Map = Program.AIMap;
        protected class Direction : IComparable//方向类
        {
            public bool feasible = true;//可行性
            public int Order = 0;//优先级
            int IComparable.CompareTo(object obj)//优先级排序
            {
                Direction d = (Direction)obj;
                if (Order > d.Order) return -1;
                else if (Order == d.Order) return 0;
                else return 1;
            }
        }
        public static ConsoleKey? AIMove(Snake snake)
        {
            GetInfor(snake);//获取游戏信息
            for (int i = 1; i < 5; i++)//每个方向判断
            {
                AllDire[i - 1].feasible = HasBarrier(i);
                if (AllDire[i - 1].feasible) AllDire[i - 1].feasible = HasBody(i);
            }
            HasOnefesible = false;
            foreach (var i in AllDire)
                if (i.feasible) HasOnefesible = true;//判断是否有路可寻
            if (HasOnefesible)//有路则进行寻食
            {
                Optional = new List<Direction>();//重置可选方向
                foreach (var d in AllDire)
                    if (d.feasible) Optional.Add(d);
                while (Optional != null)
                {
                    OrderJudge();
                    Optional.Sort();//排序然后选择优先级最高的选项
                    switch (Array.IndexOf(AllDire, Optional[0]))
                    {
                        case 0: return ConsoleKey.W;
                        case 1: return ConsoleKey.S;
                        case 2: return ConsoleKey.A;
                        case 3: return ConsoleKey.D;
                        default: return ConsoleKey.Z;
                    }
                }
                return ConsoleKey.Z;
            }
            else//无路时选择自杀
            {
                for (int i = 1; i < 5; i++)
                    if (!HasBarrier(i)) switch (i)
                        {
                            case 0: return ConsoleKey.W;
                            case 1: return ConsoleKey.S;
                            case 2: return ConsoleKey.A;
                            case 3: return ConsoleKey.D;
                            default: return ConsoleKey.Z;
                        }
                return ConsoleKey.Z;
            }
        }
        static bool HasBarrier(int d)//碰壁判断
        {
            switch (d)
            {
                case 1:
                    if (CurP.Y == 0)
                        return false;
                    else return true;
                case 2:
                    if (CurP.Y == Program.System.Map.Height - 1)
                        return false;
                    else return true;
                case 3:
                    if (CurP.X == 0)
                        return false;
                    else return true;
                case 4:
                    if (CurP.X == Program.System.Map.Width - 1)
                        return false;
                    else return true;
                default: return false;
            }
        }
        static bool HasBody(int d)//自身判断
        {
            switch (d)//检测某一方向是否有自身的身体
            {
                case 1:
                    if (Map[(CurP.X, CurP.Y - 1)] == Program.IBody|| Map[(CurP.X, CurP.Y - 1)] == Program.IBarrier)
                        return false;
                    else return true;
                case 2:
                    if (Map[(CurP.X, CurP.Y + 1)] == Program.IBody || Map[(CurP.X, CurP.Y - 1)] == Program.IBarrier)
                        return false;
                    else return true;
                case 3:
                    if (Map[(CurP.X - 1, CurP.Y)] == Program.IBody || Map[(CurP.X, CurP.Y - 1)] == Program.IBarrier)
                        return false;
                    else return true;
                case 4:
                    if (Map[(CurP.X + 1, CurP.Y)] == Program.IBody || Map[(CurP.X, CurP.Y - 1)] == Program.IBarrier)
                        return false;
                    else return true;
                default: return false;
            }
        }
        static void GetInfor(Snake snake)//获取游戏信息
        {
            CurP = snake.Head.Position;//当前位置
            Dire = snake.Head.Direction;//当前方向
            for (int i = 0; i < Map.Height; i++)
            {
                for (int j = 0; j < Map.Width; j++)
                {
                    if (Map[j, i] == Program.IFood|| Map[j, i] == 'B'|| Map[j, i] == 'U'|| Map[j, i] == 'S'|| Map[j, i] == 'f'|| Map[j, i] == 'L'|| Map[j, i] == 'M'|| Map[j, i] == 'H') 
                    { TarP = (j, i); HasFood = true; }//食物位置
                }
            }
            if (!HasFood) TarP = (0, 0);
            XDis = TarP.X - CurP.X;//X距离
            YDis = TarP.Y - CurP.Y;//Y距离
        }
        static void OrderJudge()//优先级判断
        {
            foreach (var d in AllDire)
                d.Order = 0;//重置优先级
            #region  Z型
            //if (false)
            //{
            //    if (XDis != 0)//若X距离不为零
            //    {
            //        AllDire[0].Order = 2;//优先在Y轴方向移动
            //        AllDire[1].Order = 2;
            //        if (XDis > 0) AllDire[3].Order = 1;//当Y轴无选项时,选择往目标位置转向
            //        else AllDire[2].Order = 1;
            //        if (Dire == 1)
            //        {
            //            CurP.Y--;
            //            if (!HasBarrier(1)) AllDire[0].Order = 0;
            //            CurP.Y++;
            //        }
            //        if (Dire == 2)
            //        {
            //            CurP.Y++;
            //            if (!HasBarrier(2)) AllDire[1].Order = 0;
            //            CurP.Y--;
            //        }
            //    }
            //    else
            //    {
            //        if (YDis > 0) { AllDire[1].Order = 2; AllDire[0].Order = 1; }//选择目标方向移动
            //        else { AllDire[1].Order = 1; AllDire[0].Order = 2; }
            //    }
            //}
            #endregion
            #region 直线型
            if (true)
            {
                if (XDis * XDis > YDis * YDis)
                {
                    if (XDis > 0) AllDire[3].Order = 2;
                    else if (XDis < 0) AllDire[2].Order = 2;
                    if (YDis > 0) AllDire[1].Order = 1;
                    else if (YDis < 0) AllDire[0].Order = 1;
                }
                else
                {
                    if (XDis > 0) AllDire[3].Order = 1;
                    else if (XDis < 0) AllDire[2].Order = 1;
                    if (YDis > 0) AllDire[1].Order = 2;
                    else if (YDis < 0) AllDire[0].Order = 2;
                }
            }
            #endregion
        }
    }
}

