using NewGameFrame.MathCore;
using NewGameFrame.Core;
using NewGameFrame.Render;

namespace NewFrameTest
{
    internal class Game : IGame
    {
        public Scene[] Scenes { get; set; } = new Scene[1];

        public int Step { get; set; }

        public void Init()
        {
            var scene = GameManager.CreatScene();
            Scenes[0] = scene;
      
            _ = new GameObject(scene) { Position = new(0, 0)}.AddComponet<Renderer>().Image = new('.', 21, 21);

            var p = new GameObject(scene) { Position = new Vector(0, 0, 1) };
            p.AddComponet<Renderer>().Image = new('#', 3, 3);
            p.AddComponet<Move>();

            Step = 0;
        }
    }
}
