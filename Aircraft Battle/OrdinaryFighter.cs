using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;

namespace Aircraft_Battle
{
    class OrdinaryFighter : Aircraft//普通敌机
    {
        //public override Type  { get; set; } = typeof(OrdinaryBullet);
        //public OrdinaryFighter() : base(RandomPosition.Next(Program.system.Map.Width), 0)
        //{
        //    Bullet = ;
        //    Image = 'T';
        //    Direction = Direction.Down;
        //}
        public OrdinaryFighter(int x, int y) : base(x, y)
        {
        }
    }
}
