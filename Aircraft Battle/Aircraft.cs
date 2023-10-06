using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;

namespace Aircraft_Battle
{
    public abstract class Aircraft : GameObject
    {
        public int MaxHp = 0;//飞机的初始血量x
        public int Hp;//当前血量
        public Bullet Bullet { get; set; }//当前子弹的种类

        public Aircraft(int x, int y) : base(x, y, ' ', map: Program.system.Map)
        {
            Hp = MaxHp;
        }
        public void Die()//死亡
        {
            int index = Array.IndexOf(Program.Aircrafts, this);
            Program.Aircrafts.SetValue(null, index);
            Place(true);
            Position = (-1, -1);
        }
        public void Shoot()//射击
        {
            int index = Program.BIndex;
            if (index != -1) Program.Bullets.SetValue();//生成一枚子弹
        }
    }
}
