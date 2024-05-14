using NewGameFrame.MathCore;
using NewGameFrame.Core;
using NewGameFrame.Render;
using NewGameFrame.Physics;

namespace NewFrameTest
{
    internal class Game : IGame
    {
        public int Step { get; set; }

        public Scene? GetScene(int SceneIndex)
        {
            switch (SceneIndex)
            {
                case 0:
                    var scene = GameManager.CreatScene();

                    // 背景
                    _ = new GameObject(scene) { Position = new(0, 0) }.AddComponet<Renderer>().Image = new('.', 21, 21);
                    
                    // 墙
                    var wall = new GameObject(scene, "Wall") { Position = new(0, 10) };
                    wall.AddComponet<Renderer>().Image = new('%', 10, 1);
                    wall.AddComponet<BoxCollider>().SetBoxToImage();

                    // 玩家
                    var p = new GameObject(scene, "Player") { Position = new Vector(0, 0, 1) };
                    p.AddComponet<Renderer>().Image = new('#', 3, 3);
                    p.AddComponet<Move>();
                    p.AddComponet<BoxCollider>().SetBoxToImage().ColliderEnter += (other) => Screen.Instance.HUD = ($"{p.Name} interact with {other.Name}");
                    
                    return scene;
                default:
                    return null;
            }
        }

        public void Init()
        {
            Step = 0;
        }

        
    }
}
