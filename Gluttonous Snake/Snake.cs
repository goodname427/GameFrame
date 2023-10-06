using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;

namespace Gluttonous_Snake
{
    public class Snake
    {
        public class Body : GameObject//玩家身体部分
        {
            public Body(int x, int y, char image, Map map) : base(x, y, image, map) { }
        }
        public List<Body> Bodys = new List<Body>();//玩家身体
        public Body Head;//玩家头部
        public int Hp;//玩家血量
        public Map Map;
        public Snake(Map map)//构造函数
        {
            Map = map;
            Bodys.Add(new Body((int)(Program.System.Map.Width * 0.5), (int)(Program.System.Map.Height * 0.5), Program.IHead, map));//添加身体
            Bodys.Add(new Body((int)(Program.System.Map.Width * 0.5), (int)(Program.System.Map.Height * 0.5) + 1, Program.IBody, map));
            Bodys.Add(new Body((int)(Program.System.Map.Width * 0.5), (int)(Program.System.Map.Height * 0.5) + 2, Program.IBody, map));
            Head = Bodys[0];//将头部指向第一部分身体
            Head.Direction = Direction.Up;//方向默认向上
            Hp = Program.Hp;//重置血量
        }
        public void Move()//移动
        {
            for (int i = Bodys.Count - 1; i > 0; i--)
            {
                if (i == Bodys.Count - 1) Bodys[i].Place(true);//最后一部分身体所处位置的信息设为空
                Bodys[i].Position = Bodys[i - 1].Position;//将后一部分身体的位置设为前一部分身体的位置
                Bodys[i].Place();//更改信息
            }
            Move<Body>.AutoMove(Head, false, false, true);//移动头部
            if (Head.IsOutMap() || Head.Touch(Program.IBody) || Head.Touch(Program.IBarrier))//判断是否死亡
            {
                if (--Hp <= 0)
                {
                    Program.System.Step = 0;
                }//若去除一条生命后生命仍大于零则继续游戏
            }
            foreach (var food in Program.Foods)
            {
                if (Head.Touch(food.Image))//是否吃到食物
                {
                    food.Execute(this);//食物产生效果
                    if (food == Program.Foods[0])
                    {
                        Program.SumFood(Map);//再生成一个食物
                    }
                }
            }
            Head.Place();
        }

    }
}
