using NewGameFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewFrameTest
{
    internal class Move : Componet
    {
        public Move(GameObject gameObject) : base(gameObject)
        {

        }

        public override void Update()
        {
            Position += Input.GetDirection();
        }
    }
}
