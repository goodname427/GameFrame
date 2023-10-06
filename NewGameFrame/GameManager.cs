using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGameFrame
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
            while (game.Step >= 0)
            {
                //更换场景
                if (step != game.Step)
                {
                    game.Scenes[game.Step].Start();
                }
                //场景更新
                game.Scenes[game.Step].Update();
            }
        }
    }
}
