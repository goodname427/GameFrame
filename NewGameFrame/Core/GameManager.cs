using NewGameFrame.MathCore;
using NewGameFrame.Render;
using System.Security.Cryptography;

namespace NewGameFrame.Core
{
    public static class GameManager
    {
        public static Scene CreatScene(int width = 21, int heigth = 21, int x = 0, int y = 0)
        {
            var scene = new Scene();
            var go = new GameObject(scene) { Position = new(x, y) };
            var camera = go.AddComponet<Camera>();
            camera.Size = new Vector(width, heigth);
            return scene;
        }

        public static void Start(IGame game)
        {
            _ = new Screen();
            game.Init();
            var step = -1;
            Scene? scene = null;
            while (game.Step >= 0)
            {
                //更换场景
                if (step != game.Step || scene is null)
                {
                    scene = game.GetScene(game.Step);
                    if (scene is null)
                    {
                        throw new ArgumentNullException(nameof(scene));
                    }

                    step = game.Step;
                    scene.Start();
                }
                //场景更新
                scene.Update();
            }
        }
    }
}
