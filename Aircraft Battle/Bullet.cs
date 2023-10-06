using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;

namespace Aircraft_Battle
{
    public abstract class Bullet : GameObject
    {
        public virtual int Speed { get; set; } = 0;//像素点/秒
        public Aircraft Owner { get; set; } = null;//发射者      
        public bool Enabled { get; set; } = true;//可用性

        public Bullet() : base(0, 0, (char)0, map: Program.system.Map) { }
        public abstract Bullet Creat(Vector pos, Direction direct);
        /// <summary>
        /// 子弹的运行
        /// </summary>
        public virtual void Move()
        {
            Move<Bullet>.AutoMove(this, true, false, true);
            Hit();
        }
        /// <summary>
        /// 击中目标
        /// </summary>
        void Hit()
        {

            foreach (var ac in Program.Aircrafts)
                if (ac != null && ac != Owner && Touch(ac))
                {
                    Enabled = false;
                    if (--ac.Hp < 0) ac.Die();
                    Place(true);
                    Program.bltSpdTmr[Speed].Update -= Move;
                    //if (Program.system.process.Changeable) Program.system.process.Timers.Remove(timer);
                }
            int index = Array.IndexOf(Program.Bullets, this);
            if (index != -1 && IsOutMap()) Program.Bullets.SetValue(null, index);
            Program.bltSpdTmr[Speed].Update -= Move;//删除子弹
            //if (Program.system.process.Changeable) Program.system.process.Timers.Remove(timer);
            if (Enabled) Place();
        }
    }
    public class OrdinaryBullet : Bullet
    {
        public override Bullet Creat(Vector pos, Direction direct)
        {
            var bullet = new OrdinaryBullet();
            bullet.Position = pos;
            bullet.Direction = direct;
            bullet.Speed = Speed;

            return bullet;
        }
    }
}
