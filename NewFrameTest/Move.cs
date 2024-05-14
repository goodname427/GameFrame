using NewGameFrame.Core;

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
