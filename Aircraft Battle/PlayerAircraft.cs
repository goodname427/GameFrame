using System;
using System.Collections.Generic;
using System.Text;
using GameFrame;

namespace Aircraft_Battle
{
    class PlayerAircraft : Aircraft//玩家飞机
    {
        public PlayerAircraft() : base(Program.system.Map.Width / 2, Program.system.Map.Height - 1)
        {

            Image = 'W';
            Direction = Direction.Up;
        }
    }
}
