using NewGameFrame.MathCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGameFrame.Physics
{
    public class BoxPrimitive : IPrimitive
    {
        public Vector Min { get; set; }
        public Vector Max { get; set; }

        public BoxPrimitive()
        {
            Min = Vector.Zero;
            Max = Vector.Zero;
        }

        public IEnumerable<Vector> Vertexes
        {
            get
            {
                yield return Min;
                yield return Max;
            }
        }

        public bool IsInside(Vector position)
        {
            return position.X >= Min.X && position.X <= Max.X && position.Y >= Min.Y && position.Y <= Max.Y;
        }
    }
}
