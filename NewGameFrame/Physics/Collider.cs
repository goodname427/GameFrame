using System.Runtime.CompilerServices;
using NewGameFrame.Core;

namespace NewGameFrame.Physics
{
    public class Collider : Componet
    {
        public int Layer { get; set; } = 0;
        public int TouchLayer { get; set; } = -1;

        public Collider(GameObject gameObject) : base(gameObject)
        {

        }
    }
}
