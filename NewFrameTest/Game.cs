using NewGameFrame;

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
            for (int i = -10; i <= 10; i++)
            {
                for (int j = -10; j <= 10; j++)
                {
                    _ = new GameObject(scene) { Position = new(i, -j), Image = '.' };
                }
            }
            var p = new GameObject(scene) { Position = new Vector(0, 0, 1), Image = '#' };
            p.AddComponet<Move>();
        }
    }
}
